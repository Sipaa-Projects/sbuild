namespace SipaaKernel.Builder.Model;

public class BuildEventArgs
{
    public Architecture ProjectArchitecture;
    public Target CurrentBuildTarget;
    public Project CurrentProject;
    public Dictionary<string,string> Variables;
    public bool StopBuild = false;
    public string StopBuildMessage = "";
}