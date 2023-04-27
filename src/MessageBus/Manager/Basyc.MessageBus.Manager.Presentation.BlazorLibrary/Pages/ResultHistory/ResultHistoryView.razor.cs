using Basyc.MessageBus.Manager.Application;
using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.ResultHistory;

#pragma warning disable CA1002 // Do not expose generic lists
#pragma warning disable BL0007 // Component parameters should be auto properties
#pragma warning disable CA2227 // Collection properties should be read only
public partial class ResultHistoryView
{
    private MessageRequest? selectedRequestResults;

    [Parameter]
    public List<MessageRequest>? RequestContexts { get; set; }

    [Parameter]
    public MessageRequest? SelectedRequestResults
    {
        get => selectedRequestResults;
        set
        {
            if (value == selectedRequestResults)
                return;
            selectedRequestResults = value;
        }
    }
}
