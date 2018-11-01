using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
#if ASYNC_SUPPORT
using System.Linq;
using System.Threading.Tasks;
#endif
using MicroDBHelpers;
using System.Text.RegularExpressions;

namespace MicroDBHelpers.ExpansionPack
{
    /// <summary>
    /// Helper
    /// </summary>
    internal static class DealHelper
    {        

        #region DetailPaging Helpers

        internal static void DetailPagingHelper_Prepare(string selectSql,
                                                       out string SELECTSQL)
        {
            //Check Data legitimacy firstly
            if (String.IsNullOrEmpty(selectSql.Trim()))
                throw new ArgumentException("[selectSql] cannot be empty string.", "selectSql");

            //pre-deal some chars which may effect the logic
            SELECTSQL = selectSql.Replace("\t", " ")
                                 .Replace("\r\n", "\n")
                                 .Replace("\n", " \n")
                                 .Trim();

            //Check Data legitimacy agian
            if (SELECTSQL.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase) < 0)
                throw new ArgumentException("[selectSql] must include the 'SELECT' keyword.", "selectSql");
            if (SELECTSQL.IndexOf("FROM ", StringComparison.OrdinalIgnoreCase) < 0)
                throw new ArgumentException("[selectSql] must include the 'SELECT' keyword.", "selectSql");
        }


        internal static void DetailPagingHelper_SplitStrings(string SELECTSQL,
                                                             out string orderBodyString, out string SELECTWithoutOrder,out string fromBodyString)
        {
            //start after select
            int beginPos        = SELECTSQL.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase) + 7;
            int nextFromPos     = SELECTSQL.IndexOf("FROM ", beginPos, StringComparison.OrdinalIgnoreCase);
            int nextSelectPos   = SELECTSQL.IndexOf("SELECT ", beginPos, StringComparison.OrdinalIgnoreCase);

            while (nextSelectPos > 0 && nextFromPos > nextSelectPos)
            {
                beginPos        = nextFromPos + 4;

                nextFromPos     = SELECTSQL.IndexOf("FROM ", beginPos, StringComparison.OrdinalIgnoreCase);
                nextSelectPos   = SELECTSQL.IndexOf("SELECT ", beginPos, StringComparison.OrdinalIgnoreCase);
            }

            int endPos          = SELECTSQL.LastIndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
            bool hasOrderBy     = endPos > 0;

            //orderString & SELECTWithoutOrder
            orderBodyString      = !hasOrderBy ? String.Empty                       : SELECTSQL.Substring(endPos + 8);
            SELECTWithoutOrder   = !hasOrderBy ? String.Copy(SELECTSQL)             : SELECTSQL.Substring(0, endPos);

            //fromBodyString
            fromBodyString       = !hasOrderBy ? SELECTSQL.Substring(nextFromPos)   : SELECTSQL.Substring(nextFromPos, endPos - nextFromPos - 1);
        }


        internal static Exception DetailPagingHelper_CatchException_SqlException(Exception ex, string fixedSql, string SELECTSQL)
        {
            var err = new InvalidOperationException("A sql exception was thrown when paging query: "
                                       + ex.Message
                                       + "\r\nMore informations about this exception, see the Exception.[Data] property.", ex);
            err.Data.Add("current_sql_expression", fixedSql + "\n\n" + SELECTSQL);
            err.Data.Add("original_sql_exception", ex);

            return err;
        }
        internal static Exception DetailPagingHelper_CatchException_CommonException(Exception ex, string fixedSql, string SELECTSQL, int totalCount)
        {
            var err = new InvalidOperationException("Unknown error when paging query, please try to check your sql expression. More informations about this exception, see the Exception.[Data] property.", ex);
            err.Data.Add("current_sql_expression", fixedSql + "\n\n" + SELECTSQL);
            err.Data.Add("rows_total_count", totalCount);

            return err;
        }

        /// <summary>
        /// Check is it include "distinct"
        /// </summary>
        /// <param name="sql">sql expression</param>
        internal static bool hasDistinct(string sql)
        {
            return sql.IndexOf("SELECT distinct", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion

    }
}
