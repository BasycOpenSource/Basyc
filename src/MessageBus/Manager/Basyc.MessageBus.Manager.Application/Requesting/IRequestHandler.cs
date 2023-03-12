using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public interface IRequestHandler
{
	string UniqueName { get; }

	/// <summary>
	///     Returns session id of the request
	/// </summary>
	/// <param name="requestResult"></param>
	/// <returns></returns>
	void StartRequest(MessageRequest requestResult, ILogger logger);
}
