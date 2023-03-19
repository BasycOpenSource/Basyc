﻿using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.ReactiveUi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message.Sidebar;

public class SidebarHistoryViewModel : BasycReactiveViewModelBase
{
	[Reactive] public MessageInfo? MessageInfo { get; set; }
	[Reactive] private MessageContext? messageContext { get; set; }
	[Reactive] public int HistoryCount { get; init; }
	[Reactive] public ReadOnlyObservableCollection<MessageRequest> History { get; private set; }

	private readonly IRequestManager requestManager;
	public ViewModelActivator Activator { get; } = new ViewModelActivator();

	public SidebarHistoryViewModel(IRequestManager requestManager)
	{
		this.requestManager = requestManager;

		this.ReactiveHandler(x => x.MessageInfo!,
				x =>
			   {
				   messageContext = this.ReactiveAggregatorProperty(
				   x => x.messageContext,
				   x => x.requestManager.MessageContexts,
				   x => x.FirstOrDefault(x => x.MessageInfo == MessageInfo));
			   });

		History = this.ReactiveCollectionProperty(
			   x => History!,
			   x => x.messageContext!.MessageRequests,
			   x => x);
		History = new(new());

		HistoryCount = this.ReactiveProperty(
			  x => x.HistoryCount,
			  x => x.History.Count);

		//Fixing ReactiveUI not propagating nulls
		this.ReactiveHandler(x => x.messageContext!,
				x =>
				{
					if (x is null)
					{
						messageContext = new MessageContext(null!);
					}
				});

	}
}
