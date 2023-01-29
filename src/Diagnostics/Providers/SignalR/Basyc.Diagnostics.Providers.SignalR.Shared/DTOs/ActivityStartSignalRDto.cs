using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.SignalR.Shared.DTOs;

public record ActivityStartSignalRDto(string ServiceName, string? ParentId, string Id, string TraceId, string OperatioName, DateTimeOffset StarTime)
{
	public static ActivityStartSignalRDto FromEntry(ActivityStart activity)
	{
		return new ActivityStartSignalRDto(activity.Service.ServiceName, activity.ParentId, activity.Id, activity.TraceId, activity.Name, activity.StartTime);
	}

	public static ActivityStart ToEntry(ActivityStartSignalRDto activityDto)
	{
		return new ActivityStart(new ServiceIdentity(activityDto.ServiceName), activityDto.TraceId, activityDto.ParentId, activityDto.Id,
			activityDto.OperatioName, activityDto.StarTime);
	}
}
