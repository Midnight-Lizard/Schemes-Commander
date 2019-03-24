using Newtonsoft.Json;
using System;

namespace MidnightLizard.Schemes.Commander.Requests.Base
{
    public class Request
    {
        /// <summary>
        /// ID of the color scheme you want to delete
        /// </summary>
        public Guid AggregateId { get; set; }

        internal Guid Id { get; set; }

        [JsonIgnore]
        internal Type DeserializerType { get; set; }
    }
}
