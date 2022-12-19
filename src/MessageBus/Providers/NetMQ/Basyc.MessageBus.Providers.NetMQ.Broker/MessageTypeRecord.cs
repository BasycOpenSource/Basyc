using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Broker.NetMQ
{
    public class MessageTypeRecord
    {
        public MessageTypeRecord(string MessageType, List<string> WorkerAddresses, int LastUsedWorker)
        {
            this.MessageType = MessageType;
            this.WorkerIds = WorkerAddresses;
            this.LastUsedWorkerId = LastUsedWorker;
        }

        public string MessageType { get; }
        public List<string> WorkerIds { get; }
        public int LastUsedWorkerId { get; set; }
    }
}