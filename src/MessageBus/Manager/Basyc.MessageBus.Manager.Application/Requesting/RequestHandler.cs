using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Application.Requesting;
public delegate Task<object?> RequestHandler(MessageRequest messageRequest, ILogger logger);
