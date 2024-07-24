using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Net.Http;
using Beamable;
using Newtonsoft.Json;
using Beamable.Server.Clients;
using static DeploymentResponse;
using UnityEngine.Events;

public class EdgegapManager : MonoBehaviour
{
    [SerializeField]
    string IPGetterAPI = "https://api.ipify.org/";

    //this variable should be set to true on the server and false otherwise
    public bool IsServer = false;

    public static EdgegapManager Instance;

    public Action<DeploymentResponse> OnLatestDeploymentFinished;
    public Action<DeploymentResponse> OnLatestDeploymentFailed;

    DeploymentResponse lastDeploymentResponse;

    public DeploymentResponse LastDeploymentResponse { get { return lastDeploymentResponse; } }

    bool CancelStatusChecking = false;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
    }
    /// <summary>
    /// Calls the Edgegap API to deploy a server
    /// </summary>
    /// <param name="clientIpList">A list of client IP addresses</param>
    /// <param name="environmentVariables">dictionary of values that will be sent to the deployed server as environment variables</param>
    public async Task<DeploymentResponse> Deploy(string[] clientIpList,Dictionary<string,string> environmentVariables)
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        EdgegapServiceClient edgegapServiceClient = new EdgegapServiceClient();

        lastDeploymentResponse = await edgegapServiceClient.DeployServer(clientIpList, environmentVariables);

        if (lastDeploymentResponse.Success)
        {
            CancelStatusChecking = false;
            lastDeploymentResponse.ports = JsonConvert.DeserializeObject<Dictionary<string, PortMap>>(LastDeploymentResponse.portsJson);
            StartCheckingForDeploymentStatus();
        }

        return lastDeploymentResponse;
    }

    private async void StartCheckingForDeploymentStatus()
    {
        while (true || CancelStatusChecking)
        {
            DeploymentResponse deploymentInfo = await GetDeploymentInfo(lastDeploymentResponse.request_id);
            deploymentInfo.ports = JsonConvert.DeserializeObject<Dictionary<string, PortMap>>(deploymentInfo.portsJson);

            if (deploymentInfo.Success)
            {
                if (deploymentInfo.current_status == "Status.READY")
                {
                    OnLatestDeploymentFinished?.Invoke(deploymentInfo);
                    lastDeploymentResponse = deploymentInfo;
                    break;
                }
                else if (deploymentInfo.current_status == "Status.Terminated" || deploymentInfo.current_status == "Status.ERROR")
                {
                    OnLatestDeploymentFailed?.Invoke(deploymentInfo);
                    lastDeploymentResponse = deploymentInfo;
                    break;
                }
            }
            await Task.Delay(1000);
        }
    }

    public async Task<bool> DeleteDeployment(string requestId)
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        EdgegapServiceClient edgegapServiceClient = new EdgegapServiceClient();

        bool deleteSuccess = await edgegapServiceClient.DeleteDeployment(requestId);
        if (deleteSuccess)
        {
            CancelStatusChecking = true;
        }

        return deleteSuccess;
    }

    public async Task<DeploymentStatus> GetDeploymentStatus(string requestId)
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        EdgegapServiceClient edgegapServiceClient = new EdgegapServiceClient();

        return await edgegapServiceClient.GetDeploymentStatus(requestId);
    }

    public async Task<DeploymentResponse> GetDeploymentInfo(string requestId)
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        EdgegapServiceClient edgegapServiceClient = new EdgegapServiceClient();

        return await edgegapServiceClient.GetDeploymentInfo(requestId);
    }

    public async Task<string> GetPublicIpAddress()
    {
        string ipAddress = "0.0.0.0";
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(IPGetterAPI);
            if (response != null && response.IsSuccessStatusCode) 
            {
                ipAddress = await response.Content.ReadAsStringAsync();
            }
        }
        return ipAddress;
    }


}
