using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.SignalR.Shared.DTOs
{
	public record ActivityStartSignalRDTO(string ServiceName, string? ParentId, string Id, string TraceId, string OperatioName, DateTimeOffset StarTime)
	{
		public static ActivityStartSignalRDTO FromEntry(ActivityStart activity)
		{
			return new ActivityStartSignalRDTO(activity.Service.ServiceName, activity.ParentId, activity.Id, activity.TraceId, activity.Name, activity.StartTime);
		}

		public static ActivityStart ToEntry(ActivityStartSignalRDTO activityDTO)
		{
			return new ActivityStart(new(activityDTO.ServiceName), activityDTO.TraceId, activityDTO.ParentId, activityDTO.Id, activityDTO.OperatioName, activityDTO.StarTime);
		}
	}



}
