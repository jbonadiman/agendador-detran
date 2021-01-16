namespace AgendadorDetran.Core.Interfaces
{
    public interface IAntiCaptchaService
    {
        string SendCaptcha(string siteKey);
        string PollCaptchaSolution(string taskId);
    }
}