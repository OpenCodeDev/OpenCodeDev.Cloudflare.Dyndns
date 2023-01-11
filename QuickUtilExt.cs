using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenCodeDev.Cloudflare.Dyndns
{
    internal static class QuickUtilExt
    {
        internal static string GetEnvironmentBuild(this object anyAssemblyObj)
        {
            return anyAssemblyObj.GetType().GetEnvironmentBuild();
        }

        internal static string GetEnvironmentBuild(this Type anyAssemblyObj)
        {
            return anyAssemblyObj.Assembly.GetEnvironmentBuild();
        }
        public static string GetEnvironmentBuild(this Assembly anyAssemblyObj)
        {
            var env = anyAssemblyObj.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;
            return env == null || env == string.Empty ? "Debug" : env;
        }

    }
}
