using Beamable.Server;
using Edgegap.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beamable.Microservices
{
	[Microservice("EdgegapService")]
	public class EdgegapService : Microservice
	{
		[ClientCallable]
		public async Task<DeploymentResponse> DeployServer(string[] ipAddresses,Dictionary<string,string> environmentVariables)
		{
            var realmConfig = await Services.RealmConfig.GetRealmConfigSettings();
            EdgegapAPIInterface.Initialize(realmConfig);
            return await EdgegapAPIInterface.RequestDeployment(ipAddresses, environmentVariables);
        }
        [ClientCallable]
        public async Task<DeploymentStatus> GetDeploymentStatus(string requestId)
        {
            var realmConfig = await Services.RealmConfig.GetRealmConfigSettings();
            EdgegapAPIInterface.Initialize(realmConfig);
            return await EdgegapAPIInterface.GetDeploymentStatus(requestId);
        }

        [ClientCallable]    
        public async Task<DeploymentResponse> GetDeploymentInfo(string requestId)
        {
            var realmConfig = await Services.RealmConfig.GetRealmConfigSettings();
            EdgegapAPIInterface.Initialize(realmConfig);
            return await EdgegapAPIInterface.GetDeploymentinfo(requestId);
        }

        [ClientCallable]
        public async Task<bool> DeleteDeployment(string requestId)
        {
            var realmConfig = await Services.RealmConfig.GetRealmConfigSettings();
            EdgegapAPIInterface.Initialize(realmConfig);
            return await EdgegapAPIInterface.DeleteDeploymentAsync(requestId);
        }
    }
}
