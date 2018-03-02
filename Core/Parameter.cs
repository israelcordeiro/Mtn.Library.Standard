using System;
using System.IO;
using System.Linq;
#if ! UseSystemExtensions
using Mtn.Library.Extensions;
#endif

using System.Reflection;
using System.Web;
using Mtn.Library.Configuration;
using Mtn.Library.Service;


namespace Mtn.Library.Core
{

    /// <summary>    
    /// <para>Class Mtn.Library to get parameters for the features of the library.</para>
    /// </summary>
    public static class Parameter
    {
        private static Mtn.Library.Interface.IJsonProvider _mJsonProvider;
        private static int? _configurationDepth = null;
        private static bool? _disableEventLogStatisticOnLowPermission;

        /// <summary>
        /// Get How many wildcard will try before stop, max 20 default 2
        /// </summary>
        public static int ConfigurationDepth
        {
            get
            {
                if (_configurationDepth.HasValue) return _configurationDepth.Value;
                _configurationDepth = Config.GetNullableInt32("Mtn.Library.ConfigurationDepth",false);
                if (!_configurationDepth.HasValue)
                    _configurationDepth = 2;
                return _configurationDepth.Value;
                
            }
            set { _configurationDepth = value; }
        }

        /// <summary>
        /// Disable statistics when is in Mediu trust and lower
        /// </summary>
        public static bool DisableEventLogStatisticOnLowPermission
        {
            get
            {
                if (_disableEventLogStatisticOnLowPermission.HasValue) return _disableEventLogStatisticOnLowPermission.Value;
                _disableEventLogStatisticOnLowPermission = Config.GetNullableBoolean("Mtn.Library.DisableEventLogStatisticOnLowPermission", false);
                
                if (!_disableEventLogStatisticOnLowPermission.HasValue)
                    _disableEventLogStatisticOnLowPermission = true;
                return _disableEventLogStatisticOnLowPermission.Value;
                
            }
            set { _disableEventLogStatisticOnLowPermission = value; }
        }

        /// <summary>
        /// set/get the default json provider
        /// </summary>
        public static Mtn.Library.Interface.IJsonProvider JsonProvider
        {
            get
            {
                if (_mJsonProvider != null) return _mJsonProvider;
                var jsproviderConfig = Configuration.Config.GetString("Mtn.Library.JsonProvider", false);
                if (jsproviderConfig.IsNullOrWhiteSpaceMtn())
                    jsproviderConfig = "Mtn.Library.JsonNetProvider,Mtn.Library.JsonNetProvider.JSON";
                try
                {
                    var assemblyName = jsproviderConfig.SplitMtn(",").First();
                    var jsonProvider = Assembly.Load(assemblyName);
                    if (jsonProvider != null)
                    {
                        var instanceName = jsproviderConfig.SplitMtn(",").Last();
                        _mJsonProvider = (Mtn.Library.Interface.IJsonProvider)jsonProvider.CreateInstance(instanceName, true);
                    }
                }
                catch
                {
                    _mJsonProvider = null;//new Interface.MicrosoftJsonProvider();
                }


                //return _mJsonProvider ?? (_mJsonProvider = new Interface.MicrosoftJsonProvider());
                return _mJsonProvider ?? (_mJsonProvider = null);
            }
            set { _mJsonProvider = value; }
        }

        /// <summary>
        /// GetFullpath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRootPath(string path = null)
        {

            try
            {
                //var context = HttpContext.Current;
                //return context != null ? context.Server.MapPath(path.IsNullOrWhiteSpaceMtn() ? "~" : path) : Path.GetFullPath(path.IsNullOrWhiteSpaceMtn() ? "\\" : path);
                return Path.GetFullPath(path.IsNullOrWhiteSpaceMtn() ? "\\" : path);
            }
            catch 
            {
                return path;
            }
        }

        /// <summary>
        /// Return if is full trust
        /// </summary>
        public static bool IsFulltrust
        {
            get
            {
                try
                {
                    return AppDomain.CurrentDomain.IsFullyTrusted;
                }
                catch 
                {
                    return false;
                }   
            }
        }

    }
}

