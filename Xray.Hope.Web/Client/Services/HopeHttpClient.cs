using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using Xray.Hope.Web.Client.Models.Domain;
using Xray.Hope.Web.Shared;
using Xray.Hope.Web.Shared.DTO.CheckServerConnection;
using Xray.Hope.Web.Shared.DTO.CreateInstallScripts;
using Xray.Hope.Web.Shared.DTO.ExecuteScripts;
using Xray.Hope.Web.Shared.DTO.GetAvailableConfigs;
using Xray.Hope.Web.Shared.DTO.InstallTrojanConfig;
using Xray.Hope.Web.Shared.DTO.InstallXrayConfigRequest;

namespace Xray.Hope.Web.Client.Services
{
    /// <summary>
    /// Facilitates connection the backend.
    /// </summary>
    public class HopeHttpClient
    {
        private HttpClient _httpClient;

        public HopeHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets X-UI install scripts.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns the install script.</returns>
        public async Task<Result<CreateInstallScriptsResponse>> GenerateXuiInstallScripts(CreateInstallScriptsRequest request)
        {
            var response = await _httpClient.PostAsync(
                "api/xui/scripts",
                JsonContent.Create(request));

            if (response.StatusCode is (HttpStatusCode.OK or HttpStatusCode.BadRequest))
            {
                return await response.Content.ReadFromJsonAsync<Result<CreateInstallScriptsResponse>>();
            }

            response.EnsureSuccessStatusCode();

            return null;
        }

        /// <summary>
        /// Executes a bash script on the given server.
        /// </summary>
        /// <param name="request"></param>
        public async Task ExecuteScript(ExecuteScriptsRequest request)
        {
            var response = await _httpClient.PostAsync("api/ssh/script/execute", JsonContent.Create(request));

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Gets available XRay inbound configs.
        /// </summary>
        /// <returns>Returns the list of inbounds.</returns>
        public async Task<Result<GetAvailableConfigsResponse>> GetAvailableXRayConfigs()
        {
            var response = await _httpClient.GetAsync("api/xray/inbounds");

            if (response.StatusCode is (HttpStatusCode.OK or HttpStatusCode.BadRequest))
            {
                return await response.Content.ReadFromJsonAsync<Result<GetAvailableConfigsResponse>>();
            }

            response.EnsureSuccessStatusCode();

            return null;
        }

        /// <summary>
        /// Adds a given xray config to the specified server.
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="xuiConnection"></param>
        /// <param name="sshConnection"></param>
        /// <returns>Returns the link of the created config.</returns>
        public async Task<Result<InstallXRayConfigResponse>> InstallXRayConfig(
            string configId,
            XuiConnectionOptions xuiConnection,
            SshConnectionOptions sshConnection)
        {
            var response = await _httpClient.PostAsync(
                $"api/xui/install/inbound/{configId}",
                JsonContent.Create(new InstallXrayConfigRequest
                {
                    Host = xuiConnection.ServerAddress,
                    XuiPassword = xuiConnection.XuiPassword,
                    XuiPort = xuiConnection.XuiPort,
                    XuiUsername = xuiConnection.XuiUsername,
                    SShPassword = sshConnection.Password,
                    SshPort = sshConnection.Port,
                    SshPrivateKeyContent = sshConnection.PrivateKeyContent,
                    SshUsername = sshConnection.Username,
                    XrayConfigId = configId
                }));

            if (response.StatusCode is (HttpStatusCode.OK or HttpStatusCode.BadRequest))
            {
                return await response.Content.ReadFromJsonAsync<Result<InstallXRayConfigResponse>>();
            }

            response.EnsureSuccessStatusCode();

            return null;
        }

        public async Task<Result<CheckServerConnectionResponse>> CheckServerConnection(SshConnectionOptions serverConnection)
        {
            var response = await _httpClient.PostAsync("api/server/ssh/check", JsonContent.Create(new CheckServerConnectionRequest
            {
                ServerAddress = serverConnection.Host,
                ServerUsername = serverConnection.Username,
                ServerPassword = serverConnection.Password,
                ServerPrivateKeyContent = serverConnection.PrivateKeyContent,
                ServerSshPort = serverConnection.Port,
            }));

            if (response.StatusCode is (HttpStatusCode.OK or HttpStatusCode.BadRequest))
            {
                return await response.Content.ReadFromJsonAsync<Result<CheckServerConnectionResponse>>();
            }

            response.EnsureSuccessStatusCode();

            return null;
        }

        public async Task<Result<CheckPortResponse>> IsPortOpenAsync(string host, int port)
        {
            var response = await _httpClient.PostAsync("api/server/port/check", JsonContent.Create(new CheckPortRequest
            {
                Host = host,
                Port = port,
            }));

            if (response.StatusCode is (HttpStatusCode.OK or HttpStatusCode.BadRequest))
            {
                return await response.Content.ReadFromJsonAsync<Result<CheckPortResponse>>();
            }

            response.EnsureSuccessStatusCode();

            return null;
        }

        public async Task<Result<CheckXuiConnectionResponse>> CheckXuiConnection(XuiConnectionOptions xuiConnection)
        {
            var response = await _httpClient.PostAsync("api/xui/check", JsonContent.Create(new CheckXuiConnectionRequest
            {
                ServerAddress = xuiConnection.ServerAddress,
                XuiUsername = xuiConnection.XuiUsername,
                XuiPassword = xuiConnection.XuiPassword,
                XuiPort = xuiConnection.XuiPort
            }));

            if (response.StatusCode is (HttpStatusCode.OK or HttpStatusCode.BadRequest))
            {
                return await response.Content.ReadFromJsonAsync<Result<CheckXuiConnectionResponse>>();
            }

            response.EnsureSuccessStatusCode();

            return null;
        }
    }
}
