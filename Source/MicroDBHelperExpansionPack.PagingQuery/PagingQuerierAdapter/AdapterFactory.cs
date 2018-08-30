using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDBHelpers.ExpansionPack
{
    internal sealed class AdapterFactory
    { 
        public static IAdapter CreateAdapter(string connectionAliasName)
        {
            //Get the target item
            var item = ConnectionRepository.GetRepositoryItem(connectionAliasName);
            if (item == null)
                throw new ArgumentException("connectionAliasName was not exist.", "connectionAliasName");

            //Get the SqlServer product version
            var productVersion = item.ServerProductVersion;

            //Detect the best way to PagingQuery
            if (productVersion.Major >= 11)            // 11.0.0.0 -> SQL Server 2012
                return new OffsetFetchWayAdapter();
            else
                return new RowNumberWayAdapter();
        }


    }
}
