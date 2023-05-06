using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.ReactiveUi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message.Sidebar;

public class SidebarHistoryViewModel : BasycReactiveViewModelBase
{
    private readonly IRequestManager requestManager;

    public SidebarHistoryViewModel(IRequestManager requestManager)
    {
        this.requestManager = requestManager;

        //TODO change, might create multiple active handlers
        this.ReactiveHandler(x => x.MessageInfo!,
                x =>
               {
                   MessageContext = this.ReactiveAggregatorProperty(
                   x => x.MessageContext,
                   x => x.requestManager.MessageContexts,
                   x => x.FirstOrDefault(x => x.MessageInfo == MessageInfo));
               });

        History = this.ReactiveCollectionProperty(
               x => History!,
               x => x.MessageContext!.MessageRequests,
               x => x);
        History = new(new());

        HistoryCount = this.ReactiveProperty(
              x => x.HistoryCount,
              x => x.History.Count);

        //Fixing ReactiveUI not propagating nulls
        this.ReactiveHandler(x => x.MessageContext!,
                x =>
                {
                    if (x is null)
                    {
                        MessageContext = new MessageContext(null!);
                    }
                });
    }

    [Reactive] public MessageInfo? MessageInfo { get; set; }

    [Reactive] public int HistoryCount { get; init; }

    [Reactive] public ReadOnlyObservableCollection<MessageRequest> History { get; private set; }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    [Reactive] private MessageContext? MessageContext { get; set; }
}
