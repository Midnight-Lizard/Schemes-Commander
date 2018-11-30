﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Commander
{
    public class SchemaVersion
    {
        private SchemaVersion() { }

        public SchemaVersion(string version)
        {
            Value = new SemVer.Version(version);
        }

        public virtual SemVer.Version Value { get; private set; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static SchemaVersion Latest { get; } = new SchemaVersion("10.1.6");
        public static SchemaVersion Unspecified { get; } = new SchemaVersion();
    }
}
