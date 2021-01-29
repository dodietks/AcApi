using AcApi.Controllers;
using AcApi.Controllers.Imp;
using AcApi.Infrastructure;
using AcApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
            services.AddControllers();

            var appSettingsSection = Configuration.GetSection("ACS");
            services.Configure<LoginOptions>(appSettingsSection);
            services.AddSingleton((x) => x.GetService<IOptions<LoginOptions>>().Value);

            services.AddTransient<IAccessControl, IAccessControl>();
            services.AddTransient<ISnapshot, ISnapshot>();
            services.AddTransient<IStream, IStream>();
            services.AddSingleton<DeviceConnection, DeviceConnection>();
            services.AddSingleton<SmartCardRepository, SmartCardRepository>();

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
