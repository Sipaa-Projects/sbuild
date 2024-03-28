using System.Diagnostics;
using Newtonsoft.Json;
using SipaaKernel.Builder.Model;

namespace SipaaKernel.Builder.Plugins;

/// <summary>
///     DevKitPro integration for SK-Build, allowing the developement of homebrew for Nintendo consoles.
///     Supported platforms:
///         * Nintendo Wii U
/// </summary>
public static class NintendevPlugin
{
    private const string pluginName = "nintendev";

    private static void InitConfig()
    {
        string configPath;
        NintendevConfig nc = null;
        
        if (!ConfigManager.TryFindConfig(pluginName, out configPath))
        {
            nc = new NintendevConfig();
            // Try using default paths for all the platforms
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                nc.devKitProPath = "C:\\DevKitPro";
            else if (Environment.OSVersion.Platform == PlatformID.Unix) nc.devKitProPath = "/opt/devkitpro";
            
            File.WriteAllText(configPath, JsonConvert.SerializeObject(nc));
            InitConfig();
        }
        
        PluginConfigInfo pci = new();
        pci.saveConfig = () => { File.WriteAllText(configPath, JsonConvert.SerializeObject(pci.configInstance)); };
        pci.configType = typeof(NintendevConfig);
        pci.configInstance = nc;

        SipaaBuildMain.configInfos["nintendev"] = pci;
    }

    [PluginEntryPoint]
    private static void PMain() // Will be called when
    {
        var wiiu_converttorpx = true;

        InitConfig();

        Builder.PreBuild += (s, e) =>
        {
            if (e.CurrentBuildTarget.Name == "wiiu")
            {
                if (!Directory.Exists("/opt/devkitpro"))
                {
                    e.StopBuild = true;
                    e.StopBuildMessage =
                        "DevKitPro hasn't been installed to /opt/devkitpro. Please reinstall DKP to this location, or create a symlink.";
                }

                if (!File.Exists("/opt/devkitpro/tools/bin/elf2rpl"))
                {
                    Console.WriteLine(
                        "[WARN] elf2rpl isn't present on your system! You won't be able to convert ELF to RPX.");
                    wiiu_converttorpx = false;
                }
                else
                {
                    wiiu_converttorpx = true;
                }
            }
        };

        Builder.PostLink += (s, e) =>
        {
            if (e.CurrentBuildTarget.Name == "wiiu")
            {
                Console.WriteLine(
                    $"[WUT] Converting {e.CurrentBuildTarget.OutputBinary} to {e.CurrentBuildTarget.CustomProperties["WUT_WiiUOutRpxBin"] as string} using elf2rpl.");
                if (wiiu_converttorpx)
                    Process.Start("/opt/devkitpro/tools/bin/elf2rpl",
                        $"{e.CurrentBuildTarget.OutputBinary} {e.CurrentBuildTarget.CustomProperties["WUT_WiiUOutRpxBin"] as string}");
            }
        };
    }

    public class NintendevConfig
    {
        public string devKitProPath = "/opt/devkitpro";
    }
}