using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;

namespace Sabio.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            // .UseKestrel()
            .UseIISIntegration()
            .UseIIS()
            .ConfigureAppConfiguration(ConfigConfiguration)
            .ConfigureLogging(ConfigureLogging)
            .UseStartup<Startup>();



        private static void ConfigureLogging(WebHostBuilderContext ctx, ILoggingBuilder logging)
        {
            logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));

            Action<ConsoleLoggerOptions> ConsoleOptions = delegate (ConsoleLoggerOptions opts)
            {
                opts.DisableColors = false;
                opts.IncludeScopes = true;
            };

            logging.AddConsole(ConsoleOptions);
            logging.AddDebug();
        }

        private static void ConfigConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder config)
        {
            IConfigurationBuilder root = config.SetBasePath(ctx.HostingEnvironment.ContentRootPath);

            //the settings in the env settings will override the appsettings.json values, recursively at the key level.
            // where the key could be nested. this would allow very fine tuned control over the settings
            IConfigurationBuilder appSettings = root.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            string jsonFileName = $"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json";
            IConfigurationBuilder envSettings = appSettings
                .AddJsonFile(jsonFileName, optional: true, reloadOnChange: true);
        }
    }
}