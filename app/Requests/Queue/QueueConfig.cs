using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.Queue
{
    public abstract class QueueConfig
    {
        public string TopicName { get; set; }
        public Dictionary<string, object> ProducerSettings { get; set; }
    }

    public class SCHEMES_QUEUE_CONFIG : QueueConfig { }
}
