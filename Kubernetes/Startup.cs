using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AG.ApiLibrary.DataReaderMapper;
using Kubernetes.AppConfiguration;
using Kubernetes.Business;
using Kubernetes.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;


namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
             // .MinimumLevel.Debug()
              //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
              //{                  
              //    MinimumLogEventLevel = LogEventLevel.Warning,
              //    AutoRegisterTemplate = true
              //})
              .CreateLogger();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
            services.AddCors();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddControllers();
            services.AddTransient<IAppSettingManager, AppSettingManager>();
            services.AddTransient<IDataReaderMapper, DataReaderMapper>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<ITokenManager, TokenManager>();


            var symmetricKeyAsBase64 = "A503F474-4F32-4DDE-A1F2-3635716A0A04";
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            ConfigureJwtAuthService(services);
            services.AddAuthorization(options =>
            {
               //ptions.AddPolicy("Required
            });




            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            //    facebookOptions.AccessDeniedPath = "/user";
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tutorial API", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        { 
           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(); //put css javascripts, images , etc... into the pipeline.
                                  //app.UseHttpsRedirection(); // use this middleware to force the request to redirect to https.
                                  //app.UseAuthentication();


           


            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseWebSockets();
         
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //use convention route which uses routing table
            //app.UseEndpoints(endpoints => {
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}"
            //    );

            // });


            //Use attribute route
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kubenetes Web API");
            });
        }

        #region ConfigureJwtAuthService
        private void ConfigureJwtAuthService(IServiceCollection services)
        {
            var tokenConfig = Configuration.GetSection("Token");
            var symmetricKeyAsBase64 = tokenConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!  
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim  
                ValidateIssuer = true,
                ValidIssuer = tokenConfig["Iss"],

                // Validate the JWT Audience (aud) claim  
                ValidateAudience = true,
                ValidAudience = tokenConfig["Aud"],

                // Validate the token expiry  
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero,              
             
               
            };

            services.AddAuthentication(options =>
            {
                
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParameters;
            });
        }

        #endregion
    }




}
