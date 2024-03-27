using System.Reflection;
using Newtonsoft.Json;

namespace SipaaKernel.Builder;

/// <summary>
/// Subject to be removed. Will be implemented througth a SK-Build plugin.
/// </summary>
public class SKConfig
{
    [JsonIgnore]
    public static SKConfig? Current { get; set; } = null;

    public static bool Save()
    {
        try
        {
            if (Current != null) 
            {
                string json = JsonConvert.SerializeObject(Current);
                File.WriteAllText(Path.Join(Environment.CurrentDirectory, "skconfig.json"), json);
            }
            return true;
        }
        catch 
        {
            return false;
        }
    }

    public static bool Save(string to)
    {
        try
        {
            if (Current != null) 
            {
                string json = JsonConvert.SerializeObject(Current, Formatting.Indented);
                File.WriteAllText(to, json);
            }
            return true;
        }
        catch 
        {
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (!File.Exists(Path.Join(Environment.CurrentDirectory, "skconfig.json")))
            {
                Current = new();
                return true;
            }
            Current = JsonConvert.DeserializeObject<SKConfig>(File.ReadAllText(Path.Join(Environment.CurrentDirectory, "skconfig.json")));
            return true;
        }
        catch 
        {
            return false;
        }
    }

    public bool EnablePCIC { get; set; } = false;
    public bool LogsConIO { get; set; } = false;
    public string AdditionalCompileOptions { get; set; } = "";
}

public enum ConfigRequest
{
    Load,
    Save,
    ChangeKeyValue,
    GetKeyValue
}

public class ConfigResult
{
    public string response;
    public bool success;
    public Exception error;
}

public class PluginConfigInfo
{
    public Action saveConfig;
    public object configInstance;
    public Type configType;
}

public class ConfigManager
{
    private static string SbUserDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".skb");
    private static string SbUserConfig = Path.Combine(SbUserDir, "conf");

    private static void EnsureDirectories()
    {
        if (!Directory.Exists(SbUserDir))
            Directory.CreateDirectory(SbUserDir);
        
        if (!Directory.Exists(SbUserConfig))
            Directory.CreateDirectory(SbUserConfig);
    }

    public static bool TryFindConfig(string pluginName, out string output)
    {
        EnsureDirectories();

        string cfgFileName;
        
        if (!TryFindConfig(pluginName, out output))
            return false;
        return true;
    }

    public static object GetFieldValueInType(Type t, string fieldName, object instance)
    {
        FieldInfo? p = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (p != null)
            return p.GetValue(instance);

        return null;
    }

    public static void SetFieldValueInType(Type t, string fieldName, object instance, object newValue)
    {
        FieldInfo? p = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (p != null)
             p.SetValue(instance, newValue);
    }
}