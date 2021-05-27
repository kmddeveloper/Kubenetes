using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AG.ApiLibrary.DataReaderMapper;
using Kubernetes.AppConfiguration;
using Kubernetes.Business;
using Kubernetes.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Web
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
            services.AddTransient<IAppSettingManager, AppSettingManager>();
            services.AddTransient<IDataReaderMapper, DataReaderMapper>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserManager, UserManager>();
            

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddFile("Logs/Kubernetes-Information-{Date}.txt", LogLevel.Information);
                loggerFactory.AddFile("Logs/Kubernetes-Trace-{Date}.txt", LogLevel.Trace);
                loggerFactory.AddFile("Logs/Kubernetes-Debug-{Date}.txt", LogLevel.Debug);
            }
            loggerFactory.AddFile("Logs/Kubernetes-Error-{Date}.txt", LogLevel.Error);
            loggerFactory.AddFile("Logs/Kubernetes-Critical-{Date}.txt", LogLevel.Critical);
            loggerFactory.AddFile("Logs/Kubernetes-Warning-{Date}.txt", LogLevel.Warning);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
