using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using LogLevel = Nuke.Common.LogLevel;

namespace Tasks.Dotnet.Format;

public static class ProcessExceptionHelper
{
    public static void Throw(ProcessException processException, params string[] errors)
    {
        var process = (IProcess)processException!.GetType().GetProperty("Process", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(processException)!;
        var processOutputs = (BlockingCollection<Output>)process.Output;
        processOutputs.ForEach(x => processOutputs.Take());
        foreach (string error in errors)
        {
            processOutputs.Add(new Output() { Text = error, Type = OutputType.Err });
        }

        var messageField = processException!.GetType().GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic)!;
        messageField.SetValue(processException, FormatMessage(errors));
        throw processException;
    }

    private static string FormatMessage(params string[] errors)
    {
        const string indentation = "   ";

        var messageBuilder = new StringBuilder()
            .AppendLine($"Task throwed exception.");

        string[] errorOutput = errors;
        if (errorOutput.Length > 0)
        {
            messageBuilder.AppendLine("Error output:");
            errorOutput.ForEach(x => messageBuilder.Append(indentation).AppendLine(x));
        }
        else if (Logger.LogLevel <= LogLevel.Trace)
        {
            messageBuilder.AppendLine("Error output:");
            errorOutput.ForEach(x => messageBuilder.Append(indentation).AppendLine(x));
        }

        return messageBuilder.ToString();
    }
}
