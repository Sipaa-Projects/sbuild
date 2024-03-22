using Newtonsoft.Json;

namespace SipaaKernel.Builder.Model;

public class Project : ICloneable // for cloning project templates
{
    [JsonIgnore]
    public static Project CurrentProject;

    public string Name;
    public string Description;
    public string Copyright;
    public string License;
    public Target[] Targets;

    public object Clone() 
    {
        return this.MemberwiseClone();
    }
    
    public Project SecureClone() => Clone() as Project;
}