using MidnightLizard.Commons.Domain.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander
{
    public static class AppVersion
    {
        public static DomainVersion Latest { get; } = new DomainVersion("1.3.0");
    }
}
