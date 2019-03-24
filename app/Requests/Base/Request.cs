using Newtonsoft.Json;
using System;

namespace MidnightLizard.Schemes.Commander.Requests.Base
{
    public class Request
    {
        /// <summary>
        /// ID of the color scheme you want to delete
        /// </summary>
        public virtual Guid AggregateId { get; set; }

        public virtual Guid Id { get; set; }

        [JsonIgnore]
        internal Type DeserializerType { get; set; }
    }
}
