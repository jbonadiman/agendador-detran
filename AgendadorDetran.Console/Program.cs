using System;
using System.Threading.Tasks;
using AgendadorDetran.Core.Data.Enums;
using AgendadorDetran.Core.Interfaces;
using AgendadorDetran.Core.Robots;
using AgendadorDetran.Core.Services;
using AgendadorDetran.Core.Utils;
using Serilog;
using Serilog.Events;

namespace AgendadorDetran.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo.Console(LogEventLevel.Information)
                .WriteTo.File(
                    "./Logs/log.txt",
                    LogEventLevel.Debug,
                    rollingInterval: RollingInterval.Minute)
                .MinimumLevel.Debug()
                .CreateLogger();

            IClock clock = new Clock(TimeSpan.FromMilliseconds(100));

            IOrchestrator orchestrator = new Orchestrator(logger, clock);

            var service = new AntiCaptchaService(orchestrator.HttpClient, logger, clock);

            IBrowser browser = orchestrator
                .OpenBrowser(BrowserType.Chrome, BrowserMode.Maximized);

            IRobot agendador = new SchedulerRobot(logger, browser, service);

            await agendador.Run();
        }
    }
}