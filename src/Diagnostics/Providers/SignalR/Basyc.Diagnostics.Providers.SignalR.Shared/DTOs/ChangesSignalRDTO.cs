using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.SignalR.Shared.DTOs;

public record ChangesSignalRDTO(LogEntrySignalRDTO[] Logs, ActivityStartSignalRDTO[] ActivityStarts, ActivityEndSignalRDTO[] ActivityEnds)
{
    public static DiagnosticChanges FromDto(ChangesSignalRDTO dto)
    {
        var logs = dto.Logs.Select(x => LogEntrySignalRDTO.ToEntry(x)).ToArray();
        var starts = dto.ActivityStarts.Select(x => ActivityStartSignalRDTO.ToEntry(x)).ToArray();
        var ends = dto.ActivityEnds.Select(x => ActivityEndSignalRDTO.ToEntry(x)).ToArray();
        return new DiagnosticChanges(logs, starts, ends);
    }

    public static ChangesSignalRDTO ToDto(DiagnosticChanges model)
    {
        var logDtos = model.Logs.Select(x => LogEntrySignalRDTO.FromEntry(x)).ToArray();
        var activityStartDTOs = model.ActivityStarts.Select(x => ActivityStartSignalRDTO.FromEntry(x)).ToArray();
        var activityEndDTOs = model.ActivityEnds.Select(x => ActivityEndSignalRDTO.FromEntry(x)).ToArray();
        return new ChangesSignalRDTO(logDtos, activityStartDTOs, activityEndDTOs);
    }
}
