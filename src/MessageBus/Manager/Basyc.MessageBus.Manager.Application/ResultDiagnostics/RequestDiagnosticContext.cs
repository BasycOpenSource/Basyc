﻿using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public class RequestDiagnosticContext
{
	private readonly Dictionary<string, ActivityContext> activityIdToActivityMap = new();
	private readonly object lockObject = new();

	private readonly List<LogEntry> logEntries = new();
	private readonly Dictionary<string, List<LogEntry>> missingActivityIdToLogsMap = new();
	private readonly Dictionary<string, List<ActivityContext>> missingParentIdToNestedActivityMap = new();

	public List<ServiceIdentityContext> services = new();

	public RequestDiagnosticContext(string traceId)
	{
		TraceId = traceId;
	}

	public IReadOnlyList<LogEntry> LogEntries => logEntries;
	public IReadOnlyList<ServiceIdentityContext> Services => services;

	public string TraceId { get; init; }
	public event EventHandler<LogEntry>? LogReceived;
	public event EventHandler<ActivityStart>? ActivityStartReceived;
	public event EventHandler<ActivityEnd>? ActivityEndReceived;

	public void Log(ServiceIdentity service, LogLevel logLevel, string message, string? spanId)
	{
		Log(service, DateTimeOffset.UtcNow, logLevel, message, spanId);
	}

	public void Log(ServiceIdentity service, DateTimeOffset time, LogLevel logLevel, string message, string? spanId)
	{
		LogEntry newLogEntry = new(service, TraceId, time, logLevel, message, spanId);
		Log(newLogEntry);
	}

	public void Log(LogEntry newLogEntry)
	{
		if (newLogEntry.TraceId != TraceId)
		{
			throw new ArgumentException("Request id does not match context reuqest result id", nameof(newLogEntry));
		}

		logEntries.Add(newLogEntry);
		logEntries.Sort((x, y) => x.Time.CompareTo(y.Time));
		if (newLogEntry.SpanId is not null)
		{
			if (activityIdToActivityMap.TryGetValue(newLogEntry.SpanId, out var activity))
			{
				activity.AddLog(newLogEntry);
			}
			else
			{
				missingActivityIdToLogsMap.TryAdd(newLogEntry.SpanId, new List<LogEntry>());
				missingActivityIdToLogsMap[newLogEntry.SpanId].Add(newLogEntry);
			}
		}

		OnLogAdded(newLogEntry);
	}

	public ActivityContext StartActivity(ActivityStart activityStart)
	{
		var serviceContext = EnsureServiceCreated(activityStart.Service);
		var hasParent = activityStart.ParentId is not null;
		var newActivityContext = new ActivityContext(activityStart.Service, activityStart.TraceId, hasParent, activityStart.ParentId, activityStart.Id,
			activityStart.Name, activityStart.StartTime);
		activityIdToActivityMap.Add(newActivityContext.Id, newActivityContext);
		if (missingActivityIdToLogsMap.TryGetValue(newActivityContext.Id, out var logs))
		{
			newActivityContext.AddLogs(logs);
			missingActivityIdToLogsMap.Remove(newActivityContext.Id);
		}

		if (hasParent)
		{
			if (activityStart.ParentId is null)
			{
				throw new InvalidOperationException($"{nameof(ActivityStart.ParentId)} cant be null when {nameof(ActivityStart.HasParent)} is true");
			}

			if (activityIdToActivityMap.TryGetValue(activityStart.ParentId, out var parentActivity))
			{
				newActivityContext.AssignParentData(parentActivity);
				if (newActivityContext.Service == parentActivity.Service)
				{
					parentActivity.AddNestedActivity(newActivityContext);
				}
				else
				{
					serviceContext.AddActivity(newActivityContext);
				}
			}
			else
			{
				missingParentIdToNestedActivityMap.TryAdd(activityStart.ParentId, new List<ActivityContext>());
				missingParentIdToNestedActivityMap[activityStart.ParentId].Add(newActivityContext);
			}
		}
		else
		{
			serviceContext.AddActivity(newActivityContext);
		}

		var isMissingParent = missingParentIdToNestedActivityMap.TryGetValue(activityStart.Id, out var nestedActivities);
		if (isMissingParent)
		{
			for (var nestedActivityIndex = 0; nestedActivityIndex < nestedActivities!.Count; nestedActivityIndex++)
			{
				var nestedActivity = nestedActivities![nestedActivityIndex];
				nestedActivity.AssignParentData(newActivityContext);
				newActivityContext.AddNestedActivity(nestedActivity);
			}

			missingParentIdToNestedActivityMap.Remove(activityStart.Id);
		}

		OnActivityStartReceived(activityStart);
		return newActivityContext;
	}

	public void EndActivity(ActivityEnd activityEnd)
	{
		if (activityIdToActivityMap.TryGetValue(activityEnd.Id, out var activity) is false)
		{
			activity = StartActivity(new ActivityStart(activityEnd.Service, activityEnd.TraceId, activityEnd.ParentId, activityEnd.Id, activityEnd.Name,
				activityEnd.StartTime));
		}

		activity.End(activityEnd.EndTime, activityEnd.Status);
		OnActivityEndReceived(activityEnd);
	}

	private void OnLogAdded(LogEntry newLogEntry)
	{
		LogReceived?.Invoke(this, newLogEntry);
	}

	private void OnActivityStartReceived(ActivityStart activityStart)
	{
		ActivityStartReceived?.Invoke(this, activityStart);
	}

	private void OnActivityEndReceived(ActivityEnd activityEnd)
	{
		ActivityEndReceived?.Invoke(this, activityEnd);
	}

	private ServiceIdentityContext EnsureServiceCreated(ServiceIdentity serviceIdentity)
	{
		var serviceVm = Services.FirstOrDefault(x => x.ServiceIdentity == serviceIdentity);
		if (serviceVm == null)
		{
			serviceVm = new ServiceIdentityContext(serviceIdentity);
			services.Add(serviceVm);
		}

		return serviceVm;
	}

	public ActivityContext GetActivity(string activityId)
	{
		return activityIdToActivityMap[activityId];
	}

	public bool TryGetActivity(string activityId, [NotNullWhen(true)] out ActivityContext? activityContext)
	{
		return activityIdToActivityMap.TryGetValue(activityId, out activityContext);
	}
}
