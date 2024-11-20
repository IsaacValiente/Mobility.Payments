namespace Mobility.Payments.Api
{
    using System;
    using Figgle;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    /// <summary>
    /// The entry point for the Mobility Payments API application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Protected constructor to prevent instantiation of the <see cref="Program"/> class.
        /// </summary>
        protected Program()
        {
        }

        /// <summary>
        /// The main method, serving as the application's entry point.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine(FiggleFonts.Standard.Render("Mobility Inc. Payments"));
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

            try
            {
                Log.Information("Mobility Payments Api starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Creates and configures the host builder for the application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        /// <returns>An <see cref="IHostBuilder"/> configured for the application.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, configuration) =>
                {
                    configuration
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    if (host.HostingEnvironment.IsDevelopment())
                    {
                        configuration.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    }

                    configuration.AddEnvironmentVariables();
                    configuration.AddUserSecrets<Startup>();

                })
                .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration))
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}