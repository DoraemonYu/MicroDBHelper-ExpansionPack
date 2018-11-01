using System;
using System.Collections.Generic;
using System.Text;
#if ASYNC_SUPPORT
using System.Linq;
using System.Threading.Tasks;
#endif

namespace MicroDBHelpers.ExpansionPack
{
    /// <summary>
    /// Richer control for "Column" for "Property of Entity Class"
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class ColumnAttribute : Attribute
    {

        //--------Members--------

        #region Match the Name from DateTable's Column name

        /// <summary>
        /// Match the Name from DateTable's Column name
        /// </summary>
        public string MatchedName { get; set; }

        /// <summary>
        ///  Is it Case Sensitive to MatchedName
        /// </summary>
        public bool CaseSensitiveToMatchedName { get; set; }

        #endregion


        //--------Control--------

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="matchedName">the Name to match with</param>
        public ColumnAttribute(string matchedName)
        {
            this.MatchedName                = matchedName;
            this.CaseSensitiveToMatchedName = EntityConversionDefaultSettings.CaseSensitiveToColumnName;
        }

        #endregion


    }
}
