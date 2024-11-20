namespace Mobility.Payments.Api.Configuration
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Mobility.Payments.Api.Filters;
    using Mobility.Payments.Api.Mappings;
    using Mobility.Payments.Application.Interfaces;
    using Mobility.Payments.Application.Services;
    using Mobility.Payments.Crosscuting.Options;
    using Mobility.Payments.Data;
    using Mobility.Payments.Data.Repositories;
    using Mobility.Payments.Domain.Repositories;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.Json.Serialization;

    internal static class Bootstrapper
    {
        internal static IServiceCollection BindOptions(this IServiceCollection services)
        {
            services.AddOptions<ApiKeyConfiguration>()
                .Configure<IConfiguration>((settings, config) => config.GetSection(ApiKeyConfiguration.Section).Bind(settings));
            services.AddOptions<JwtConfiguration>()
                .Configure<IConfiguration>((settings, config) => config.GetSection(JwtConfiguration.Section).Bind(settings));

            return services;
        }

        internal static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            return services;
        }

        internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenProviderService, TokenProviderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPaymentService, PaymentService>();
            return services;
        }

        internal static IServiceCollection SwaggerConfiguration(this IServiceCollection services)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mobility.Payments.Api", Version = "v1" });
                options.IncludeXmlComments(xmlPath);
                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Name = "x-api-key",
                    Description = "Authorization by x-api-key inside request's header",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token in this field",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });

                var securityRequeriment = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            },
                            In = ParameterLocation.Header
                        },
                        []
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        []
                    },

                };
                options.AddSecurityRequirement(securityRequeriment);
            });

            return services;
        }

        internal static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseLazyLoadingProxies()
                .UseSqlServer(
                    connectionString: configuration.GetConnectionString(ApplicationDbContext.ConnectionName),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }

        internal static void MigrateDatabase(this IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }

        internal static IServiceCollection ControllerConfiguration(this IServiceCollection services)
        {
            services
                .AddMvcOptions()
                .AddControllersWithViews(delegate (MvcOptions options)
            {
                options.Conventions.Add(new RoutePrefixConvention("api"));
            }).AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            return services;
        }

        internal static IServiceCollection UseAuthorizationContext(this IServiceCollection services)
        {
            return services
                   .AddScoped<UserContextProviderService>()
                   .AddScoped<IUserContextProviderService>(x => x.GetRequiredService<UserContextProviderService>())
                   .AddScoped<IUserProviderService>(x => x.GetRequiredService<UserContextProviderService>());
        }

        internal static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            return services;
        }

        internal static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfiguration = configuration.GetSection(JwtConfiguration.Section).Get<JwtConfiguration>();
            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer( o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret)),
                        ValidIssuer = jwtConfiguration.Issuer,
                        ValidAudience = jwtConfiguration.Audience,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            return services;
        }

        internal static IServiceCollection AddMvcOptions(this IServiceCollection services)
        {
            services.AddScoped<AuthorizationFilter>();

            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add<AuthorizationFilter>();
                });
            return services;
        }
    }
}
