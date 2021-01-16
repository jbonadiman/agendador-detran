using System;
using AgendadorDetran.Core.Data.Enums;
using AgendadorDetran.Core.Utils;

namespace AgendadorDetran.Core.Interfaces
{
    public interface IOrchestrator
    {
        IBrowser OpenBrowser(BrowserType browserType, BrowserMode browserMode);
    }
}