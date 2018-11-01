using System;
using System.Collections.Generic;
using System.Text;
#if NET45 || NET46
using System.Linq;
#endif

namespace System
{
    /// <summary>
    /// Alternate for Linq's Search
    /// </summary>
    internal sealed class LinqSearchAlternate
    {
        #region Handlers

#if NET20
        public delegate TResult Func<in T, out TResult>(T arg);
#endif

        #endregion


        #region FirstOrDefault

        public static T FirstOrDefault<T>(IEnumerable<T> list)
        {
#if NET45 || NET46
            return list.FirstOrDefault();
#else
            var enumerator = list.GetEnumerator();
            if (enumerator.MoveNext())
                return enumerator.Current;
            else
                return default(T);
#endif
        }

        public static T FirstOrDefault<T>(IEnumerable<T> list, Func<T,bool> function)
        {
#if NET45 || NET46
            return list.FirstOrDefault(function);
#else
            foreach (var item in list)
            {                
                if (function(item))     //fount it!
                    return item;
            }
            //if cannot found anything
            return default(T);
#endif
        }

        #endregion

        #region Select

        public static IEnumerable<TRet> Select<TData,TRet>(IEnumerable<TData> list, Func<TData, TRet> function)
        {
#if NET45 || NET46
            return list.Select(function);
#else
            List<TRet> ret = new List<TRet>();
            foreach (var item in list)
            {
                ret.Add(function(item));
            }
            return ret;
#endif
        }

        #endregion

        #region ToDictionary

        public static IDictionary<TRet, TData> ToDictionary<TData, TRet>(IEnumerable<TData> list, Func<TData, TRet> function)
        {
#if NET45 || NET46
            return list.ToDictionary(function);
#else
            Dictionary<TRet, TData> ret = new Dictionary<TRet, TData>();
            foreach (var item in list)
            {
                ret.Add(function(item),item);
            }
            return ret;
#endif
        }

        #endregion


    }
}
