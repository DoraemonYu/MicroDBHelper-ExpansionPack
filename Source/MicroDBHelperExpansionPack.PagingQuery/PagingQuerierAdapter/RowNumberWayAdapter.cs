using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
#if ASYNC_SUPPORT
using System.Linq;
using System.Threading.Tasks;
#endif
using static MicroDBHelpers.ExpansionPack.PagingQuerier;

namespace MicroDBHelpers.ExpansionPack
{
    /// <summary>
    /// use way of [RowNumber()] (require SqlServer 2005+)
    /// </summary>
    internal class RowNumberWayAdapter : IAdapter
    {

        #region Interfaces

        public DetailPagingRet DetailPaging(ExecuteDelegate executeAction,
                                                    int pageIndex, int pageSize,
                                                    string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                    CommandType commandType = CommandType.Text
                                                    )
        {
            //##0 init
            string SELECTSQL, orderBodyString, SELECTWithoutOrder, fromBodyString, FINALSQL;
            int totalCount      = 0;
            var paras           = new List<SqlParameter>();

            //##1 Check
            DealHelper.DetailPagingHelper_Prepare(selectSql, out SELECTSQL);

            try
            {
                DealHelper.DetailPagingHelper_SplitStrings(SELECTSQL, out orderBodyString, out SELECTWithoutOrder, out fromBodyString);

                string sqlCount;
                DetailPagingHelper_SqlCount(fixedSql, SELECTSQL, SELECTWithoutOrder, fromBodyString, orderBodyString, out sqlCount);

                //##2 get record total and begin to page
                DataTable countDt = executeAction(sqlCount, paramValues, commandType);
                DetailPagingHelper_SqlPage(countDt, paramValues, orderBodyString, SELECTWithoutOrder, fixedSql,
                                           pageIndex, pageSize,
                                           ref SELECTSQL, ref paras,
                                           out totalCount, out FINALSQL);


                //##3 exec sql expreession and return result
                DataTable querydt = executeAction(FINALSQL, paras.ToArray(), commandType);
                return DetailPagingHelper_returnResult(querydt, totalCount);
            }
            catch (SqlException ex)
            {
                throw DealHelper.DetailPagingHelper_CatchException_SqlException(ex, fixedSql, SELECTSQL);
            }
            catch (Exception ex)
            {
                throw DealHelper.DetailPagingHelper_CatchException_CommonException(ex, fixedSql, SELECTSQL, totalCount);
            }
        }

#if ASYNC_SUPPORT
        /// <summary>
        /// Detail to PagingAsync
        /// </summary>
        public async Task<DetailPagingRet> DetailPagingAsync(ExecuteAsyncDelegate executeAction,
                                                                     int pageIndex, int pageSize,
                                                                     string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                                     CommandType commandType = CommandType.Text
                                                                     )
        {
            //##0 init
            string SELECTSQL, orderBodyString, SELECTWithoutOrder, fromBodyString, FINALSQL;
            int totalCount      = 0;
            var paras           = new List<SqlParameter>();

            //##1 Check
            DealHelper.DetailPagingHelper_Prepare(selectSql, out SELECTSQL);

            try
            {
                DealHelper.DetailPagingHelper_SplitStrings(SELECTSQL, out orderBodyString, out SELECTWithoutOrder, out fromBodyString);

                string sqlCount;
                DetailPagingHelper_SqlCount(fixedSql, SELECTSQL, SELECTWithoutOrder, fromBodyString, orderBodyString, out sqlCount);

                //##2 get record total and begin to page
                DataTable countDt = await executeAction(sqlCount, paramValues, commandType);
                DetailPagingHelper_SqlPage(countDt, paramValues, orderBodyString, SELECTWithoutOrder, fixedSql,
                                           pageIndex, pageSize,
                                           ref SELECTSQL, ref paras,
                                           out totalCount, out FINALSQL);


                //##3 exec sql expreession and return result
                DataTable querydt = await executeAction(FINALSQL, paras.ToArray(), commandType);
                return DetailPagingHelper_returnResult(querydt, totalCount);
            }
            catch (SqlException ex)
            {
                throw DealHelper.DetailPagingHelper_CatchException_SqlException(ex, fixedSql, SELECTSQL);
            }
            catch (Exception ex)
            {
                throw DealHelper.DetailPagingHelper_CatchException_CommonException(ex, fixedSql, SELECTSQL, totalCount);
            }
        }
#endif

        #endregion

        #region DetailPaging Helpers


        private static void DetailPagingHelper_SqlCount(string fixedSql, string SELECTSQL, string SELECTWithoutOrder, string fromBodyString, string orderBodyString,
                                                        out string sqlCount)
        {
            bool hasOrderBy = !String.IsNullOrEmpty(orderBodyString);

            //sqlCount
            if (!DealHelper.hasDistinct(SELECTSQL))
            {//## no DISTINCT
                sqlCount     = "SELECT COUNT(1) " + fromBodyString;
            }
            else
            {//## has DISTINCT
                if (hasOrderBy)
                    sqlCount = "SELECT COUNT(1) FROM ( " + SELECTWithoutOrder + ") as ___temp___sqlCount___";
                else
                    sqlCount = "SELECT COUNT(1) FROM ( " + SELECTSQL + ") as ___temp___sqlCount___";
            }

            //----processing fixed sql----
            sqlCount = fixedSql + "\r\n" + sqlCount;
            //----------------------------   
        }

        private static void DetailPagingHelper_SqlPage(DataTable countDt, SqlParameter[] paramValues, string orderBodyString, string SELECTWithoutOrder, string fixedSql, int pageIndex, int pageSize,
                                                       ref string SELECTSQL, ref List<SqlParameter> paras,
                                                       out int totalCount, out string FINALSQL)
        {
            if (countDt.Rows.Count > 0)
            {
                totalCount = Convert.ToInt32(countDt.Rows[0][0]);
            }
            else
            {
                totalCount = 0;
            }

            //Search by paging
            bool hasOffset = (pageIndex != 1);

            //create limit sql expression
            SELECTSQL = GetLimitSql(SELECTSQL, pageIndex, pageSize, orderBodyString, SELECTWithoutOrder);

            //add pading parameter

            if (paramValues != null && paramValues.Length > 0)
                paras.AddRange(paramValues);

            if (hasOffset)
            {
                paras.Add(new SqlParameter("@x_rownum_from", ((pageIndex - 1) * pageSize + 1)));
                paras.Add(new SqlParameter("@x_rownum_to", pageSize * pageIndex));
            }
            else
            {
                paras.Add(new SqlParameter("@x_rownum_to", pageSize * pageIndex));
            }

            //----processing fixed sql----
            FINALSQL = fixedSql + "\r\n" + SELECTSQL;
            //----------------------------
        }

        private static DetailPagingRet DetailPagingHelper_returnResult(DataTable querydt, int totalCount)
        {
            //drop the [rownumber] column, which to only for paging
            if (querydt != null)
                querydt.Columns.Remove("___rownumber___");

            //return the result
            return new DetailPagingRet
            {
                querydt = querydt,
                totalCount = totalCount
            };
        }
        

        /// <summary>
        /// Create limit sql
        /// </summary>
        /// <param name="sql">sql expression</param>
        /// <param name="pageIndex">the index of page</param>
        /// <param name="pageSize">the size of each page</param>
        /// <param name="orderBodyString">main body of order-by-sql</param>
        /// <param name="SELECTWithoutOrder">sql without order-by-sql</param>
        private static string GetLimitSql(
            string sql,
            int pageIndex,
            int pageSize,
            string orderBodyString, string SELECTWithoutOrder
            )
        {

            bool hasOffset = (pageIndex != 1);
            int startOfSelect = sql.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase);


            var pagingSelect = new StringBuilder(sql.Length + 100);

            pagingSelect.Append("SELECT * FROM ( SELECT ");                             // nest the main query in an outer select
            pagingSelect.Append(GetRowNumber(sql, orderBodyString, SELECTWithoutOrder));     // add the rownnumber bit into the outer query select list

            //make to support "DISTINCT"
            {
                pagingSelect.Append(" row_.* FROM ( ")                                  // add another (inner) nested select
                        .Append(SELECTWithoutOrder.Substring(startOfSelect))            // add the main query
                        .Append(" ) as row_");                                          // close off the inner nested select
            }

            pagingSelect.Append(" ) as ___temp___limitSql___ where ___rownumber___ ");

            //add the restriction to the outer select
            if (hasOffset)
            {
                pagingSelect.Append("between @x_rownum_from and @x_rownum_to");
            }
            else
            {
                pagingSelect.Append("<= @x_rownum_to");
            }

            return pagingSelect.ToString();
        }
        

        /// <summary>
        /// Get number of row
        /// </summary>
        /// <param name="sql">sql expression</param>
        /// <param name="orderBodyString">main body of order-by-sql</param>
        /// <param name="sqlWithoutOrder">sql without order-by-sql</param>
        private static string GetRowNumber(string sql, string orderBodyString, string sqlWithoutOrder)
        {
            StringBuilder rownumber = new StringBuilder(50).Append("ROW_NUMBER() OVER( ");

            bool hasOrderBy = !String.IsNullOrEmpty(orderBodyString.Trim());

            if (hasOrderBy)
            {
                Helper_GetRowNumberHelper(rownumber, orderBodyString, sqlWithoutOrder);
            }
            else
            {
                rownumber.Append(" ORDER BY (SELECT NULL) ");
            }

            rownumber.Append(") as ___rownumber___,");

            return rownumber.ToString();
        }

        #region especial logic for DISTINCT

        static Regex regex_fieldNameMapping     = new Regex(@"(?<field>[^\s,]+)\s+as\s+(?<alias>[^\s,]+)", RegexOptions.IgnoreCase);
        static Regex regex_fieldOrderby         = new Regex(@"[\s,](?<field>[^\s,]+)(\s+(?<direction>(asc)|(desc)))*", RegexOptions.IgnoreCase);
        static Regex regex_minimumFieldName     = new Regex(@"((\.)*)(?<name>[^\s,\.]+)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// change the SELECT FIELD name if has 'AS' 
        /// </summary>
        /// <param name="rownumber"></param>
        /// <param name="orderString"></param>
        /// <param name="sqlWithoutOrder"></param>
        private static void Helper_GetRowNumberHelper(StringBuilder rownumber, string orderString, string sqlWithoutOrder)
        {
            string pureSELECTPart = sqlWithoutOrder.Substring(6, sqlWithoutOrder.IndexOf("FROM ", StringComparison.OrdinalIgnoreCase) - 5);

            //Create mapping
            Dictionary<string, string> dic_fieldNameMapping = new Dictionary<string, string>();
            {
                var ms = regex_fieldNameMapping.Matches(pureSELECTPart);
                if (ms.Count > 0)
                {
                    for (int i = 0; i < ms.Count; i++)
                    {
                        var field = Helper_GetPureFieldName(ms[i].Groups["field"].Value);
                        var alias = Helper_GetPureFieldName(ms[i].Groups["alias"].Value);
                        dic_fieldNameMapping[field] = alias;
                    }
                }
            }

            //Check and mapping
            StringBuilder newOrderby = new StringBuilder("ORDER BY ");
            {
                var ms = regex_fieldOrderby.Matches(orderString);
                if (ms.Count > 0)
                {
                    for (int i = 0; i < ms.Count; i++)
                    {
                        var direction = ms[i].Groups["direction"].Value;
                        var rawField = ms[i].Groups["field"].Value;
                        var field = Helper_GetPureFieldName(rawField);

                        if (dic_fieldNameMapping.ContainsKey(field))
                        {//Has alias name
                            newOrderby.Append("[");
                            newOrderby.Append(dic_fieldNameMapping[field]);
                            newOrderby.Append("]");
                        }
                        else
                        {//remove multi-part identifier if exist
                            newOrderby.Append("[");
                            newOrderby.Append(Helper_GetPureFieldName(regex_minimumFieldName.Match(field).Groups["name"].Value));
                            newOrderby.Append("]");
                        }

                        //append direction
                        if (String.IsNullOrEmpty(direction.Trim()) == false)
                        {
                            newOrderby.Append(" ");
                            newOrderby.Append(direction);
                        }

                        //append ','
                        if (i < ms.Count - 1)
                            newOrderby.Append(",");
                    }
                }
            }

            //finally append it 
            rownumber.Append(newOrderby);
        }

        /// <summary>
        /// Get Pure Field Name
        /// </summary>
        private static string Helper_GetPureFieldName(string text)
        {
            return text.Replace("\'", String.Empty).Replace("[", String.Empty).Replace("]", String.Empty);
        }

        #endregion
                

        #endregion
        
    }
}
