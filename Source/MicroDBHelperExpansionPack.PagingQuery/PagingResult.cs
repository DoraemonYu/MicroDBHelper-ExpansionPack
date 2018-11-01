using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#if ASYNC_SUPPORT
using System.Linq;
using System.Threading.Tasks;
#endif


namespace MicroDBHelpers.ExpansionPack.Internals
{
#region BasePagingResult

    /// <summary>
    /// base class of PagingResult models. <para />
    /// (please NOT to use this class directly, use specific PagingResult models instead )
    /// </summary>
    public abstract class BasePagingResult<TData>
                                     where TData : class
    {

        //--------Members---------

#region Data Collection

        /// <summary>
        /// Datas (Read Only)
        /// </summary>
        public TData Datas { get; private set; }

#endregion

#region Status

        /// <summary>
        /// Current Index (Read Only)
        /// </summary>
        public int CurrentPageIndex { get; private set; }

        /// <summary>
        /// Size of per Page (Read Only)
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Count of all items
        /// </summary>
        public int TotalItemsCount { get; private set; }

        /// <summary>
        /// Count of all pages (Read Only)
        /// </summary>
        public int TotalPages
        {
            get
            {
                if (TotalItemsCount > 0)
                    return Convert.ToInt32(Math.Ceiling((TotalItemsCount + 0.0) / PageSize));
                else
                    return 1;
            }
        }

#endregion


        //--------Control---------

#region Constructor

        //Hide
        private BasePagingResult()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="datas">Entitys</param>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="TotalCount">Count of all items</param>
        internal BasePagingResult(TData datas, int pageIndex, int pageSize, int TotalCount)
        {
            Datas               = datas;
            CurrentPageIndex    = pageIndex;
            PageSize            = pageSize;
            TotalItemsCount     = TotalCount;
        }

#endregion

    }

#endregion
}


namespace MicroDBHelpers.ExpansionPack
{
#region DataTable Type Result

    /// <summary>
    /// The result after Paging Query (DataTable Type)
    /// </summary>
    public sealed class PagingResult : MicroDBHelpers.ExpansionPack.Internals.BasePagingResult<System.Data.DataTable>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="datas">DataTable</param>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="TotalCount">Count of all items</param>
        public PagingResult(System.Data.DataTable datas, int pageIndex, int pageSize, int TotalCount)
                      :base(datas, pageIndex, pageSize, TotalCount)
        {
        }
    }

#endregion


    /// <summary>
    /// The result after Paging Query (Entity Type)
    /// </summary>
    public sealed class PagingResult<T> : MicroDBHelpers.ExpansionPack.Internals.BasePagingResult<IEnumerable<T>>, IEnumerable<T>, IEnumerable
                               where T : class
    {

        //--------Members---------

#region Data Collection
        
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return Datas.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Datas.GetEnumerator();
        }

#endregion


        //--------Control---------

#region Constructor
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="datas">Entitys</param>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="TotalCount">Count of all items</param>
        public PagingResult(IEnumerable<T> datas, int pageIndex, int pageSize, int TotalCount)
                     : base(datas, pageIndex, pageSize, TotalCount)
        {
        }
        		 
#endregion

    }
}
