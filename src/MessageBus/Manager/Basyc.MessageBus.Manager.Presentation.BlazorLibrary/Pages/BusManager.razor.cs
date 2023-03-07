using Basyc.MessageBus.Client;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel;
using System.Text.Json;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;

public partial class BusManager
{
	private readonly Dictionary<RequestItemViewModel, List<RequestContext>> resultHistory = new();

	private RequestItemViewModel? selectedRequestViewModel;
	[Inject] private IDomainInfoProviderManager DomainInfoProviderManager { get; set; } = null!;
	[Inject] private ITypedMessageBusClient MessageBusManager { get; set; } = null!;
	[Inject] private IDialogService DialogService { get; set; } = null!;
	[Inject] private IRequestManager RequestClient { get; set; } = null!;
	[Inject] private BusManagerJsInterop BusManagerJSInterop { get; set; } = null!;

	public List<DomainItemViewModel> DomainInfoViewModel { get; private set; } = new();

	public RequestItemViewModel? SelectedRequestViewModel
	{
		get => selectedRequestViewModel;
		private set
		{
			if (selectedRequestViewModel is not null)
				selectedRequestViewModel.IsSelected = false;

			selectedRequestViewModel = value;
			if (value is null)
				return;

			value.IsSelected = true;
			resultHistory.TryAdd(value, new List<RequestContext>());
		}
	}

	protected override void OnInitialized()
	{
		DomainInfoViewModel = DomainInfoProviderManager.GetDomainInfos()
			.Select(domainInfo => new DomainItemViewModel(domainInfo, domainInfo.Requests
				.Select(requestInfo => new RequestItemViewModel(requestInfo))
				.OrderBy(x => x.RequestInfo.RequestType)))
			.ToList();

		base.OnInitialized();
	}

	protected override async Task OnParametersSetAsync()
	{
		await BusManagerJSInterop.ApplyChangesToIndexHtml();
		await base.OnParametersSetAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await BusManagerJSInterop.ApplyChangesToIndexHtml();
		await base.OnInitializedAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
			await BusManagerJSInterop.ApplyChangesToIndexHtml();

		await base.OnAfterRenderAsync(firstRender);
	}

	public Task SendMessage(RequestItemViewModel requestItem)
	{
		var requestInfo = requestItem.RequestInfo;
		var parameters = new List<Parameter>(requestInfo.Parameters.Count);
		for (var i = 0; i < requestInfo.Parameters.Count; i++)
		{
			var paramInfo = requestInfo.Parameters[i];
			var paramStringValue = requestItem.ParameterValues[i];
			var castedParamValue = ParseParamInputValue(paramStringValue, paramInfo);
			parameters.Add(new Parameter(paramInfo, castedParamValue));
		}

		var request = new Request(requestInfo, parameters);
		var requestResult = RequestClient.StartRequest(request);
		requestItem.RequestContext = requestResult;
		resultHistory.TryAdd(requestItem, new List<RequestContext>());
		var requestHistory = resultHistory[requestItem];
		requestHistory.Add(requestResult);
		StateHasChanged();
		return Task.CompletedTask;
	}

	private static object? ParseParamInputValue(string paramStringValue, ParameterInfo parameterInfo)
	{
		if (paramStringValue == "@null")
			return null;

		if (paramStringValue == string.Empty)
			return parameterInfo.Type.GetDefaultValue();

		if (parameterInfo.Type == typeof(string))
			return paramStringValue;

		var converter = TypeDescriptor.GetConverter(parameterInfo.Type);
		object? castedParam;
		if (converter.CanConvertFrom(typeof(string)))
		{
			castedParam = converter.ConvertFromInvariantString(paramStringValue);
			return castedParam;
		}

		var converter2 = TypeDescriptor.GetConverter(typeof(string));
		if (converter2.CanConvertFrom(parameterInfo.Type))
		{
			castedParam = converter2.ConvertFromInvariantString(paramStringValue);
			return castedParam;
		}

		try
		{
			castedParam = Convert.ChangeType(paramStringValue, parameterInfo.Type);
			return castedParam;
		}
		catch
		{
			//Try change type, if fails, use json Deserialize
		}

		castedParam = JsonSerializer.Deserialize(paramStringValue, parameterInfo.Type);
		return castedParam;
	}

	private void OnSelectedRequestMenuItemChanged(RequestItemViewModel newSelectedRequest)
	{
		SelectedRequestViewModel = newSelectedRequest;
		StateHasChanged();
	}

	private async void OnRequested(RequestItemViewModel request)
	{
		await SendMessage(request);
	}
}
