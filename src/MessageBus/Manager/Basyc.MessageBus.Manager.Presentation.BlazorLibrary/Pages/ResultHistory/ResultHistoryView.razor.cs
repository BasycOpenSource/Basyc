using Basyc.MessageBus.Manager.Application;
using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.ResultHistory;

public partial class ResultHistoryView
{
    private MessageRequest? selectedRequestResults;

    [Parameter]
    public List<MessageRequest>? RequestContexts { get; set; }

    [Parameter]
#pragma warning disable BL0007 // Component parameters should be auto properties
    public MessageRequest? SelectedRequestResults
#pragma warning restore BL0007 // Component parameters should be auto properties
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
