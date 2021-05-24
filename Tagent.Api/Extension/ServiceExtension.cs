using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using Tagent.Bussiness.Define;
using Tagent.Bussiness.Implement;
using Tagent.Domain.Database;
using Tagent.Domain.Repository.Define;
using Tagent.Domain.Repository.Implement;
using Tagent.EmailService.Define;
using Tagent.EmailService.Implement;
using Tagent.LoggerService;

namespace Tagent.Api.Extension
{
    public static class ServiceExtension
    {
        //Configure Cors
        public static void ConfigureCors(this IServiceCollection service) =>
                    service.AddCors(options =>
                    {
                        options.AddPolicy("CorsPolicy", builder =>
                        builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
                    });
        //Configure LoggerService
        public static void ConfigureLoggerService(this IServiceCollection service) =>
                    service.AddScoped<ILoggerManager, LoggerManager>();
        //Configure Swagger
        public static void ConfigureSwagger(this IServiceCollection service)
        {
            service.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tagent", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer iJIUzI1NiIsInR5cCI6IkpXVCGlzIElzc2'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        public static void UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tagent v1"));
        }

        //Configure Ioc
        public static void ConfigureIoc(this IServiceCollection service)
        {
            service.AddScoped<IEntityContext,TagentDBContext>();
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            service.AddScoped<IAccountService, AccountService>();
            service.AddScoped<IAccountRoleService, AccountRoleService>();
            service.AddScoped<IRoleService, RoleService>();
            service.AddScoped<IAdvisorService, AdvisorService>();
            service.AddScoped<IAgencyService, AgencyService>();
            service.AddScoped<ICustomerService, CustomerService>();
            service.AddScoped<IVerifierService, VerifierService>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IEmailSender, EmailSender>();
            service.AddScoped<ICompanyService, CompanyService>();
            service.AddScoped<ILoggerManager, LoggerManager>();
        }



        public class AuthorizeCheckOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                bool hasAuth = (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                    || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any())
                    && !context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

                if (hasAuth)
                {
                    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                    operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }
                        ] = new string[]{ }
                    }
                };
                }
            }
        }
    }
}
