﻿using Basyc.Diagnostics.Shared.Durations;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Application;

public class MessageRequest : ReactiveObject
{
	private readonly IDurationMapBuilder durationMapBuilder;

	public MessageRequest(RequestInput request, DateTimeOffset requestCreationTime, string traceId, IDurationMapBuilder durationMapBuilder,
		RequestDiagnostic requestDiagnostics, int orderIndex)
	{
		Request = request;
		this.durationMapBuilder = durationMapBuilder;
		Diagnostics = requestDiagnostics;
		CreationTime = requestCreationTime;
		State = RequestResultState.Started;
		TraceId = traceId;
		OrderIndex = orderIndex;
	}

	public RequestInput Request { get; init; }

	/// <summary>
	///     Time when request was created
	/// </summary>
	public DateTimeOffset CreationTime { get; init; }

	/// <summary>
	///     Time when request started
	/// </summary>
	public DateTimeOffset StartTime => durationMapBuilder.StartTime;

	public DateTimeOffset EndTime => durationMapBuilder.EndTime;

	public TimeSpan Duration => State == RequestResultState.Started ? default : durationMapBuilder.EndTime - durationMapBuilder.StartTime;

	public string TraceId { get; init; }
	public int OrderIndex { get; init; }
	public RequestDiagnostic Diagnostics { get; }
	[Reactive] public RequestResultState State { get; private set; }
	public object? Response { get; private set; }
	public string? ErrorMessage { get; private set; }

	///// <summary>
	///// You should call <see cref="Start"/> in moment that all internal processes are done and from now only work related to handeling a request are in process.
	///// </summary>
	//public void Start()
	//{
	//	if (durationMapBuilder.HasStarted)
	//		throw new InvalidOperationException($"{nameof(Start)} was already called");
	//}

	//public IDurationSegmentBuilder StartNewSegment(string segmentName)
	//{
	//	return durationMapBuilder.StartNewSegment(segmentName);
	//}

	public void Complete(object? response)
	{
		FinishDurationMap();
		if (Request.MessageInfo.HasResponse is false)
			throw new InvalidOperationException("Can't complete with return value becuase this message does not have return value");

		State = RequestResultState.Completed;
		Response = response;
		OnStateChanged();
	}

	public void Complete()
	{
		FinishDurationMap();

		if (Request.MessageInfo.HasResponse)
			throw new InvalidOperationException(
				$"Can't complete without return value becuase this message has return value. Use {nameof(Fail)} method when error occured and no return value is avaible");

		State = RequestResultState.Completed;
		OnStateChanged();
	}

	public void Fail(string errorMessage)
	{
		FinishDurationMap();
		State = RequestResultState.Failed;
		ErrorMessage = errorMessage;
		OnStateChanged();
	}

	public event EventHandler? StateChanged;

	private void OnStateChanged()
	{
		StateChanged?.Invoke(this, EventArgs.Empty);
	}

	public IDurationSegmentBuilder StartDurationMap()
	{
		return durationMapBuilder.StartNewSegment("Start");
	}

	public void FinishDurationMap()
	{
		durationMapBuilder.End();
	}
}