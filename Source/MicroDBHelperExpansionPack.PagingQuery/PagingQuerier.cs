using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroDBHelpers;

namespace MicroDBHelpers.ExpansionPack
{
    /// <summary>
    /// Paging Querier
    /// </summary>
    public static class PagingQuerier
    {

        //##----Public--------

        #region Paging Datas by Database

        /// <summary>
        /// Paging Datas by Database
        /// </summary>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="connectionAliasName">the Alias Name of Connection (if not pass name,it will use the DEFAULT name instead.)</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static PagingResult PagingAsDatatable(int pageIndex,int pageSize,
                                                     string fixedSql,string selectSql, SqlParameter[] paramValues,
                                                     string connectionAliasName = MicroDBHelper.ALIAS_NAME_DEFAULT, CommandType commandType = CommandType.Text
                                                     )
        {
            try
            {
                return PagingAsDatatableAsync(pageIndex, pageSize, fixedSql, selectSql, paramValues, connectionAliasName, commandType).Result;
            }
            catch (Exception ex)
            {                 
                throw ex.GetBaseException();
            }
        }

        /// <summary>
        /// async Paging Datas by Database
        /// </summary>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="connectionAliasName">the Alias Name of Connection (if not pass name,it will use the DEFAULT name instead.)</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static async Task<PagingResult> PagingAsDatatableAsync(int pageIndex, int pageSize,
                                                                      string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                                      string connectionAliasName = MicroDBHelper.ALIAS_NAME_DEFAULT, CommandType commandType = CommandType.Text
                                                                      )
        {
            ExecuteDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
            {
                return await MicroDBHelper.ExecuteDataTableAsync(m_Sql, m_paramValues, connectionAliasName, m_commandType);
            };

            var ret = await DetailPagingAsync(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult(ret.querydt, pageIndex, pageSize, ret.totalCount);
        }

        /// <summary>
        /// Paging Datas by Database
        /// </summary>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="transaction">transaction</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static PagingResult PagingAsDatatable(int pageIndex, int pageSize,
                                                     string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                     MicroDBTransaction transaction, CommandType commandType = CommandType.Text
                                                     )
        {
            try
            {
                return PagingAsDatatableAsync(pageIndex, pageSize, fixedSql, selectSql, paramValues, transaction, commandType).Result;
            }
            catch (Exception ex)
            {
                throw ex.GetBaseException();
            }
        }

        /// <summary>
        /// async Paging Datas by Database
        /// </summary>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="transaction">transaction</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static async Task<PagingResult> PagingAsDatatableAsync(int pageIndex, int pageSize,
                                                                      string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                                      MicroDBTransaction transaction, CommandType commandType = CommandType.Text
                                                                      )
        {
            ExecuteDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
            {
                return await MicroDBHelper.ExecuteDataTableAsync(m_Sql, m_paramValues, transaction, m_commandType);
            };

            var ret = await DetailPagingAsync(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult(ret.querydt, pageIndex, pageSize, ret.totalCount);
        }




        /// <summary>
        /// Paging Datas by Database <para />
        /// (need to reference: MicroDBHelperExpansionPack.EntityConversion )
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="connectionAliasName">the Alias Name of Connection (if not pass name,it will use the DEFAULT name instead.)</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static PagingResult<T> PagingAsEntity<T>(int pageIndex, int pageSize,
                                                        string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                        string connectionAliasName = MicroDBHelper.ALIAS_NAME_DEFAULT, CommandType commandType = CommandType.Text
                                                        )
                                               where T : class
        {
            try
            {
                return PagingAsEntityAsync<T>(pageIndex, pageSize, fixedSql, selectSql, paramValues, connectionAliasName, commandType).Result;
            }
            catch (Exception ex)
            {
                throw ex.GetBaseException();
            }
        }

        /// <summary>
        /// async Paging Datas by Database <para />
        /// (need to reference: MicroDBHelperExpansionPack.EntityConversion )
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="connectionAliasName">the Alias Name of Connection (if not pass name,it will use the DEFAULT name instead.)</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static async Task<PagingResult<T>> PagingAsEntityAsync<T>(int pageIndex, int pageSize,
                                                                         string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                                         string connectionAliasName = MicroDBHelper.ALIAS_NAME_DEFAULT, CommandType commandType = CommandType.Text
                                                                         )
                                        where T : class
        {
            ExecuteDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
            {
                return await MicroDBHelper.ExecuteDataTableAsync(m_Sql, m_paramValues, connectionAliasName, m_commandType);
            };

            var ret = await DetailPagingAsync(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult<T>(EntityConvert.ConvertToList<T>(ret.querydt), pageIndex, pageSize, ret.totalCount);
        }


        /// <summary>
        /// Paging Datas by Database <para />
        /// (need to reference: MicroDBHelperExpansionPack.EntityConversion )
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="transaction">transaction</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static PagingResult<T> PagingAsEntity<T>(int pageIndex, int pageSize,
                                                        string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                        MicroDBTransaction transaction, CommandType commandType = CommandType.Text
                                                        )
                                               where T : class
        {
            try
            {
                return PagingAsEntityAsync<T>(pageIndex, pageSize, fixedSql, selectSql, paramValues, transaction, commandType).Result;
            }
            catch (Exception ex)
            {
                throw ex.GetBaseException();
            }
        }

        /// <summary>
        /// async Paging Datas by Database <para />
        /// (need to reference: MicroDBHelperExpansionPack.EntityConversion )
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="transaction">transaction</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static async Task<PagingResult<T>> PagingAsEntityAsync<T>(int pageIndex, int pageSize,
                                                                         string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                                         MicroDBTransaction transaction, CommandType commandType = CommandType.Text
                                                                         )
                                        where T : class
        {
            ExecuteDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
            {
                return await MicroDBHelper.ExecuteDataTableAsync(m_Sql, m_paramValues, transaction, m_commandType);
            };

            var ret = await DetailPagingAsync(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult<T>(EntityConvert.ConvertToList<T>(ret.querydt), pageIndex, pageSize, ret.totalCount);
        }
        

        #endregion

        #region Directly Paging in Memory

        /// <summary>
        /// Just a helper function for developers who hope to "Paging Datas in Memory" and use the "PagingResult Model"
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="datas">datas</param>
        /// <param name="pageIndex">target index</param>
        /// <param name="pageSize">size of per page</param>
        public static PagingResult<T> PagingByList<T>(IEnumerable<T> datas, int pageIndex, int pageSize)
                             where T : class
        {
            //Get total count
            int totalCount = datas == null ? 0 : datas.Count();

            //split datas
            IEnumerable<T> targetDatas = null;
            if (datas != null)
            {
                int skipNumber = pageSize * (pageIndex - 1);
                targetDatas = datas.Skip(skipNumber).Take(pageSize);
            }

            //combine result
            PagingResult<T> result = new PagingResult<T>(targetDatas,pageIndex,pageSize,totalCount);
            return result;
        }

        #endregion


        //##----Private-------

        #region Helpers

        /// <summary>
        /// Delegate
        /// </summary>
        delegate Task<DataTable> ExecuteDelegate(string Sql, SqlParameter[] paramValues, CommandType commandType);

        /// <summary>
        /// result from DetailPaging
        /// </summary>
        private class DetailPagingRet
        {
            public DataTable querydt    { get; set; }
            public int       totalCount { get; set; }
        }

        /// <summary>
        /// Detail to PagingAsync
        /// </summary>
        private static async Task<DetailPagingRet> DetailPagingAsync(ExecuteDelegate executeAction,
                                                                     int pageIndex, int pageSize,
                                                                     string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                                     CommandType commandType = CommandType.Text                                                   
                                                                     )
        {
            //Check Data legitimacy firstly
            if (String.IsNullOrWhiteSpace(selectSql))
                throw new ArgumentException("[selectSql] cannot be empty string.", "selectSql");

            //init total count
            int totalCount   = 0;

            //pre-deal some chars which may effect the logic
            string SELECTSQL = selectSql.Replace("\t"," ")
                                        .Replace("\r\n", "\n")
                                        .Replace("\n", " \n")
                                        .Trim();

            //Check Data legitimacy agian
            if (SELECTSQL.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase) < 0)
                throw new ArgumentException("[selectSql] must include the 'SELECT' keyword.", "selectSql");
            if (SELECTSQL.IndexOf("FROM ", StringComparison.OrdinalIgnoreCase) < 0)
                throw new ArgumentException("[selectSql] must include the 'SELECT' keyword.", "selectSql");


            try
            {            
                //create total count sql expression
                string sqlCount     = String.Empty;

                //start after select
                int beginPos        = SELECTSQL.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase) + 7;
                int nextFormPos     = SELECTSQL.IndexOf("FROM ", beginPos, StringComparison.OrdinalIgnoreCase);
                int nextSelectPos   = SELECTSQL.IndexOf("SELECT ", beginPos, StringComparison.OrdinalIgnoreCase);

                while (nextSelectPos > 0 && nextFormPos > nextSelectPos)
                {
                    beginPos        = nextFormPos + 4;

                    nextFormPos     = SELECTSQL.IndexOf("FROM ", beginPos, StringComparison.OrdinalIgnoreCase);
                    nextSelectPos   = SELECTSQL.IndexOf("SELECT ", beginPos, StringComparison.OrdinalIgnoreCase);
                }

                string orderString  = "";
                int endPos          = SELECTSQL.LastIndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);

                if (endPos > 0)
                {
                    sqlCount        = "SELECT COUNT(*) " + SELECTSQL.Substring(nextFormPos, endPos - nextFormPos - 1);
                    orderString     = SELECTSQL.Substring(endPos);
                }
                else
                {
                    sqlCount        = "SELECT COUNT(*) " + SELECTSQL.Substring(nextFormPos);
                }


                //----processing fixed sql----
                sqlCount = fixedSql + "\r\n" + sqlCount;
                //----------------------------


                //get record total
                DataTable dt = await executeAction(sqlCount, paramValues, commandType);

                if (dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(dt.Rows[0][0]);
                }

                //Search by paging
                bool hasOffset = (pageIndex != 1);

                //create limit sql expression
                SELECTSQL = GetLimitSql(SELECTSQL, pageIndex, pageSize);

                //add pading parameter
                List<SqlParameter> paras = new List<SqlParameter>();
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
                var finalSql = fixedSql + "\r\n" + SELECTSQL;
                //----------------------------

                //exec sql expreession
                DataTable querydt = await executeAction(finalSql, paras.ToArray(), commandType);

                //drop the [rownumber] column, which to only for paging
                if (querydt != null)
                    querydt.Columns.Remove("___rownumber___");

                //return the result
                return new DetailPagingRet
                {
                    querydt     = querydt,
                    totalCount  = totalCount
                };
            }
            catch (Exception ex)
            {
                var err = new InvalidOperationException("Unknown error when paging query, please try to check your sql expression. More informations about this exception, see the Exception.[Data] property.", ex);
                err.Data.Add("current_sql_expression", fixedSql + "\n\n" + SELECTSQL);
                err.Data.Add("rows_total_count", totalCount);

                throw err;
            }  
        }
        

        /// <summary>
        /// Create limit sql
        /// </summary>
        /// <param name="sql">sql expression</param>
        /// <param name="pageIndex">the index of page</param>
        /// <param name="pageSize">the size of each page</param>
        private static string GetLimitSql(
            string sql,
            int pageIndex,
            int pageSize)
        {

            bool hasOffset      = (pageIndex != 1);
            int startOfSelect   = sql.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase);
            int orderByIndex    = sql.LastIndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);

            var pagingSelect    = new StringBuilder(sql.Length + 100);

            pagingSelect.Append("SELECT * FROM ( SELECT ");         // nest the main query in an outer select
            pagingSelect.Append(GetRowNumber(sql));                 // add the rownnumber bit into the outer query select list

            if (orderByIndex > 0)
            {
                sql = sql.Substring(0, orderByIndex);
            }

            if (hasDistinct(sql))
            {
                pagingSelect.Append(" row_.* FROM ( ")              // add another (inner) nested select
                        .Append(sql.Substring(startOfSelect))       // add the main query
                        .Append(" ) as row_");                      // close off the inner nested select
            }
            else
            {
                pagingSelect.Append(sql.Substring(startOfSelect + 6)); // add the main query
            }

            pagingSelect.Append(" ) as temp_ where ___rownumber___ ");

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
        private static string GetRowNumber(string sql)
        {

            StringBuilder rownumber = new StringBuilder(50).Append("ROW_NUMBER() OVER(");

            int orderByIndex = sql.LastIndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
            if (orderByIndex > 0 && !hasDistinct(sql))
            {
                rownumber.Append(sql.Substring(orderByIndex));
            }
            else if (orderByIndex > 0)
            {
                string orderby = sql;
                rownumber.Append(string.Format(" ORDER BY {0} ", orderby.Substring(orderby.LastIndexOf("["), orderby.LastIndexOf("]") - orderby.LastIndexOf("[") + 1)));
            }
            else
            {
                rownumber.Append(" ORDER BY (SELECT NULL) ");
            }

            rownumber.Append(") as ___rownumber___,");

            return rownumber.ToString();
        }

        /// <summary>
        /// Check is it include "distinct"
        /// </summary>
        /// <param name="sql">sql expression</param>
        private static bool hasDistinct(
            string sql)
        {
            return sql.IndexOf("SELECT distinct", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion

    }
}
