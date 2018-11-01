using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
#if NET45 || NET46
using System.Linq;
#endif

namespace MicroDBHelpers.ExpansionPack
{
    internal class InformationForPropertyInfo
    {
        public PropertyInfo Properties { get; set; }

        public ColumnAttribute ColumnAttribute { get; set; }
    }
}
