using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Schemes.Commander.Infrastructure.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    [AdvertiseApiVersions("1.0", "1.1", "1.2", "1.3")]
    public class UnpublishSchemeRequestDeserializer_Latest : AggregateIdRequestDeserializer<UnpublishSchemeRequest>
    {
    }
}
