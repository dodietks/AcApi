using AcApi.BackgroundServices;
using AcApi.Controllers;
using AcApi.Infrastructure;
using AcApi.Infrastructure.Http;
using AcApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AcApi
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
            services.AddHttpClient();
            services.AddControllers();
            var appSettingsSection = Configuration.GetSection("Configurations");
            services.Configure<ConfigurationOptions>(appSettingsSection);
            services.AddSingleton((x) => x.GetService<IOptions<ConfigurationOptions>>().Value);
            services.AddSingleton<DeviceConnection, DeviceConnection>();
            services.AddSingleton<SmartCardRepository, SmartCardRepository>();

            services.AddHostedService<GetNewEventsService>();

            services.AddTransient<IHttpRequest, HttpRequest>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
