namespace SipaaKernel.Builder.Model;

public class Target
{
    public Architecture Architecture;
    public string ASM;
    public string[] ASMFlags;
    public string CC;
    public string[] CCFlags;
    public Dictionary<string, object> CustomProperties;
    public string CXX;
    public string[] CXXFlags;
    public string Description;
    public string LD;
    public string[] LDFlags;
    public string LDScript;
    public string Name;
    public string OutputBinary;
}