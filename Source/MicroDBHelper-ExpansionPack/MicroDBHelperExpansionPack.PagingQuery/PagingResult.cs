using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDBHelpers.ExpansionPack
{
    /// <summary>
    /// The result after Paging Query
    /// </summary>
    public class PagingResult<T> : IEnumerable<T>, IEnumerable
                        where T : class
    {

        //--------Members---------

        #region Data Collection

        /// <summary>
        /// Datas
        /// </summary>
        IEnumerable<T> Datas { get; set; }
        
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return Datas.AsQueryable().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Datas.GetEnumerator();
        }

        #endregion

        #region Status

        /// <summary>
        /// Current Index
        /// </summary>
        public int CurrentPageIndex { get; set; }

        /// <summary>
        /// Size of per Page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Count of all items
        /// </summary>
        public int TotalItemsCount { get; set; }

        /// <summary>
        /// Count of all pages
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
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="datas">Entitys</param>
        /// <param name="pageIndex">Current Index</param>
        /// <param name="pageSize">Size of per Page</param>
        /// <param name="TotalCount">Count of all items</param>
        public PagingResult(IEnumerable<T> datas, int pageIndex, int pageSize, int TotalCount)
        {
            Datas               = datas;            
            CurrentPageIndex    = pageIndex;
            PageSize            = pageSize;
            TotalItemsCount     = TotalCount;
        }
        		 
	    #endregion

    }
}
