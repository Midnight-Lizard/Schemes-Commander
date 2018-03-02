using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MidnightLizard.Schemes.Commander.Requests.Serialization
{
    public class RequestSerializer
    {
        public virtual object Deserialize(Type requestType, ApiVersion testApiVersion, string requestJson)
        {
            return null;
        }
    }
}
