namespace SipaaKernel.Builder.Model;

/// <summary>
/// Applies to functions. Will be called before the command handler.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PluginEntryPointAttribute : Attribute
{

}