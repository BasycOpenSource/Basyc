using Basyc.Diagnostics.Providers.SignalR.Shared.DTOs;
using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.SignalR.Shared.DTOs;

#pragma warning disable CA1819 // Properties should not return arrays
public record ChangesSignalRDto(LogEntrySignalRDto[] Logs, ActivityStartSignalRDto[] ActivityStarts, ActivityEndSignalRDto[] ActivityEnds)
{
    public static DiagnosticChanges FromDto(ChangesSignalRDto dto)
    {
        var logs = dto.Logs.Select(LogEntrySignalRDto.ToEntry).ToArray();
        var starts = dto.ActivityStarts.Select(ActivityStartSignalRDto.ToEntry).ToArray();
        var ends = dto.ActivityEnds.Select(ActivityEndSignalRDto.ToEntry).ToArray();
        return new DiagnosticChanges(logs, starts, ends);
    }

    public static ChangesSignalRDto ToDto(DiagnosticChanges model)
    {
        var logDtos = model.Logs.Select(LogEntrySignalRDto.FromEntry).ToArray();
        var activityStartDtos = model.ActivityStarts.Select(ActivityStartSignalRDto.FromEntry).ToArray();
        var activityEndDtos = model.ActivityEnds.Select(ActivityEndSignalRDto.FromEntry).ToArray();
        return new ChangesSignalRDto(logDtos, activityStartDtos, activityEndDtos);
    }
}
