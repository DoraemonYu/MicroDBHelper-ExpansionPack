using MicroDBHelpers.ExpansionPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
#if ASYNC_SUPPORT
using System.Linq;
using System.Threading.Tasks;
#endif

namespace System.Data
{
    /// <summary>
    /// Extensions for IEnumerable collection
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Convert the "Entity List" to the "DataTable" object
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        /// <param name="list">Entity List</param>
        /// <returns>a DataTable Object</returns>
        public static DataTable ToDatatable<T>(this IEnumerable<T> list)
                                     where  T : class
        {
            //get Properties
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());

            //get dic<Propertie,ColumnAttribute>
            var pdic_col_raw    = LinqSearchAlternate.Select(plist, (o => new InformationForPropertyInfo { Properties = o, ColumnAttribute = Attribute.GetCustomAttribute(o, typeof(ColumnAttribute)) as ColumnAttribute }));
            var pdic_col        = LinqSearchAlternate.ToDictionary(pdic_col_raw,(o => o.Properties));
            //get dic<Propertie,IgnoreAttribute>
            var pdic_Ign_raw    = LinqSearchAlternate.Select(plist, (o => new { Properties = o, IgnoreAttribute = Attribute.GetCustomAttribute(o, typeof(IgnoreAttribute)) as IgnoreAttribute }));
            var pdic_Ign        = LinqSearchAlternate.ToDictionary(pdic_Ign_raw,(o => o.Properties));

            var dt              = new DataTable();
            dt.CaseSensitive    = true;

#region deal Columns

            var dic_colNames = new Dictionary<PropertyInfo, string>();
            {
                var colNamesCache   = new List<string>();

                foreach (var prop in plist)
                {
#region Get Target Property

                    /* You can overwrite this folded code, in order to simply combine your logic.
                     * For example, just use:
                           dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                     */


                    //Richer logic :

#region Ignore this column Or Not

                    if (pdic_Ign[prop].IgnoreAttribute != null)
                        continue;

#endregion

                    string currentColumnName; bool ignoreCase;
                    {
                        var colAtt = pdic_col[prop].ColumnAttribute;
                        if (colAtt != null)
                        {
                            /* own ColumnAttribute, then use the rule from ColumnAttribute  */

                            ignoreCase          = !colAtt.CaseSensitiveToMatchedName;
                            currentColumnName   = colAtt.MatchedName;
                        }
                        else
                        {
                            /* Not own ColumnAttribute, then check "CaseSensitive" rule from EntityConversionDefaultSettings and "MatchedName" from  "Property" */

                            ignoreCase          = !EntityConversionDefaultSettings.CaseSensitiveToColumnName;
                            currentColumnName   = prop.Name;
                        }
                    }


                    //##Add new Column to Datatable when it's not already exist 
#if NET20 || NET35 || NET40
                    bool isMatch;
                    if (ignoreCase)
                        isMatch = colNamesCache.Contains(currentColumnName);
                    else
                        isMatch = colNamesCache.FindIndex(x => x.Equals(currentColumnName, StringComparison.OrdinalIgnoreCase)) != -1;

                    if (isMatch == false)
#else

                    if (colNamesCache.Contains(currentColumnName, 
                                               ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal) 
                                      == false)
#endif
                    {
                        colNamesCache.Add(currentColumnName);
                        
                        dic_colNames[prop]  = currentColumnName;                        
                        dt.Columns.Add(currentColumnName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }

#endregion
                }
            }

#endregion


#region deal Rows

            //#check is it NULL
            if (list == null)
                return dt;

            foreach (T item in list)
            {
                List<object> values = new List<object>();

                foreach (var prop in dic_colNames.Keys)
                {
                    //Set Value
                    values.Add(prop.GetValue(item,null) ?? DBNull.Value);
                }

                dt.Rows.Add(values.ToArray());
            }

#endregion


            //#return all result
            return dt;
        }

    }
}
