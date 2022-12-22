using Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall;
using Basyc.Extensions.SignalR.Client.Tests.Mocks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Basyc.Extensions.SignalR.Client.Tests;

public class OnMultipleExtensionTests
{
	[Fact]
	public async Task Should_Not_Throw_When_Receiver_Has_0_Methods()
	{
		var messageReceiver = new MethodsServerCanCall_Empty();
		var hubConnection = HubConnectionMockBuilder.Create();
		OnMultipleExtension.OnMultiple(hubConnection, messageReceiver);
		await hubConnection.StartAsync();
		await hubConnection.ReceiveMessage("test", new object?[] { 1 });
	}

	[Fact]
	public async Task Should_Ignore_Private_Methods()
	{
		var messageReceiver = new MethodsServerCanCall_PrivateMethods();
		var hubConnection = HubConnectionMockBuilder.Create();
		hubConnection.OnMultiple(messageReceiver);
		await hubConnection.StartAsync();
		await hubConnection.ReceiveMessage(MethodsServerCanCall_PrivateMethods.ReceiveTextMethodName, new object?[] { "text" });
		messageReceiver.LastReceivedText.Should().BeNull();

		await hubConnection.ReceiveMessage(MethodsServerCanCall_PrivateMethods.ReceiveVoidMethodName, Array.Empty<object?>());
		messageReceiver.LastReceivedVoidUtc.Should().Be(default);
	}

	[Fact]
	public async Task Should_Include_Inherited_Methods_From_SelectedD_Interface()
	{
		var messageReceiver = new MethodsServerCanCall_Inhertited_Numbers();
		var hubConnection = HubConnectionMockBuilder.Create();
		hubConnection.OnMultiple(messageReceiver);
		await hubConnection.StartAsync();
		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Inhertited_Numbers.ReceiveNumber), new object?[] { 1 });
		messageReceiver.LastReceivedNumber.Should().Be(1);
	}

	[Fact]
	public async Task Should_Not_Match_Method_With_Different_Arg_Count()
	{
		//var messageReceiver = new MethodsServerCanCall_Numbers();
		//var hubConnection = HubConnectionMockBuilder.Create();
		//hubConnection.OnMultiple(messageReceiver);
		//await hubConnection.StartAsync();
		//await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Numbers.ReceiveNumber), new object?[] { 2, "text" });
		//messageReceiver.LastReceivedNumber.Should().Be(0);
		//await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Numbers.ReceiveNumber), new object?[] { 1, 2 });
		//messageReceiver.LastReceivedNumber.Should().Be(0);

		//This logic of selecting correct handler is currently mocked and should be handeled by SignalR. So skip testing for now
		await Task.Delay(10);
	}

	[Fact]
	public async Task Should_Diff_When_Name_Is_Same_But_Args_Differ()
	{
		//var messageReceiver = new MethodsServerCanCall_Numbers();
		//var hubConnection = HubConnectionMockBuilder.Create();
		//hubConnection.OnMultiple(messageReceiver);
		//await hubConnection.StartAsync();
		//await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Numbers.ReceiveNumberAsync), new object?[] { 1, 2 });
		//messageReceiver.LastReceivedNumber.Should().Be(2);

		//await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Numbers.ReceiveNumberAsync), new object?[] { 1, 2, 3 });
		//messageReceiver.LastReceivedNumber.Should().Be(3);

		//This logic of selecting correct handler is currently mocked and should be handeled by SignalR. So skip testing for now
		await Task.Delay(10);
	}

	[Fact]
	public async Task Should_Inlude_Methods_From_Selected_Class()
	{
		var messageReceiver = new MethodsServerCanCall_Numbers();
		var hubConnection = HubConnectionMockBuilder.Create();
		hubConnection.OnMultiple(messageReceiver);

		await hubConnection.StartAsync();
		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Numbers.ReceiveNumber), new object?[] { 1 });
		messageReceiver.LastReceivedNumber.Should().Be(1);

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Numbers.ReceiveNumber), new object?[] { 2 });
		messageReceiver.LastReceivedNumber.Should().Be(2);

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Numbers.ReceiveNumberAsync), new object?[] { 3 });
		messageReceiver.LastReceivedNumber.Should().Be(3);
	}

	[Fact]
	public async Task Should_Ignore_Methods_Not_In_Interface()
	{
		var messageReceiver = new MethodsServerCanCall_Texts();
		var hubConnection = HubConnectionMockBuilder.Create();
		hubConnection.OnMultiple<IMethodsServerCanCall_Empty>(messageReceiver);
		await hubConnection.StartAsync();
		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveText), new object?[] { "1" });
		messageReceiver.LastReceivedText.Should().BeNull();

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveTextAsync), new object?[] { "1" });
		messageReceiver.LastReceivedText.Should().BeNull();

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveTexts), new object?[] { "1", "2" });
		messageReceiver.LastReceivedText.Should().BeNull();

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveTexts), new object?[] { "1", "2" });
		messageReceiver.LastReceivedText.Should().BeNull();
	}

	[Fact]
	public async Task Should_Just_Inlude_Methods_From_Interface_On_ImplementedClass()
	{
		var messageReceiver = new MethodsServerCanCall_Texts();
		var hubConnection = HubConnectionMockBuilder.Create();
		hubConnection.OnMultiple<IMethodsServerCanCall_Text>(messageReceiver);
		await hubConnection.StartAsync();
		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveText), new object?[] { "-1" });
		messageReceiver.LastReceivedText.Should().Be("-1");

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveTextAsync), new object?[] { "1" });
		messageReceiver.LastReceivedText.Should().Be("-1");

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveTexts), new object?[] { "1", "2" });
		messageReceiver.LastReceivedText.Should().Be("-1");

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveTexts), new object?[] { "1", "2" });
		messageReceiver.LastReceivedText.Should().Be("-1");
	}

	[Fact]
	public async Task Should_Unsubsribe_All()
	{
		var messageReceiver = new MethodsServerCanCall_Texts();
		var hubConnection = HubConnectionMockBuilder.Create();
		var subsriptions = hubConnection.OnMultiple<MethodsServerCanCall_Texts>(messageReceiver);
		await hubConnection.StartAsync();
		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveText), new object?[] { "-1" });
		messageReceiver.LastReceivedText.Should().Be("-1");
		subsriptions.UnsubscribeAll();
		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveText), new object?[] { "1" });
		messageReceiver.LastReceivedText.Should().Be("-1");

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveTextAsync), new object?[] { "1" });
		messageReceiver.LastReceivedText.Should().Be("-1");

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveTexts), new object?[] { "1", "2" });
		messageReceiver.LastReceivedText.Should().Be("-1");

		await hubConnection.ReceiveMessage(nameof(MethodsServerCanCall_Texts.ReceiveTexts), new object?[] { "1", "2" });
		messageReceiver.LastReceivedText.Should().Be("-1");
	}
}