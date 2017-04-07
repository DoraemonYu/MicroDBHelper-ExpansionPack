using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDBHelpers.ExpansionPack
{
    /// <summary>
    /// Convertor
    /// </summary>
    public static class EntityConvert
    {
        /// <summary>
        /// Convert the "DataTable" object to the "Entity List" with target Type 
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        /// <param name="dt">DateTable object</param>
        /// <returns>a list of result</returns>
        public static IList<T> ConvertToList<T>(DataTable dt)
                     where  T : class
        {
            return dt.ToList<T>();
        }
    }

}
