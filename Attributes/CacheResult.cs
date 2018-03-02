using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Mtn.Library.Enums;
using Mtn.Library.Reflection;
using Mtn.Library.Service;
#if ! UseSystemExtensions
using Mtn.Library.Extensions;

#endif

namespace Mtn.Library.Attributes
{

    #region CacheResult

    /// <summary>
    ///     <para>
    ///         Using this attribute in the class will indicate that it must be use cache. Method must be virtual and used
    ///         with ServiceProxy or on AjaxMethod.
    ///     </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false), ComVisible(false)]
    public class CacheResult : MtnBaseAttribute
    {
        private Enum _mContainerType;

        /// <summary>
        ///     Contructor
        /// </summary>
        public CacheResult()
        {
            _mContainerType = CacheType.Hash;
        }

        /// <summary>
        ///     <para>Represents the enum with the type of storage caching, whether it is in memory, disk, etc..</para>
        /// </summary>
        public object ContainerType
        {
            get { return _mContainerType; }
            set { _mContainerType = (Enum) value; }
        }

        /// <summary>
        ///     <para>The hierarchy of named attributes.</para>
        /// </summary>
        public override int Hierarchy
        {
            get { return 0; }
        }

        /// <summary>
        ///     <para>Default call for atributtes by the Service Proxy before the methody body.</para>
        /// </summary>
        /// <param name="method">
        ///     <para>Method .</para>
        /// </param>
        /// <param name="il">
        ///     <para>Il from  assembly to override the method.</para>
        /// </param>
        /// <returns>
        ///     <para>true if can execute method</para>
        /// </returns>
        public override bool MtnBeforeExecution(MethodInfo method, ILHelper il)
        {
            if (method.ReturnType == typeof (void))
                throw new InvalidOperationException("Cache needs a return type, void is not allowed");

            ParameterInfo[] parameters = method.GetParameters();

            il
                .CreateLocalVar("Mtn.CacheKey", typeof (string), false)
                .ArrayCreate(typeof (object), parameters.Length + 1)
                .CreateLocalVar("Mtn.CacheParameters", typeof (object[]), false)
                .SetVar("Mtn.CacheParameters")
                .LoadVar("Mtn.CacheParameters")
                .LoadInt(0)
                .LoadString(GetKeyPrefix(method))
                .ArrayAdd()
                ;


            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parm = parameters[i];
                if (parm.ParameterType.IsByRef || parm.ParameterType.IsPointer)
                    throw new InvalidOperationException(
                        string.Format(
                            "Parameter '{0}' is not allowed. Cannot use reference or pointer types in cache method.",
                            parm.Name));

                il
                    .LoadVar("Mtn.CacheParameters") // load variable by name
                    .LoadInt(i + 1) // load a int value in stack
                    .LoadArgument(i + 1) // load a parameter
                    .Box(parm.ParameterType) // do boxing "cast"
                    .ArrayAdd(); // add to array
            }

            il
                .LoadVar("Mtn.CacheParameters")
                .InvokeMethod(typeof (CacheResult), "GetKey")
                .SetVar("Mtn.CacheKey")
                .LoadInt(Convert.ToInt32(ContainerType))
                .Box(ContainerType.GetType())
                .LoadVar("Mtn.CacheKey")
                .InvokeMethod(typeof (CacheResult), "GetCache")
                .SetVar("Mtn.ReturnValue")
                .LoadVar("Mtn.ReturnValue")
                .GotoIfNotNullOrTrue("MtnAfterCall")
                ;

            return true;
        }

        /// <summary>
        ///     <para>Default call for atributtes by the Service Proxy after the methody body.</para>
        /// </summary>
        /// <param name="method">
        ///     <para>Method .</para>
        /// </param>
        /// <param name="il">
        ///     <para>Il from  assembly to override the method.</para>
        /// </param>
        /// <returns>
        ///     <para>true if can execute method</para>
        /// </returns>
        public override bool MtnAfterExecution(MethodInfo method, ILHelper il)
        {
            if (method.ReturnType == typeof (void))
                throw new InvalidOperationException("Cache needs a return type, void is not allowed");

            il
                .LoadInt(Convert.ToInt32(ContainerType))
                .Box(ContainerType.GetType())
                .LoadVar("Mtn.CacheKey")
                .LoadVar("Mtn.ReturnValue")
                .InvokeMethod(typeof (CacheResult), "SetCache")
                ;
            return true;
        }

        /// <summary>
        ///     <para>Caches the result reported in value.</para>
        /// </summary>
        /// <param name="cacheType">
        ///     <para>Represents the enum with the type of storage caching, whether it is in memory, disk, etc..</para>
        /// </param>
        /// <param name="key">
        ///     <para>Represents the cache key.</para>
        /// </param>
        /// <param name="value">
        ///     <para>Represents the object to be put in cache.</para>
        /// </param>
        public static void SetCache(Enum cacheType, string key, object value)
        {
            Cache.Instance.Add(cacheType, value, key);
        }

        /// <summary>
        ///     <para>Returns caches result or null if don't find the key.</para>
        /// </summary>
        /// <param name="cacheType">
        ///     <para>Represents the enum with the type of storage caching, whether it is in memory, disk, etc..</para>
        /// </param>
        /// <param name="key">
        ///     <para>Represents the cache key.</para>
        /// </param>
        /// <returns>
        ///     <para>Returns caches result or null if don't find the key.</para>
        /// </returns>
        public static object GetCache(Enum cacheType, string key)
        {
            return Cache.Instance[cacheType, key];
        }

        /// <summary>
        ///     <para>Returns prefix to cache key by method token.</para>
        /// </summary>
        /// <param name="method">
        ///     <para>Represents the method body.</para>
        /// </param>
        /// <returns>
        ///     <para>Returns prefix to cache key by method token.</para>
        /// </returns>
        public static string GetKeyPrefix(MethodInfo method)
        {
            if (method.DeclaringType != null)
            {
                string key = method.DeclaringType.Name + "#" + method.MetadataToken.ToTrimStringMtn() + "#";

                return key;
            }
            return null;
        }

        /// <summary>
        ///     <para>Returns cache key by the parameters.</para>
        /// </summary>
        /// <param name="parameters">
        ///     <para>Represents the parameters.</para>
        /// </param>
        /// <returns>
        ///     <para>Returns cache key by the parameters</para>
        /// </returns>
        public static string GetKey(object[] parameters)
        {
            string key = "";
            foreach (object parm in parameters)
            {
                if (parm.IsPrimitiveMtn())
                    key += parm.ToTrimStringMtn();
                else
                {
                    string data = parm.ToJsonMtn();
                    byte[] arrByte = Encoding.UTF8.GetBytes(data);
                    byte[] arrByteHash = MD5.Create().ComputeHash(arrByte);
                    data = Encoding.UTF8.GetString(arrByteHash);
                    key += data.ToTrimStringMtn();
                }
            }
            return key;
        }
    }

    #endregion
}