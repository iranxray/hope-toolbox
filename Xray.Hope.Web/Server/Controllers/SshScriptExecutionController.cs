using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Xray.Hope.Service.Ssh;
using Xray.Hope.Web.Server.Hubs;
using Xray.Hope.Web.Shared;
using Xray.Hope.Web.Shared.DTO.ExecuteScripts;

namespace Xray.Hope.Web.Server.Controllers
{
    /// <summary>
    /// Controls ssh script execution on a given server.
    /// </summary>

    [EnableRateLimiting("IP_BASED_RATE_LIMITER")]
    [ApiController]
    [Route("api/ssh/script")]
    public class SshScriptExecutionController : ControllerBase
    {
        private readonly IHubContext<ConsoleHub> _hubContext;
        private readonly HopeSshClient _sshClient;
        private readonly ILogger<SshScriptExecutionController> _logger;

        public SshScriptExecutionController(
            IHubContext<ConsoleHub> hubContext,
            HopeSshClient sshClient,
            ILogger<SshScriptExecutionController> logger)
        {
            _hubContext = hubContext;
            _sshClient = sshClient;
            _logger = logger;
        }

        // Runs the given script on the specified server.
        [HttpPost(template: "execute")]
        public async Task<IActionResult> ExecuteInstallScriptsAsync(ExecuteScriptsRequest request)
        {
            var timeoutInSeconds = 120;
            _logger.LogInformation("Execute the given script on the specified server via ssh. The timeout duration will be `{timeout}`", timeoutInSeconds);

            using var cancelationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSeconds));

            await _sshClient.ExecuteScriptAsync(new SshConnectionOptions
            {
                Host = request.ServerAddress,
                Password = request.ServerPassword,
                PrivateKeyContent = request.ServerPrivateKeyContent,
                Username = request.ServerUsername,
                Port = request.ServerSshPort
            },
            request.Script,
            (line) => _hubContext.Clients.Client(request.HubConnectionId).SendAsync(HopeHubMethod.ReceiveScriptExecutionOutput, line),
            () => _hubContext.Clients.Client(request.HubConnectionId).SendAsync(HopeHubMethod.FinishScriptExection),
            cancelationTokenSource.Token);

            return Ok();
        }
    }
}