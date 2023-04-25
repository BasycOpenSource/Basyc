namespace Basyc.MessageBus.Client.Tests.Diagnostics;

public class BusHandlerLoggerTests
{
    //[Fact]
    //public void Test()
    //{
    //  int currectionSessionId = 1;
    //  string currentHandlerName = "testHandler";
    //  var normalLogger = new Mock<ILogger>();
    //  var logSink = new Mock<IBusClientLogExporter>();
    //  logSink.Setup(x => x.SendLog<It.IsAnyType>(
    //        It.IsAny<string>(),
    //        It.IsAny<LogLevel>(),
    //        It.IsAny<string>(),
    //        It.IsAny<It.IsAnyType>(),
    //        It.IsAny<Exception>(),
    //        It.IsAny<Func<It.IsAnyType, Exception, string>>()))
    //  .Callback<string, LogLevel, int, object, Exception, Delegate>((handlerName, logLevel, sessionId, x, y, z) =>
    //  {
    //      sessionId.Should().Be(currectionSessionId);
    //      handlerName.Should().Be(currentHandlerName);
    //  });
    //  var handlerLogger = new BusHandlerLogger(normalLogger.Object, new IBusClientLogExporter[] { logSink.Object }, currentHandlerName);
    //  //var handlerLoggerScope = handlerLogger.BeginHandlerScope(new HandlerScopeState(currectionSessionId));
    //  BusHandlerLoggerSessionManager.StartSession(new LoggingSession("1", "asd");
    //  handlerLogger.LogInformation("mess1");
    //}
}
