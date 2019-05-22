using co2Device;
using Co2WebApp.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Co2WebApp {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddMemoryCache();
            services.AddSingleton<IDataProcessor, DataProcessor>();
            services.AddSingleton<ICo2DeviceHandler, Co2DeviceHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {            		            
            app.UseMiddleware<UsbConnectedMiddleware>();
         
            app.UseHttpsRedirection();
            app.UseMvc();            
        }
    }
}