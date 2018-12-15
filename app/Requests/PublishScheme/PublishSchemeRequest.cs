using MidnightLizard.Schemes.Commander.Requests.Base;

namespace MidnightLizard.Schemes.Commander.Requests.PublishScheme
{
    public class PublishSchemeRequest : Request
    {
        public string Description { get; set; }
        public ColorScheme ColorScheme { get; set; }
    }
}
