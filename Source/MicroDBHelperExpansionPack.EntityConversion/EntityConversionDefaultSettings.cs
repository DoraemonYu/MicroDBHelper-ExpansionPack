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
    /// Default Settings of "Entity Conversion" Features
    /// </summary>
    public static class EntityConversionDefaultSettings
    {

        //--------Members--------

        #region Settings

        /// <summary>
        /// Is it Case Sensitive To the Column's Name (Default is true)
        /// </summary>
        public static bool CaseSensitiveToColumnName { get; set; }
        
        #endregion


        //--------Control--------

        #region Static Constructor

        static EntityConversionDefaultSettings()
        {
            CaseSensitiveToColumnName = true;
        }

        #endregion

    }
}
