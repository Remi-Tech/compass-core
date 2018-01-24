using Microsoft.AspNetCore.Hosting;
using Autofac.Extensions.DependencyInjection;
using Compass.Shared;
using Microsoft.AspNetCore;

namespace Compass.CoreServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = CompassEnvironment.AppInsightsKey;
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

    }
}
