using System.Threading.Tasks;
using AgendadorDetran.Core.Data.Models;

namespace AgendadorDetran.Core.Interfaces
{
    public interface IAntiCaptchaService
    {
        Task<CreateTaskResponse> SendCaptcha(string siteKey);
        Task<GetTaskResultResponse> PollCaptchaSolution(int taskId);
    }
}