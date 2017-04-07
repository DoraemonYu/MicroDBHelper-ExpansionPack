﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="connectionAliasName">the Alias Name of Connection (if not pass name,it will use the DEFAULT name instead.)</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static PagingResult<T> Paging<T>(int pageIndex,int pageSize,
                                         string fixedSql,string selectSql, SqlParameter[] paramValues,
                                         string connectionAliasName = MicroDBHelper.ALIAS_NAME_DEFAULT, CommandType commandType = CommandType.Text
                                         )
                             where T : class
        {
            return PagingAsync<T>(pageIndex, pageSize, fixedSql, selectSql, paramValues, connectionAliasName, commandType).Result;
        }

        /// <summary>
        /// async Paging Datas by Database
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="connectionAliasName">the Alias Name of Connection (if not pass name,it will use the DEFAULT name instead.)</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static async Task<PagingResult<T>> PagingAsync<T>(int pageIndex, int pageSize,
                                                    string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                    string connectionAliasName = MicroDBHelper.ALIAS_NAME_DEFAULT, CommandType commandType = CommandType.Text
                                                    )
                                        where T : class
        {
            ExecuteDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
            {
                return await MicroDBHelper.ExecuteDataTableAsync(m_Sql, m_paramValues, connectionAliasName, m_commandType);
            };

            return await DetailPagingAsync<T>(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType); ;
        }


        /// <summary>
        /// Paging Datas by Database
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="transaction">transaction</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static PagingResult<T> Paging<T>(int pageIndex, int pageSize,
                                                string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                MicroDBTransaction transaction, CommandType commandType = CommandType.Text
                                                )
                             where T : class
        {
            return PagingAsync<T>(pageIndex, pageSize, fixedSql, selectSql, paramValues, transaction, commandType).Result;
        }

        /// <summary>
        /// async Paging Datas by Database
        /// </summary>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="fixedSql">The Sql which in the Front ( ex.  CTE Query begin with the Keyword "WITH" ) </param>
        /// <param name="selectSql">The Sql which begin with Keyword "SELECT" </param>
        /// <param name="paramValues">Parameters</param>
        /// <param name="transaction">transaction</param>
        /// <param name="commandType">Text | StoredProcedure</param>
        public static async Task<PagingResult<T>> PagingAsync<T>(int pageIndex, int pageSize,
                                                                 string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                                 MicroDBTransaction transaction, CommandType commandType = CommandType.Text
                                                                 )
                                        where T : class
        {
            ExecuteDelegate action = async (m_Sql, m_paramValues, m_commandType) =>
            {
                return await MicroDBHelper.ExecuteDataTableAsync(m_Sql, m_paramValues, transaction, m_commandType);
            };

            return await DetailPagingAsync<T>(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType); ;
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
        /// Detail to PagingAsync
        /// </summary>
        private static async Task<PagingResult<T>> DetailPagingAsync<T>(ExecuteDelegate executeAction,
                                                   int pageIndex, int pageSize,
                                                   string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                   CommandType commandType = CommandType.Text                                                   
                                                   )
                                        where T : class
        {
            //init total count
            int totalCount = 0;

            //create total count sql expression
            string sqlCount = String.Empty;

            string SELECTSQL = selectSql.Trim();

            // start after select
            int beginPos = SELECTSQL.IndexOf("select ", StringComparison.OrdinalIgnoreCase) + 7;
            int nextFormPos = SELECTSQL.IndexOf("from ", beginPos, StringComparison.OrdinalIgnoreCase);
            int nextSelectPos = SELECTSQL.IndexOf("select ", beginPos, StringComparison.OrdinalIgnoreCase);

            while (nextSelectPos > 0 && nextFormPos > nextSelectPos)
            {
                beginPos = nextFormPos + 4;

                nextFormPos = SELECTSQL.IndexOf("from ", beginPos, StringComparison.OrdinalIgnoreCase);
                nextSelectPos = SELECTSQL.IndexOf("select ", beginPos, StringComparison.OrdinalIgnoreCase);
            }

            string orderString = "";
            int endPos = SELECTSQL.LastIndexOf("order by", StringComparison.OrdinalIgnoreCase);

            if (endPos > 0)
            {
                sqlCount = "SELECT COUNT(*) " + SELECTSQL.Substring(nextFormPos, endPos - nextFormPos - 1);
                orderString = SELECTSQL.Substring(endPos);
            }
            else
            {
                sqlCount = "SELECT COUNT(*) " + SELECTSQL.Substring(nextFormPos);
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
            selectSql = fixedSql + "\r\n" + SELECTSQL;
            //----------------------------

            //exec sql expreession
            DataTable querydt = await executeAction(SELECTSQL, paras.ToArray(), commandType);

            //combine result
            PagingResult<T> result = new PagingResult<T>(EntityConvert.ConvertToList<T>(querydt), pageIndex, pageSize, totalCount);
            return result;
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
            int startOfSelect   = sql.IndexOf("select", StringComparison.OrdinalIgnoreCase);
            int orderByIndex    = sql.LastIndexOf("order by", StringComparison.OrdinalIgnoreCase);

            var pagingSelect    = new StringBuilder(sql.Length + 100);

            pagingSelect.Append("select * from ( select ");         // nest the main query in an outer select
            pagingSelect.Append(GetRowNumber(sql));                 // add the rownnumber bit into the outer query select list

            if (orderByIndex > 0)
            {
                sql = sql.Substring(0, orderByIndex);
            }

            if (hasDistinct(sql))
            {
                pagingSelect.Append(" row_.* from ( ")              // add another (inner) nested select
                        .Append(sql.Substring(startOfSelect))       // add the main query
                        .Append(" ) as row_");                      // close off the inner nested select
            }
            else
            {
                pagingSelect.Append(sql.Substring(startOfSelect + 6)); // add the main query
            }

            pagingSelect.Append(" ) as temp_ where rownumber_ ");

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
        private static string GetRowNumber(
            string sql)
        {

            StringBuilder rownumber = new StringBuilder(50).Append("ROW_NUMBER() OVER(");

            int orderByIndex = sql.LastIndexOf("order by", StringComparison.OrdinalIgnoreCase);
            if (orderByIndex > 0 && !hasDistinct(sql))
            {
                rownumber.Append(sql.Substring(orderByIndex));
            }
            else
            {
                string orderby      = sql;
                rownumber.Append(string.Format(" order by {0} ", orderby.Substring(orderby.LastIndexOf("["), orderby.LastIndexOf("]") - orderby.LastIndexOf("[") + 1)));
            }

            rownumber.Append(") as rownumber_,");

            return rownumber.ToString();
        }

        /// <summary>
        /// Check is it include "distinct"
        /// </summary>
        /// <param name="sql">sql expression</param>
        private static bool hasDistinct(
            string sql)
        {
            return sql.IndexOf("select distinct", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion

    }
}
