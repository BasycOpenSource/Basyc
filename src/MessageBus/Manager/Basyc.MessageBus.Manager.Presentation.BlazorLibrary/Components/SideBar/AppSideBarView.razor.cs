using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.SideBar;

public partial class AppSideBarView
{
    private ReadOnlyObservableCollection<MessageGroup> messageGroups = null!;

    [Inject] private IMessagesInfoProvidersAggregator DomainInfoInfoProvidersAggregatorManager { get; set; } = null!;

    [Inject] private INavigationService NavigationService { get; init; } = null!;

    protected override void OnInitialized()
    {
        messageGroups = new ReadOnlyObservableCollection<MessageGroup>(new ObservableCollection<MessageGroup>(DomainInfoInfoProvidersAggregatorManager.GetMessageGroups()));
        NavigationService.GoTo<MessagePageView, MessagePageViewModel, MessageInfo>(messageGroups.First().Messages.First());
        base.OnInitialized();
    }
}
