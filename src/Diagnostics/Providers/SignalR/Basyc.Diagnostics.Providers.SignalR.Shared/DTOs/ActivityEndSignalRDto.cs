using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Logging;
using System.Diagnostics;

namespace Basyc.Diagnostics.Providers.SignalR.Shared.DTOs;

public record ActivityEndSignalRDto(string ServiceName, string? ParentId, string Id, string TraceId, string OperatioName, DateTimeOffset StarTime,
	DateTimeOffset EndTime, ActivityStatusCode Status)
{
	public static ActivityEndSignalRDto FromEntry(ActivityEnd activity)
	{
		return new ActivityEndSignalRDto(activity.Service.ServiceName, activity.ParentId, activity.Id, activity.TraceId, activity.Name, activity.StartTime,
			activity.EndTime, activity.Status);
	}

	public static ActivityEnd ToEntry(ActivityEndSignalRDto activityDtO)
	{
		return new ActivityEnd(new ServiceIdentity(activityDtO.ServiceName), activityDtO.TraceId, activityDtO.ParentId, activityDtO.Id,
			activityDtO.OperatioName, activityDtO.StarTime, activityDtO.EndTime, activityDtO.Status);
	}
}
