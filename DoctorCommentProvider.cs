namespace SipaaKernel.Builder;

internal class DoctorCommentProvider
{
    public static string GetCommentForApp(string appName)
    {
        if (IsCompilerOrLinker(appName))
        {
            if (IsLLVMToolchain(appName))
                return "It isn't necessary, but you need it to build with LLVM.";
            if (IsGNUToolchain(appName))
                return
                    $"It isn't necessary, but you need it to build for {GetArchitecture(appName)} with the GNU toolchain.";
            return "At least one compiler/linker is required to build.";
        }

        if (IsRequiredApp(appName))
            return "This app is required.";
        return "No specific comment for this app.";
    }

    public static bool IsCompilerOrLinker(string appName)
    {
        // You might need to adjust this logic based on your specific criteria
        return appName.EndsWith("-gcc") || appName.EndsWith("-g++") || appName.EndsWith("-ld") ||
               appName.Contains("clang") || appName.Contains("clang++") || appName.Contains("lld");
    }

    public static bool IsGNUToolchain(string appName)
    {
        // You might need to adjust this logic based on your specific criteria
        return appName.Contains("gcc") || appName.Contains("g++") || appName.Contains("ld");
    }

    public static bool IsLLVMToolchain(string appName)
    {
        // You might need to adjust this logic based on your specific criteria
        return appName.Contains("clang") || appName.Contains("clang++") || appName.Contains("lld");
    }

    public static bool IsRequiredApp(string appName)
    {
        // You might need to adjust this logic based on your specific criteria
        return appName.Equals("make", StringComparison.OrdinalIgnoreCase) ||
               appName.Equals("xorriso", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetArchitecture(string appName)
    {
        if (appName.Contains("x86_64"))
            return "x86_64";
        if (appName.Contains("aarch64"))
            return "AArch64";
        if (appName.Contains("i386") || appName.Contains("i686"))
            return "i386/i686";
        if (appName.Contains("riscv64"))
            return "RISC-V 64";
        return "unknown";
    }
}