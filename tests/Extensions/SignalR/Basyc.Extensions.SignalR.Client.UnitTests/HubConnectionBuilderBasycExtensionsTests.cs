using Basyc.Extensions.SignalR.Client.Tests.Helpers;
using Basyc.Extensions.SignalR.Client.Tests.Mocks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Basyc.Extensions.SignalR.Client.Tests;

public class HubConnectionBuilderBasycExtensionsTests
{
    [Fact]
    public void When_BuildStrongTyped_With_CorrectHub_Should_Not_Throw()
    {
        var hubClient = new HubConnectionMockBuilder()
            .BuildStrongTyped<ICorrectMethodsClientCanCallVoids>();

        var mockConnection = (HubConnectionMock)hubClient.UnderlyingHubConnection;
        hubClient.Call.SendNumber(1);

        mockConnection.LastSendCoreCall.Should().NotBeNull();
        mockConnection.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCallVoids.SendNumber));
    }

    [Fact]
    public void When_CreateStrongTyped_With_CorrectHub_Should_Not_Throw()
    {
        var connectionMock = new HubConnectionMockBuilder().BuildAsMock();
        var hubClient = connectionMock.CreateStrongTyped<ICorrectMethodsClientCanCallAllCorrect>();

        hubClient.Call.SendNumber(1);
        connectionMock.LastSendCoreCall.Should().NotBeNull();
        connectionMock.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCallAllCorrect.SendNumber));
        connectionMock.LastSendCoreCall!.Args.Should().Equal(1);

        hubClient.Call.SendNothing();
        connectionMock.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCallAllCorrect.SendNothing));
        connectionMock.LastSendCoreCall!.Args.Should().Equal(Array.Empty<object?>());

        var sendNumberTask = hubClient.Call.SendIntAsync(2);
        sendNumberTask.Should().NotBeNull();
        sendNumberTask.Should().BeAssignableTo<Task>();
        connectionMock.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCallAllCorrect.SendIntAsync));
        connectionMock.LastSendCoreCall!.Args.Should().Equal(2);
    }

    [Fact]
    public void Should_Throw_When_Creating_ClassCanClientCall()
    {
        var connectionMock = new HubConnectionMockBuilder().BuildAsMock();
        connectionMock.Invoking(x => x.CreateStrongTyped<WrongHubClientIsClass>()).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_Be_Able_To_Call_Inherited()
    {
        var connectionMock = new HubConnectionMockBuilder().BuildAsMock();
        var hubClient = connectionMock.CreateStrongTyped<ICorrectMethodsClientCanCallInheritedVoids>();
        hubClient.Call.SendNumber(1);

        connectionMock.LastSendCoreCall.Should().NotBeNull();
        connectionMock.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCallInheritedVoids.SendNumber));
        connectionMock.LastSendCoreCall!.Args.Should().Equal(1);
    }

    [Fact]
    public void Should_Throw_When_ReceivingMethod_Throws()
    {
        var connectionMock = new HubConnectionMockBuilder().BuildAsMock();
        var hubClient = connectionMock.CreateStrongTyped(new WrongHubClientIsClassNumbersExceptions());
        hubClient.Call.Invoking(x => WrongHubClientIsClassNumbersExceptions.ThrowNumberVoid(1)).Should().Throw<MethodExceptionHelperException>();
    }
}
