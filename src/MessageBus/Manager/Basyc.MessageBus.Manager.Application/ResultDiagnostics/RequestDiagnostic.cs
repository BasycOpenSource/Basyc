using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public class RequestDiagnostic
{
	private readonly Dictionary<string, ActivityContext> activityIdToActivityMap = new();
	private readonly Dictionary<string, List<LogEntry>> missingActivityIdToLogsMap = new();
	private readonly Dictionary<string, List<ActivityContext>> missingParentIdToNestedActivityMap = new();
	private readonly List<ServiceIdentityContext> services = new();

	public RequestDiagnostic(string traceId)
	{
		TraceId = traceId;
		LogEntries = new ReadOnlyObservableCollection<LogEntry>(logEntries);
	}

	private readonly ObservableCollection<LogEntry> logEntries = new();
	public ReadOnlyObservableCollection<LogEntry> LogEntries { get; init; }
	public IReadOnlyList<ServiceIdentityContext> Services => services;

	public string TraceId { get; init; }
	public event EventHandler<LogEntry>? LogAdded;
	public event EventHandler<ActivityStart>? ActivityStartAdded;
	public event EventHandler<ActivityEnd>? ActivityEndAdded;

	public void AddLog(ServiceIdentity service, LogLevel logLevel, string message, string? spanId)
	{
		AddLog(service, DateTimeOffset.UtcNow, logLevel, message, spanId);
	}

	public void AddLog(ServiceIdentity service, DateTimeOffset time, LogLevel logLevel, string message, string? spanId)
	{
		LogEntry newLogEntry = new(service, TraceId, time, logLevel, message, spanId);
		AddLog(newLogEntry);
	}

	public void AddLog(LogEntry newLogEntry)
	{
		if (newLogEntry.TraceId != TraceId)
			throw new ArgumentException("Request id does not match context request result id", nameof(newLogEntry));

		logEntries.Add(newLogEntry);
		//Sort(logEntries); //Not nice when using Reactive -> publishes multiple events for one adding

		if (newLogEntry.SpanId is not null)
		{
			if (activityIdToActivityMap.TryGetValue(newLogEntry.SpanId, out var activity))
				activity.AddLog(newLogEntry);
			else
			{
				missingActivityIdToLogsMap.TryAdd(newLogEntry.SpanId, new List<LogEntry>());
				missingActivityIdToLogsMap[newLogEntry.SpanId].Add(newLogEntry);
			}
		}

		OnLogAdded(newLogEntry);
	}

	private static ObservableCollection<LogEntry> Sort(ObservableCollection<LogEntry> collection)
	{
		ObservableCollection<LogEntry> temp;
		temp = new ObservableCollection<LogEntry>(collection.OrderBy(x => x.Time));
		collection.Clear();
		foreach (var j in temp) collection.Add(j);
		return collection;

	}

	public ActivityContext AddStartActivity(ActivityStart activityStart)
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
				throw new InvalidOperationException($"{nameof(ActivityStart.ParentId)} cant be null when {nameof(ActivityStart.HasParent)} is true");

			if (activityIdToActivityMap.TryGetValue(activityStart.ParentId, out var parentActivity))
			{
				newActivityContext.AssignParentData(parentActivity);
				if (newActivityContext.Service == parentActivity.Service)
					parentActivity.AddNestedActivity(newActivityContext);
				else
					serviceContext.AddActivity(newActivityContext);
			}
			else
			{
				missingParentIdToNestedActivityMap.TryAdd(activityStart.ParentId, new List<ActivityContext>());
				missingParentIdToNestedActivityMap[activityStart.ParentId].Add(newActivityContext);
			}
		}
		else
			serviceContext.AddActivity(newActivityContext);

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

		OnActivityStartAdded(activityStart);
		return newActivityContext;
	}

	public void AddEndActivity(ActivityEnd activityEnd)
	{
		if (activityIdToActivityMap.TryGetValue(activityEnd.Id, out var activity) is false)
			activity = AddStartActivity(new ActivityStart(activityEnd.Service, activityEnd.TraceId, activityEnd.ParentId, activityEnd.Id, activityEnd.Name,
				activityEnd.StartTime));

		activity.End(activityEnd.EndTime, activityEnd.Status);
		OnActivityEndAdded(activityEnd);
	}

	private void OnLogAdded(LogEntry newLogEntry)
	{
		LogAdded?.Invoke(this, newLogEntry);
	}

	private void OnActivityStartAdded(ActivityStart activityStart)
	{
		ActivityStartAdded?.Invoke(this, activityStart);
	}

	private void OnActivityEndAdded(ActivityEnd activityEnd)
	{
		ActivityEndAdded?.Invoke(this, activityEnd);
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
