using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MicroDBHelperExpansionPack.Internals
{

    /// <summary>
    /// Interface
    /// </summary>
    internal interface IAssemblyInspectorContext
    {
        bool CheckAssembly();
    }

    /// <summary>
    /// Main logic to check [MicroDBHelper.dll]
    /// </summary>
    internal class AssemblyInspectorContext : MarshalByRefObject, IAssemblyInspectorContext
    {
        public bool CheckAssembly()
        {
            //Try to load target DLL
            var assembly        = typeof(MicroDBHelpers.MicroDBTransaction).Assembly; ;
            
            //Check GUID
            var guidAttribute   = assembly.GetCustomAttributes(typeof(GuidAttribute), true).FirstOrDefault() as GuidAttribute;
            if (guidAttribute == null)
                return false;
            if (guidAttribute.Value.Equals("1fc8371f-18f4-4693-b233-a8736f9cded7", StringComparison.OrdinalIgnoreCase) == false)
                return false;

            //Check PublicKeyToken
            var publicKeyToken = BitConverter.ToString(assembly.GetName().GetPublicKeyToken()).Replace("-", "");
            if (publicKeyToken.Equals("68750db11b24ebb2", StringComparison.OrdinalIgnoreCase) == false)
                return false;

            //Finally
            return true;
        }
    }


    /// <summary>
    /// Inspector for checking [MicroDBHelper.dll] is can be loaded successfully
    /// </summary>
    internal sealed class AssemblyInspector
    {
        /// <summary>
        /// Check [MicroDBHelper.dll] is can be loaded successfully
        /// </summary>
        /// <returns></returns>
        public static bool CheckAssembly()
        {
            #region Solution1. Well in Winform, but Bad in Asp.net
            /*
            try
            {
                var assmblyLoaderType = typeof(AssemblyInspectorContext);
                var assemblyLoader = (IAssemblyInspectorContext)appDomain.CreateInstanceFromAndUnwrap(assmblyLoaderType.Assembly.Location, assmblyLoaderType.FullName);
                return assemblyLoader.CheckAssembly();
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                //Release the temporary AppDomain
                AppDomain.Unload(appDomain);
            }
            */
            #endregion

            #region Solution2

            try
            {
                var assembly        =  Assembly.Load("MicroDBHelper");

                //Check GUID
                var guidAttribute   = assembly.GetCustomAttributes(typeof(GuidAttribute), true).FirstOrDefault() as GuidAttribute;
                if (guidAttribute == null)
                    return false;
                if (guidAttribute.Value.Equals("1fc8371f-18f4-4693-b233-a8736f9cded7", StringComparison.OrdinalIgnoreCase) == false)
                    return false;

                //Check PublicKeyToken
                var publicKeyToken = BitConverter.ToString(assembly.GetName().GetPublicKeyToken()).Replace("-", "");
                if (publicKeyToken.Equals("68750db11b24ebb2", StringComparison.OrdinalIgnoreCase) == false)
                    return false;

                //Finally
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
            #endregion

        }
    }
}
