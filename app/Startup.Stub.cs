using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MidnightLizard.Schemes.Commander.Infrastructure.Middlewares;

namespace MidnightLizard.Schemes.Commander
{
    public class StartupStub : Startup
    {
        public StartupStub(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<AuthenticationMiddlewareStub>();
            base.Configure(app, env);
        }
    }
}
