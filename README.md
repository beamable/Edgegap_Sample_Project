# **Edgegap_Sample_Project**
This sample project demonstrates an integration between [Beamable](https://beamable.com/) and Edgegap that allows deploying a server hosted on Edgegap through Beamable C# MS.

### **Project configuration**

- <mark>Unity Target</mark> - PC
- <mark>Unity Version</mark> - 2022.3.11f1
- <mark>Beamable SDK version</mark> - 1.19.21
- <mark>Docker Desktop</mark> - https://www.docker.com/products/docker-desktop/
  
### **Project structure**

 - `README.md` - This README file
 - `Assets/` - Core files of the project
 - `Assets/Scripts/Edgegap` - Contains all the game scripts that are communicating with the C# MS
 - `Assets/Beamable/Microservices/EdgegapService` - Contains all the C# microservices classes that communicate with Edgegap API

### **Important files**

   - `Assets/Beamable/Microservices/EdgegapService/EdgegapAPIInterface.cs` - This is the class that communicates with Edgegap API
   - `Assets/Beamable/Microservices/EdgegapService/EdgegapService.cs` - The main C# microservice that acts as a wrapper between the microservice and Edgegap interface
   - `Assets/Beamable/Microservices/EdgegapService/EdgegapConfiguration.cs` - Configuration file for edgegap server
    
   - `Assets/Scripts/Edgegap/Edgegap Manager/EdgegapManager.cs` - Acts as an interface between the game scripts and the C# microservices
   - `Assets/Scripts/Edgegap/Edgegap Server/EdgegapServerAPIInterface.cs` - Used to make calls to the Edgegapi API from the deployed server itself
    
### **How to setup the project**

  1. Clone the repository locally and open the scene `EdgeGapExampleScene` inside of `Assets/Scenes/`
  2. Login with your beamable account in Unity: https://docs.beamable.com/docs/unity-editor-login
  3. Choose the realm that you want to work on.
  4. Build and push your server using Edegap unity plugin (included in the project)  : https://edgegap.com/integration/unity-plugin-quickstart
  5. Open `Assets/Beamable/Microservices/EdgegapService/EdgegapConfiguration.cs` file and put your Edgegap configurations there : the app name and app version (you can get them from Edgegap portal)
  6. Add Edgegap API token into the realm configuration, under the namespace "edgegap" and key value "apiToken", check here how to edit the realm configuration from Beamable portal : https://docs.venly.io/docs/configure-realm-config
  7. Make sure that you have docker setup and running on your PC.
  8. Run `Edgegap Service` Microservice, Check here how to run a microservice: https://docs.beamable.com/docs/microservices-guide
  9. Run the `EdgeGapExampleScene`, which contains `EdegapExample.cs`script, it will automatically deploy a server and print its IP and external Port
  
### **What is Beamable?**

Beamable is the low-code option for rapidly adding social, commerce, and content management features to your live game. Learn how to do that with Beamable's online product documentation.

[docs.beamable.com](docs.beamable.com)

### **Got feedback?**

Let us know what you think or ask any questions you might have.

[Contact Us](https://docs.beamable.com/discuss)
