using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Helpers;
using Basyc.MessageBus.Client.NetMQ.Sessions;
using Basyc.MessageBus.NetMQ.Shared;
using Basyc.MessageBus.NetMQ.Shared.Cases;
using Basyc.MessageBus.Shared;
using Basyc.Serialization.Abstraction;
using Basyc.Serializaton.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using OneOf;
using System.Diagnostics;
using System.Text;

namespace Basyc.MessageBus.Client.NetMQ;

//https://zguide.zeromq.org/docs/chapter3/#A-Load-Balancing-Message-Broker
public partial class NetMQByteMessageBusClient : IByteMessageBusClient
{
	private readonly IOptions<NetMQMessageBusClientOptions> options;
	private readonly IMessageHandlerManager handlerManager;
	private readonly ILogger<NetMQByteMessageBusClient> logger;
	private readonly ISessionManager<NetMQSessionResult> sessionManager;
	private readonly INetMQMessageWrapper netMQMessageWrapper;
	private readonly IObjectToByteSerailizer objectToByteSerailizer;
	private readonly NetMQPoller poller = new();
	private readonly DealerSocket dealerSocket;

	public NetMQByteMessageBusClient(
		IOptions<NetMQMessageBusClientOptions> options,
		IMessageHandlerManager handlerManager,
		ILogger<NetMQByteMessageBusClient> logger,
		ISessionManager<NetMQSessionResult> sessionManager,
		INetMQMessageWrapper netMQByteSerializer,
		IObjectToByteSerailizer objectToByteSerailizer)
	{
		this.options = options;
		this.handlerManager = handlerManager;
		this.logger = logger;
		this.sessionManager = sessionManager;
		this.netMQMessageWrapper = netMQByteSerializer;
		this.objectToByteSerailizer = objectToByteSerailizer;
		dealerSocket = CreateSocket(options);
		poller.Add(dealerSocket);
	}

	private DealerSocket CreateSocket(IOptions<NetMQMessageBusClientOptions> options)
	{
		var dealerSocket = new DealerSocket($"tcp://{options.Value.BrokerServerAddress}:{options.Value.BrokerServerPort}");

		if (options.Value.WorkerId is null)
			options.Value.WorkerId = "Client-" + Guid.NewGuid();

		dealerSocket.Options.Identity = Encoding.ASCII.GetBytes(options.Value.WorkerId);
		dealerSocket.ReceiveReady += async (s, a) =>
		{
			var messageFrames = dealerSocket.ReceiveMultipartMessage(3);
			await Task.Run(async () =>
			{
				await HandleMessage(messageFrames, CancellationToken.None);
			});
		};

		return dealerSocket;
	}

	public Task StartAsync(CancellationToken cancellationToken = default)
	{
		poller.RunAsync();
		CheckInMessage checkIn = new CheckInMessage(options.Value.WorkerId!, handlerManager.GetConsumableMessageTypes());
		byte[] messageWrapperBytes = netMQMessageWrapper.CreateWrapperMessage(checkIn, TypedToSimpleConverter.ConvertTypeToSimple(typeof(CheckInMessage)), default, "checkInShouldNotHaveTraceId", "noParent", MessageCase.CheckIn);
		var messageToServer = new NetMQMessage();
		messageToServer.AppendEmptyFrame();
		messageToServer.Append(messageWrapperBytes);
		logger.LogInformation($"Sending CheckIn message");
		dealerSocket.SendMultipartMessage(messageToServer);
		logger.LogInformation($"CheckIn message sent");
		return Task.CompletedTask;
	}

	private async Task HandleMessage(NetMQMessage messageFrames, CancellationToken cancellationToken)
	{
		var senderAddressBytes = messageFrames[1].Buffer;
		var senderAddressString = Encoding.ASCII.GetString(senderAddressBytes);
		byte[] messageDataBytes = messageFrames[3].Buffer;
		var wrapperReadResult = netMQMessageWrapper.ReadWrapperMessage(messageDataBytes);
		await wrapperReadResult.Match(
			checkIn =>
			{
				logger.LogError("Client is not supposed to receive CheckIn messages");
				return Task.CompletedTask;
			},
			async requestCase =>
			{
				using (var requestCaseActivity = DiagnosticHelper.Start("NetMQ RequestCase", requestCase.TraceId, requestCase.ParentSpanId))
				{
					logger.LogDebug($"Request received from {senderAddressString}:{requestCase.SessionId}");

					using var deseriActivity = DiagnosticHelper.Start("Deserializating request");
					var deserializedRequest = objectToByteSerailizer.Deserialize(requestCase.RequestBytes, requestCase.RequestType);
					deseriActivity.Stop();

					using var consumeActivity = DiagnosticHelper.Start("Consume message");
					var consumeResult = await handlerManager.ConsumeMessage(requestCase.RequestType, deserializedRequest, cancellationToken, requestCase.TraceId, consumeActivity.Activity?.SpanId.ToString());
					consumeActivity.Stop();

					var connsumerResultData = consumeResult.Value;
					if (consumeResult.Value is Exception ex)
						logger.LogCritical($"Message handler throwed exception. {ex.Message}");

					using var seriActivity = DiagnosticHelper.Start("Serializating repsonse");
					string responseType = TypedToSimpleConverter.ConvertTypeToSimple(connsumerResultData.GetType());
					byte[] wrapperMessageBytes = netMQMessageWrapper.CreateWrapperMessage(connsumerResultData, responseType, requestCase.SessionId, requestCase.TraceId, requestCase.ParentSpanId, MessageCase.Response);
					seriActivity.Stop();

					var messageToBroker = new NetMQMessage();
					messageToBroker.AppendEmptyFrame();
					messageToBroker.Append(wrapperMessageBytes);
					messageToBroker.AppendEmptyFrame();
					messageToBroker.Append(senderAddressBytes);

					logger.LogInformation($"Sending response message");
					using (DiagnosticHelper.Start("Sending response to broker"))
					{
						dealerSocket.SendMultipartMessage(messageToBroker);
					}
				}

				logger.LogInformation($"Response message sent");
			},
			responseCase =>
			{
				using (var requestCaseActivity = DiagnosticHelper.Start("NetMQ ResponseCase", responseCase.TraceId))
				{
					logger.LogInformation($"Response received from {senderAddressString}:{responseCase.SessionId}");
					if (sessionManager.TryCompleteSession(responseCase.SessionId, new NetMQSessionResult(responseCase.ResponseBytes, responseCase.ResponseType)) is false)
						logger.LogError($"Session '{responseCase.SessionId}' completation failed. Session does not exist");

					return Task.CompletedTask;
				}
			},
			async eventCase =>
			{
				using (var requestCaseActivity = DiagnosticHelper.Start("NetMQ EventCase", eventCase.TraceId))
				{
					logger.LogInformation($"Event received from {senderAddressString}:{eventCase.SessionId}");
					var eventRequest = objectToByteSerailizer.Deserialize(eventCase.EventBytes, eventCase.EventType);
					var responseData = await handlerManager.ConsumeMessage(eventCase.EventType, eventRequest, cancellationToken, eventCase.TraceId, requestCaseActivity.Activity?.SpanId.ToString());
				}
			},
			failureCase =>
			{
				logger.LogError($"Failure received from {senderAddressString}:{failureCase.SessionId}");
				switch (failureCase.MessageCase)
				{
					case MessageCase.Response:
						var sessionResult = CreateErrorMessageBytes(failureCase.ErrorMessage);
						if (sessionManager.TryCompleteSession(failureCase.SessionId, sessionResult) is false)
							logger.LogCritical($"Session '{failureCase.SessionId}' failed. Session does not exist");
						break;
					default:
						throw new NotImplementedException();
				}

				return Task.CompletedTask;

			});

	}

	private BusTask PublishAsync(byte[]? eventBytes, string eventType, RequestContext requestContext, CancellationToken cancellationToken)
	{
		//string traceId = requestContext.TraceId is null ? IdGeneratorHelper.GenerateNewSpanId() : requestContext.TraceId;
		string traceId;
		if (requestContext.TraceId is not null)
		{
			traceId = requestContext.TraceId;
		}
		else
		{
			if (Activity.Current is not null)
			{
				traceId = Activity.Current.TraceId.ToString();
			}
			else
			{
				traceId = IdGeneratorHelper.GenerateNewTraceId();
			}
		}

		string requesterSpanId;
		if (requestContext.ParentSpanId is not null)
		{
			requesterSpanId = requestContext.ParentSpanId;
		}
		else
		{
			if (Activity.Current is not null)
			{
				requesterSpanId = Activity.Current.SpanId.ToString();
			}
			else
			{
				requesterSpanId = IdGeneratorHelper.GenerateNewSpanId();
			}
		}
		//var publishActivity = DiagnosticHelper.Start("NetMQByteMessageBusClient.PublishAsync", traceId, requesterSpanId);

		var newSession = sessionManager.CreateSession(eventType, traceId);

		var task = Task.Run(() =>
		{
			eventBytes ??= new byte[0];
			cancellationToken.ThrowIfCancellationRequested();
			var netMQByteMessage = netMQMessageWrapper.CreateWrapperMessage(eventBytes, eventType, newSession.SessionId, traceId, requesterSpanId, MessageCase.Event);
			var messageToBroker = new NetMQMessage();
			messageToBroker.AppendEmptyFrame();
			messageToBroker.Append(netMQByteMessage);

			cancellationToken.ThrowIfCancellationRequested();

			logger.LogInformation($"Publishing '{FormatType(eventType)}' session: '{newSession.SessionId}'");
			try
			{
				dealerSocket.SendMultipartMessage(messageToBroker);
			}
			catch (Exception ex)
			{
				logger.LogCritical(ex, "Failed to send request");
				var sessionResult = CreateErrorMessageBytes("Failed to publish");
				sessionManager.TryCompleteSession(newSession.SessionId, sessionResult);
			}

			logger.LogInformation($"Published '{FormatType(eventType)}'");
			var publishResult = "Published";
			var publisResultType = TypedToSimpleConverter.ConvertTypeToSimple(typeof(string));
			var publishResultBytes = netMQMessageWrapper.CreateWrapperMessage(publishResult, publisResultType, 0, traceId, requesterSpanId, MessageCase.Response);
			sessionManager.TryCompleteSession(newSession.SessionId, new NetMQSessionResult(publishResultBytes, publisResultType));
			//publishActivity.Stop();

		});
		return BusTask.FromTask(newSession.TraceId, task);
	}

	private NetMQSessionResult CreateErrorMessageBytes(string errorMessage)
	{
		var errorResult = new ErrorMessage(errorMessage);
		var errorResultType = TypedToSimpleConverter.ConvertTypeToSimple(typeof(ErrorMessage));
		var errorResultBytes = netMQMessageWrapper.CreateWrapperMessage(errorResult, errorResultType, 0, "noTraceID", "noParentId", MessageCase.Response);
		return new NetMQSessionResult(errorResultBytes, errorResultType);
	}

	private BusTask SendAsync(byte[]? commnadData, string commandType, RequestContext context, CancellationToken cancellationToken)
	{
		return RequestAsync(commnadData, commandType, context, cancellationToken).ToBusTask();
	}

	private BusTask<ByteResponse> RequestAsync(byte[]? requestBytes, string requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		string traceId = requestContext.TraceId is null ? IdGeneratorHelper.GenerateNewSpanId() : requestContext.TraceId;
		string requesterSpanId = requestContext.ParentSpanId is null ? traceId : requestContext.ParentSpanId;
		var requestActivity = DiagnosticHelper.Start("NetMQByteMessageBusClient.RequestAsync", traceId, requesterSpanId);
		var newSession = sessionManager.CreateSession(requestType, requestContext.TraceId);
		Task<OneOf<ByteResponse, ErrorMessage>> task = Task.Run<OneOf<ByteResponse, ErrorMessage>>(async () =>
		{
			requestBytes ??= new byte[0];
			cancellationToken.ThrowIfCancellationRequested();

			var seriActivity = DiagnosticHelper.Start("NetMQByteMessageBusClient.RequestAsync serialization");
			var netMQByteMessage = netMQMessageWrapper.CreateWrapperMessage(requestBytes, requestType, newSession.SessionId, traceId, requesterSpanId, MessageCase.Request);
			seriActivity.Stop();

			var messageToBroker = new NetMQMessage();
			messageToBroker.AppendEmptyFrame();
			messageToBroker.Append(netMQByteMessage);

			cancellationToken.ThrowIfCancellationRequested();

			logger.LogInformation($"Requesting '{FormatType(requestType)}'");
			try
			{
				using (var sendingActivity = DiagnosticHelper.Start("NetMQ.SendMultipartMessage"))
				{
					dealerSocket.SendMultipartMessage(messageToBroker);
				}
			}
			catch (Exception ex)
			{
				logger.LogCritical(ex, "Failed to send request");
				var sessionResultError = CreateErrorMessageBytes("Failed to send request");
				sessionManager.TryCompleteSession(newSession.SessionId, sessionResultError);
			}

			logger.LogInformation($"Requested '{FormatType(requestType)}'");
			var sessionResult = await newSession.ResponseSource.Task;
			requestActivity.Stop();
			return new ByteResponse(sessionResult.bytes, sessionResult.responseType);
		});

		return BusTask<ByteResponse>.FromTask(newSession.TraceId, task);

	}

	BusTask IByteMessageBusClient.PublishAsync(string eventType, RequestContext context, CancellationToken cancellationToken)
	{
		return PublishAsync(null, eventType, context, cancellationToken);
	}

	BusTask IByteMessageBusClient.PublishAsync(string eventType, byte[] eventBytes, RequestContext context, CancellationToken cancellationToken)
	{
		return PublishAsync(eventBytes, eventType, context, cancellationToken);
	}

	BusTask IByteMessageBusClient.SendAsync(string commandType, RequestContext context, CancellationToken cancellationToken)
	{
		return SendAsync(null, commandType, context, cancellationToken);
	}

	BusTask IByteMessageBusClient.SendAsync(string commandType, byte[] commandData, RequestContext context, CancellationToken cancellationToken)
	{
		return SendAsync(commandData, commandType, context, cancellationToken);
	}

	BusTask<ByteResponse> IByteMessageBusClient.RequestAsync(string requestType, RequestContext context, CancellationToken cancellationToken)
	{
		return RequestAsync(null, requestType, context, cancellationToken);
	}

	BusTask<ByteResponse> IByteMessageBusClient.RequestAsync(string requestType, byte[] requestData, RequestContext context, CancellationToken cancellationToken)
	{
		var reqeustTask = RequestAsync(requestData, requestType, context, cancellationToken);
		return reqeustTask;

	}
	private static readonly string tokenString = "PublicKeyToken=null";
	private static readonly int tokenLenght = "PublicKeyToken=nul".Length;
	private static string FormatType(string messageType)
	{
		if (messageType.AsSpan(messageType.Length - tokenLenght) == (tokenString))
		{
			//string[] splits = messageType.Split(',');
			//var type = splits[0];
			//return type;

			var firstCommaIndex = messageType.IndexOf(',');
			var clrType = messageType.AsSpan(0, firstCommaIndex);
			var lastDotIndex = clrType.LastIndexOf('.');
			var typeName = clrType.Slice(lastDotIndex);
			return typeName.ToString();

		}

		return messageType;
	}
	public void Dispose()
	{
		dealerSocket.Dispose();
		poller.Dispose();
	}
}