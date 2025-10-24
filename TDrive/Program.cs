using Belem.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace TDrive
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug);
                //.AddConsole()
                //.AddEventLog();
            });
            ILogger logger = loggerFactory.CreateLogger<Login>();
            logger.LogInformation("Example log message");

            var config = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
     .Build();
            var serviceProvider = new ServiceCollection()
                       .AddScoped(typeof(ILogger<>), typeof(Logger<>))
                .AddWinForms()
                
                .ConfigureDependencies(config)

                .BuildServiceProvider();

            FormFactory._serviceProvide = serviceProvider;

            var frm = FormFactory.GetForm("main");
            Application.Run(frm) ;
        }
    }
}