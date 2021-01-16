using System;
using System.Linq.Expressions;
using AgendadorDetran.Core.Data.Enums;
using AgendadorDetran.Core.Data.Models;
using AgendadorDetran.Core.Interfaces;
using AgendadorDetran.Core.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V87.Emulation;

namespace AgendadorDetran.Core.Robots
{
    public class SchedulerRobot : IRobot
    {
        private const string WebPage =
            "http://www2.detran.rj.gov.br/portal/IdentificacaoCivil/agendamentoDados";

        private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(20);

        private readonly IBrowser _browser;
        private readonly IAntiCaptchaService _captchaService;

        public SchedulerRobot(IBrowser browser, IAntiCaptchaService captchaService)
        {
            this._browser = browser;
            this._captchaService = captchaService;
        }

        public void Run()
        {
            var formData = new FormData
            {
                ***REMOVED***
                ***REMOVED***
                ***REMOVED***
                ***REMOVED***
                ***REMOVED***
                ***REMOVED***
                ***REMOVED***
            };
            
            this.GoToDataPage();
            //this.FillData(formData);
            this.SolveCaptcha();
            
            new Clock(TimeSpan.FromSeconds(1)).StopFor(TimeSpan.MaxValue);
            this._browser.Dispose();
        }

        private void GoToDataPage()
        {
            this._browser.Driver().Navigate().GoToUrl(WebPage);
        }

        private void FillData(FormData formData)
        {
            string expiryRadioId =
                "IdentificacaoCivilPossuiValidade" + (formData.RgHasExpiryDate? "1" : "0");
            
            string hasPreviousDocumentRadioId =
                "IdentificacaoCivilPossuiCarteira" + (formData.HasPreviousDocument? "1" : "0"); 

            
            IWebElement fullNameElement = this._browser.FindElement(
                By.Id("identificacaoCivilNomeCidadao"),
                this._defaultTimeout
            );
            
            IWebElement fatherNameElement = this._browser.FindElement(
                By.Id("identificacaoCivilNomePai"),
                this._defaultTimeout
            );
            
            IWebElement motherNameElement = this._browser.FindElement(
                By.Id("identificacaoCivilNomeMae"),
                this._defaultTimeout
            );
            
            IWebElement birthdayElement = this._browser.FindElement(
                By.Id("identificacaoCivilDataNascimento"),
                this._defaultTimeout
            );
            
            IWebElement hasPreviousDocumentElement = this._browser.FindElement(
                By.Id(hasPreviousDocumentRadioId),
                this._defaultTimeout
            );
            
            IWebElement rgElement = this._browser.FindElement(
                By.Id("identificacaoCivilRgEnvio"),
                this._defaultTimeout
            );
            
            IWebElement hasExpiryDateElement = this._browser.FindElement(
                By.Id(expiryRadioId),
                this._defaultTimeout
            );
            
            fullNameElement.SendKeys(formData.FullName);
            fatherNameElement.SendKeys(formData.FatherName);
            motherNameElement.SendKeys(formData.MotherName);
            birthdayElement.SendKeys(formData.BirthDay);

            hasPreviousDocumentElement.Click();

            if (!formData.HasPreviousDocument) return;
            
            rgElement.SendKeys(formData.Rg);
            hasExpiryDateElement.Click();
        }

        private void SolveCaptcha()
        {
            IWebElement captchaKeyElement = this._browser.FindElement(
                By.XPath("//*[contains(@class, 'g-recaptcha')]"),
                this._defaultTimeout);

            string siteKey = captchaKeyElement.GetAttribute("data-sitekey");

            this._captchaService.SendCaptcha(siteKey);
        }

        private void SendForm()
        {
            IWebElement submitButton = this._browser.FindElement(
                By.Id("btPesquisar"),
                this._defaultTimeout
            );
        }
    }
}