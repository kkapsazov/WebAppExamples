using System;
using System.Security.Claims;
using System.Text;
using GraphQL.Authorization;
using GraphQL.Server;
using GraphQL.Validation;
using GraphQLExample.GraphQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace GraphQLExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            this.Configuration = configuration;
            this.Environment = hostEnvironment;
            StaticConfig = configuration;
        }

        public IHostEnvironment Environment { get; set; }

        public IConfiguration Configuration { get; }

        public static IConfiguration StaticConfig { get; private set; }

        public static readonly ILoggerFactory CustomLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CoreDbContext>(options => options
                .UseSqlite(StaticConfig.GetConnectionString("SQLite"))
                .UseLoggerFactory(CustomLoggerFactory));

            services.AddScoped<CoreSchema>();

            services.AddControllers();

            services.TryAddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, MaskAuthValidationRule>();

            services.TryAddTransient(s =>
            {
                AuthorizationSettings authSettings = new();
                authSettings.AddPolicy("AdminPolicy", p => p.RequireClaim(ClaimTypes.Role, "Admin"));
                authSettings.AddPolicy("UserPolicy", p => p.RequireClaim(ClaimTypes.Role, "User"));
                return authSettings;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidIssuer = this.Configuration["Jwt:Issuer"],
                        ValidAudience = this.Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Jwt:Key"]))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine(context);
                            return context.Response.WriteAsync("result");
                        }
                    };
                });

            services.AddGraphQL(o =>
                {
                    o.EnableMetrics = false;
                    o.UnhandledExceptionDelegate = context => Console.WriteLine(context.Exception);
                })
                .AddSystemTextJson()
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddDataLoader()
                .AddUserContextBuilder(context => new GraphQLUserContext {User = context.User});

            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseGraphQL<CoreSchema>();

            app.UseGraphQLPlayground();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
