using System;
using System.Collections.Generic;
using System.Text;
#if ASYNC_SUPPORT
using System.Linq;
using System.Threading.Tasks;
#endif

namespace MicroDBHelpers.ExpansionPack
{
    /// <summary>
    /// Ignore control for "Column" for "Property of Entity Class"
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class IgnoreAttribute : Attribute
    {
    }

}
