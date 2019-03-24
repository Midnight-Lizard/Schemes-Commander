using Newtonsoft.Json;
using System;

namespace MidnightLizard.Schemes.Commander.Requests.Base
{
    public class Request
    {
        /// <summary>
        /// ID of the Aggregate that should process current Request
        /// </summary>
        public virtual Guid AggregateId { get; set; }

        /// <summary>
        /// Optional Request ID
        /// </summary>
        public virtual Guid Id { get; set; }

        [JsonIgnore]
        internal Type DeserializerType { get; set; }
    }
}
