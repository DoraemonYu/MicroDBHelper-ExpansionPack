using System;
using System.Collections.Generic;
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
            //Force load DLL
            var assembly = typeof(MicroDBHelpers.MicroDBTransaction).Assembly;
            
            //Check GUID
            var guidAttribute = assembly.GetCustomAttributes(typeof(GuidAttribute), true).FirstOrDefault() as GuidAttribute;
            if (guidAttribute == null)
                return false;
            if (guidAttribute.Value.Equals("1fc8371f-18f4-4693-b233-a8736f9cded7", StringComparison.OrdinalIgnoreCase) == false)
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
            //Create an new temporary AppDomain
            AppDomain appDomain = AppDomain.CreateDomain("AppDomain4AssemblyInspector", null, new AppDomainSetup
            {
                ShadowCopyFiles     = "true",
                LoaderOptimization  = LoaderOptimization.MultiDomainHost,
            });
            
            try
            {
                var assmblyLoaderType   = typeof(AssemblyInspectorContext);
                var assemblyLoader      = (IAssemblyInspectorContext)appDomain.CreateInstanceFromAndUnwrap(assmblyLoaderType.Assembly.Location, assmblyLoaderType.FullName);
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

        }
    }
}
