using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.Shared.Helpers;
using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;

public partial class RequestItem
{
    private RequestItemViewModel requestItemViewModel;

    [Parameter]
    public EventCallback OnMessageSending { get; set; }

    [Parameter]
    public EventCallback<string> OnValueChanged { get; set; }

    [Parameter]
    public RequestItemViewModel RequestItemViewModel { get => requestItemViewModel; set => requestItemViewModel = value; }

    public async Task SendMessage(RequestInfo request)
    {
        await OnMessageSending.InvokeAsync(this);
    }

    private string GetDefaultValueString(Type type)
    {
        if (type.IsValueType)
        {
            return type.GetDefaultValue().ToString();
        }
        else if (type == typeof(string))
        {
            return string.Empty;
        }
        else
        {
            return "@null";
        }
    }

    protected override void OnInitialized()
    {
        RequestItemViewModel.ParameterValues.CollectionChanged += ParameterValues_CollectionChanged;
        for (int paramIndex = 0; paramIndex < RequestItemViewModel.RequestInfo.Parameters.Count; paramIndex++)
        {
            var defaultValue = GetDefaultValueString(RequestItemViewModel.RequestInfo.Parameters[paramIndex].Type);
            RequestItemViewModel.ParameterValues[paramIndex] = defaultValue;
        }

        base.OnInitialized();
    }

    private void ParameterValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        var newValue = (string)e.NewItems[0];
        var defaultValue = GetDefaultValueString(RequestItemViewModel.RequestInfo.Parameters[e.NewStartingIndex].Type);
        if (newValue == string.Empty && newValue != defaultValue)
        {
            RequestItemViewModel.ParameterValues[e.NewStartingIndex] = defaultValue;
        }
    }
}
