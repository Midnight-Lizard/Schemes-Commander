using MidnightLizard.Schemes.Commander.Requests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class PublishSchemeRequest : Request
    {
        public Guid PublisherId { get; set; }
        public ColorScheme ColorScheme { get; set; }
    }
}
