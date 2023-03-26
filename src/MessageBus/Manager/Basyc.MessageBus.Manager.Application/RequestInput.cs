using Basyc.MessageBus.Manager.Application.Building;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Application;

public record RequestInput(MessageInfo MessageInfo, ReadOnlyCollection<Parameter> Parameters);
