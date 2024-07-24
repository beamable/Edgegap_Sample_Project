

public struct DeploymentRequest
{
    public struct EnvVar
    {
        public EnvVar(string key, string val)
        {
            this.key = key;
            this.value = val;
        }

        public string key;
        public string value;
    }

    public string app_name;
    public string version_name;
    public bool is_public_app;
    public string[] ip_list;
    public EnvVar[] env_vars;
}