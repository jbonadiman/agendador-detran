using System;
using System.Net.Http;
using AgendadorDetran.Core.Data.Enums;
using AgendadorDetran.Core.Interfaces;
using Serilog;

namespace AgendadorDetran.Core.Utils
{
    public class Orchestrator : IOrchestrator
    {
        private static readonly HttpClient InnerHttpClient = new();

        public HttpClient HttpClient => InnerHttpClient;

        private readonly ILogger _logger;
        private readonly IClock _clock;

        public Orchestrator(ILogger logger, IClock clock)
        {
            this._logger = logger;
            this._clock = clock;
        }

        public IBrowser OpenBrowser(BrowserType browserType, BrowserMode browserMode)
        {
            this._logger.Debug("Opening a [{BrowserType}] browser...", browserType);
            IBrowser browser = browserType switch
            {
                BrowserType.Chrome => new Chrome(this._clock),
                _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, null)
            };

            switch (browserMode)
            {
                case BrowserMode.Maximized:
                    this._logger.Debug("Maximizing the browser...");
                    browser.Driver().Manage().Window.Maximize();
                    break;
                case BrowserMode.Minimized:
                    this._logger.Debug("Minimizing the browser...");
                    browser.Driver().Manage().Window.Minimize();
                    break;
                case BrowserMode.Headlesss:
                    break;
                default:
                    this._logger.Error("Invalid browser mode: [{browserMode}]", browserMode);

                    throw new InvalidOperationException();
            }

            return browser;
        }
    }
}