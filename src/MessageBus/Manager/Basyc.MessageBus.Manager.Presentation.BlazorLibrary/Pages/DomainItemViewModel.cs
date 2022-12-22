using Basyc.MessageBus.Manager.Application.Initialization;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;

public class DomainItemViewModel
{
    public DomainItemViewModel(DomainInfo requestDomainInfo, IEnumerable<RequestItemViewModel> requestViewModels)
    {
        RequestDomainInfo = requestDomainInfo;
        RequestItemViewModels = new List<RequestItemViewModel>(requestViewModels);
    }

    public DomainInfo RequestDomainInfo { get; }
    public IReadOnlyCollection<RequestItemViewModel> RequestItemViewModels { get; }
}
