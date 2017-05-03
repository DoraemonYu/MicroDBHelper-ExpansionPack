using MicroDBHelpers.ExpansionPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            var pdic_col = plist.Select(o => new { Properties = o, ColumnAttribute = Attribute.GetCustomAttribute(o, typeof(ColumnAttribute)) as ColumnAttribute })
                                .ToDictionary(o => o.Properties);
            //get dic<Propertie,IgnoreAttribute>
            var pdic_Ign = plist.Select(o => new { Properties = o, IgnoreAttribute = Attribute.GetCustomAttribute(o, typeof(IgnoreAttribute)) as IgnoreAttribute })
                                .ToDictionary(o => o.Properties);


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
                    if (colNamesCache.Contains(currentColumnName, 
                                               ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal) 
                                      == false)
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
