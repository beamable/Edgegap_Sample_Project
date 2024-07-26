using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Beamable.Server.Api.RealmConfig;
using static Beamable.Common.Constants.Features;

namespace Edgegap.Tools
{
    public static partial class EdgegapAPIInterface
    {
        private static EdgegapConfiguration _settings;
        private static bool _initialized = false;

        private static string _apiToken;//To be added to in the project config from beamable portal3

        public static void Initialize(string apiToken)
        {
            _settings = new EdgegapConfiguration();

            _apiToken = apiToken;

            _initialized = true;
        }

        public static void Initialize(RealmConfig realmConfig)
        {
            _settings = new EdgegapConfiguration();

            _apiToken = realmConfig.GetSetting("edgegap", "apiToken");
            if (String.IsNullOrEmpty(_apiToken))
            {
                Debug.LogError("[EdgegapInterface] Edgegap token is not set in the realm config, make sure to add the API token in realm configuration, with namespace edgegap and key apiToken");
            }

            _initialized = true;
        }
         public static async Task<DeploymentResponse> RequestDeployment(string[] ipAddresses, Dictionary<string, string> envVars)
        {
            DeploymentResponse deploymentResponse = new DeploymentResponse();
            deploymentResponse.Success = false;

            if (!_initialized)
            {
                Debug.LogError("[EdgegapInterface] Attempting to use EdgegapInterface before initialization");
                return  new DeploymentResponse();
            }

            DeploymentRequest reqData = new DeploymentRequest()
            {
                app_name = _settings.AppName,
                version_name = _settings.Version,
                is_public_app = _settings.PublicApp,
                ip_list = ipAddresses,
                env_vars = envVars.Select(nvp => new DeploymentRequest.EnvVar(nvp.Key, nvp.Value)).ToArray()
            };

            string url = _settings.ApiEndpoint + _settings.ApiVersion + "/deploy";

            var reqDataString = JsonConvert.SerializeObject(reqData);
            var reqDataContent = new StringContent(reqDataString, Encoding.UTF8, "application/json");

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiToken);

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, reqDataContent);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    deploymentResponse = JsonConvert.DeserializeObject<DeploymentResponse>(responseBody);
                    deploymentResponse.portsJson = JsonConvert.SerializeObject(deploymentResponse.ports);
                    deploymentResponse.Success = true;
                    return deploymentResponse;
                }
                else
                {
                    Debug.LogError($"[EdgegapInterface] Deployment Error: {response.ReasonPhrase}");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.LogError(responseBody);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"[EdgegapInterface] Deployment request error: {e.Message}");
            }

            return deploymentResponse;
        }

        public static async Task<DeploymentStatus> GetDeploymentStatus(string deploymentRequestId)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiToken);

            try
            {
                HttpResponseMessage response = await client.GetAsync(_settings.ApiEndpoint + _settings.ApiVersion + "/status/" + deploymentRequestId);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    DeploymentResponse deploymentResponse = JsonConvert.DeserializeObject<DeploymentResponse>(responseBody);

                    if (deploymentResponse.current_status == "Status.INITIALIZING")
                    {
                        return DeploymentStatus.Initializing;
                    }
                    else if (deploymentResponse.current_status == "Status.SEEKING")
                    {
                        return DeploymentStatus.Seeking;
                    }
                    else if (deploymentResponse.current_status == "Status.SEEKED")
                    {
                        return DeploymentStatus.Seeked;
                    }
                    else if (deploymentResponse.current_status == "Status.SCANNING")
                    {
                        return DeploymentStatus.Scanning;
                    }
                    else if (deploymentResponse.current_status == "Status.DEPLOYING")
                    {
                        return DeploymentStatus.Deploying;
                    }
                    else if (deploymentResponse.current_status == "Status.READY")
                    {
                        return DeploymentStatus.Ready;
                    }
                    else if (deploymentResponse.current_status == "Status.TERMINATED")
                    {
                        return DeploymentStatus.Terminated;
                    }
                    else if (deploymentResponse.current_status == "Status.ERROR")
                    {
                        return DeploymentStatus.Error;
                    }

                }
                else
                {
                    Debug.LogError($"[EdgegapInterface] Getting deployment status Error: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"[EdgegapInterface] Getting deployment request Error:  {e.Message}");
            }

            return DeploymentStatus.RequestFailure;
        }

        public static async Task<DeploymentResponse> GetDeploymentinfo(string deploymentRequestId)
        {
            DeploymentResponse deploymentResponse = new DeploymentResponse();
            deploymentResponse.Success = false;

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", _apiToken);

            try
            {
                HttpResponseMessage response = await client.GetAsync(_settings.ApiEndpoint + _settings.ApiVersion + "/status/" + deploymentRequestId);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    deploymentResponse = JsonConvert.DeserializeObject<DeploymentResponse>(responseBody);
                    deploymentResponse.portsJson = JsonConvert.SerializeObject(deploymentResponse.ports);
                    deploymentResponse.Success = true;
                    return deploymentResponse;

                }
                else
                {
                    Debug.LogError($"[EdgegapInterface] Getting deployment status Error: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"[EdgegapInterface] Getting deployment request Error:  {e.Message}");
            }

            return deploymentResponse;
        }

        public static async Task<bool> DeleteDeploymentAsync(string requestId)
        {
            string url = _settings.ApiEndpoint + _settings.ApiVersion + $"/stop/{requestId}";

            using HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_settings.ApiEndpoint);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", _apiToken);
            HttpResponseMessage response = await client.DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }
    }
}