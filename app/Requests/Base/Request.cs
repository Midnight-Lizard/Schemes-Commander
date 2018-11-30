using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.Base
{
    public class Request
    {
        public Guid AggregateId { get; set; }
        public Guid Id { get; set; }
        [JsonIgnore]
        public Type DeserializerType { get; set; }
    }
}
