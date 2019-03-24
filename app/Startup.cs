using Autofac;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MidnightLizard.Schemes.Commander.AutofacModules;
using MidnightLizard.Schemes.Commander.Configuration;
using MidnightLizard.Schemes.Commander.Infrastructure.Middlewares;
using MidnightLizard.Schemes.Commander.Infrastructure.ModelBinding;
using MidnightLizard.Schemes.Commander.Infrastructure.Queue;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
                    options.RequireHttpsMetadata = true;

                    // base-address of your identityserver
                    options.Authority = this.Configuration
                        .GetValue<string>("IDENTITY_URL");

                    // name of the API resource
                    options.ApiName = "schemes-commander";
                    options.ApiSecret = this.Configuration.GetValue<string>("IDENTITY_SCHEMES_COMMANDER_API_SECRET");

                    options.EnableCaching = false;
                    options.CacheDuration = TimeSpan.FromMinutes(1); // default = 10
                    options.JwtValidationClockSkew = TimeSpan.FromMinutes(4);
                });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Midnight Lizard Schemes Commander API",
                    Version = "v1"
                });

                c.DescribeAllEnumsAsStrings();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.OperationFilter<SwaggerDefaultValuesFilter>();

                var authUrl = new Uri(new Uri(
                    this.Configuration.GetValue<string>("IDENTITY_URL") ?? "http://localhost:7001"),
                    "connect/authorize")
                    .AbsoluteUri;

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "implicit",
                    AuthorizationUrl = authUrl,
                    Scopes = new Dictionary<string, string>
                        { { "schemes-commander", "Schemes Commander API - Full Access" } }
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", new string[] { } }
                });
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
            app.UseMiddleware<ExceptionMiddleware>();
            var corsConfig = new CorsConfig();
            this.Configuration.Bind(corsConfig);
            app.UseCors(builder => builder
                .WithOrigins(corsConfig.ALLOWED_ORIGINS.Split(','))
                .AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();
            var rewriteTargetRegex = this.Configuration.GetValue<string>("REWRITE_TARGET");
            if (!string.IsNullOrEmpty(rewriteTargetRegex))
            {
                app.UseRewriter(new RewriteOptions().AddRewrite(
                    rewriteTargetRegex, "$1", skipRemainingRules: true));
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                var swgPrefix = this.Configuration.GetValue<string>("SWAGGER_API_PREFIX");
                if (!string.IsNullOrEmpty(swgPrefix))
                {
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        IDictionary<string, PathItem> paths = new Dictionary<string, PathItem>();
                        foreach (var path in swaggerDoc.Paths)
                        {
                            paths.Add(swgPrefix + path.Key, path.Value);
                        }
                        swaggerDoc.Paths = paths;
                    });
                }
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "schemes-commander-v1");
                c.RoutePrefix = string.Empty;
                c.OAuthClientId("schemes-commander-swagger");
                c.OAuthAppName("Schemes Commander API - Swagger UI");
            });

            app.UseMvc();
        }
    }
}
