using System;
using GraphQL.Server;
using GraphQLExample.GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            services.AddGraphQL(o =>
                {
                    o.EnableMetrics = false;
                    o.UnhandledExceptionDelegate = context => Console.WriteLine(context.Exception);
                })
                .AddSystemTextJson()
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddDataLoader();

            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseGraphQL<CoreSchema>();

            app.UseGraphQLPlayground();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
