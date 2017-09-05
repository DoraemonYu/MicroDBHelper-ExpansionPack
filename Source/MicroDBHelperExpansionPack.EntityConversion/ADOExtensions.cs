using MicroDBHelpers.ExpansionPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Data
{
    /// <summary>
    /// Extensions for ADO Objects
    /// </summary>
    public static class ADOExtensions
    {
        /// <summary>
        /// Convert the "DataTable" object to the "Entity List" with target Type 
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        /// <param name="dt">DateTable object</param>
        /// <returns>a list of result</returns>
        public static IList<T> ToList<T>(this DataTable dt)
                     where  T : class
        {
            var list = new List<T>();

            //check is it NULL
            if (dt == null)
                return list;

            //get Properties
            var plist       = new List<PropertyInfo>(typeof(T).GetProperties());

            //get dic<Propertie,ColumnAttribute>
            var pdic_col    = plist.Select(o => new { Properties = o,   ColumnAttribute = Attribute.GetCustomAttribute(o,typeof(ColumnAttribute)) as ColumnAttribute })
                                   .ToDictionary(o => o.Properties );
            //get dic<Propertie,IgnoreAttribute>
            var pdic_Ign = plist.Select(o => new { Properties = o,      IgnoreAttribute = Attribute.GetCustomAttribute(o, typeof(IgnoreAttribute)) as IgnoreAttribute })
                                .ToDictionary(o => o.Properties);
            

            //loop to convert Properties and Values
            foreach (DataRow item in dt.Rows)
            {
                try
                {
                    T current = Activator.CreateInstance<T>();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        PropertyInfo info = null;

                        #region Get Target Property

                        /* You can overwrite this folded code, in order to simply combine your logic.
                            * For example, just use:
                                info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                            */


                        //Richer logic :
                        info = plist.FirstOrDefault(o =>
                        {
                            var colAtt = pdic_col[o].ColumnAttribute;
                            if (colAtt != null)
                            {
                                /* own ColumnAttribute, then use the rule from ColumnAttribute  */

                                bool ignoreCase = !colAtt.CaseSensitiveToMatchedName;
                                return String.Compare(dt.Columns[i].ColumnName , colAtt.MatchedName, ignoreCase) == 0;
                            }
                            else
                            {
                                /* Not own ColumnAttribute, then check "CaseSensitive" rule from EntityConversionDefaultSettings and "MatchedName" from  "Same as DataColumn::ColumnName" */

                                bool ignoreCase = !EntityConversionDefaultSettings.CaseSensitiveToColumnName;
                                return String.Compare(dt.Columns[i].ColumnName , o.Name, ignoreCase) == 0;
                            }
                        });

                        #endregion
                        
                        try
                        {
                            if (info != null && info.CanWrite)
                            {
                                #region Ignore this column Or Not

                                if ( pdic_Ign[info].IgnoreAttribute != null )
                                    continue;

                                #endregion

                                object rawValue = item[i];

                                //Set Value
                                if (rawValue != null && !Convert.IsDBNull(rawValue))
                                {
                                    //Support Nullable Type
                                    Type safeType = Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType;

                                    //Support Enum:
                                    if (safeType.IsEnum)
                                    {
                                        info.SetValue(current, Enum.Parse(safeType, rawValue.ToString()), null);
                                        continue;
                                    }

                                    //Normal:
                                    info.SetValue(current, Convert.ChangeType(rawValue, safeType), null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine(String.Format("## [MicroDBHelperExpansionPack.EntityConversion] System.Data.ADOExtensions::ToList<T> found an exception when convert [{0}] property, message is {1} ", info.Name, ex.Message));
                            continue;
                        }
                    }

                    list.Add(current);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            //return all result
            return list;
        }

    }
}
