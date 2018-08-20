using Autofac;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MidnightLizard.Schemes.Commander.AutofacModules;
using MidnightLizard.Schemes.Commander.Configuration;
using MidnightLizard.Schemes.Commander.Infrastructure.Middlewares;
using MidnightLizard.Schemes.Commander.Infrastructure.ModelBinding;
using MidnightLizard.Schemes.Commander.Infrastructure.Queue;
using Newtonsoft.Json;
using System;

namespace MidnightLizard.Schemes.Commander
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SCHEMES_QUEUE_CONFIG>(x => JsonConvert
                .DeserializeObject<SCHEMES_QUEUE_CONFIG>(this.Configuration
                .GetValue<string>(nameof(SCHEMES_QUEUE_CONFIG))));

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version", "x-api-version"),
                    new QueryStringApiVersionReader());
            });

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
                    options.Authority = this.Configuration
                        .GetValue<string>("IDENTITY_URL");

                    // name of the API resource
                    options.ApiName = "schemes-commander";
                    options.ApiSecret = this.Configuration.GetValue<string>("IDENTITY_SCHEMES_COMMANDER_API_SECRET");

                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10); // default = 10
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
            //app.UseHttpsRedirection();
            var corsConfig = new CorsConfig();
            this.Configuration.Bind(corsConfig);
            app.UseCors(builder => builder
                .WithOrigins(corsConfig.ALLOWED_ORIGINS.Split(','))
                .AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMvc();
        }
    }
}
