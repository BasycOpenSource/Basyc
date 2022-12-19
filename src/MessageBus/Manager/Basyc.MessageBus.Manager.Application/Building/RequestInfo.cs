using Basyc.MessageBus.Manager.Application.Requesting;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Basyc.MessageBus.Manager.Application.Initialization
{
    public class RequestInfo
    {
        public RequestInfo(RequestType requestType, IEnumerable<ParameterInfo> parameters, Type responseType, string requestDisplayName, string responseDisplayName)
            : this(requestType, parameters, requestDisplayName, true, responseType)
        {
            RequestType = requestType;
            ResponseDisplayName = responseDisplayName;
        }

        public RequestInfo(RequestType requestType, IEnumerable<ParameterInfo> parameters, string requestDisplayName)
            : this(requestType, parameters, requestDisplayName, false, null)
        {
        }

        private RequestInfo(RequestType requestType, IEnumerable<ParameterInfo> parameters, string requestDisplayName, bool hasResponse, Type? responseType)
        {
            RequestType = requestType;
            Parameters = parameters.ToList();
            RequestDisplayName = requestDisplayName;
            HasResponse = hasResponse;
            ResponseType = responseType;

        }

        public string RequestDisplayName { get; init; }
        public RequestType RequestType { get; init; }
        public IReadOnlyList<ParameterInfo> Parameters { get; init; }
        public bool HasResponse { get; init; }
        public Type? ResponseType { get; init; }
        public string ResponseDisplayName { get; init; } = string.Empty;
        /// <summary>
        /// Custom metadata that can be created in custom <see cref="IDomainInfoProvider"/> and later be used in custom <see cref="IRequester."/>
        /// </summary>
        public Dictionary<string, object> AdditionalMetadata { get; } = new();
    }
}