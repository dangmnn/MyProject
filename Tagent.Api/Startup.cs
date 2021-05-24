using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Tagent.Api.Extension;
using Tagent.Domain.Database;
using Tagent.EmailService;

namespace Tagent.Api
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
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            services.ConfigureIoc();
            services.AddControllers();
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(name: "All",
            //        builder =>
            //        {
            //            builder
            //            //.WithOrigins("http://localhost:3000", "http://tagent.vn")
            //            .AllowAnyOrigin()
            //            .AllowAnyMethod()
            //            .AllowAnyHeader();
            //        });
            //});
            services.AddCors();
            services.ConfigureLoggerService();
            services.AddDbContextPool<TagentDBContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("TagentDbLocal")));
            services.ConfigureSwagger();
            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(opts =>
                {
                    var firebaseProjectName = Configuration["FirebaseProjectName"];
                    opts.Authority = "https://securetoken.google.com/" + firebaseProjectName;
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"]
                    };
                })
                .AddFacebook(opts =>
                {
                    opts.AppId = "614617749937693";
                    opts.AppSecret = "f69ce294a86cbf261a95f0513a2d27a2";
                })
                ;
            //Configuration AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            var pathToKey = Path.Combine(Directory.GetCurrentDirectory(), "key", "firebase_admin_sdk.json");

            //if (HostingEnvironment.IsEnvironment("local"))
            //    pathToKey = Path.Combine(Directory.GetCurrentDirectory(), "keys", "firebase_admin_sdk.local.json");

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(pathToKey)
            });
            services.AddControllersWithViews()
             .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            var emailConfig = Configuration.GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddApplicationInsightsTelemetry();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseCustomSwagger();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
