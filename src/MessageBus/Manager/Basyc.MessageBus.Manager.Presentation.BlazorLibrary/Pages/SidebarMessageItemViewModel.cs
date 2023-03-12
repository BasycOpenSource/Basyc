using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;

public class SidebarMessageItemViewModel
{
	public SidebarMessageItemViewModel(MessageInfo requestInfo) : this(null, requestInfo)
	{
	}

	public SidebarMessageItemViewModel(MessageRequest? requestContext, MessageInfo requestInfo)
	{
		RequestContext = requestContext!;
		RequestInfo = requestInfo;
		//ParameterValues = new ObservableCollection<string>(Enumerable.Range(0, RequestInfo.Parameters.Count).Select(x => string.Empty).ToList());
	}

	public MessageRequest RequestContext { get; set; } = null!;
	public MessageInfo RequestInfo { get; init; }
	//public ObservableCollection<string> ParameterValues { get; init; }
	public bool IsLoading => RequestContext?.State == RequestResultState.Started;
	public bool IsSelected { get; set; }
}
