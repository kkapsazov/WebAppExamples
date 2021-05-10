using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace RestExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            StaticConfig = configuration;
        }

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

            services.AddControllers()
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.InvalidModelStateResponseFactory = context =>
                    {
                        ILogger<Startup> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        Exception ex = new(context.ModelState.Values.First().Errors.First().ErrorMessage);
                        logger.Log(LogLevel.Error, ex, ex.Message);
                        Console.WriteLine(ex);
                        context.HttpContext.Response.ContentType = "text/plain";
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.HttpContext.Response.WriteAsync("Error");
                        return new EmptyResult();
                    };
                })
                .AddNewtonsoftJson(o => o.AllowInputFormatterExceptionMessages = false);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RestExample",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestExample v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    Exception ex = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                    Console.WriteLine(ex);
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync("Es trat ein unvorhergesehenes Problem auf - bitte kontaktieren Sie Ihren Administrator!");
                });
            });
        }
    }
}
