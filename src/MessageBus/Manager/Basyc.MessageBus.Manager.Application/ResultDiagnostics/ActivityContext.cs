﻿using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Logging;
using System.Diagnostics;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public record class ActivityContext(
	ServiceIdentity Service,
	string TraceId,
	bool HasParent,
	string? ParentId,
	string Id,
	string DisplayName,
	RelativeTime StartTime)
{

	public event EventHandler? ActivityEnded;
	public event EventHandler? NestedActivityAdded;
	public event EventHandler? NestedActivityEnded;
	public event EventHandler? ParentAssigned;

	private readonly List<ActivityContext> nestedActivities = new();
	public IReadOnlyList<ActivityContext> NestedActivities => nestedActivities;
	private readonly List<LogEntry> logs = new();
	public IReadOnlyList<LogEntry> Logs => logs;
	public bool HasEnded { get; private set; }
	public RelativeTime EndTime { get; private set; }
	public TimeSpan Duration { get; private set; }
	public ActivityStatusCode Status { get; private set; }
	public ActivityContext? ParentActivity { get; private set; }
	public void End(DateTimeOffset endTime, ActivityStatusCode status)
	{
		var endTimeRelative = new RelativeTime(endTime, StartTime.BaseTime);
		EndTime = endTimeRelative;
		Duration = EndTime.Value - StartTime.Value;
		Status = status;
		HasEnded = true;
		ActivityEnded?.Invoke(this, EventArgs.Empty);
	}
	public void AddNestedActivity(ActivityContext activity)
	{
		nestedActivities.Add(activity);
		activity.ActivityEnded += NestedActivity_Ended;
		NestedActivityAdded?.Invoke(this, EventArgs.Empty);
	}

	private void NestedActivity_Ended(object? sender, EventArgs e)
	{
		NestedActivityEnded?.Invoke(this, EventArgs.Empty);
	}

	public void AssignParentData(ActivityContext parentContext)
	{
		if (HasParent is false)
			throw new InvalidOperationException("Cant assign parent when HasParent is false");

		if (parentContext.Id != ParentId)
			throw new InvalidOperationException("Context id does not match ParentId");

		ParentActivity = parentContext;
		ParentAssigned?.Invoke(this, EventArgs.Empty);
	}

	public void AddLog(LogEntry logEntry)
	{
		logs.Add(logEntry);
	}

	public void AddLogs(IEnumerable<LogEntry> logsEntries)
	{
		foreach (var log in logsEntries)
		{
			logs.Add(log);
		}
	}
}
