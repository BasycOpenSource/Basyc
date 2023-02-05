using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages;

public class RequestItemViewModel
{
	public RequestItemViewModel(RequestInfo requestInfo) : this(null, requestInfo)
	{
	}

	public RequestItemViewModel(RequestContext? requestContext, RequestInfo requestInfo)
	{
		RequestContext = requestContext!;
		RequestInfo = requestInfo;
		ParameterValues = new ObservableCollection<string>(Enumerable.Range(0, RequestInfo.Parameters.Count).Select(x => string.Empty).ToList());
	}

	public RequestContext RequestContext { get; set; } = null!;
	public RequestInfo RequestInfo { get; init; }
	public ObservableCollection<string> ParameterValues { get; init; }
	public bool IsLoading => RequestContext?.State == RequestResultState.Started;
	public bool IsSelected { get; set; }
}
