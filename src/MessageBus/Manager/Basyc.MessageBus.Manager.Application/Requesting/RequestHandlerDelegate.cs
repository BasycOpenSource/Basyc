using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Application.Requesting;
public delegate object? RequestHandlerDelegate(MessageRequest messageRequest, ILogger logger);
