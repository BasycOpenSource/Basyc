using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using LogLevel = Nuke.Common.LogLevel;

namespace Tasks.Dotnet.Format
{
    public static class ProcessExceptionHelper
    {
        //public static void Throw(IProcess process, params string[] errors)
        //{
        //    var processOutputs = (BlockingCollection<Output>)process.Output;
        //    processOutputs.ForEach(x => processOutputs.Take());
        //    foreach (var error in errors)
        //    {
        //        processOutputs.Add(new Output() { Text = error, Type = OutputType.Err });
        //    }
        //    var dummyProcess = new DummyProcess(process.FileName, process.Arguments, process.WorkingDirectory, processOutputs, -1);
        //    var exception = new ProcessException(dummyProcess);
        //    throw exception;

        //}

        //public static void Throw(IProcess process, IReadOnlyCollection<Output> output)
        //{
        //    Throw(process, output.Select(x => x.Text).ToArray());
        //}

        //public static void Throw(ProcessException processException, IReadOnlyCollection<Output> output)
        //{
        //    var process = (IProcess)processException!.GetType().GetProperty("Process", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(processException)!;
        //    Throw(process, output);
        //}

        public static void Throw(ProcessException processException, params string[] errors)
        {
            var process = (IProcess)processException!.GetType().GetProperty("Process", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(processException)!;
            var processOutputs = (BlockingCollection<Output>)process.Output;
            processOutputs.ForEach(x => processOutputs.Take());
            foreach (var error in errors)
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

            var errorOutput = errors;
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

    file class DummyProcess : IProcess
    {
        public DummyProcess(string fileName, string arguments, string workingDirectory, IReadOnlyCollection<Output> output, int exitCode)
        {
            FileName = fileName;
            Arguments = arguments;
            WorkingDirectory = workingDirectory;
            Output = output;
            ExitCode = exitCode;
        }

        public string FileName { get; }

        public string Arguments { get; }

        public string WorkingDirectory { get; }

        public IReadOnlyCollection<Output> Output { get; }

        public int ExitCode { get; }

        public bool HasExited => throw new NotImplementedException();

        public int Id => throw new NotImplementedException();

        public void Dispose() => throw new NotImplementedException();
        public void Kill() => throw new NotImplementedException();
        public bool WaitForExit() => throw new NotImplementedException();
    }
}
