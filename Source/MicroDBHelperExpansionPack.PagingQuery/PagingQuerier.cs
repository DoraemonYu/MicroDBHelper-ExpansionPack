using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroDBHelpers;
using System.Text.RegularExpressions;

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
            ExecuteDelegate action = (m_Sql, m_paramValues, m_commandType) =>
            {
                return MicroDBHelper.ExecuteDataTable(m_Sql, m_paramValues, connectionAliasName, m_commandType);
            };

            var ret = DetailPaging(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult(ret.querydt, pageIndex, pageSize, ret.totalCount);
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
            ExecuteAsyncDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
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
            ExecuteDelegate action = (m_Sql, m_paramValues, m_commandType) =>
            {
                return MicroDBHelper.ExecuteDataTable(m_Sql, m_paramValues, transaction, m_commandType);
            };

            var ret = DetailPaging(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult(ret.querydt, pageIndex, pageSize, ret.totalCount);
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
            ExecuteAsyncDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
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
            ExecuteDelegate action = (m_Sql, m_paramValues, m_commandType) =>
            {
                return MicroDBHelper.ExecuteDataTable(m_Sql, m_paramValues, connectionAliasName, m_commandType);
            };

            var ret = DetailPaging(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult<T>(EntityConvert.ConvertToList<T>(ret.querydt), pageIndex, pageSize, ret.totalCount);
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
            ExecuteAsyncDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
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
            ExecuteDelegate action = (m_Sql, m_paramValues, m_commandType) =>
            {
                return MicroDBHelper.ExecuteDataTable(m_Sql, m_paramValues, transaction, m_commandType);
            };

            var ret = DetailPaging(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult<T>(EntityConvert.ConvertToList<T>(ret.querydt), pageIndex, pageSize, ret.totalCount);
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
            ExecuteAsyncDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
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
        delegate Task<DataTable> ExecuteAsyncDelegate(string Sql, SqlParameter[] paramValues, CommandType commandType);
        /// <summary>
        /// Delegate
        /// </summary>
        delegate DataTable ExecuteDelegate(string Sql, SqlParameter[] paramValues, CommandType commandType);

        /// <summary>
        /// result from DetailPaging
        /// </summary>
        private class DetailPagingRet
        {
            public DataTable querydt    { get; set; }
            public int       totalCount { get; set; }
        }


        private static DetailPagingRet DetailPaging(ExecuteDelegate executeAction,
                                                    int pageIndex, int pageSize,
                                                    string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                    CommandType commandType = CommandType.Text
                                                    )
        {
            //##0 init
            string SELECTSQL, orderBodyString, SELECTWithoutOrder, FINALSQL;
            int totalCount          = 0;
            var paras               = new List<SqlParameter>();

            //##1 Check
            DetailPagingHelper_Prepare(selectSql, out SELECTSQL);

            try
            {
                string sqlCount;
                DetailPagingHelper_SqlCount(selectSql, fixedSql, SELECTSQL, out sqlCount, out orderBodyString, out SELECTWithoutOrder);

                //##2 get record total and begin to page
                DataTable countDt   = executeAction(sqlCount, paramValues, commandType);
                DetailPagingHelper_SqlPage(countDt, paramValues, orderBodyString, SELECTWithoutOrder, fixedSql,
                                           pageIndex, pageSize,
                                           ref SELECTSQL, ref paras,
                                           out totalCount,out FINALSQL);


                //##3 exec sql expreession and return result
                DataTable querydt   = executeAction(FINALSQL, paras.ToArray(), commandType);
                return DetailPagingHelper_returnResult(querydt, totalCount);
            }
            catch (SqlException ex)
            {
                throw DetailPagingHelper_CatchException_SqlException(ex, fixedSql, SELECTSQL);
            }
            catch (Exception ex)
            {
                throw DetailPagingHelper_CatchException_CommonException(ex, fixedSql, SELECTSQL, totalCount);
            }
        }

        /// <summary>
        /// Detail to PagingAsync
        /// </summary>
        private static async Task<DetailPagingRet> DetailPagingAsync(ExecuteAsyncDelegate executeAction,
                                                                     int pageIndex, int pageSize,
                                                                     string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                                     CommandType commandType = CommandType.Text                                                   
                                                                     )
        {
            //##0 init
            string SELECTSQL, orderBodyString, SELECTWithoutOrder, FINALSQL;
            int totalCount          = 0;
            var paras               = new List<SqlParameter>();

            //##1 Check
            DetailPagingHelper_Prepare(selectSql,out SELECTSQL);

            try
            {
                string sqlCount;
                DetailPagingHelper_SqlCount(selectSql, fixedSql, SELECTSQL,out sqlCount,out orderBodyString,out SELECTWithoutOrder);

                //##2 get record total and begin to page
                DataTable countDt   = await executeAction(sqlCount, paramValues, commandType);
                DetailPagingHelper_SqlPage(countDt, paramValues, orderBodyString, SELECTWithoutOrder, fixedSql,
                                           pageIndex, pageSize, 
                                           ref SELECTSQL, ref paras,
                                           out totalCount, out FINALSQL);


                //##3 exec sql expreession and return result
                DataTable querydt = await executeAction(FINALSQL, paras.ToArray(), commandType);
                return DetailPagingHelper_returnResult(querydt,totalCount);
            }
            catch(SqlException ex)
            {
                throw DetailPagingHelper_CatchException_SqlException(ex, fixedSql, SELECTSQL);
            }
            catch (Exception ex)
            {
                throw DetailPagingHelper_CatchException_CommonException(ex, fixedSql, SELECTSQL, totalCount);
            }  
        }

        #region DetailPaging Helpers

        private static void DetailPagingHelper_Prepare(string selectSql,
                                                       out string SELECTSQL)
        {
            //Check Data legitimacy firstly
            if (String.IsNullOrWhiteSpace(selectSql))
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


        private static Exception DetailPagingHelper_CatchException_SqlException(Exception ex,string fixedSql,string SELECTSQL)
        {
            var err = new InvalidOperationException("A sql exception was thrown when paging query: "
                                       + ex.Message
                                       + "\r\nMore informations about this exception, see the Exception.[Data] property.", ex);
            err.Data.Add("current_sql_expression", fixedSql + "\n\n" + SELECTSQL);
            err.Data.Add("original_sql_exception", ex);

            return err;
        }
        private static Exception DetailPagingHelper_CatchException_CommonException(Exception ex, string fixedSql, string SELECTSQL,int totalCount)
        {
            var err = new InvalidOperationException("Unknown error when paging query, please try to check your sql expression. More informations about this exception, see the Exception.[Data] property.", ex);
            err.Data.Add("current_sql_expression", fixedSql + "\n\n" + SELECTSQL);
            err.Data.Add("rows_total_count", totalCount);

            return err;
        }


        private static void DetailPagingHelper_SqlCount(string selectSql, string fixedSql, string SELECTSQL,
                                                        out string sqlCount,out string orderBodyString,out string SELECTWithoutOrder)
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

            //orderString & SELECTWithoutOrder
            orderBodyString      = endPos < 0 ? String.Empty : SELECTSQL.Substring(endPos + 8);
            SELECTWithoutOrder   = endPos < 0 ? String.Copy(SELECTSQL) : SELECTSQL.Substring(0, endPos);

            //sqlCount
            if (!hasDistinct(SELECTSQL))
            {//## no DISTINCT
                if (endPos > 0)
                    sqlCount    = "SELECT COUNT(1) " + SELECTSQL.Substring(nextFromPos, endPos - nextFromPos - 1);
                else
                    sqlCount    = "SELECT COUNT(1) " + SELECTSQL.Substring(nextFromPos);
            }
            else
            {//## has DISTINCT
                if (endPos > 0)
                    sqlCount    = "SELECT COUNT(1) FROM ( " + SELECTSQL.Substring(0, endPos) + ") as ___temp___sqlCount___";
                else
                    sqlCount    = "SELECT COUNT(1) FROM ( " + SELECTSQL + ") as ___temp___sqlCount___";
            }

            //----processing fixed sql----
            sqlCount            = fixedSql + "\r\n" + sqlCount;
            //----------------------------   
        }

        private static void DetailPagingHelper_SqlPage(DataTable countDt, SqlParameter[] paramValues, string orderBodyString,string SELECTWithoutOrder, string fixedSql, int pageIndex, int pageSize, 
                                                       ref string SELECTSQL,ref List<SqlParameter> paras,
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
                querydt         = querydt,
                totalCount      = totalCount
            };
        }

        #endregion
                

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

            bool hasOffset          = (pageIndex != 1);
            int startOfSelect       = sql.IndexOf("SELECT ", StringComparison.OrdinalIgnoreCase);


            var pagingSelect        = new StringBuilder(sql.Length + 100);

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

            bool hasOrderBy = !String.IsNullOrWhiteSpace(orderBodyString);

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

        static Regex regex_fieldNameMapping = new Regex(@"(?<field>[^\s,]+)\s+as\s+(?<alias>[^\s,]+)", RegexOptions.IgnoreCase);
        static Regex regex_fieldOrderby = new Regex(@"[\s,](?<field>[^\s,]+)(\s+(?<direction>(asc)|(desc)))*", RegexOptions.IgnoreCase);
        static Regex regex_minimumFieldName = new Regex(@"((\.)*)(?<name>[^\s,\.]+)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// especial logic for DISTINCT (change the SELECT FIELD name if has 'AS' )
        /// </summary>
        /// <param name="rownumber"></param>
        /// <param name="orderString"></param>
        /// <param name="sqlWithoutOrder"></param>
        private static void Helper_GetRowNumberHelper(StringBuilder rownumber, string orderString, string sqlWithoutOrder)
        {
            string pureSELECTPart = sqlWithoutOrder.Substring(6, sqlWithoutOrder.IndexOf("FROM ",StringComparison.OrdinalIgnoreCase) - 5 ); 

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
            StringBuilder newOrderby    = new StringBuilder("ORDER BY ");
            {
                var ms = regex_fieldOrderby.Matches(orderString);
                if (ms.Count > 0)
                {
                    for (int i = 0; i < ms.Count; i++)
                    {
                        var direction       = ms[i].Groups["direction"].Value;
                        var rawField        = ms[i].Groups["field"].Value;
                        var field           = Helper_GetPureFieldName(rawField);

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
                        if (String.IsNullOrWhiteSpace(direction) == false)
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
