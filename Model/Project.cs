using Newtonsoft.Json;

namespace SipaaKernel.Builder.Model;

public class Project : ICloneable // for cloning project templates
{
    [JsonIgnore] public static Project CurrentProject;

    public string Copyright;
    public string Description;
    public string License;

    public string Name;
    public string[] PredefinedTargets;
    public Target[] Targets;

    public object Clone()
    {
        return MemberwiseClone();
    }

    public Project SecureClone()
    {
        return Clone() as Project;
    }

    public static Project LoadProject(string projectJson)
    {
        var p = JsonConvert.DeserializeObject<Project>(projectJson);
        return p;
    }
}