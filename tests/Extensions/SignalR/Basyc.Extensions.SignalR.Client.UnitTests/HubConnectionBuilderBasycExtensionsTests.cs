using Basyc.Extensions.SignalR.Client.Tests.Helpers;
using Basyc.Extensions.SignalR.Client.Tests.Mocks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Basyc.Extensions.SignalR.Client.Tests
{
	public partial class HubConnectionBuilderBasycExtensionsTests
	{

		[Fact]
		public void When_BuildStrongTyped_With_CorrectHub_Should_Not_Throw()
		{
			var hubClient = new HubConnectionMockBuilder()
				.BuildStrongTyped<ICorrectMethodsClientCanCall_Voids>();

			var mockConnection = (HubConnectionMock)hubClient.UnderlyingHubConnection;
			hubClient.Call.SendNumber(1);

			mockConnection.LastSendCoreCall.Should().NotBeNull();
			mockConnection.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCall_Voids.SendNumber));
		}

		[Fact]
		public void When_CreateStrongTyped_With_CorrectHub_Should_Not_Throw()
		{
			var connectionMock = new HubConnectionMockBuilder().BuildAsMock();
			var hubClient = connectionMock.CreateStrongTyped<ICorrectMethodsClientCanCall_AllCorrect>();

			hubClient.Call.SendNumber(1);
			connectionMock.LastSendCoreCall.Should().NotBeNull();
			connectionMock.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCall_AllCorrect.SendNumber));
			connectionMock.LastSendCoreCall!.Args.Should().Equal(new object?[] { 1 });

			hubClient.Call.SendNothing();
			connectionMock.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCall_AllCorrect.SendNothing));
			connectionMock.LastSendCoreCall!.Args.Should().Equal(Array.Empty<object?>());

			var sendNumberTask = hubClient.Call.SendIntAsync(2);
			sendNumberTask.Should().NotBeNull();
			sendNumberTask.Should().BeAssignableTo<Task>();
			connectionMock.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCall_AllCorrect.SendIntAsync));
			connectionMock.LastSendCoreCall!.Args.Should().Equal(new object?[] { 2 });

		}

		[Fact]
		public void Should_Throw_When_Creating_ClassCanClientCall()
		{
			var connectionMock = new HubConnectionMockBuilder().BuildAsMock();
			connectionMock.Invoking(x => x.CreateStrongTyped<WrongHubClient_Is_Class>()).Should().Throw<ArgumentException>();
		}

		[Fact]
		public void Should_Be_Able_To_Call_Inherited()
		{
			var connectionMock = new HubConnectionMockBuilder().BuildAsMock();
			var hubClient = connectionMock.CreateStrongTyped<ICorrectMethodsClientCanCall_Inherited_Voids>();
			hubClient.Call.SendNumber(1);

			connectionMock.LastSendCoreCall.Should().NotBeNull();
			connectionMock.LastSendCoreCall!.MethodName.Should().Be(nameof(ICorrectMethodsClientCanCall_Inherited_Voids.SendNumber));
			connectionMock.LastSendCoreCall!.Args.Should().Equal(new object?[] { 1 });

		}

		[Fact]
		public void Should_Throw_When_ReceivingMethod_Throws()
		{
			var connectionMock = new HubConnectionMockBuilder().BuildAsMock();
			var hubClient = connectionMock.CreateStrongTyped<WrongHubClient_Is_Class_Numbers_Exceptions>(new WrongHubClient_Is_Class_Numbers_Exceptions());
			hubClient.Call.Invoking(x => x.ThrowNumberVoid(1)).Should().Throw<MethodExceptionHelperException>();
		}
	}
}