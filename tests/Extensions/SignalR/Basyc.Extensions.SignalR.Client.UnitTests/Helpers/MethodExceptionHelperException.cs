using System.Runtime.CompilerServices;

namespace Basyc.Extensions.SignalR.Client.Tests.Helpers;

public class MethodExceptionHelperException : Exception
{
    public MethodExceptionHelperException(string methodName, DateTimeOffset time, object?[] methodArguments)
    {
        MethodName = methodName;
        Time = time;
        MethodArguments = methodArguments;
    }

    public string MethodName { get; }

    public DateTimeOffset Time { get; }

    public object?[] MethodArguments { get; }

    public static MethodExceptionHelperException CreateForCurrentMethod(object?[] methodArguments, [CallerMemberName] string memberName = "") =>
        new(memberName, DateTimeOffset.UtcNow, methodArguments);

    public static MethodExceptionHelperException CreateForCurrentMethod([CallerMemberName] string memberName = "") =>
        CreateForCurrentMethod(Array.Empty<object?>(), memberName);
}
