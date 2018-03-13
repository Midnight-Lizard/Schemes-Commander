using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MidnightLizard.Schemes.Commander.AutofacModules;
using FluentValidation;
using FluentValidation.AspNetCore;
using MidnightLizard.Schemes.Commander.Requests;
using MidnightLizard.Schemes.Commander.Infrastructure.ModelBinding;
using MidnightLizard.Schemes.Commander.Infrastructure.Middlewares;
using MidnightLizard.Schemes.Commander.Infrastructure.Queue;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityServer4.AccessTokenValidation;

namespace MidnightLizard.Schemes.Commander
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SCHEMES_QUEUE_CONFIG>(x => JsonConvert
                .DeserializeObject<SCHEMES_QUEUE_CONFIG>(Configuration
                .GetValue<string>(nameof(SCHEMES_QUEUE_CONFIG))));

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("version", "api-version", "x-api-version"),
                    new QueryStringApiVersionReader());
            });

            services.AddCors(x => x.AddPolicy("all", p => p
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowAnyOrigin()));

            services.AddMvc(opt =>
            {
                opt.ModelBinderProviders.Insert(0, new RequestModelBinderProvider());
            }).AddFluentValidation(fv => fv
                .RegisterValidatorsFromAssemblyContaining<Startup>()
                .ImplicitlyValidateChildProperties = true);

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.RequireHttpsMetadata = false;

                    // base-address of your identityserver
                    options.Authority = Configuration
                        .GetValue<string>("IDENTITY_URL");

                    // name of the API resource
                    options.ApiName = "schemes-commander";
                });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SerializationModule>();
            builder.RegisterModule<ModelBindingModule>();
            builder.RegisterModule<VersionModule>();
            builder.RegisterModule<QueueModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("all");
            app.UseAuthentication();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMvc();
        }
    }
}
