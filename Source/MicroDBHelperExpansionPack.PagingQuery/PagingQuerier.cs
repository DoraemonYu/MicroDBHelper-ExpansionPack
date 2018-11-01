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
    /// Paging Querier
    /// </summary>
    public static class PagingQuerier
    {
        //------Delegate-------

        #region Execute

#if ASYNC_SUPPORT
        /// <summary>
        /// Delegate
        /// </summary>
        internal delegate Task<DataTable> ExecuteAsyncDelegate(string Sql, SqlParameter[] paramValues, CommandType commandType);
#endif
        /// <summary>
        /// Delegate
        /// </summary>
        internal delegate DataTable ExecuteDelegate(string Sql, SqlParameter[] paramValues, CommandType commandType);


        #endregion


        //------Structs--------

        #region DetailPagingRet
        /// <summary>
        /// result from DetailPaging
        /// </summary>
        internal class DetailPagingRet
        {
            public DataTable querydt { get; set; }
            public int totalCount { get; set; }
        }
        #endregion


        //------Control--------

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

            var ret = AdapterFactory.CreateAdapter(connectionAliasName).DetailPaging(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult(ret.querydt, pageIndex, pageSize, ret.totalCount);
        }

#if ASYNC_SUPPORT
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

            var ret = await AdapterFactory.CreateAdapter(connectionAliasName).DetailPagingAsync(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult(ret.querydt, pageIndex, pageSize, ret.totalCount);
        }
#endif

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

            var ret = AdapterFactory.CreateAdapter(transaction.ConnectionAliasName).DetailPaging(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult(ret.querydt, pageIndex, pageSize, ret.totalCount);
        }

#if ASYNC_SUPPORT
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

            var ret = await AdapterFactory.CreateAdapter(transaction.ConnectionAliasName).DetailPagingAsync(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult(ret.querydt, pageIndex, pageSize, ret.totalCount);
        }
#endif




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
            
            var ret = AdapterFactory.CreateAdapter(connectionAliasName).DetailPaging(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult<T>(EntityConvert.ConvertToList<T>(ret.querydt), pageIndex, pageSize, ret.totalCount);
        }

#if ASYNC_SUPPORT
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

            var ret = await AdapterFactory.CreateAdapter(connectionAliasName).DetailPagingAsync(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult<T>(EntityConvert.ConvertToList<T>(ret.querydt), pageIndex, pageSize, ret.totalCount);
        }
#endif


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

            var ret = AdapterFactory.CreateAdapter(transaction.ConnectionAliasName).DetailPaging(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult<T>(EntityConvert.ConvertToList<T>(ret.querydt), pageIndex, pageSize, ret.totalCount);
        }

#if ASYNC_SUPPORT
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

            var ret = await AdapterFactory.CreateAdapter(transaction.ConnectionAliasName).DetailPagingAsync(action, pageIndex, pageSize, fixedSql, selectSql, paramValues, commandType);
            return new PagingResult<T>(EntityConvert.ConvertToList<T>(ret.querydt), pageIndex, pageSize, ret.totalCount);
        }
#endif

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
            int totalCount = datas == null ? 0 : LinqSearchAlternate.Count(datas);

            //split datas
            IEnumerable<T> targetDatas = null;
            if (datas != null)
            {
                int skipNumber  = pageSize * (pageIndex - 1);

                var data_skiped = LinqSearchAlternate.Skip(datas, skipNumber);
                var data_taked  = LinqSearchAlternate.Take(datas, skipNumber);
                targetDatas     = data_taked;
            }

            //combine result
            PagingResult<T> result = new PagingResult<T>(targetDatas,pageIndex,pageSize,totalCount);
            return result;
        }

        #endregion

    }
}
