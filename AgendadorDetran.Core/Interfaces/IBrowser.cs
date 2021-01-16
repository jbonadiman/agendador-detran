using System;
using OpenQA.Selenium;

namespace AgendadorDetran.Core.Interfaces
{
    public interface IBrowser : IDisposable
    {
        DriverService Service();

        IWebDriver Driver();

        IWebElement FindElement(By by, TimeSpan timeout);
    }
}