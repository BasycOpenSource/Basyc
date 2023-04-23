using Basyc.Diagnostics.Providers.SignalR.Shared.DTOs;
using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.SignalR.Shared.DTOs;

public record ChangesSignalRDto(LogEntrySignalRDto[] Logs, ActivityStartSignalRDto[] ActivityStarts, ActivityEndSignalRDto[] ActivityEnds)
{
    public static DiagnosticChanges FromDto(ChangesSignalRDto dto)
    {
        var logs = dto.Logs.Select(x => LogEntrySignalRDto.ToEntry(x)).ToArray();
        var starts = dto.ActivityStarts.Select(x => ActivityStartSignalRDto.ToEntry(x)).ToArray();
        var ends = dto.ActivityEnds.Select(x => ActivityEndSignalRDto.ToEntry(x)).ToArray();
        return new DiagnosticChanges(logs, starts, ends);
    }

    public static ChangesSignalRDto ToDto(DiagnosticChanges model)
    {
        var logDtos = model.Logs.Select(x => LogEntrySignalRDto.FromEntry(x)).ToArray();
        var activityStartDtos = model.ActivityStarts.Select(x => ActivityStartSignalRDto.FromEntry(x)).ToArray();
        var activityEndDtos = model.ActivityEnds.Select(x => ActivityEndSignalRDto.FromEntry(x)).ToArray();
        return new ChangesSignalRDto(logDtos, activityStartDtos, activityEndDtos);
    }
}
