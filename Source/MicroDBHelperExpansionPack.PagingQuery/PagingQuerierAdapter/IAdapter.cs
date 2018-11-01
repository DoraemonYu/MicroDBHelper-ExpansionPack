using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
#if ASYNC_SUPPORT
using System.Linq;
using System.Threading.Tasks;
#endif
using static MicroDBHelpers.ExpansionPack.PagingQuerier;

namespace MicroDBHelpers.ExpansionPack
{
    interface IAdapter
    {
        DetailPagingRet DetailPaging(ExecuteDelegate executeAction,
                                     int pageIndex, int pageSize,
                                     string fixedSql, string selectSql, SqlParameter[] paramValues,
                                     CommandType commandType = CommandType.Text
                                     );

#if ASYNC_SUPPORT
        Task<DetailPagingRet> DetailPagingAsync(ExecuteAsyncDelegate executeAction,
                                                int pageIndex, int pageSize,
                                                string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                CommandType commandType = CommandType.Text
                                                );
#endif
    }
}
