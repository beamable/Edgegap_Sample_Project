using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class  EdgegapServerAPIInterface
{
    //This function should only be called from the deployed server to delete itself
    public static IEnumerator DeleteSelf()
    {
        var deleteUrl = Environment.GetEnvironmentVariable("ARBITRIUM_DELETE_URL");
        var token = Environment.GetEnvironmentVariable("ARBITRIUM_DELETE_TOKEN");
        if (deleteUrl == null || token == null)
        {
            Debug.LogError("[EdgegapServerAPIInterface] Error while deleting self deployment, Environment variables are not set, make sure this is called from the deployed server");
            yield break;
        }
       
        using (var req = UnityWebRequest.Delete(deleteUrl))
        {
            req.certificateHandler = new ByPassCertificate();
            req.SetRequestHeader("Authorization", token);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req.error);
            }
            else
            {
                Debug.Log("Deleting succeeded");
            }
        }
    }

    //This function should only be called from the deployed server to get its own public ip and port
    public static IEnumerator GetSelfIpAndPort(UnityAction<string, ushort> OnServerInfoRetrieved)
    {
        var contextUrl = Environment.GetEnvironmentVariable("ARBITRIUM_CONTEXT_URL");
        var token = Environment.GetEnvironmentVariable("ARBITRIUM_CONTEXT_TOKEN");
        if (contextUrl == null || token == null)
        {
            Debug.LogError("[EdgegapServerAPIInterface] Error while getting self ip and port, Environment variables are not set, make sure this is called from the deployed server");
            OnServerInfoRetrieved(null, 0);
            yield break;
        }


        using (var req = UnityWebRequest.Get(contextUrl))
        {
            req.SetRequestHeader("Authorization", token);
            req.certificateHandler = new ByPassCertificate();
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[EdgegapServerAPIInterface] Error getting self ip and port\n" + req.error);
                OnServerInfoRetrieved(null, 0);
            }
            else
            {
                var response = JsonConvert.DeserializeObject<DeploymentResponse>(req.downloadHandler.text);
                var port = response.ports.First().Value.external;
                var publicIp = response.public_ip;
                yield return new WaitForSeconds(1); //give it a second to make sure port is open
                OnServerInfoRetrieved(publicIp, (ushort)port);
            }
        }
    }
    class ByPassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
