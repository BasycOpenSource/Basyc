﻿using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
using Basyc.ReactiveUi;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.SideBar;
public class AppSidebarMessageItemViewModel : BasycReactiveViewModelBase
{
    private readonly NavigationService navigationService;

    public AppSidebarMessageItemViewModel(NavigationService navigationService)
    {
        this.navigationService = navigationService;

        IsSelected = this.ReactiveProperty(
            x => x.IsSelected,
            x => x.navigationService.CurrentQueryParams,
            x => x.MessageInfo,
            x =>
            {
                var isSelected = x.Item1 is MessageInfo messageInfo && messageInfo == MessageInfo;
                return isSelected;
            });
    }

    [Reactive] public MessageInfo? MessageInfo { get; set; }

    [Reactive] public bool IsSelected { get; private set; }
}
