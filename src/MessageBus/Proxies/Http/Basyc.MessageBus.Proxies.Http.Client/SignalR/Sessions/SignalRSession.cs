using Basyc.MessageBus.HttpProxy.Shared.SignalR;
using Basyc.MessageBus.Shared;
using OneOf;
using System.Threading.Tasks;

namespace Basyc.MessageBus.HttpProxy.Client.SignalR.Sessions
{
	public readonly struct SignalRSession
	{
		private readonly TaskCompletionSource<OneOf<ResponseSignalRDTO, ErrorMessage>> taskSource;

		public string SessionId { get; }
		public string TraceId { get; }

		public SignalRSession(string sessionId, string TraceId)
		{
			SessionId = sessionId;
			this.TraceId = TraceId;
			this.taskSource = new TaskCompletionSource<OneOf<ResponseSignalRDTO, ErrorMessage>>();
		}

		public void Complete(OneOf<ResponseSignalRDTO, ErrorMessage> result)
		{
			taskSource.SetResult(result);
		}

		public Task<OneOf<ResponseSignalRDTO, ErrorMessage>> WaitForCompletion()
		{
			return taskSource.Task;
		}
	}
}
