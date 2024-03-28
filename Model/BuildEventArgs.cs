namespace SipaaKernel.Builder.Model;

public class BuildEventArgs
{
    public Target CurrentBuildTarget;
    public Project CurrentProject;
    public Architecture ProjectArchitecture;
    public bool StopBuild = false;
    public string StopBuildMessage = "";
    public Dictionary<string, string> Variables;
}