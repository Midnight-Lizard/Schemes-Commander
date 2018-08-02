using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander
{
    public class AppVersion
    {
        private AppVersion() { }

        public AppVersion(string version)
        {
            Value = new SemVer.Version(version);
        }

        public virtual SemVer.Version Value { get; private set; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static AppVersion Latest { get; } = new AppVersion("1.3.0");
        public static AppVersion Unspecified { get; } = new AppVersion();
    }
}
