namespace SFA.DAS.RoATPService.Application.Api
{
    using System;
    using global::NLog.Web;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using StartupConfiguration;

    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Starting up host");

                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .UseKestrel()
                .UseNLog();
        }
    }
}