using System;
using System.Runtime.Versioning;
using AgendadorDetran.Core.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AgendadorDetran.Core.Utils
{
    public class Chrome : IBrowser
    {
        private ChromeDriverService _service;
        private ChromeDriver _driver;
        private IClock _clock;

        public Chrome(IClock clock)
        {
            this._clock = clock;
            this._service = ChromeDriverService.CreateDefaultService();
            this._driver = new ChromeDriver(this._service);
        }

        DriverService IBrowser.Service()
        {
            return this._service;
        }

        IWebDriver IBrowser.Driver()
        {
            return this._driver;
        }

        public IWebElement FindElement(By by, TimeSpan timeout)
        {
            IWebElement element = null;
            
            this._clock.StopUntilCondition(
                () =>
                {
                    element = this._driver.FindElement(@by);
                    return element != null;
                }, timeout);

            return element;
        }


        public void Dispose()
        {
            this._service?.Dispose();
            this._driver?.Dispose();
        }
    }
}