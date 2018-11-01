/* ExtensionMethod was began since C#3.0.
 * Add this Attribute to make it work.
 */

#if NET20
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// ExtensionMethod support For .Net 2.0
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
         | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}
#endif