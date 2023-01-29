using System.Diagnostics.CodeAnalysis;

namespace Basyc.MessageBus.Broker.NetMQ;

public class WorkerRegistry : IWorkerRegistry
{
	private readonly Dictionary<string, MessageTypeRecord> workerStorage = new();

	public void RegisterWorker(string workerId, string[] suppportedMessages)
	{
		foreach (var supportedMessage in suppportedMessages)
		{
			if (workerStorage.TryGetValue(supportedMessage, out var existingWorkerList))
			{
				existingWorkerList.WorkerIds.Add(workerId);
			}
			else
			{
				var newWorkerList = new List<string>();
				newWorkerList.Add(workerId);
				workerStorage.Add(supportedMessage, new MessageTypeRecord(supportedMessage, newWorkerList, 0));
			}
		}
	}

	public bool TryGetWorkerFor(string messageType, [NotNullWhen(true)] out string? workerId)
	{
		if (workerStorage.TryGetValue(messageType, out var workers))
		{
			if (workers.LastUsedWorkerId == workers.WorkerIds.Count - 1)
			{
				workers.LastUsedWorkerId = 0;
			}
			else
			{
				workers.LastUsedWorkerId++;
			}

			workerId = workers.WorkerIds[workers.LastUsedWorkerId];
			return true;
		}

		workerId = null;
		return false;
	}

	public bool TryGetWorkersFor(string messageType, out string[] workerIds)
	{
		if (workerStorage.TryGetValue(messageType, out var workers))
		{
			workerIds = workers.WorkerIds.ToArray();
			return true;
		}

		workerIds = Array.Empty<string>();
		return false;
	}
}
