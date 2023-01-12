using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xray.Hope.Service.XRayConfigs;
using Xray.Hope.Web.Shared;

namespace Xray.Hope.Service.Xui
{
    /// <summary>
    /// Facilitates communication with X-UI's APIs.
    /// Since X-UI authentication is based on cookies, we need to create one instance of this class per request in order to track and carry cookies.
    /// </summary>
    public class HopeXuiHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookies;
        private readonly ILogger<HopeXuiHttpClient> _logger;

        public HopeXuiHttpClient(ILogger<HopeXuiHttpClient> logger)
        {
            _cookies = new CookieContainer();
            var handler = new HttpClientHandler();
            handler.CookieContainer = _cookies;

            _httpClient = new HttpClient(handler);
            _logger = logger;
        }

        /// <summary>
        /// Adds the requested inbound config to X-UI.
        /// </summary>
        /// <param name="xuiConnectionOptions"></param>
        /// <param name="config"></param>
        /// <returns>Returns the link of the created config.</returns>
        public async Task<Result<XRayConfigLink>> CreateXRayConfigAsync(XuiConnectionOptions xuiConnectionOptions, XRayConfig config)
        {
            var loginSucceeded = await LoginAsync(xuiConnectionOptions);

            if (!loginSucceeded)
            {
                return Result<XRayConfigLink>.Failure("Failed to Login into X-UI", "XuiLoginFailed");
            }

            var xuiResponse = await CreateInboundAsync(xuiConnectionOptions, config);

            if (!xuiResponse.Success)
            {
                return Result<XRayConfigLink>.Failure(xuiResponse.Message, xuiResponse.ErrorType.ToString());
            }

            return Result<XRayConfigLink>.Success(config.Link);
        }

        /// <summary>
        /// Logins into X-UI and saves the cookies.
        /// </summary>
        /// <param name="xuiConnectionOptions"></param>
        /// <returns>Returns true if authentication is successful.</returns>
        public async Task<bool> LoginAsync(XuiConnectionOptions xuiConnectionOptions)
        {
            try
            {
                _logger.LogInformation("login into X-UI.");

                var loginUrl = $"{GetBaseAddress(xuiConnectionOptions)}/login";

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", xuiConnectionOptions.Username),
                    new KeyValuePair<string, string>("password", xuiConnectionOptions.Password)
                });

                var response = await _httpClient.PostAsync(loginUrl, formContent);

                if (_cookies.Count == 0)
                {
                    _logger.LogInformation("Failed to login into X-UI. Username and password might be incorrect.");
                }

                _logger.LogInformation("Successfully did login into X-UI.");

                return _cookies.Count != 0;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to login into X-UI.");
                return false;
            }
        }

        private async Task<XuiResponse> CreateInboundAsync(XuiConnectionOptions xuiConnectionOptions, XRayConfig config)
        {
            var addUrl = new Uri($"{GetBaseAddress(xuiConnectionOptions)}/xui/inbound/add/");

            var response = await _httpClient.PostAsync(
                addUrl,
                new StringContent(config.CreateXrayConfig(), Encoding.UTF8, "application/x-www-form-urlencoded"));

            return await response.Content.ReadFromJsonAsync<XuiResponse>();
        }

        private string GetBaseAddress(XuiConnectionOptions xuiConnectionOptions)
        {
            return $"http://{xuiConnectionOptions.Host}:{xuiConnectionOptions.Port}";
        }
    }
}
