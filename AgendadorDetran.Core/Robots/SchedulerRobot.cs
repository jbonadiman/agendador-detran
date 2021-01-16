using System;
using System.Threading.Tasks;
using AgendadorDetran.Core.Data.Models;
using AgendadorDetran.Core.Interfaces;
using AgendadorDetran.Core.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V84.Overlay;
using Serilog;

namespace AgendadorDetran.Core.Robots
{
    public class SchedulerRobot : IRobot
    {
        private const string WebPage =
            "http://www2.detran.rj.gov.br/portal/IdentificacaoCivil/agendamentoDados";

        private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(20);

        private readonly IBrowser _browser;
        private readonly IAntiCaptchaService _captchaService;
        private readonly ILogger _logger;

        public SchedulerRobot(ILogger logger, IBrowser browser, IAntiCaptchaService captchaService)
        {
            this._browser = browser;
            this._captchaService = captchaService;
            this._logger = logger;
        }

        public async Task Run()
        {
            this._logger.Information("Running robot main flow...");
            
            this.GoToDataPage();
            var captchaTask = this.SolveCaptcha();

            var formData = new FormData(); // Fill your personal data.
            
            this.FillData(formData);

            captchaTask.Wait();
            this.SendForm();
            
            this.CheckForNoSlotsError();
            
            new Clock(TimeSpan.FromHours(2)).StopFor(TimeSpan.MaxValue);
            this._browser.Dispose();
        }

        private void GoToDataPage()
        {
            this._logger.Debug("Browsing: [{webPage}]", WebPage);
            this._browser.Driver().Navigate().GoToUrl(WebPage);
        }

        private void FillData(FormData formData)
        {
            string expiryRadioId =
                "IdentificacaoCivilPossuiValidade" + (formData.RgHasExpiryDate? "1" : "0");
            
            string hasPreviousDocumentRadioId =
                "IdentificacaoCivilPossuiCarteira" + (formData.HasPreviousDocument? "1" : "0"); 

            
            this._logger.Debug("Getting Full Name form element...");
            IWebElement fullNameElement = this._browser.FindElement(
                By.Id("identificacaoCivilNomeCidadao"),
                this._defaultTimeout
            );
            
            this._logger.Debug("Getting Father's Name form element...");
            IWebElement fatherNameElement = this._browser.FindElement(
                By.Id("identificacaoCivilNomePai"),
                this._defaultTimeout
            );
            
            this._logger.Debug("Getting Mother's Name form element...");
            IWebElement motherNameElement = this._browser.FindElement(
                By.Id("identificacaoCivilNomeMae"),
                this._defaultTimeout
            );
            
            this._logger.Debug("Getting Birthday form element...");
            IWebElement birthdayElement = this._browser.FindElement(
                By.Id("identificacaoCivilDataNascimento"),
                this._defaultTimeout
            );
            
            this._logger.Debug("Getting Has Previous Document radio button element...");
            IWebElement hasPreviousDocumentElement = this._browser.FindElement(
                By.Id(hasPreviousDocumentRadioId),
                this._defaultTimeout
            );
            
            this._logger.Debug("Getting RG form element...");
            IWebElement rgElement = this._browser.FindElement(
                By.Id("identificacaoCivilRgEnvio"),
                this._defaultTimeout
            );
            
            this._logger.Debug("Getting Has Expiry Date radio button element...");
            IWebElement hasExpiryDateElement = this._browser.FindElement(
                By.Id(expiryRadioId),
                this._defaultTimeout
            );
            
            this._logger.Information("Inputting the following data in the form: {FormData}", formData);
            fullNameElement.SendKeys(formData.FullName);
            fatherNameElement.SendKeys(formData.FatherName);
            motherNameElement.SendKeys(formData.MotherName);
            birthdayElement.SendKeys(formData.BirthDay);

            hasPreviousDocumentElement.Click();

            if (!formData.HasPreviousDocument) return;
            
            rgElement.SendKeys(formData.Rg);
            hasExpiryDateElement.Click();
        }

        private async Task SolveCaptcha()
        {
            this._logger.Debug("Getting Captcha key element...");
            IWebElement captchaKeyElement = this._browser.FindElement(
                By.XPath("//*[contains(@class, 'g-recaptcha')]"),
                this._defaultTimeout);

            string siteKey = captchaKeyElement.GetAttribute("data-sitekey");

            this._logger.Debug("Sending {key} to Anti-Captcha service...", siteKey);
            CreateTaskResponse? createTaskResponse = 
                await this._captchaService.SendCaptcha(siteKey);

            GetTaskResultResponse? getTaskResultResponse = null;
            
            if (createTaskResponse.TaskId != null)
            {
                getTaskResultResponse =
                    await this._captchaService
                        .PollCaptchaSolution(createTaskResponse.TaskId.Value);
            }

            if (getTaskResultResponse.RecaptchaSolution != null)
            {
                this._logger.Debug("Captcha solution: {id}", getTaskResultResponse.RecaptchaSolution.Id);
                
                this._logger.Information("Solving captcha...");
                var jsExecutor = (IJavaScriptExecutor) this._browser.Driver();
                jsExecutor.ExecuteScript(
                    $"document.getElementById('g-recaptcha-response').value = '{getTaskResultResponse.RecaptchaSolution.Id}';");
            }
        }

        private void SendForm()
        {
            this._logger.Debug("Getting Submit button element...");
            IWebElement submitButton = this._browser.FindElement(
                By.Id("btPesquisar"),
                this._defaultTimeout
            );
            
            this._logger.Information("Submitting form...");
            submitButton.Click();
        }

        private void CheckForNoSlotsError()
        {
            this._logger.Information("Verifying if there's still slots left...");
            IWebElement noSlotsErrorElement = this._browser.FindElement(
                By.Id("caixaExibicao"),
                this._defaultTimeout
            );

            if (noSlotsErrorElement.Text !=
                "Infelizmente, já atingimos o total de vagas disponíveis hoje. O agendamento foi encerrado e será reaberto amanhã."
            ) return;
            
            this._logger.Error("No more slots available. Try again tomorrow 😥");
            Environment.Exit(1);
        }
    }
}