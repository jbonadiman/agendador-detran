using System;
using AgendadorDetran.Core.Data.Enums;
using AgendadorDetran.Core.Interfaces;

namespace AgendadorDetran.Core.Utils
{
    public class Orchestrator : IOrchestrator
    {
        public IBrowser OpenBrowser(BrowserType browserType, BrowserMode browserMode)
        {
            IBrowser browser = browserType switch
            {
                BrowserType.Chrome => new Chrome(
                    new Clock(TimeSpan.FromMilliseconds(100))
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, null)
            };

            switch (browserMode)
            {
                case BrowserMode.Maximized:
                    browser.Driver().Manage().Window.Maximize();
                    break;
                case BrowserMode.Minimized:
                    browser.Driver().Manage().Window.Minimize();
                    break;
                case BrowserMode.Headlesss:
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return browser;
        }
    }
}