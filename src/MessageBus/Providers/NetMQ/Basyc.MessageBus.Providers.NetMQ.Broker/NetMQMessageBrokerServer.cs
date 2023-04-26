﻿using System.Diagnostics.CodeAnalysis;
using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.NetMQ.Shared;
using Basyc.MessageBus.NetMQ.Shared.Cases;
using Basyc.MessageBus.Shared;
using Basyc.Serializaton.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;

namespace Basyc.MessageBus.Broker.NetMQ;

//https://netmq.readthedocs.io/en/latest/xpub-xsub/
[SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Lazy to fix")]
public class NetMqMessageBrokerServer : IMessageBrokerServer
{
    private readonly RouterSocket brokerSocket;
    private readonly ILogger<NetMqMessageBrokerServer> logger;
    private readonly INetMqMessageWrapper messageToByteSerializer;
    private readonly NetMQPoller poller = new();

    public NetMqMessageBrokerServer(IOptions<NetMqMessageBrokerServerOptions> options,
        IWorkerRegistry workerRegistry,
        ILogger<NetMqMessageBrokerServer> logger,
        INetMqMessageWrapper messageToByteSerializer,
        IDiagnosticsExporter diagnosticsProducer)
    {
        this.logger = logger;
        this.messageToByteSerializer = messageToByteSerializer;
        brokerSocket = new RouterSocket($"@tcp://{options.Value.BrokerServerAddress}:{options.Value.BrokerServerPort}");

        brokerSocket.ReceiveReady += (s, a) =>
        {
            var recievedMessageFrame = a.Socket.ReceiveMultipartMessage(3);
            //0 WorkerId 1 Empty 2 Data 3 Empty 4 ProducerId

            var senderAddressFrame = recievedMessageFrame[0];
            var senderAddressString = senderAddressFrame.ConvertToString();
            var deserializationResult = messageToByteSerializer.ReadWrapperMessage(recievedMessageFrame[2].Buffer);
            deserializationResult.Switch(
                checkIn =>
                {
                    var handledTypesNamesString = string.Join(',', checkIn.SupportedMessageTypes
                        .Select(assemblyTypeString => assemblyTypeString.Split(',')
                            .First()
                            .Split('.')
                            .Last()));
                    logger.LogInformation(
                        $"Received checkin from {senderAddressString}. WorkerId: {checkIn.WorkerId}, HandeledTypes: {handledTypesNamesString}");
                    workerRegistry.RegisterWorker(checkIn.WorkerId, checkIn.SupportedMessageTypes);
                },
                request =>
                {
                    using var requestStartActivity = DiagnosticHelper.Start("RequestCase handeling", request.TraceId, request.ParentSpanId);
                    logger.LogInformation($"Recieved request {request.RequestType} from producer {senderAddressString}:{request.SessionId}");
                    if (workerRegistry.TryGetWorkerFor(request.RequestType, out var workerAddressString))
                    {
                        var requestBytes = recievedMessageFrame[2].Buffer;
                        //var messageToProducer = new NetMQMessage();
                        //messageToProducer.Append(workerAddressString);
                        //messageToProducer.AppendEmptyFrame();
                        //messageToProducer.Append(senderAddressFrame);
                        //messageToProducer.AppendEmptyFrame();
                        //messageToProducer.Append(requestBytes);
                        var messageToProducer = CreateRequestNetMqMessage(workerAddressString, senderAddressFrame, requestBytes);
                        logger.LogInformation($"Sending request {request.RequestType} to {workerAddressString}:{request.SessionId}");
                        brokerSocket.SendMultipartMessage(messageToProducer);
                        logger.LogInformation($"Request sent to {workerAddressString}");
                        requestStartActivity.Stop();
                    }
                    else
                    {
                        var failure = new ErrorMessage("Worker for this request not found!");
                        //var messageToProducer = new NetMQMessage();
                        //messageToProducer.Append(senderAddressFrame);
                        //messageToProducer.AppendEmptyFrame();
                        //messageToProducer.AppendEmptyFrame();
                        //messageToProducer.AppendEmptyFrame();
                        //messageToProducer.Append(messageToByteSerializer.CreateWrapperMessage(failure, TypedToSimpleConverter.ConvertTypeToSimple(typeof(ErrorMessage)), request.SessionId, request.TraceId, request.ParentSpanId, MessageCase.Response));

                        var messageToProducer = CreateResponseNetMqMessage(senderAddressFrame,
                            failure,
                            request.SessionId,
                            request.TraceId,
                            request.ParentSpanId,
                            MessageCase.Response);

                        logger.LogError($"Sending failure: '{failure}' to {senderAddressString}");
                        brokerSocket.SendMultipartMessage(messageToProducer);
                        logger.LogError($"Failure sent to {senderAddressFrame}");
                        requestStartActivity.Stop();
                    }
                },
                response =>
                {
                    using var responseActivityStart = DiagnosticHelper.Start("ResponseCase handeling", response.TraceId, response.ParentSpanId);
                    logger.LogInformation($"Recieved response {response.ResponseType} from consumer {senderAddressString}:{response.SessionId}");
                    var producerAddress = recievedMessageFrame[4];
                    var producerAddressString = producerAddress.ConvertToString();
                    //var messageToConsumer = new NetMQMessage();
                    //messageToConsumer.Append(producerAddress);
                    //messageToConsumer.AppendEmptyFrame();
                    //messageToConsumer.Append(senderAddressFrame);
                    //messageToConsumer.AppendEmptyFrame();
                    //messageToConsumer.Append(recievedMessageFrame[2].Buffer);
                    var messageToConsumer = CreateRequestNetMqMessage(producerAddressString, senderAddressFrame, recievedMessageFrame[2].Buffer);

                    logger.LogInformation($"Sending response {response.ResponseType} to producer {producerAddressString}:{response.SessionId}");
                    brokerSocket.SendMultipartMessage(messageToConsumer);
                    logger.LogInformation($"Sent response {response.ResponseType} to producer {producerAddressString}:{response.SessionId}");
                    responseActivityStart.Stop();
                },
                @event =>
                {
                    using var eventStartActivity = DiagnosticHelper.Start("EventCase handeling", @event.TraceId, @event.ParentSpanId);

                    logger.LogInformation($"Received event {@event.EventType} from producer {senderAddressString}:{@event.SessionId}");

                    if (workerRegistry.TryGetWorkersFor(@event.EventType, out var workers))
                    {
                        foreach (var worker in workers)
                        {
                            var eventData = recievedMessageFrame[2].Buffer;
                            //var messageToProducer = new NetMQMessage();
                            //messageToProducer.Append(worker);
                            //messageToProducer.AppendEmptyFrame();
                            //messageToProducer.Append(senderAddressFrame);
                            //messageToProducer.AppendEmptyFrame();
                            //messageToProducer.Append(eventData);
                            var messageToProducer = CreateRequestNetMqMessage(worker, senderAddressFrame, eventData);
                            logger.LogInformation($"Sending event to consumer {worker}:{@event.SessionId}");
                            brokerSocket.SendMultipartMessage(messageToProducer);
                            logger.LogInformation($"Sent event {@event.EventType} to consumer {worker}:{@event.SessionId}");
                        }
                    }
                    else
                    {
                        logger.LogInformation($"No worker for {@event.EventType} checked in");
                    }

                    eventStartActivity.Stop();
                },
                failure =>
                {
                    logger.LogError($"Serialization failed, details: '{failure}'");
                    var sendFailToAddress = failure.MessageCase is MessageCase.Response ? recievedMessageFrame[4] : senderAddressFrame;
                    var sendFailToAddressString = sendFailToAddress.ConvertToString();
                    var failResult = new ErrorMessage(failure.ErrorMessage);
                    //var messageToProducer = new NetMQMessage();
                    //messageToProducer.Append(sendFailToAddress);
                    //messageToProducer.AppendEmptyFrame();
                    //messageToProducer.AppendEmptyFrame();
                    //messageToProducer.AppendEmptyFrame();
                    //messageToProducer.Append(messageToByteSerializer.CreateWrapperMessage(failResult, TypedToSimpleConverter.ConvertTypeToSimple(typeof(ErrorMessage)), failure.SessionId, failure.TraceId, failure.ParentSpanId, MessageCase.Response));
                    var messageToProducer = CreateResponseNetMqMessage(sendFailToAddress,
                        failResult,
                        failure.SessionId,
                        failure.TraceId,
                        failure.ParentSpanId,
                        MessageCase.Response);
                    logger.LogInformation($"Sending failure: '{failure}' to {sendFailToAddressString}");
                    brokerSocket.SendMultipartMessage(messageToProducer);
                    logger.LogInformation($"Failure sent to {sendFailToAddressString}");
                });
        };

        poller.Add(brokerSocket);
    }

    public void Start()
    {
        try
        {
            logger.LogDebug("Starting poller");
            poller.RunAsync();
            logger.LogDebug("poller started");
        }
        catch (Exception ex)
        {
            logger.LogError("NetMQ proxy stopped. Reason: {Reason}", ex.Message);
            throw;
        }
    }

    public Task StartAsync(CancellationToken cancellationToken = default) => Task.Run(Start, cancellationToken);

    public void Dispose()
    {
        brokerSocket.Dispose();
        logger.LogInformation("NetMQ proxy disposed");
    }

    private static NetMQMessage CreateRequestNetMqMessage(string addressToSend, NetMQFrame requestProducerAddress, byte[] byteData)
    {
        var mqMessage = new NetMQMessage();
        mqMessage.Append(addressToSend);
        mqMessage.AppendEmptyFrame();
        mqMessage.Append(requestProducerAddress);
        mqMessage.AppendEmptyFrame();
        mqMessage.Append(byteData);
        return mqMessage;
    }

    private NetMQMessage CreateResponseNetMqMessage<TData>(NetMQFrame addressToSend,
        TData data,
        int sessionId,
        string traceId,
        string parentSpanId,
        MessageCase messageCase)
    {
        var mqMessage = new NetMQMessage();
        mqMessage.Append(addressToSend);
        mqMessage.AppendEmptyFrame();
        mqMessage.AppendEmptyFrame();
        mqMessage.AppendEmptyFrame();
        mqMessage.Append(messageToByteSerializer.CreateWrapperMessage(data,
            TypedToSimpleConverter.ConvertTypeToSimple(typeof(TData)),
            sessionId,
            traceId,
            parentSpanId,
            messageCase));
        return mqMessage;
    }
}
