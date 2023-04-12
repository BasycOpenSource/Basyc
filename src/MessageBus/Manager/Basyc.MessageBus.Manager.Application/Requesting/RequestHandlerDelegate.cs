using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Application.Requesting;
public delegate Task<object?> RequestHandlerDelegate(MessageRequest messageRequest, ILogger logger);
