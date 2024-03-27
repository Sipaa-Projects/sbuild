using System.Reflection;
using CommandLine;
using Newtonsoft.Json;
using SipaaKernel.Builder.Model;

namespace SipaaKernel.Builder;

[Verb("config", HelpText = "Configure a plugin")]
public class ConfigOptions
{
    [Option('k', "key", Required = true, HelpText = "The target config key")]
    public string Key { get; set; }

    [Option('s', "key", Required = false, HelpText = "The new value")]
    public string NewValue { get; set; } = "";
}

[Verb("new", HelpText = "Create a project in the current directory.")]
public class NewOptions
{
    [Option('t', "template", Required = true, HelpText = "The template that will be used to make the project.")]
    public string Template { get; set; }

    [Option('l', "license", Required = false, HelpText = "The license that will be used for the project.")]
    public string License { get; set; } = "MIT";

    [Option('c', "copyright", Required = false, HelpText = "The copyright text.")]
    public string Copyright { get; set; } = $"Copyright (C) {DateTime.Now.Year}-present My Author";

    [Option('d', "description", Required = false, HelpText = "The project's description")]
    public string Description { get; set; } = "This is my project built with Sipaa Builder";

    [Option("show-templates", Required = false, HelpText = "Show all the available templates")]
    public bool ShowTemplates { get; set; } = false;
}

[Verb("build", HelpText = "Build a project.")]
public class BuildOptions {
    //normal options here
    [Option('t', "target", Required = false, HelpText = "The target that SK-Build will use to build the project.")]
    public string Target { get; set; } = "none";

    [Option('l', "license", Required = false, HelpText = "The project's license.")]
    public string License { get; set; } = "MIT";

    [Option('c', "copyright", Required = false, HelpText = "The project's copyright text.")]
    public string Copyright { get; set; } = "Copyright (C) 2024-present My Author";

    [Option('d', "description", Required = false, HelpText = "The project's description.")]
    public string Description { get; set; } = "This is a basic application made with SK-Build.";
}

[Verb("clean", HelpText = "Clean all the output of a project.")]
public class CleanOptions
{
    //normal options here
}

[Verb("doctor", HelpText = "Check if the required packages are on your PC.")]
public class DoctorOptions {
    [Option('t', "toolchain", Required = false, HelpText = "The toolchain that SK-Build will use to build a project.")]
    public string Toolchain { get; set; } = "gnu";
}

public class SipaaBuildMain
{
    public static List<Project> ProjectTemplates = new();
    public static Dictionary<string, PluginConfigInfo> configInfos = new();
    
    public static void CallPluginEntries()
    {
        // Get all loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList<Assembly>();
        assemblies.Add(Assembly.GetExecutingAssembly());

        foreach (var assembly in assemblies)
        {
            //Console.WriteLine($"[PLOAD] Scanning assembly {assembly.FullName}");
            // Get all types from the assembly
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                //Console.WriteLine($"[PLOAD] Scanning type {type.FullName} in {assembly.FullName}");
                // Get all methods from the type
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (var method in methods)
                {
                    //Console.WriteLine($"[PLOAD] Scanning method {method.Name} in {type.FullName} in {assembly.FullName}");

                    // Check if the method has the FooAttribute
                    if (method.GetCustomAttributes(typeof(PluginEntryPointAttribute), true).Length > 0)
                    {
                        method.Invoke(null, new object[] {});
                        //Console.WriteLine($"[PLOAD] Found method '{method.Name}' with FooAttribute in class '{type.FullName}'");
                    }
                }
            }
        }
    }

    static bool IsCliAppInPath(string appName)
    {
        string pathVariable = Environment.GetEnvironmentVariable("PATH");
        string[] paths = pathVariable.Split(';');

        foreach (string path in paths)
        {
            string fullPath = System.IO.Path.Combine(path, appName);
            if (File.Exists(fullPath))
                return true;
        }

        return false;
    }
    static void CheckApp(string appName, string appDesc, List<string> appsToInstall)
    {
        if (IsCliAppInPath(appName))
            Console.WriteLine($"            * {appDesc} => available");
        else
        {
            Console.WriteLine($"            * {appDesc} => not available");
            appsToInstall.Add(appName);
        }
    }
    static int Doctor(DoctorOptions opts)
    {
        Console.WriteLine("[INFO] Running doctor...");
        List<string> ati = new();

        CheckApp("qemu-system-x86_64", "QEMU for x86_64", ati);
        CheckApp("qemu-system-i386", "QEMU for x86_64", ati);
        CheckApp("qemu-system-riscv64", "QEMU for x86_64", ati);
        CheckApp("qemu-system-aarch64", "QEMU for x86_64", ati);

        CheckApp("x86_64-elf-gcc", "GNU C Compiler for ELF x86_64", ati);
        CheckApp("x86_64-elf-g++", "GNU C++ Compiler for ELF x86_64", ati);
        CheckApp("x86_64-elf-ld", "GNU Linker for ELF x86_64", ati);
        CheckApp("i686-elf-gcc", "GNU C Compiler for ELF i686", ati);
        CheckApp("i686-elf-g++", "GNU C++ Compiler for ELF i686", ati);
        CheckApp("i686-elf-ld", "GNU Linker Compiler for ELF i686", ati);
        CheckApp("aarch64-elf-gcc", "GNU C Compiler for ELF AArch64", ati);
        CheckApp("aarch64-elf-g++", "GNU C++ Compiler for ELF AArch64", ati);
        CheckApp("aarch64-elf-ld", "GNU Linker Compiler for ELF AArch64", ati);
        CheckApp("riscv64-elf-gcc", "GNU C Compiler for ELF RISC-V 64", ati);
        CheckApp("riscv64-elf-g++", "GNU C++ Compiler for ELF RISC-V 64", ati);
        CheckApp("riscv64-elf-ld", "GNU Linker Compiler for ELF RISC-V 64", ati);

        CheckApp("clang", "LLVM C Compiler for ELF i686, RISC-V 64, x86_64 & AArch64", ati);
        CheckApp("clang++", "LLVM C++ Compiler for ELF i686, RISC-V 64, x86_64 & AArch64", ati);
        CheckApp("lld", "LLVM Linker for ELF i686, RISC-V 64, x86_64 & AArch64", ati);

        CheckApp("xorriso", "Build SipaaKernel's ISO image", ati);

        if (ati.Count > 0)
        {
            Console.WriteLine("[INFO] Not every tool required to build SK isn't present.");
            Console.WriteLine("[INST] Please install the following packages and try again :");
            foreach (string a in ati)
            {
                Console.WriteLine($"            * {a} : {DoctorCommentProvider.GetCommentForApp(a)}");
            }
        }
        else
        {
            Console.WriteLine("[INFO] All the tools are present.");
        }

        return 0;
    }
    static int Clean(CleanOptions opts)
    {
        if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, ".skb")))
            Directory.Delete(Path.Combine(Environment.CurrentDirectory, ".skb"), true);
        if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "obj-x86_64")))
            Directory.Delete(Path.Combine(Environment.CurrentDirectory, "obj-x86_64"), true);
        if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "obj-AArch64")))
            Directory.Delete(Path.Combine(Environment.CurrentDirectory, "obj-AArch64"), true);
        if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "obj-RiscV64")))
            Directory.Delete(Path.Combine(Environment.CurrentDirectory, "obj-RiscV64"), true);
        if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "obj-i686")))
            Directory.Delete(Path.Combine(Environment.CurrentDirectory, "obj-i686"), true);
        if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "obj-PowerPC")))
            Directory.Delete(Path.Combine(Environment.CurrentDirectory, "obj-PowerPC"), true);
        if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "output")))
            Directory.Delete(Path.Combine(Environment.CurrentDirectory, "output"), true);
        return 0;
    }
    
    static int New(NewOptions opts)
    {
        if (opts.ShowTemplates)
        {
            if (ProjectTemplates.Count == 0)
                Console.WriteLine("No templates are available. You can still create empty projects by entering whatever you want in the template command line argument.");
            else
            {
                Console.WriteLine("Available templates: ");
                foreach (var ptemp in ProjectTemplates)
                    Console.WriteLine($"{ptemp.Name}: {ptemp.Description}");
            }
        }
        else
        {
            if (File.Exists("project.json"))
            {
                Console.Write($"[?] A project already exists in '{Environment.CurrentDirectory}'. Do you want to overwrite it? (n) ");
                ConsoleKeyInfo k = Console.ReadKey();
                if (k.KeyChar != 'y')
                {
                    Console.WriteLine("[INFO] Cancelled.");
                    return 0;
                }
            }
            
            Project projectTemplate = new();
            bool foundTemplate = false;
            bool isEmptyProject = false;
            foreach (var i in ProjectTemplates)
            {
                if (i.Name == opts.Template)
                {
                    foundTemplate = true;
                    projectTemplate = i.SecureClone();
                    if (projectTemplate == null)
                    {
                        Console.WriteLine($"[WARN] Trying to clone project template '{i.Name}' failed. Creating an empty project...");
                        projectTemplate = new();
                        isEmptyProject = true;
                    }
                }
            }

            if (!foundTemplate)
            {
                Console.WriteLine($"[WARN] Cannot find any project template with name '{opts.Template}'. Creating an empty project...");
                projectTemplate = new();
                isEmptyProject = true;
            }
            
            DirectoryInfo di = new(Environment.CurrentDirectory);
            projectTemplate.Name = di.Name;
            projectTemplate.Description = opts.Description;
            projectTemplate.Copyright = opts.Copyright;
            projectTemplate.License = opts.License;
            projectTemplate.PredefinedTargets = new[] { "" };
            if (projectTemplate.Targets == null)
                projectTemplate.Targets = new Target[] {

                };

            File.WriteAllText("project.json", JsonConvert.SerializeObject(projectTemplate, Formatting.Indented));
            
            if (isEmptyProject)
                Console.WriteLine($"Created project '{di.Name}' in '{di.FullName}'.");
            else
                Console.WriteLine($"Created project '{di.Name}' in '{di.FullName}' from template '{opts.Template}'.");
        }
        return 0;
    }

    static int Config(ConfigOptions opts)
    {
        if (!string.IsNullOrEmpty(opts.Key))
        {
            string[] keySplit = opts.Key.Split('.');
            PluginConfigInfo pci = null;

            foreach (var v in configInfos)
            {
                if (v.Key == keySplit[0])
                {
                    pci = v.Value;
                    break;
                }
            }

            if (pci == null)
            {
                Console.WriteLine($"[FAIL] Key references an unknown plugin config entry '{keySplit[0]}'");
                return 1;
            }

            if (string.IsNullOrEmpty(opts.NewValue))
            {
                Console.WriteLine($"Value of '{opts.Key}': '{ConfigManager.GetFieldValueInType(pci.configType, keySplit[1], pci.configInstance).ToString()}'");
                return 0;
            }
            
            ConfigManager.SetFieldValueInType(pci.configType, keySplit[1], pci.configInstance, opts.NewValue);
            pci.saveConfig();
            Console.WriteLine($"Successfully set '{opts.Key}' to '{opts.NewValue}'");
            return 0;
        }
        
        Console.WriteLine("[FAIL] Key should doesn't be empty");
        return 1;
    }
    
    static int Main(string[] args)
    {
        SKConfig.Load();

        if (File.Exists("project.json"))
            Project.CurrentProject = Project.LoadProject(File.ReadAllText("project.json"));

        CallPluginEntries();

        return CommandLine.Parser.Default.ParseArguments<BuildOptions, NewOptions, CleanOptions, DoctorOptions, ConfigOptions>(args)
            .MapResult(
            (NewOptions opts) => New(opts),
            (DoctorOptions opts) => Doctor(opts),
            (BuildOptions opts) => Builder.Build(opts),
            (CleanOptions opts) => Clean(opts),
            (ConfigOptions opts) => Config(opts),
            errs => 1);
    }
}