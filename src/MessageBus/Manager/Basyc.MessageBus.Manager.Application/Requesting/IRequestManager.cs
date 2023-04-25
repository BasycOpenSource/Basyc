using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public interface IRequestManager
{
    ReadOnlyObservableCollection<MessageContext> MessageContexts { get; }

    MessageRequest StartRequest(RequestInput request);
}
