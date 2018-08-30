using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
