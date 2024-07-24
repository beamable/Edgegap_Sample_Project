using System.Collections.Generic;

public struct DeploymentResponse
{
    public struct Location
    {
        public string city;
        public string country;
    }

    public struct PortMap
    {
        public int external;
        public int @internal;
        public string protocol;
    }

    public string request_id;
    public string request_dns;
    public string fqdn;
    public string public_ip;
    public string current_status;
    public bool running;
    public Location location;
    public Dictionary<string, PortMap> ports;
  

    // Added Field
    public bool Success;
    public string portsJson;
}
public struct CreateTagResponse
{
    public string name;
    public string create_time;
    public string last_updated;
}

public enum DeploymentStatus
{
    Initializing,
    Seeking,
    Seeked,
    Scanning,
    Deploying,
    Ready,
    Terminated,
    Error,
    RequestFailure
}

