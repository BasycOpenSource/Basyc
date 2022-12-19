using Basyc.MessageBus.Client;
using Basyc.MessageBus.HttpProxy.Shared.Http;
using Basyc.MessageBus.Shared;
using Basyc.Serialization.Abstraction;
using Basyc.Serializaton.Abstraction;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Wrap;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.MessageBus.HttpProxy.Client.Http
{
	public class HttpProxyObjectMessageBusClient : IObjectMessageBusClient
	{
		private static readonly string proxyResponseSimpleDataType = TypedToSimpleConverter.ConvertTypeToSimple(typeof(ResponseHttpDTO));
		private readonly AsyncPolicyWrap retryPolicy;
		private readonly HttpClient httpClient;
		private readonly IOptions<HttpProxyObjectMessageBusClientOptions> options;
		private readonly IObjectToByteSerailizer objectToByteSerializer;
		private readonly string wrapperMessageType = TypedToSimpleConverter.ConvertTypeToSimple(typeof(RequestHttpDTO));


		public HttpProxyObjectMessageBusClient(IOptions<HttpProxyObjectMessageBusClientOptions> options,
			IObjectToByteSerailizer byteSerializer)
		{
			retryPolicy = Policy.Handle<Exception>().RetryAsync(0).WrapAsync(Policy.TimeoutAsync(10, Polly.Timeout.TimeoutStrategy.Pessimistic));
			httpClient = new HttpClient() { BaseAddress = options.Value.ProxyHostUri };
			this.options = options;
			objectToByteSerializer = byteSerializer;
		}

		private BusTask<ProxyResponse> HttpCallToProxyServer(string messageType, object? messageData, Type? responseType = null, CancellationToken cancellationToken = default)
		{
			if (objectToByteSerializer.TrySerialize(messageData, messageType, out var requestBytes, out var seriError) is false)
			{
				return BusTask<ProxyResponse>.FromValue("-1", new ProxyResponse(seriError, true, true, null));
			}

			var responseTypeString = responseType?.AssemblyQualifiedName;
			var hasResponse = responseType != null;
			var proxyRequest = new RequestHttpDTO(messageType, hasResponse, requestBytes, responseTypeString);

			if (objectToByteSerializer.TrySerialize(proxyRequest, wrapperMessageType, out var proxyRequestBytes, out var error) is false)
			{
				return BusTask<ProxyResponse>.FromValue("-1", new ProxyResponse(error, true, true, null));

			}

			var httpContent = new ByteArrayContent(proxyRequestBytes);
			//var httpResult = retryPolicy.ExecuteAsync(async () => await httpClient.PostAsync("", httpContent)).GetAwaiter().GetResult();
			var httpResultTask = retryPolicy.ExecuteAsync(async () => await httpClient.PostAsync("", httpContent));
			httpResultTask.Wait();
			var httpResult = httpResultTask.Result;

			if (httpResult.IsSuccessStatusCode is false)
			{
				var httpErrorContent = httpResult.Content.ReadAsStringAsync().GetAwaiter().GetResult();
				var errorMessageText = $"Message bus response failure, code: {(int)httpResult.StatusCode},\nreason: {httpResult.ReasonPhrase},\ncontent: {httpErrorContent}";
				//throw new Exception($"Message bus response failure, code: {(int)httpResult.StatusCode},\nreason: {httpResult.ReasonPhrase},\ncontent: {httpErrorContent}");
				return BusTask<ProxyResponse>.FromValue("-1", new ProxyResponse(new ErrorMessage(errorMessageText), true, true, null));
			}



			cancellationToken.ThrowIfCancellationRequested();

			using MemoryStream httpMemomoryStream = new MemoryStream();
			httpResult.Content.CopyToAsync(httpMemomoryStream).GetAwaiter().GetResult();
			var proxyResponseResponseBytes = httpMemomoryStream.ToArray();

			cancellationToken.ThrowIfCancellationRequested();

			var proxyResponse = (ResponseHttpDTO)objectToByteSerializer.Deserialize(proxyResponseResponseBytes, proxyResponseSimpleDataType);
			if (hasResponse is false)
				return BusTask<ProxyResponse>.FromValue(proxyResponse.TraceId, new ProxyResponse(null, false, false, proxyResponse.TraceId));

			var deserializedResponse = objectToByteSerializer.Deserialize(proxyResponse.ResponseBytes, proxyResponse.ResponseType);
			return BusTask<ProxyResponse>.FromValue(proxyResponse.TraceId, new ProxyResponse(deserializedResponse, true, false, proxyResponse.TraceId));
		}

		public void Dispose()
		{

		}

		public BusTask PublishAsync(string eventType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			return HttpCallToProxyServer(eventType, null, null, cancellationToken).ToBusTask();
		}

		public BusTask PublishAsync(string eventType, object eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			return HttpCallToProxyServer(eventType, eventData, null, cancellationToken).ToBusTask();
		}

		public BusTask SendAsync(string commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			return HttpCallToProxyServer(commandType, null, null, cancellationToken).ToBusTask();
		}

		public BusTask SendAsync(string commandType, object commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			return HttpCallToProxyServer(commandType, commandData, null, cancellationToken).ToBusTask();
		}

		public BusTask<object> RequestAsync(string requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			var innerBusTask = HttpCallToProxyServer(requestType, null, typeof(UknownResponseType), cancellationToken);
			return innerBusTask.ContinueWith<object>(x => (object)x);
		}

		public BusTask<object> RequestAsync(string requestType, object requestData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
		{
			var innerBusTask = HttpCallToProxyServer(requestType, requestData, typeof(UknownResponseType), cancellationToken);
			return innerBusTask.ContinueWith<object>(x => (object)x);
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		void IDisposable.Dispose()
		{

		}

		private class UknownResponseType { };
	}
}