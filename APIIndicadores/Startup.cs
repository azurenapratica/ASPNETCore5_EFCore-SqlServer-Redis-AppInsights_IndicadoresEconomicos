using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.ApplicationInsights.DependencyCollector;
using APIIndicadores.Data;

namespace APIIndicadores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIIndicadores", Version = "v1" });
            });

            if (!String.IsNullOrWhiteSpace(Configuration["ApplicationInsights:InstrumentationKey"]))
            {
                services.AddApplicationInsightsTelemetry(Configuration);
                services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>(
                    (module, o) =>
                    {
                        module.EnableSqlCommandTextInstrumentation = true;
                    });
            }

            services.AddDbContext<IndicadoresContext>(
                options => options.UseSqlServer(
                    Configuration.GetConnectionString("BaseIndicadores")));

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration =
                    Configuration.GetConnectionString("CacheRedis");
                options.InstanceName = "Cache-APIIndicadores-";
            });

            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIIndicadores v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Ativando o middlweare de Health Check
            app.UseHealthChecks("/status");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}