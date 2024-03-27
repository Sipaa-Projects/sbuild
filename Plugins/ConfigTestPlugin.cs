using SipaaKernel.Builder.Model;

namespace SipaaKernel.Builder.Plugins;

public class ConfigTestPlugin
{
    public class HelloWorldConfig
    {
        public string Testouille = "Hello config!";
    }
    
    [PluginEntryPoint]
    static void PMain()
    {
        HelloWorldConfig hwc = new();
        hwc.Testouille = "Hi";
        
        PluginConfigInfo pci = new();
        pci.saveConfig = () =>
        {
            Console.WriteLine("Config save requested");
        };
        pci.configType = typeof(HelloWorldConfig);
        pci.configInstance = hwc;

        SipaaBuildMain.configInfos["helloWorld"] = pci;
    }
}