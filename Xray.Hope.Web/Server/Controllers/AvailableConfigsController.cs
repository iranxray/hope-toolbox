using Microsoft.AspNetCore.Mvc;
using Xray.Hope.Service.XRayConfigs;
using Xray.Hope.Web.Shared;
using Xray.Hope.Web.Shared.DTO.GetAvailableConfigs;

namespace Xray.Hope.Web.Server.Controllers
{
    /// <summary>
    /// Controls cooked inbounds retrievals.
    /// </summary>
    [ApiController]
    [Route("api/xray/inbounds")]
    public class AvailableConfigsController : ControllerBase
    {
        private readonly ILogger<AvailableConfigsController> _logger;

        public AvailableConfigsController(
            ILogger<AvailableConfigsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns the list of cooked/pre-made inbound configs.
        /// </summary>
        /// <returns>The list of inbounds.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAvailableXRayConfigsAsync()
        {
            _logger.LogInformation("Return available inbound configs.");

            var xrayConfigs = new XRayConfigCollection().All.Select(config => new XrayConfig
            {
                Protocol = config.Protocol,
                Encryption = config.Encryption,
                Id = config.Id,
                Port = config.Port,
                Remark = config.Remark,
                Network = config.Network,
                Description = config.Description,
                Recommended = config.Recommended

            });

            var result = Result.Success(new GetAvailableConfigsResponse
            {
                VlessConfigs = xrayConfigs.Where(config => config.Protocol == XRayProtocol.VLESS),
                TrojanConfigs = xrayConfigs.Where(config => config.Protocol == XRayProtocol.Trojan),
            });

            return Ok(result);
        }
    }
}