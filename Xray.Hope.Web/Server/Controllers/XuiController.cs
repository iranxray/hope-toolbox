using Microsoft.AspNetCore.Mvc;
using Xray.Hope.Service.Ssh;
using Xray.Hope.Service.Telnet;
using Xray.Hope.Service.XRayConfigs;
using Xray.Hope.Service.Xui;
using Xray.Hope.Web.Shared;
using Xray.Hope.Web.Shared.DTO.CheckServerConnection;
using Xray.Hope.Web.Shared.DTO.CreateInstallScripts;
using Xray.Hope.Web.Shared.DTO.InstallTrojanConfig;
using Xray.Hope.Web.Shared.DTO.InstallXrayConfigRequest;
using XuiConnectionOptions = Xray.Hope.Service.Xui.XuiConnectionOptions;

namespace Xray.Hope.Web.Server.Controllers
{
    /// <summary>
    /// Controls the interaction with the X-UI instance.
    /// </summary>
    [ApiController]
    [Route("api/xui/")]
    public class XuiController : ControllerBase
    {
        private readonly XuiInstallScriptGenerator _xuiInstallScriptGenerator;
        private readonly HopeXuiHttpClient _xuiHttpClient;
        private readonly HopeSshClient _sshClient;
        private readonly ILogger<XuiController> _logger;
        private readonly TelnetClient _telnetClient;


        public XuiController(
            XuiInstallScriptGenerator xuiInstallScriptGenerator,
            HopeXuiHttpClient xuiHttpClient,
            HopeSshClient sshClient,
            TelnetClient telnetClient,
            ILogger<XuiController> logger)
        {
            _xuiInstallScriptGenerator = xuiInstallScriptGenerator;
            _xuiHttpClient = xuiHttpClient;
            _sshClient = sshClient;
            _telnetClient = telnetClient;
            _logger = logger;
        }

        /// <summary>
        /// Generates X-UI install scripts.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns the script.</returns>
        [HttpGet("scripts")]
        public async Task<IActionResult> GenerateInstallScriptsAsync([FromQuery] CreateInstallScriptsRequest request)
        {
            _logger.LogInformation("Generate X-UI install scripts.");

            var scripts = _xuiInstallScriptGenerator.Generate(request.ServerAddress, new XuiConnectionOptions
            {
                Port = int.Parse(request.XuiPort),
                Username = request.XuiUsername,
                Password = request.XuiPassword
            });

            _logger.LogInformation("Successfully generated X-UI install script.");

            var result = Result.Success(new CreateInstallScriptsResponse
            {
                Script = scripts
            });

            return Ok(result);
        }

        /// <summary>
        /// Installs inbound xray config via calling X-UI APIs and opens the associated port on the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns the status.</returns>
        [HttpPost("install/inbound/{configId}")]
        public async Task<IActionResult> InstallXRayConfigAsync(InstallXrayConfigRequest request)
        {
            var domainName = request.Host;
            var isPortOpen = true;

            var xrayConfig = new XRayConfigCollection().All
                .FirstOrDefault(config => config.Id == request.XrayConfigId);

            if (xrayConfig is null)
            {
                _logger.LogInformation("Failed to find the config.");
                return NotFound();
            }

            _logger.LogInformation("Install inbound config of protocol `{protocol}` with encryption `{encryption}` and network `{network}` on port {port}.",
                xrayConfig.Protocol,
                xrayConfig.Encryption,
                xrayConfig.Network,
                xrayConfig.Port);

            var response = await _xuiHttpClient.CreateXRayConfigAsync(new XuiConnectionOptions
            {
                Host = request.Host,
                Port = request.XuiPort,
                Password = request.XuiPassword,
                Username = request.XuiUsername,
            }, xrayConfig with
            {
                Sni = domainName,
                PrivateKeyFile = $"/etc/letsencrypt/live/{domainName}/fullchain.pem",
                PublicKeyFile = $"/etc/letsencrypt/live/{domainName}/privkey.pem",
                Host = string.IsNullOrEmpty(xrayConfig.Host) ? domainName : xrayConfig.Host,
                ClientId = Guid.NewGuid().ToString(),
            });

            if (response.IsSuccess)
            {
                _logger.LogInformation("Successfully Installed inbound config of protocol `{protocol}` with encryption `{encryption}` and network `{network}` on port {port}.",
                   xrayConfig.Protocol,
                   xrayConfig.Encryption,
                   xrayConfig.Network,
                   xrayConfig.Port);

                var sshConnection = new SshConnectionOptions
                {
                    Host = request.Host,
                    Port = request.SshPort,
                    Password = request.SShPassword,
                    PrivateKeyContent = request.SshPrivateKeyContent,
                    Username = request.SshUsername,
                };

                try
                {
                    _logger.LogInformation("Open port {port} on server to allow inbound traffic.", xrayConfig.Port);

                    var script = "sudo -i;" + Environment.NewLine + $"ufw allow {xrayConfig.Port}/tcp;";
                    await _sshClient.ExecuteScriptAsync(sshConnection, script);


                    _logger.LogInformation("Successfully opened port {port} on server to allow inbound traffic.", xrayConfig.Port);
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Failed to open port {port} on server to allow inbound traffic.", xrayConfig.Port);
                    isPortOpen = false;
                }
            }
            else
            {
                _logger.LogInformation("Failed to Install inbound config of protocol `{protocol}` with encryption `{encryption}` and network `{network}` on port {port}.",
                   xrayConfig.Protocol,
                   xrayConfig.Encryption,
                   xrayConfig.Network,
                   xrayConfig.Port);
            }

            return ActionResultFactory.Create(response, (response) => new InstallXRayConfigResponse
            {
                ConfigText = response.Value.Value,
                IsPortOpen = isPortOpen,
            });
        }

        /// <summary>
        /// Checks if the provided x-ui connection params are valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns if x-ui is accessible.</returns>
        [HttpPost("check")]
        public async Task<IActionResult> CheckXuiConnectionAsync(CheckXuiConnectionRequest request)
        {
            _logger.LogInformation("Connect to X-UI using the given the params.");

            var isPortOpen = await _telnetClient.IsPortOpenAsync(request.ServerAddress, request.XuiPort);
            var canLogin = !isPortOpen ? false : await _xuiHttpClient.LoginAsync(new XuiConnectionOptions
            {
                Host = request.ServerAddress,
                Password = request.XuiPassword,
                Username = request.XuiUsername,
                Port = request.XuiPort,
            });

            _logger.LogInformation("Successfully connected to X-UI using the given the params.");

            var result = Result.Success(new CheckXuiConnectionResponse
            {
                IsXuiPortOpen = isPortOpen,
                CanLogin = canLogin,
            });

            return Ok(result);
        }
    }
}