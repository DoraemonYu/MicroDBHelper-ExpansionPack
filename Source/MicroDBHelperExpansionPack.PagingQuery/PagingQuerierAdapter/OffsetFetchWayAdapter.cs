using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MicroDBHelpers.ExpansionPack.PagingQuerier;

namespace MicroDBHelpers.ExpansionPack
{
    /// <summary>
    /// use way of [Offset Fetch] (require SqlServer 2012+)
    /// </summary>
    internal sealed class OffsetFetchWayAdapter : IAdapter
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

            //##1 Check
            DealHelper.DetailPagingHelper_Prepare(selectSql, out SELECTSQL);

            try
            {
                DealHelper.DetailPagingHelper_SplitStrings(SELECTSQL, out orderBodyString, out SELECTWithoutOrder, out fromBodyString);

                //##2 get finall sql
                DetailPagingHelper_SqlPageAndCount(paramValues, orderBodyString, SELECTWithoutOrder, fixedSql, pageIndex, pageSize,
                                                   out FINALSQL);

                //##3 exec sql expreession and return result
                DataTable querydt = executeAction(FINALSQL, paramValues, commandType);
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
            int totalCount = 0;
            var paras = new List<SqlParameter>();

            //##1 Check
            DealHelper.DetailPagingHelper_Prepare(selectSql, out SELECTSQL);

            try
            {
                DealHelper.DetailPagingHelper_SplitStrings(SELECTSQL, out orderBodyString, out SELECTWithoutOrder, out fromBodyString);

                //##2 get finall sql
                DetailPagingHelper_SqlPageAndCount(paramValues, orderBodyString, SELECTWithoutOrder, fixedSql, pageIndex, pageSize,
                                                   out FINALSQL);

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

        #endregion

        #region DetailPaging Helpers

        private static void DetailPagingHelper_SqlPageAndCount(SqlParameter[] paramValues, string orderBodyString, string SELECTWithoutOrder, string fixedSql, int pageIndex, int pageSize,
                                                               out string FINALSQL)
        {
            StringBuilder adjustedSQL   = new StringBuilder();
            bool hasDistinct            = DealHelper.hasDistinct(SELECTWithoutOrder);

            //##1 Add TOTAL column
            if (!hasDistinct)
            {
                int firstSelectPos = SELECTWithoutOrder.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase);
                adjustedSQL.Append    ("SELECT COUNT(*) OVER() AS ___totalCount___,");
                adjustedSQL.AppendLine(SELECTWithoutOrder.Substring(firstSelectPos + 7));
            }
            else
            {
                adjustedSQL.Append    ("SELECT COUNT(*) OVER() AS ___totalCount___, * FROM ( ");
                adjustedSQL.Append    (SELECTWithoutOrder);
                adjustedSQL.AppendLine(" ) as ___temp___selectBody___ ");
            }

            //##2 Handle OrderBy
            bool HasOrderBy = !String.IsNullOrEmpty(orderBodyString);
            if (HasOrderBy)
            {
                adjustedSQL.Append("ORDER BY ");
                adjustedSQL.AppendLine(orderBodyString);
            }
            else
            {
                adjustedSQL.AppendLine("ORDER BY (select null) ");
            }

            //##3 Offset & Fetch
            adjustedSQL.Append("OFFSET ");
            adjustedSQL.Append((pageIndex - 1) * pageSize);
            adjustedSQL.AppendLine(" ROWS ");
            adjustedSQL.Append("FETCH NEXT  ");
            adjustedSQL.Append(pageSize);
            adjustedSQL.AppendLine(" ROWS ONLY ");


            //----processing fixed sql----
            FINALSQL = fixedSql + "\r\n" + adjustedSQL.ToString();
            //----------------------------
        }


        private static DetailPagingRet DetailPagingHelper_returnResult(DataTable querydt, int totalCount)
        {
            DetailPagingRet result = new DetailPagingRet
            {
                querydt     = querydt,
                totalCount  = 0
            };

            if (querydt.Rows.Count > 0)            
            {
                result.totalCount = Convert.ToInt32(querydt.Rows[0]["___totalCount___"]);
            }

            //drop the [rownumber] column, which to only for paging
            if (querydt != null)
                querydt.Columns.Remove("___totalCount___");

            //return the result
            return result;
        }

        #endregion

    }
}
