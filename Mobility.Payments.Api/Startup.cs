namespace Mobility.Payments.Api
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Mobility.Payments.Api.Configuration;
    using Mobility.Payments.Api.Middlewares;

    /// <summary>
    /// Entry point for configuring the application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration for the application.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }


        /// <summary>
        /// Configures services and dependencies for the application.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .BindOptions();

            services.AddLocalization();
            services.UseAuthorizationContext();
            services.AddApplicationServices();
            services.ConfigureDatabase(this.Configuration);
            services.AddRepositories();
            services.RegisterAutoMapper();


            services.ControllerConfiguration();

            services.AddHttpContextAccessor();
            services.SwaggerConfiguration();
            services.ConfigureAuth(this.Configuration);
        }

        /// <summary>
        /// Configures the HTTP request pipeline for the application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web host environment.</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mobility.Payments.Api v1"));
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });

            app.MigrateDatabase();
        }
    }
}
