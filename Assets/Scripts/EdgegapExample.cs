using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EdgegapExample : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        EdgegapManager.Instance.OnLatestDeploymentFinished += OnServerDeploymentSucceed;
        EdgegapManager.Instance.OnLatestDeploymentFailed += OnServerDeploymentFailed;

        if (!EdgegapManager.Instance.IsServer)
        {
            //Add player ips who are going to join the server
            string[] playerIds = { };
            ExampleDeployment(playerIds);
        }
    }

    async void ExampleDeployment(string[] playerIds)
    {
        //Get the ip for local player
        string myIP = await EdgegapManager.Instance.GetPublicIpAddress();

        // add the ips for other players that are going to join the server
        List<string> ips = new List<string>();
        ips.AddRange(playerIds);
        ips.Add(myIP);

        //request deployment for edgegap server
        DeploymentResponse response = await EdgegapManager.Instance.Deploy(ips.ToArray(), new Dictionary<string, string>());

        //The request id is what is needed for all the operations after that, such as checking the deployment status or deleting it.
        Debug.Log("Edgegap deployment request id " + response.request_id);

        //You can also access the latest deployment information from the manager itself
        Debug.Log("Edgegap deployment request id " + EdgegapManager.Instance.LastDeploymentResponse.request_id);

        //Example of deployment deletion : 
        //await EdgegapManager.Instance.DeleteDeployment(response.request_id);

        //Example of checking server status or getting server info
        //DeploymentStatus status = await EdgegapManager.Instance.GetDeploymentStatus(response.request_id);
        //DeploymentResponse serverInfo = await EdgegapManager.Instance.GetDeploymentInfo(response.request_id);
    }

    void OnServerDeploymentSucceed(DeploymentResponse deploymentResponse)
    {
        //Get server ip and port to connect players to
        string serverIp = deploymentResponse.public_ip;
        int serverPort = deploymentResponse.ports["Game Port"].external;

        Debug.Log($"Edgegap deployment succeeded, server ip : {serverIp}, server port {serverPort}");
    }
    void OnServerDeploymentFailed(DeploymentResponse deploymentResponse)
    {
        Debug.Log($"Edgegap deployment with request id : {deploymentResponse.request_id} failed");
    }
}
