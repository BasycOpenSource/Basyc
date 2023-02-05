using Basyc.MessageBus.Manager.Application.Initialization;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;

public partial class RequestItem
{
	[Parameter] public EventCallback OnMessageSending { get; set; }

	[Parameter] public EventCallback<string> OnValueChanged { get; set; }

	[Parameter][EditorRequired] public RequestItemViewModel RequestItemViewModel { get; set; } = null!;

	public async Task SendMessage(RequestInfo request)
	{
		await OnMessageSending.InvokeAsync(this);
	}

	private string GetDefaultValueString(Type type)
	{
		if (type.IsValueType)
		{
			var defaultValue = type.GetDefaultValue();
			if (defaultValue is null)
			{
				return "null";
			}

			return defaultValue.ToString()!;
		}

		if (type == typeof(string))
		{
			return string.Empty;
		}

		return "@null";
	}

	protected override void OnInitialized()
	{
		RequestItemViewModel.ParameterValues.CollectionChanged += ParameterValues_CollectionChanged;
		for (var paramIndex = 0; paramIndex < RequestItemViewModel.RequestInfo.Parameters.Count; paramIndex++)
		{
			var defaultValue = GetDefaultValueString(RequestItemViewModel.RequestInfo.Parameters[paramIndex].Type);
			RequestItemViewModel.ParameterValues[paramIndex] = defaultValue;
		}

		base.OnInitialized();
	}

	private void ParameterValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		var newValue = (string)e.NewItems![0]!;
		var defaultValue = GetDefaultValueString(RequestItemViewModel.RequestInfo.Parameters[e.NewStartingIndex].Type);
		if (newValue == string.Empty && newValue != defaultValue)
		{
			RequestItemViewModel.ParameterValues[e.NewStartingIndex] = defaultValue;
		}
	}
}
