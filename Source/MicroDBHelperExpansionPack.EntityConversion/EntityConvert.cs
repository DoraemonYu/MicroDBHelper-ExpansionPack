using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
#if ASYNC_SUPPORT
using System.Linq;
using System.Threading.Tasks;
#endif

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
        

        /// <summary>
        /// Convert the "Entity List" to the "DataTable" object
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        /// <param name="list">Entity List</param>
        /// <returns>a DataTable Object</returns>
        public static DataTable ConvertToDatatable<T>(this IEnumerable<T> list)
                     where T : class
        {
            return list.ToDatatable();
        }

    }

}
