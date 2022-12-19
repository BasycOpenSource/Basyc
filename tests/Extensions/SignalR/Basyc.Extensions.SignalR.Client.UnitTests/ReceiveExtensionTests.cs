using Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall;
using Basyc.Extensions.SignalR.Client.Tests.Mocks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Basyc.Extensions.SignalR.Client.Tests
{
    public class ReceiveExtensionTests
    {
        [Fact]
        public async Task Correct_Should_Work()
        {
            var messageReceiver = new MethodsServerCanCall_Numbers();
            var hubConnection = HubConnectionMockBuilder.Create();
            var strongConnection = hubConnection.CreateStrongTypedReceiver<MethodsServerCanCall_Numbers>(messageReceiver);
            await hubConnection.StartAsync();
            bool received = false;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ReceiveExtension.Receive<MethodsServerCanCall_Numbers, Action<int>>(strongConnection, x => x.ReceiveNumber, x =>
            {
                received = true;
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Numbers.ReceiveNumber), new object?[] { 1 });

            await Task.Delay(400);
            received.Should().BeTrue();

        }

        [Fact]
        public async Task RandomName_Should_Not_Trigger()
        {
            var messageReceiver = new MethodsServerCanCall_Numbers();
            var hubConnection = HubConnectionMockBuilder.Create();
            var strongConnection = hubConnection.CreateStrongTypedReceiver<MethodsServerCanCall_Numbers>(messageReceiver);
            await hubConnection.StartAsync();
            bool received = false;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ReceiveExtension.Receive<MethodsServerCanCall_Numbers, Action<int>>(strongConnection, x => x.ReceiveNumber, x =>
            {
                received = true;
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await hubConnection.ReceiveMessage("RandomName", new object?[] { 1 });

            await Task.Delay(400);
            received.Should().BeFalse();

        }

        [Fact]
        public async Task Should_Wait_Until_Received()
        {
            var messageReceiver = new MethodsServerCanCall_Numbers();
            var hubConnection = HubConnectionMockBuilder.Create();
            var strongConnection = hubConnection.CreateStrongTypedReceiver<MethodsServerCanCall_Numbers>(messageReceiver);
            await hubConnection.StartAsync();
            bool received = false;

            var receiveTask = ReceiveExtension.Receive<MethodsServerCanCall_Numbers, Action<int>>(strongConnection, x => x.ReceiveNumber, x =>
            {
                received = true;
            });

            await Task.Delay(150);
            receiveTask.Status.Should().NotBe(TaskStatus.RanToCompletion);
            await Task.Delay(300);
            await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Numbers.ReceiveNumber), new object?[] { 1 });
            await Task.Delay(300);
            receiveTask.Status.Should().Be(TaskStatus.RanToCompletion);
            received.Should().BeTrue();

        }
    }
}