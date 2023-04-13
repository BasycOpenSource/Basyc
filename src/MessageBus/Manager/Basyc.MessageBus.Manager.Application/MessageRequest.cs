using Basyc.Diagnostics.Shared.Durations;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Application;

public class MessageRequest : ReactiveObject
{
	private readonly IDurationMapBuilder durationMapBuilder;
	private IDurationSegmentBuilder? requestActivity;

	public MessageRequest(RequestInput request, DateTimeOffset requestCreationTime, string traceId, IDurationMapBuilder durationMapBuilder,
		RequestDiagnostic requestDiagnostics, int orderIndex)
	{
		RequestInput = request;
		this.durationMapBuilder = durationMapBuilder;
		Diagnostics = requestDiagnostics;
		CreationTime = requestCreationTime;
		State = RequestResultState.Started;
		TraceId = traceId;
		OrderIndex = orderIndex;
		Duration = default;
	}

	public RequestInput RequestInput { get; init; }

	/// <summary>
	///     Time when request was created
	/// </summary>
	public DateTimeOffset CreationTime { get; init; }

	/// <summary>
	///     Time when request started
	/// </summary>
	public DateTimeOffset StartTime => requestActivity.Value().StartTime;

	public DateTimeOffset EndTime => requestActivity.Value().EndTime;

	[Reactive] public TimeSpan Duration { get; private set; }

	public string TraceId { get; init; }
	public int OrderIndex { get; init; }
	public RequestDiagnostic Diagnostics { get; }
	[Reactive] public RequestResultState State { get; private set; }
	public object? Response { get; private set; }
	public string? ErrorMessage { get; private set; }

	public void SetResponse(object? response)
	{
		//FinishDurationMap();
		if (RequestInput.MessageInfo.HasResponse is false)
			throw new InvalidOperationException("Can't complete with return value becuase this message does not have return value");

		Response = response;
		Duration = requestActivity.Value().EndTime - requestActivity.Value().StartTime;
		State = RequestResultState.Completed;
		OnStateChanged();
	}

	public void SetResponse()
	{
		//FinishDurationMap();

		if (RequestInput.MessageInfo.HasResponse)
			throw new InvalidOperationException(
				$"Can't complete without return value becuase this message has return value. Use {nameof(Fail)} method when error occured and no return value is avaible");

		Duration = requestActivity.Value().EndTime - requestActivity.Value().StartTime;
		State = RequestResultState.Completed;
		OnStateChanged();
	}

	public void Fail(string errorMessage)
	{
		Stop();
		ErrorMessage = errorMessage;
		Duration = requestActivity.Value().EndTime - requestActivity.Value().StartTime;
		State = RequestResultState.Failed;
		OnStateChanged();
	}

	public event EventHandler? StateChanged;

	private void OnStateChanged()
	{
		StateChanged?.Invoke(this, EventArgs.Empty);
	}

	public IDurationSegmentBuilder Start()
	{
		//var startTime = durationMapBuilder.Start();
		requestActivity = durationMapBuilder.StartNewSegment("MessageRequest Start");
		return requestActivity;
	}

	public void Stop()
	{
		var endTime = requestActivity.Value().End();
		//durationMapBuilder.End(endTime);
	}

	public void Stop(DateTimeOffset endTime)
	{
		requestActivity.Value().End(endTime);
		//durationMapBuilder.End(endTime);
	}
}
