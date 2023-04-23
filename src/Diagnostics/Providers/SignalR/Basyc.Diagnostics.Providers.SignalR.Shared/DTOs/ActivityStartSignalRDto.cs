using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.SignalR.Shared.DTOs;

public record ActivityStartSignalRDto(string ServiceName, string? ParentId, string Id, string TraceId, string OperatioName, DateTimeOffset StarTime)
{
    public static ActivityStartSignalRDto FromEntry(ActivityStart activity) => new(activity.Service.ServiceName, activity.ParentId, activity.Id, activity.TraceId, activity.Name, activity.StartTime);

    public static ActivityStart ToEntry(ActivityStartSignalRDto activityDto) => new(
        new ServiceIdentity(activityDto.ServiceName),
        activityDto.TraceId,
        activityDto.ParentId,
        activityDto.Id,
        activityDto.OperatioName,
        activityDto.StarTime);
}
