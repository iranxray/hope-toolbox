using Microsoft.AspNetCore.Mvc;
using Xray.Hope.Service.Ssh;
using Xray.Hope.Service.Telnet;
using Xray.Hope.Web.Shared;
using Xray.Hope.Web.Shared.DTO.CheckServerConnection;

namespace Xray.Hope.Web.Server.Controllers
{
    /// <summary>
    /// Controls the interaction with the server.
    /// </summary>
    [ApiController]
    [Route("api/server")]
    public class ServerController : ControllerBase
    {
        private readonly HopeSshClient _sshClient;
        private readonly TelnetClient _telnetClient;
        private readonly ILogger<ServerController> _logger;

        public ServerController(
            HopeSshClient sshClient,
            TelnetClient telnetClient,
            ILogger<ServerController> logger)
        {
            _sshClient = sshClient;
            _telnetClient = telnetClient;
            _logger = logger;
        }

        /// <summary>
        /// Checks if ssh connection can be made to the given server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns the connection check results.</returns>
        [HttpPost("ssh/check")]
        public async Task<IActionResult> CheckServerConnectionAsync(CheckServerConnectionRequest request)
        {
            _logger.LogInformation("Check if ssh connection can be made to the requested server.");

            var isPortOpen = await _telnetClient.IsPortOpenAsync(request.ServerAddress, request.ServerSshPort);
            var canConnectSsh = !isPortOpen ? false : _sshClient.CanConnect(new SshConnectionOptions
            {
                Host = request.ServerAddress,
                Password = request.ServerPassword,
                Username = request.ServerUsername,
                Port = request.ServerSshPort,
                PrivateKeyContent = request.ServerPrivateKeyContent
            });

            _logger.LogInformation("Successfully checked if ssh connection can be made to the requested server. Port status is `{portStatus}` and can connect via ssh is `{sshStatus}`.",
                isPortOpen,
                canConnectSsh);

            var result = Result.Success(new CheckServerConnectionResponse
            {
                IsSshPortOpen = isPortOpen,
                CanConnectSsh = canConnectSsh,
            });

            return Ok(result);
        }

        /// <summary>
        /// Checks if the given port is open on the specified server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns the status of the port.</returns>
        [HttpPost("port/check")]
        public async Task<IActionResult> CheckXuiConnectionAsync(CheckPortRequest request)
        {
            _logger.LogInformation("Check if the given port `{port}` is open on the server.", request.Port);

            var isPortOpen = await _telnetClient.IsPortOpenAsync(request.Host, request.Port);

            _logger.LogInformation("Successfully checked if port is open on the server. Port status is `{portStatus}`.",
                isPortOpen);

            var result = Result.Success(new CheckPortResponse
            {
                IsPortOpen = isPortOpen,
            });

            return Ok(result);
        }
    }
}