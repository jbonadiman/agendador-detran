using System;
using System.Net.Http;
using AgendadorDetran.Core.Data.Enums;
using AgendadorDetran.Core.Utils;

namespace AgendadorDetran.Core.Interfaces
{
    public interface IOrchestrator
    {
        public HttpClient HttpClient { get; }
        IBrowser OpenBrowser(BrowserType browserType, BrowserMode browserMode);
    }
}