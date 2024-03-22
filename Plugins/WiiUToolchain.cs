using System.Diagnostics;
using SipaaKernel.Builder.Model;

namespace SipaaKernel.Builder.Plugins;

/// <summary>
/// DevKitPro's WUT integration for SK-Build.
/// </summary>
public static class WiiUToolchainPlugin
{
    [PluginEntryPoint]
    static void PMain() // Will be called when
    {
        bool wiiu_converttorpx = true;
        Builder.PreBuild += new((s,e) => {
            if (e.CurrentBuildTarget.Name == "wiiu")
            {
                if (!Directory.Exists("/opt/devkitpro"))
                {
                    e.StopBuild = true;
                    e.StopBuildMessage = "DevKitPro hasn't been installed to /opt/devkitpro. Please reinstall DKP to this location, or create a symlink.";
                }
                if (!File.Exists("/opt/devkitpro/tools/bin/elf2rpl"))
                {
                    Console.WriteLine("[WARN] elf2rpl isn't present on your system! You won't be able to convert ELF to RPX.");
                    wiiu_converttorpx = false;
                }
                else
                    wiiu_converttorpx = true;
            }
        });

        Builder.PostLink += new((s,e) => {
            if (e.CurrentBuildTarget.Name == "wiiu")
            {
                Console.WriteLine($"[WUT] Converting {e.CurrentBuildTarget.OutputBinary} to {e.CurrentBuildTarget.CustomProperties["WUT_WiiUOutRpxBin"] as string} using elf2rpl.");
                if (wiiu_converttorpx)
                    Process.Start("/opt/devkitpro/tools/bin/elf2rpl", $"{e.CurrentBuildTarget.OutputBinary} {e.CurrentBuildTarget.CustomProperties["WUT_WiiUOutRpxBin"] as string}");
            }
        });
    }
}