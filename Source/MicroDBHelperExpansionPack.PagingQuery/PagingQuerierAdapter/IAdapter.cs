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
    interface IAdapter
    {
        DetailPagingRet DetailPaging(ExecuteDelegate executeAction,
                                     int pageIndex, int pageSize,
                                     string fixedSql, string selectSql, SqlParameter[] paramValues,
                                     CommandType commandType = CommandType.Text
                                     );


        Task<DetailPagingRet> DetailPagingAsync(ExecuteAsyncDelegate executeAction,
                                                int pageIndex, int pageSize,
                                                string fixedSql, string selectSql, SqlParameter[] paramValues,
                                                CommandType commandType = CommandType.Text
                                                );
    }
}
