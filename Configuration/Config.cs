using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;

using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Mtn.Library.Service;
#if ! UseSystemExtensions
using Mtn.Library.Extensions;

#endif

namespace Mtn.Library.Configuration
{
    /// <summary>
    ///     Configuration class.
    /// </summary>
    public static class Config
    {
        public static bool IsHosted { get; set; }
        public static string AppDomainAppPath { get; set; }

        #region AspNetCore
        #region Interfaces
        /// <summary>
        /// Provides information about the web hosting environment an application is running in.
        /// </summary>
        public class HostingEnvironment
        {
            /// <summary>
            /// Gets or sets the name of the environment. This property is automatically set by the host to the value
            /// of the "ASPNETCORE_ENVIRONMENT" environment variable.
            /// </summary>
            public string EnvironmentName { get; set; }

            /// <summary>
            /// Gets or sets the name of the application. This property is automatically set by the host to the assembly containing
            /// the application entry point.
            /// </summary>
            public string ApplicationName { get; set; }

            /// <summary>
            /// Gets or sets the absolute path to the directory that contains the web-servable application content files.
            /// </summary>
            public string WebRootPath { get; set; }
            
            /// <summary>
            /// Gets or sets the absolute path to the directory that contains the application content files.
            /// </summary>
            public string ContentRootPath { get; set; }
            
        }
        
        #endregion

        public static IConfiguration ConfigurationCore { get; set; }
        public static HostingEnvironment HostingEnvironmentCore { get; set; }
        public static bool UseCoreConfiguration { get; set; }
        public static void ConfigureAspnetCore(IConfiguration configuration, dynamic env)
        {
            IsHosted = true;
            AppDomainAppPath = env.ContentRootPath;
            ConfigurationCore = configuration;
            HostingEnvironmentCore = new HostingEnvironment()
            {
                ContentRootPath = env.ContentRootPath,
                ApplicationName = env.ApplicationName,
                EnvironmentName = env.EnvironmentName,
                WebRootPath = env.WebRootPath
            };
            
            UseCoreConfiguration = true;
        }
        #endregion
        #region GetConnectionString
        private static ConnectionStringSettings GetByHostConnectionString(String keyName, String server)
        {
            if (m_CnnConfigs != null && m_CnnConfigs.ContainsKey(server))
            {
                var dic = m_CnnConfigs[server];
                if (dic != null) return dic[keyName];
                LoadConfigFiles(server);
                dic = m_CnnConfigs[server];
                return (dic == null || !dic.ContainsKey(keyName)) ? new ConnectionStringSettings() : dic[keyName];
            }
            else
            {
                LoadConfigFiles(server);
                if (m_CnnConfigs != null && m_CnnConfigs.ContainsKey(server))
                {
                    var dic = m_CnnConfigs[server];
                    if (dic != null) return dic[keyName];
                    LoadConfigFiles(server);
                    dic = m_CnnConfigs[server];
                    return (dic == null || !dic.ContainsKey(keyName)) ? new ConnectionStringSettings() : dic[keyName];
                }
            }
            return null;
        }

        /// <summary>
        ///     Returns the connection string from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in ConnectionStrings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns the connection string from config file (using servername check).
        /// </returns>
        public static String GetConnectionString(String keyName = "Mtn.Library.ConnectionString",
            Boolean serverCheck = true)
        {
            string result = null;

            try
            {
                //try
                //{
                //    if (!Core.Parameter.IsFulltrust)
                //        serverCheck = false;
                //}
                //catch { serverCheck = false; }
                var cnnKey = ConfigurationManager.ConnectionStrings[keyName];
                if(cnnKey != null)
                    result = ConfigurationManager.ConnectionStrings[keyName].ConnectionString;

                if (result == null && serverCheck)
                {
                    var serverName = "";
                    serverName = IsHosted ? GetWebServerName() : GetMachineName();
                    serverName = serverName ?? GetMachineName();
                    var ret = GetByHostConnectionString(keyName, serverName);
                    result = ret!=null?ret.ConnectionString:null;
                    if (result == null)
                    {
                        serverName = GetHostName();
                        ret = GetByHostConnectionString(keyName, serverName);
                        result = ret != null ? ret.ConnectionString : null;
                    }

                    if (result == null)
                    {
                        ret = GetByHostConnectionString(keyName, "Fallback");
                        result = ret != null ? ret.ConnectionString : null;
                    }

                    if (result == null)
                        result = ConfigurationManager.ConnectionStrings[keyName + "." + serverName]?.ConnectionString;

                    if (result == null)
                    {
                        serverName = GetHostName();
                        result = ConfigurationManager.ConnectionStrings[keyName + "." + serverName]?.ConnectionString;
                    }
                    else
                        return result;
                }
                if (result == null)
                {
                    result = ConfigurationManager.ConnectionStrings[keyName + ".Fallback"]?.ConnectionString;
                }
            }
            catch (Exception ex)
            {
                Statistics.EventLogEntry(ex.GetAllMessagesMtn());
            }

            return result;
        }

        #endregion

        #region GetGUID

        /// <summary>
        ///     Returns Guid from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns Guid from config file (using servername check).
        /// </returns>
        public static Guid GetGuid(String keyName, Boolean serverCheck = true)
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                return Guid.Empty;

            var value = new Guid(val);

            return value;
        }

        #endregion

        #region GetDateTime

        /// <summary>
        ///     Returns DateTime ? from config file.
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <param name="dtProvider">
        ///     Provides a mechanism for retrieving an object to control formatting.
        /// </param>
        /// <param name="dtStyle">
        ///     Defines the formatting options that customize string parsing for the System.DateTime.Parse().
        /// </param>
        /// <returns>
        ///     Returns DateTime ? from config file.
        /// </returns>
        public static DateTime? GetNullableDateTime(String keyName, Boolean serverCheck = true,
            IFormatProvider dtProvider = null, DateTimeStyles dtStyle = DateTimeStyles.None)
        {
            try
            {
                return GetDateTime(keyName, serverCheck, dtProvider, dtStyle);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Returns DateTime from config file.
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <param name="dtProvider">
        ///     Provides a mechanism for retrieving an object to control formatting.
        /// </param>
        /// <param name="dtStyle">
        ///     Defines the formatting options that customize string parsing for the System.DateTime.Parse().
        /// </param>
        /// <returns>
        ///     Returns DateTime from config file.
        /// </returns>
        public static DateTime GetDateTime(String keyName, Boolean serverCheck = true, IFormatProvider dtProvider = null,
            DateTimeStyles dtStyle = DateTimeStyles.None)
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                throw new InvalidCastException("Cannot convert null or empty to datetime");

            DateTime value;
            if (dtProvider == null)
            {
                if (DateTime.TryParse(val, out value) == false)
                    throw new InvalidCastException("Cannot convert any string different to datetime format to datetime");
            }
            else
            {
                if (DateTime.TryParse(val, dtProvider, dtStyle, out value) == false)
                    throw new InvalidCastException("Cannot convert any string different to datetime format to datetime");
            }
            return value;
        }

        #endregion

        #region GetInt

        /// <summary>
        ///     Returns int from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns int from config file (using servername check).
        /// </returns>
        public static Int32 GetInt32(String keyName, Boolean serverCheck = true)
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                throw new InvalidCastException("Cannot convert null or empty to int");

            val = val.ToLowerInvariant();
            Int32 value;

            if (Int32.TryParse(val, out value) == false)
                throw new InvalidCastException("Cannot convert any string different to number to int");

            return value;
        }

        /// <summary>
        ///     Returns int from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns int from config file (using servername check).
        /// </returns>
        public static Int64 GetInt64(String keyName, Boolean serverCheck = true)
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                throw new InvalidCastException("Cannot convert null or empty to int");

            val = val.ToLowerInvariant();
            Int64 value;

            if (Int64.TryParse(val, out value) == false)
                throw new InvalidCastException("Cannot convert any string different to number to int");

            return value;
        }

        /// <summary>
        ///     Returns int ? from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns int ? from config file (using servername check).
        /// </returns>
        public static Int32? GetNullableInt32(String keyName, Boolean serverCheck = true)
        {
            try
            {
                return GetInt32(keyName, serverCheck);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Returns int ? from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns int ? from config file (using servername check).
        /// </returns>
        public static Int64? GetNullableInt64(String keyName, Boolean serverCheck = true)
        {
            try
            {
                return GetInt64(keyName, serverCheck);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region GetBool

        /// <summary>
        ///     Returns Bool from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns Bool from config file (using servername check).
        /// </returns>
        public static bool GetBoolean(String keyName, Boolean serverCheck = true)
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                throw new InvalidCastException("Cannot convert null or empty to bool");
            val = val.ToLowerInvariant();
            bool value;

            if (bool.TryParse(val, out value))
                return value;

            var nVal = 999;
            if (int.TryParse(val, out nVal))
            {
                value = nVal != 0;
            }
            else
                throw new InvalidCastException(
                    "Cannot convert any string different to true,false or number(0=false,any other=true) to bool");
            return value;
        }


        /// <summary>
        ///     Returns Enum from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>
        ///     Returns Enum from config file (using servername check).
        /// </returns>
        /// <exception cref="InvalidCastException"></exception>
        public static T GetEnum<T>(String keyName, Boolean serverCheck = true) where T : struct
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                throw new InvalidCastException("Cannot convert null or empty to enum");
            val = val.ToLowerInvariant();

            return val.ToEnumMtn<T>();
        }

        /// <summary>
        ///     Returns Enum from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>
        ///     Returns Enum from config file (using servername check).
        /// </returns>
        /// <exception cref="InvalidCastException"></exception>
        public static T? GetNullableEnum<T>(String keyName, Boolean serverCheck = true) where T : struct
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                return null;

            val = val.ToLowerInvariant();

            return val.ToEnumMtn<T>();
        }


        /// <summary>
        ///     Returns Bool ? from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns Bool ? from config file (using servername check).
        /// </returns>
        public static bool? GetNullableBoolean(String keyName, Boolean serverCheck = true)
        {
            try
            {
                if (!GetString(keyName).IsNullOrWhiteSpaceMtn())
                    return GetBoolean(keyName, serverCheck);
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region GetDecimal

        /// <summary>
        ///     Returns Decimal ? from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns Decimal ? from config file (using servername check).
        /// </returns>
        public static Decimal? GetNulllableDecimal(String keyName, Boolean serverCheck = true)
        {
            try
            {
                return GetDecimal(keyName, serverCheck);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Returns Decimal from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns Decimal from config file (using servername check).
        /// </returns>
        public static Decimal GetDecimal(String keyName, Boolean serverCheck = true)
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                throw new InvalidCastException("Cannot convert null or empty to Decimal");

            val = val.ToLowerInvariant();
            Decimal value;

            if (Decimal.TryParse(val, out value) == false)
                throw new InvalidCastException("Cannot convert any string different to number to Decimal");

            return value;
        }

        #endregion

        #region GetSingle

        /// <summary>
        ///     Returns Single ? from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns Single ? from config file (using servername check).
        /// </returns>
        public static Single? GetNulllableSingle(String keyName, Boolean serverCheck = true)
        {
            try
            {
                return GetSingle(keyName, serverCheck);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Returns Single from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns Single from config file (using servername check).
        /// </returns>
        public static Single GetSingle(String keyName, Boolean serverCheck = true)
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                throw new InvalidCastException("Cannot convert null or empty to Single");

            val = val.ToLowerInvariant();
            Single value;

            if (Single.TryParse(val, out value) == false)
                throw new InvalidCastException("Cannot convert any string different to number to Single");

            return value;
        }

        #endregion

        #region GetDouble

        /// <summary>
        ///     Returns Double ? from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns Double ? from config file (using servername check).
        /// </returns>
        public static Double? GetNulllableDouble(String keyName, Boolean serverCheck = true)
        {
            try
            {
                return GetDouble(keyName, serverCheck);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Returns Double from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns Double from config file (using servername check).
        /// </returns>
        public static Double GetDouble(String keyName, Boolean serverCheck = true)
        {
            var val = GetString(keyName, serverCheck);
            if (val.IsNullOrEmptyMtn())
                throw new InvalidCastException("Cannot convert null or empty to Double");

            val = val.ToLowerInvariant();
            Double value;

            if (Double.TryParse(val, out value) == false)
                throw new InvalidCastException("Cannot convert any string different to number to Double");

            return value;
        }

        #endregion

        #region GetString

        /// <summary>
        ///     Returns string from config file (using servername check).
        /// </summary>
        /// <param name="keyName">
        ///     Key name in AppSettings.
        /// </param>
        /// <param name="serverCheck">
        ///     Indicates whether to verify the key by adding the verification of the server / machine: "keyName.server ",
        ///     "keyName.machine" or "keyName.Alternative"
        /// </param>
        /// <returns>
        ///     Returns string from config file (using servername check).
        /// </returns>
        public static String GetString(String keyName, Boolean serverCheck = true)
        {
            string result = null;

            try
            {
                //try
                //{
                //    if (!Core.Parameter.IsFulltrust)
                //        serverCheck = false;
                //}
                //catch { serverCheck = false;}

                result = ConfigurationManager.AppSettings[keyName];

                if (result == null && serverCheck)
                {
                    var serverName = "";
                    serverName = IsHosted ? GetWebServerName() : GetMachineName();
                    serverName = serverName ?? GetMachineName();
                    result = GetByHostString(keyName, serverName);
                    if (result == null)
                    {
                        serverName = GetHostName();
                        result = GetByHostString(keyName, serverName);
                    }

                    if (result == null)
                        result = GetByHostString(keyName, "Fallback");

                    if (result == null)
                        result = ConfigurationManager.AppSettings[keyName + "." + serverName];

                    if (result == null)
                    {
                        serverName = GetHostName();
                        result = ConfigurationManager.AppSettings[keyName + "." + serverName];
                    }
                    else
                        return result;
                }
                
            }
            catch (Exception ex)
            {
                Statistics.EventLogEntry(ex.GetAllMessagesMtn());
            }

            return result;
        }

        #endregion

        #region Aux Get Server / Machine / Host Name

        #region LoadConfigFiles

        private static readonly object m_syncLock = new object();

        private static readonly SortedDictionary<string, SortedDictionary<string, string>> m_Configs =
            new SortedDictionary<string, SortedDictionary<string, string>>();

        private static readonly SortedDictionary<string, SortedDictionary<string, ConnectionStringSettings>>
            m_CnnConfigs = new SortedDictionary<string, SortedDictionary<string, ConnectionStringSettings>>();
        
        private static void LoadConfigFiles(String server)
        {
            lock (m_syncLock)
            {
                var path = "";
                if(!UseCoreConfiguration)
                    path = ConfigurationManager.AppSettings["Mtn.Library.ConfigFolder"];
                else
                {
                    path = ConfigurationCore.GetSection("Mtn.Library")?["ConfigFolder"];
                }

                if (string.IsNullOrWhiteSpace(path))
                {
                    path = IsHosted
                        ? AppDomainAppPath
                        : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    if (path == null)
                        return;

                    path = Path.Combine(path, "config");
                    if (!Directory.Exists(path))
                        return;
                }
                if (server == null)
                    return;


                var file = string.Empty;
                if (!UseCoreConfiguration)
                {
                    file = ConfigurationManager.AppSettings["Mtn.Library.Config.Default"];
                }
                else
                {
                    file = ConfigurationCore.GetSection("Mtn.Library")?["Config.Default"];   
                }

                if (file.IsNullOrWhiteSpaceMtn())
                {
                    if (!UseCoreConfiguration)
                    {
                        file = ConfigurationManager.AppSettings["Mtn.Library.Config." + server];
                    }
                    else
                    {
                        file = ConfigurationCore.GetSection("Mtn.Library")?["Config." + server];   
                    }
                }
                    
                if (file.IsNullOrWhiteSpaceMtn())
                {
                    var depthWildcard = "*.";
                    var depthServer = server.SplitMtn(".").Count() > 3?
                        String.Join(".", server.SplitMtn(".").Skip(1)):server;
                    if (!UseCoreConfiguration)
                    {
                        file = ConfigurationManager.AppSettings["Mtn.Library.Config." + depthWildcard + depthServer];
                    }
                    else
                    {
                        file = ConfigurationCore.GetSection("Mtn.Library")?["Config." +depthWildcard + depthServer];   
                    }
                    
                    if (file.IsNullOrWhiteSpaceMtn())
                    {
                        for (var i = 1; i < Core.Parameter.ConfigurationDepth; i++)
                        {
                            depthWildcard += "*.";
                            depthServer = depthServer.SplitMtn(".").Count() > 2 ? String.Join(".", depthServer.SplitMtn(".").Skip(1)) : depthServer;
                            if (!UseCoreConfiguration)
                            {
                                file = ConfigurationManager.AppSettings["Mtn.Library.Config." + depthWildcard + depthServer];
                            }
                            else
                            {
                                file = ConfigurationCore.GetSection("Mtn.Library")?["Config." +depthWildcard + depthServer];   
                            }
                            
                            if (!file.IsNullOrWhiteSpaceMtn()) break;
                        }
                    }
                }

                if (file.IsNullOrWhiteSpaceMtn())
                {
                    if (!UseCoreConfiguration)
                    {
                        file = ConfigurationManager.AppSettings["Mtn.Library.Config.Fallback"];
                    }
                    else
                    {
                        file = ConfigurationCore.GetSection("Mtn.Library")?["Config.Fallback"];   
                    }
                }
                    

                if (file.IsNullOrWhiteSpaceMtn())
                    return;
                var filePath = Path.Combine(path, file);

                if (!File.Exists(filePath))
                    throw new FileNotFoundException(
                        string.Format("File {0} error. Check if file exists and have permission to read", filePath));

                if (!m_Configs.ContainsKey(server))
                {
                    m_Configs.Add(server, new SortedDictionary<string, string>());
                    m_CnnConfigs.Add(server, new SortedDictionary<string, ConnectionStringSettings>());
                }

                var fileExtension = file.SplitMtn(".").Last().ToLower();
                switch (fileExtension)
                {
                    case "xml":
                    case "config":
                        //LoadConfigFiles(server);
                        LoadXmlConfig(server, filePath);
                        break;
                    case "json":
                    case "js":
                    case "javascript":
                        //LoadConfigFiles(server);
                        LoadJsonConfig(server,filePath);
                        break;
                    //case "csv":
                    //case "ini":
                    default:
                        throw new Exception(
                            "Mtn do not read this extension, try [name].config.xml or [name].config.json ");
                }
            }
        }

        #endregion

        #region Xml

        private static void LoadXmlConfig(string server, string filePath)
        {
            var xml = new XmlDocument();
            for (var i = 0; i < 6; i++)
            {
                try
                {
                    StreamReader reader;
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        reader = new StreamReader(fs, Encoding.UTF8);
                        var xmlText = reader.ReadToEnd();
                        xml.LoadXml(xmlText);
                        fs.Close();
                    }
                    
                    break;
                }
                catch (Exception ex)
                {
                    Statistics.EventLogEntry(ex.GetAllMessagesMtn());
                    Thread.Sleep(400);
                }
            }
            
            if(!xml.HasChildNodes)
                return;

            var listNodes = xml.SelectNodes("//configuration/appSettings/add").Cast<XmlNode>();
            foreach (var node in listNodes)
            {
                var key = node.Attributes["key"].Value;
                var value = node.Attributes["value"].Value;
                m_Configs[server].Add(key, value);
            }

            listNodes = xml.SelectNodes("//configuration/connectionStrings/add").Cast<XmlNode>();
            foreach (var node in listNodes)
            {
                var key = node.Attributes["name"].Value;
                var value = node.Attributes["connectionString"].Value;
                var provider = node.Attributes["providerName"];
                var providerName = provider != null ? provider.Value : null;

                var cnn = new ConnectionStringSettings(key, value, providerName);
                m_CnnConfigs[server].Add(key, cnn);
            }
        }

        #endregion

        #region Json

        private static void LoadJsonConfig(string server, string filePath)
        {
            var configObj = new JsonConfig();
            for (var i = 0; i < 6; i++)
            {
                try
                {
                    var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    var reader = new StreamReader(fs, Encoding.UTF8);
                    var text = reader.ReadToEnd();
                    configObj = text.ToObjectFromJsonMtn<JsonConfig>();
                    break;
                }
                catch (Exception ex)
                {
                    Statistics.EventLogEntry(ex.GetAllMessagesMtn());
                    Thread.Sleep(400);
                }
            }

            var listNodes = configObj.config;
            foreach (var item in configObj.config)
            {
                m_Configs[server].Add(item.name, item.value);
            }

            foreach (var item in configObj.config)
            {
                var cnn = new ConnectionStringSettings(item.name, item.value, item.provider);
                m_CnnConfigs[server].Add(item.name, cnn);
            }
        }

        private class JsonConfig
        {
            public List<JsonConfigItem> config { get; set; }
            public List<JsonConfigItem> connection { get; set; }
        }

        private class JsonConfigItem
        {
            public string name { get; set; }
            public string value { get; set; }
            public string provider { get; set; }
        }

        #endregion

        private static string GetByHostString(String keyName, String server)
        {
            if (keyName.IsNullOrWhiteSpaceMtn() || server.IsNullOrWhiteSpaceMtn())
                return null;
            if (!m_Configs.ContainsKey(server))
            {
                LoadConfigFiles(server);
                if (!m_Configs.ContainsKey(server)) return null;
                
                var dic = m_Configs[server];
                if (dic == null)
                    return null;
                if (dic.ContainsKey(keyName))
                    return dic[keyName];
            }
            else
            {
                var dic = m_Configs[server];
                if (dic != null) return dic.ContainsKey(keyName)?dic[keyName]:null;
                LoadConfigFiles(server);
                dic = m_Configs[server];
                if (dic == null)
                    return null;
                if (dic.ContainsKey(keyName))
                    return dic[keyName];
            }
            return null;
        }
        public static string UrlHost { get; set; }
        private static string GetWebServerName()
        {
            string result = null;
            try
            {
                
                if (UrlHost != null)
                    result = UrlHost.ToLowerInvariant();
                else if (UseCoreConfiguration && HostingEnvironmentCore != null)
                {
                    result = HostingEnvironmentCore.EnvironmentName;
                }
            }
            catch (Exception ex)
            {
                Statistics.EventLogEntry(ex.GetAllMessagesMtn());
            }
            return result;
        }

        private static string GetHostName()
        {
            string result = null;
            try
            {
                return result = Dns.GetHostName().ToLowerInvariant();
            }
            catch (Exception ex)
            {
                Statistics.EventLogEntry(ex.GetAllMessagesMtn());
            }
            return result;
        }

        private static string GetMachineName()
        {
            string result = null;
            try
            {
                result = Environment.MachineName.ToLowerInvariant();
            }
            catch (Exception ex)
            {
                Statistics.EventLogEntry(ex.GetAllMessagesMtn());
            }
            return result;
        }

        #endregion
    }
}