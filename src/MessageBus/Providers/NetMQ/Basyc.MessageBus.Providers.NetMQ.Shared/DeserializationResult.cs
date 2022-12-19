using Basyc.MessageBus.NetMQ.Shared.Cases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.NetMQ.Shared;

public class DeserializationResult
{
    public RequestCase? Request { get; init; }
    public ResponseCase? Response { get; init; }
    public CheckInMessage? CheckIn { get; init; }
    public EventCase? Event { get; init; }
    public DeserializationFailureCase? Failure { get; init; }
    public MessageCase MessageCase { get; init; }
    public bool Failed { get; init; }

    private DeserializationResult(MessageCase messageCase)
    {
        this.MessageCase = messageCase;
        Failed = false;
    }
    private DeserializationResult(RequestCase requestCase) : this(MessageCase.Request)
    {
        Request = requestCase;
    }

    private DeserializationResult(ResponseCase responseCase) : this(MessageCase.Response)
    {
        Response = responseCase;
    }

    private DeserializationResult(CheckInMessage checkIn) : this(MessageCase.CheckIn)
    {
        CheckIn = checkIn;
    }
    private DeserializationResult(EventCase @event) : this(MessageCase.Event)
    {
        Event = @event;
    }

    private DeserializationResult(DeserializationFailureCase failure) : this(failure.MessageCase)
    {
        Failure = failure;
        Failed = true;
    }

    public static DeserializationResult CreateCheckIn(CheckInMessage checkIn)
    {
        return new DeserializationResult(checkIn);
    }

    public static DeserializationResult CreateRequest(RequestCase requestCase)
    {
        return new DeserializationResult(requestCase);
    }

    public static DeserializationResult CreateResponse(ResponseCase responseCase)
    {
        return new DeserializationResult(responseCase);
    }

    public static DeserializationResult CreateEvent(EventCase eventCase)
    {
        return new DeserializationResult(eventCase);
    }

    public static DeserializationResult CreateDeserializationFailure(DeserializationFailureCase failure)
    {
        return new DeserializationResult(failure);
    }
}
