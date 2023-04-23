using Basyc.MessageBus.Shared;
using OneOf;
using Throw;

namespace Basyc.MessageBus.Client;

public struct BusTaskCompleted
{
}

public class BusTask : BusTask<BusTaskCompleted>
{
    protected BusTask(string sessionId, Task<OneOf<BusTaskCompleted, ErrorMessage>> value) : base(sessionId, value) { }
    protected BusTask(string sessionId, ErrorMessage error) : base(sessionId, error) { }
    protected BusTask(string sessionId, BusTaskCompleted value) : base(sessionId, value) { }

    public static BusTask FromTask(string sessionId, Task nestedTask)
    {
        var wrapperTask = nestedTask.ContinueWith(x =>
        {
            return (OneOf<BusTaskCompleted, ErrorMessage>)new BusTaskCompleted();
        });
        return new BusTask(sessionId, wrapperTask);
    }

    public static BusTask FromBusTask(string sessionId, BusTask<BusTaskCompleted> busTask) => new BusTask(sessionId, busTask.Task);

    public static BusTask FromBusTask(BusTask<BusTaskCompleted> busTask) => new BusTask(busTask.TraceId, busTask.Task);
}

public class BusTask<TValue>
{
    protected BusTask(string sessionId, Task<OneOf<TValue, ErrorMessage>> value)
    {
        TraceId = sessionId;
        Task = value;
    }

    protected BusTask(string sessionId, ErrorMessage error)
    {
        TraceId = sessionId;
        Task = System.Threading.Tasks.Task.FromResult<OneOf<TValue, ErrorMessage>>(error);
    }

    protected BusTask(string sessionId, TValue value)
    {
        TraceId = sessionId;
        Task = System.Threading.Tasks.Task.FromResult<OneOf<TValue, ErrorMessage>>(value);
    }

    public Task<OneOf<TValue, ErrorMessage>> Task { get; init; }
    public string TraceId { get; init; }

    public static BusTask<TValue> FromTask(string sessionId, Task<OneOf<TValue, ErrorMessage>> nestedTask) => new BusTask<TValue>(sessionId, nestedTask);

    public static BusTask<TValue> FromTask(string sessionId, Task<TValue> nestedTask)
    {
        var wrapperTask = nestedTask.ContinueWith<OneOf<TValue, ErrorMessage>>(x =>
        {
            if (x.IsCompletedSuccessfully)
            {
                return x.Result;
            }

            if (x.IsCanceled)
            {
                return new ErrorMessage("Canceled");
            }

            x.Exception.ThrowIfNull();

            return new ErrorMessage(x.Exception.Message);
        });
        return FromTask(sessionId, wrapperTask);
    }

    public static BusTask<TValue> FromTask<TNestedValue>(string sessionId, Task<TNestedValue> nestedTask,
        Func<TNestedValue, OneOf<TValue, ErrorMessage>> converter)
    {
        var wrapperTask = nestedTask.ContinueWith<OneOf<TValue, ErrorMessage>>(x =>
        {
            if (x.IsCompletedSuccessfully)
            {
                var converterResult = converter.Invoke(x.Result);
                return converterResult;
            }

            if (x.IsCanceled)
            {
                return new ErrorMessage("Canceled");
            }

            x.Exception.ThrowIfNull();

            return new ErrorMessage(x.Exception.Message);
        });
        return FromTask(sessionId, wrapperTask);
    }

    public static BusTask<TValue> FromTask<TNestedValue>(string sessionId, Task<OneOf<TNestedValue, ErrorMessage>> nestedTask,
        Func<TNestedValue, OneOf<TValue, ErrorMessage>> converter)
    {
        var wrapperTask = nestedTask.ContinueWith<OneOf<TValue, ErrorMessage>>(x =>
        {
            if (x.IsCompletedSuccessfully)
            {
                return x.Result.Match<OneOf<TValue, ErrorMessage>>(
                    nestedValue => converter.Invoke(nestedValue),
                    error => error);
            }

            if (x.IsCanceled)
            {
                return new ErrorMessage("Canceled");
            }

            x.Exception.ThrowIfNull();

            return new ErrorMessage(x.Exception.Message);
        });
        return FromTask(sessionId, wrapperTask);
    }

    public static BusTask<TValue> FromError(string sessionId, ErrorMessage error) => new BusTask<TValue>(sessionId, error);

    public static BusTask<TValue> FromValue(string sessionId, TValue value) => new BusTask<TValue>(sessionId, value);

    public static BusTask<TValue> FromBusTask<TNestedValue>(BusTask<TNestedValue> nestedBusTask, Func<TNestedValue, OneOf<TValue, ErrorMessage>> converter) => FromBusTask(nestedBusTask.TraceId, nestedBusTask, converter);

    public static BusTask<TValue> FromBusTask<TNestedValue>(string sessionId, BusTask<TNestedValue> nestedBusTask,
        Func<TNestedValue, OneOf<TValue, ErrorMessage>> converter) => FromTask(sessionId, nestedBusTask.Task, converter);

    public BusTask<TNestedValue> ContinueWith<TNestedValue>(Func<TValue, OneOf<TNestedValue, ErrorMessage>> converter) => BusTask<TNestedValue>.FromBusTask(this, converter);

    public BusTask ToBusTask() => BusTask.FromTask(TraceId, Task);
}
