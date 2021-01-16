using System;
using AgendadorDetran.Core.Data.Enums;
using AgendadorDetran.Core.Interfaces;
using AgendadorDetran.Core.Robots;
using AgendadorDetran.Core.Utils;

namespace AgendadorDetran.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IOrchestrator orchestrator = new Orchestrator();

            IBrowser browser = orchestrator
                .OpenBrowser(BrowserType.Chrome, BrowserMode.Maximized);
            
            IRobot agendador = new SchedulerRobot(browser);

            agendador.Run();
        }
    }
}