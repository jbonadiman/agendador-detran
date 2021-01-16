using System.Net;
using AgendadorDetran.Core.Data.Models;
using AgendadorDetran.Core.Interfaces;

namespace AgendadorDetran.Core.Services
{
    public class AntiCaptchaService : IAntiCaptchaService
    {
        public string SendCaptcha(string siteKey)
        {
            var payload = new CaptchaParameters
            {
                ClientKey = "",
                Task = new CaptchaTask
                {
                    WebsiteKey = siteKey,
                    WebsiteUrl = ""
                }
            };
            
        }

        public string PollCaptchaSolution(string taskId)
        {
            throw new System.NotImplementedException();
        }
    }
}