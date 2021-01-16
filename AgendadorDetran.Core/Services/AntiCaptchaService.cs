using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using AgendadorDetran.Core.Data.Models;
using AgendadorDetran.Core.Interfaces;
using Serilog;

namespace AgendadorDetran.Core.Services
{
    public class AntiCaptchaService : IAntiCaptchaService
    {
        private const string SecretEnvVarName = "ANTI_CAPTCHA_SECRET";
        private const string BaseUrl = "https://api.anti-captcha.com/";

        private const string CreateTaskEndpoint = BaseUrl + "createTask";
        private const string GetTaskResultEndpoint = BaseUrl + "getTaskResult";

        private readonly string? _clientSecret;

        private readonly HttpClient _httpClient;
        private readonly IClock _clock;
        private readonly ILogger _logger;

        public AntiCaptchaService(HttpClient client, ILogger logger, IClock clock)
        {
            this._logger = logger;

            this._logger.Debug("Recovering client secret from environment variable '{var}'", SecretEnvVarName);
            this._clientSecret = Environment.GetEnvironmentVariable(SecretEnvVarName);

            if (string.IsNullOrWhiteSpace(this._clientSecret))
            {
                this._logger.Error("Client secret could not be found", SecretEnvVarName);

                throw new AuthenticationException(
                    $"Anti-Captcha client secret must be supplied as a environment variable called '{SecretEnvVarName}'");
            }

            this._clock = clock;
            this._httpClient = client;
        }

        public async Task<CreateTaskResponse> SendCaptcha(string siteKey)
        {
            var payload = new CreateTaskRequest
            {
                ClientKey = this._clientSecret,
                Task = new RecaptchaTask
                {
                    WebsiteKey = siteKey,
                }
            };

            this._logger.Information("Creating task to Anti-Captcha...", payload);
            this._logger.Debug("Sending request with payload: {Payload}", payload);

            HttpResponseMessage? response = await this._httpClient.PostAsync(
                CreateTaskEndpoint, new StringContent(JsonSerializer.Serialize(payload))
            );

            var taskResponse = await response.Content.ReadFromJsonAsync<CreateTaskResponse>();

            if (taskResponse != null)
            {
                this._logger.Debug("Received task id: {TaskID}", taskResponse.TaskId);

                this._logger.Information("Task created successfully!");
                return taskResponse;
            }

            this._logger.Error("Failed creating task to Anti-Captcha");
            throw new InvalidOperationException();
        }

        public async Task<GetTaskResultResponse> PollCaptchaSolution(int taskId)
        {
            TimeSpan newTick = TimeSpan.FromSeconds(2);

            this._logger.Debug("Replacing Clock tick for {tick}", newTick);
            TimeSpan oldTick = this._clock.Tick;
            this._clock.Tick = newTick;

            var payload = new GetTaskResultRequest
            {
                ClientKey = this._clientSecret,
                TaskId = taskId
            };

            HttpResponseMessage? response;
            GetTaskResultResponse? taskResponse = null;

            TimeSpan resultInitialDelay = TimeSpan.FromSeconds(8);

            this._logger.Information("Waiting {delay} before getting results...", resultInitialDelay);
            this._clock.StopFor(resultInitialDelay);

            TimeSpan elapsedTime = await this._clock.StopUntilConditionAsync(async () =>
                {
                    this._logger.Debug("Querying for results...");
                    response = await this._httpClient.PostAsync(
                        GetTaskResultEndpoint,
                        new StringContent(JsonSerializer.Serialize(payload))
                    );

                    taskResponse = await response.Content.ReadFromJsonAsync<GetTaskResultResponse>();
                    this._logger.Debug("Received task status: {Status}", taskResponse.Status);

                    return taskResponse?.Status == "ready";
                },
                TimeSpan.FromMinutes(1)
            );

            this._logger.Information(
                "Captcha solution is ready! It took {time}...", 
                elapsedTime + resultInitialDelay
                );

            this._logger.Debug("Setting Clock's old tick back");
            this._clock.Tick = oldTick;

            return taskResponse ?? throw new InvalidOperationException();
        }
    }
}