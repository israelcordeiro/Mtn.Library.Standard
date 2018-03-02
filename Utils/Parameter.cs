using System;
using System.Linq;
#if ! UseSystemExtensions
using Mtn.Library.Extensions;
#endif
using System.Reflection;


namespace Mtn.Library.Utils
{

    /// <summary>    
    /// <para>Class Mtn.Library to get parameters for the features of the library.</para>
    /// </summary>
    public class Parameter
    {

        #region GetUnauthorizedMethod
        private static Func<bool> _mGetValidateMethod;
        private static MethodInfo _mValidateMethod;
        /// <summary>    
        /// <para>Returns the method that will be used to validate methods with Authorization attribute.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the method that will be used to validate methods with Authorization attribute.</para>
        /// </returns>
        public static Func<bool> GetValidateMethod()
        {
            if (_mGetValidateMethod == null)
            {
                try
                {
                    string strAsm = Configuration.Config.GetString("Mtn.Library.ValidateAssembly");
                    string strClass = Configuration.Config.GetString("Mtn.Library.ValidateClass");
                    string strMethod = Configuration.Config.GetString("Mtn.Library.ValidateMethod");

                    var asm = Assembly.Load(strAsm);
                    var module = asm.GetModules().FirstOrDefault();
                    if (module != null)
                    {
                        var methods = module.GetType(strClass).GetMethods();
                        _mValidateMethod =
                            methods
                                .Where(method => method.Name == strMethod).FirstOrDefault(method => method.GetParameters().Length == 0);
                    }

                    Func<bool> fnReturnValidate = ()
                        => (bool)_mValidateMethod.Invoke(null, null);

                    _mGetValidateMethod = fnReturnValidate;
                    return _mGetValidateMethod;
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
            }
            return _mGetValidateMethod;
        }
        /// <summary>
        /// Set validate function
        /// </summary>
        /// <param name="validateFunction"></param>
        public static void SetValidateMethod(Func<bool> validateFunction)
        {
            _mGetValidateMethod = validateFunction;
        }
        #endregion

        #region GetUnauthorizedMethod
        private static Func<object, bool> _mGetPermissionMethod1;
        private static Func<object, object, bool> _mGetPermissionMethod2;
        private static Func<object, object, object, bool> _mGetPermissionMethod3;
        private static Func<object, object, object, object, bool> _mGetPermissionMethod4;
        private static int _mPermissionParameterCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstParameterMethod"></param>
        /// <param name="secondParameterMethod"></param>
        /// <param name="thirdParameterMethod"></param>
        /// <param name="fourthParameterMethod"></param>
        public static void SetPermissionMethod(Func<object, bool> firstParameterMethod = null,
            Func<object, object, bool> secondParameterMethod  = null,
            Func<object, object, object, bool> thirdParameterMethod  = null,
           Func<object, object, object, object, bool> fourthParameterMethod  = null)
        {
            if (_mPermissionParameterCount == 0)
                _mPermissionParameterCount = Configuration.Config.GetInt32("Mtn.Library.PermissionTotalParameters");
            if (_mPermissionParameterCount == 0 || _mPermissionParameterCount > 4)
                throw new ArgumentException("You need set PermissionParameterCount to 1, 2, 3 or 4");
            switch (_mPermissionParameterCount)
            {
                case 1:
                    _mGetPermissionMethod1 = firstParameterMethod;
                    break;
                case 2:
                    _mGetPermissionMethod2 = secondParameterMethod;
                    break;
                case 3:
                    _mGetPermissionMethod3 =  thirdParameterMethod;
                    break;
                case 4:
                    _mGetPermissionMethod4 = fourthParameterMethod;
                    break;
            }
            
        }
        /// <summary>    
        /// <para>Returns the number of parameters that the method for attribute Permission has.</para>
        /// </summary>
        public static int PermissionParameterCount
        {
            get { return Parameter._mPermissionParameterCount; }
            set
            {
                Parameter._mPermissionParameterCount = value;
                if (_mPermissionParameterCount == 0 || _mPermissionParameterCount > 4)
                    throw new ArgumentException("You need set PermissionParameterCount to 1, 2, 3 or 4");
            }


        }

        private static MethodInfo _mPermissionMethod;

       

        /// <summary>    
        /// <para>Returns the method that will be used to check methods with Permission attribute.</para>
        /// </summary>
        /// <returns>
        /// <para>Returns the method that will be used to check methods with Permission attribute.</para>
        /// </returns>
        public static object GetPermissionMethod()
        {
            if (_mPermissionParameterCount == 0)
            {
                try
                {
                    string strAsm = Configuration.Config.GetString("Mtn.Library.PermissionAssembly");
                    string strClass = Configuration.Config.GetString("Mtn.Library.PermissionClass");
                    string strMethod = Configuration.Config.GetString("Mtn.Library.PermissionMethod");
                    var lperm = Configuration.Config.GetNullableInt32("Mtn.Library.PermissionTotalParameters");
                    _mPermissionParameterCount = lperm.HasValue?lperm.Value: 0;
                    if (_mPermissionParameterCount == 0 || _mPermissionParameterCount > 4)
                       return null;

                    var asm = Assembly.Load(strAsm);
                    var module = asm.GetModules().FirstOrDefault();
                    if (module != null)
                    {
                        var methods = module.GetType(strClass).GetMethods();
                        _mPermissionMethod =
                            methods
                                .Where(method => method.Name == strMethod).FirstOrDefault(method => method.GetParameters().Length == _mPermissionParameterCount);
                    }

                    switch (_mPermissionParameterCount)
                    {
                        case 1:
                            {
                                Func<object, bool> fnReturnPermission1 = (value1)
                                    =>
                                {
                                    object[] parms = new object[1];
                                    parms[0] = value1;
                                    return (bool)_mPermissionMethod.Invoke(null, parms);

                                };
                                _mGetPermissionMethod1 = fnReturnPermission1;
                            }
                            break;
                        case 2:
                            {
                                Func<object, object, bool> fnReturnPermission2 = (value1, value2)
                                    =>
                                {
                                    object[] parms = new object[2];
                                    parms[0] = value1;
                                    parms[1] = value2;
                                    return (bool)_mPermissionMethod.Invoke(null, parms);

                                };
                                _mGetPermissionMethod2 = fnReturnPermission2;
                            }
                            break;
                        case 3:
                            {
                                Func<object, object, object, bool> fnReturnPermission3 = (value1, value2, value3)
                                    =>
                                {
                                    object[] parms = new object[3];
                                    parms[0] = value1;
                                    parms[1] = value2;
                                    parms[2] = value3;
                                    return (bool)_mPermissionMethod.Invoke(null, parms);

                                };
                                _mGetPermissionMethod3 = fnReturnPermission3;
                            }
                            break;
                        case 4:
                            {
                                Func<object, object, object, object, bool> fnReturnPermission4 = (value1, value2, value3, value4)
                                    =>
                                {
                                    object[] parms = new object[4];
                                    parms[0] = value1;
                                    parms[1] = value2;
                                    parms[2] = value3;
                                    parms[3] = value4;
                                    return (bool)_mPermissionMethod.Invoke(null, parms);

                                };
                                _mGetPermissionMethod4 = fnReturnPermission4;
                            }
                            break;
                    }

                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
            }
            switch (_mPermissionParameterCount)
            {
                case 1:
                    return _mGetPermissionMethod1;
                case 2:
                    return _mGetPermissionMethod2;
                case 3:
                    return _mGetPermissionMethod3;
                case 4:
                    return _mGetPermissionMethod4;
                default:
                    return null;
            }
        }
        #endregion

        #region GetUnauthorizedEntity and GetUnallowedEntity
        private static object _mGetUnauthorizedEntity;
        /// <summary>    
        /// <para>Returns the object that should be returned when the methods with Authorization attribute to not having authorization.</para>
        /// </summary>        
        /// <returns>
        /// <para>Returns the object that should be returned when the methods with Authorization attribute to not having authorization.</para>
        /// </returns>
        public static object GetUnauthorizedEntity()
        {
            if (_mGetUnauthorizedEntity == null)
            {
                try
                {
                    string strEnt = Configuration.Config.GetString("Mtn.Library.UnauthorizedEntity");
                    string strEntType = Configuration.Config.GetString("Mtn.Library.UnauthorizedEntity.Type");
                    if (strEntType.IsNullOrEmptyMtn(true))
                        _mGetUnauthorizedEntity = strEnt.ToObjectFromJsonMtn();
                    else
                        _mGetUnauthorizedEntity = strEnt.ToObjectFromJsonMtn(Type.GetType(strEntType));
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
            }
            return _mGetUnauthorizedEntity;
        }

        private static object _mGetUnallowedEntity;
        /// <summary>    
        /// <para>Returns the object that should be returned when the methods with Permission attribute to not having permission.</para>
        /// </summary>    
        /// <returns>
        /// <para>Returns the object that should be returned when the methods with Permission attribute to not having permission.</para>
        /// </returns>
        public static object GetUnallowedEntity()
        {
            if (_mGetUnallowedEntity == null)
            {
                try
                {
                    string strEnt = Configuration.Config.GetString("Mtn.Library.UnallowedEntity");
                    string strEntType = Configuration.Config.GetString("Mtn.Library.UnallowedEntity.Type");
                    if (strEntType.IsNullOrEmptyMtn(true))
                        _mGetUnallowedEntity = strEnt.ToObjectFromJsonMtn();
                    else
                        _mGetUnallowedEntity = strEnt.ToObjectFromJsonMtn(Type.GetType(strEntType));
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
            }
            return _mGetUnallowedEntity;
        }
        #endregion

        #region GetCacheTime
        /// <summary>    
        /// <para>Time in seconds for automatic verification of cache expired, default is 20 seconds.</para>
        /// </summary>   
        public static int GetCacheTime
        {
            get
            {
                try
                {
                    string retVal = Configuration.Config.GetString("Mtn.Library.CacheInterval");
                    return retVal.IsNullOrEmptyMtn(true) ? 20 : int.Parse(retVal);
                }
                catch
                {
                    return 20;
                }
            }
        }
        #endregion

        #region Statistics
        /// <summary>
        /// Default event source for statistics
        /// </summary>
        public static String EventSource
        {
            get
            {
                string strEnt = Configuration.Config.GetString("Mtn.Library.EventSource");
                if (strEnt.IsNullOrWhiteSpaceMtn())
                {
                    strEnt = "MtnLibrary";
                }
                return strEnt;
            }
        }
        /// <summary>
        /// Default event source for statistics
        /// </summary>
        public static String EventLogName
        {
            get
            {
                string strEnt = Configuration.Config.GetString("Mtn.Library.EventLogName");
                if (strEnt.IsNullOrWhiteSpaceMtn())
                {
                    strEnt = "Application";
                }
                return strEnt;
            }
        }
        /// <summary>
        /// Default event source for statistics
        /// </summary>
        public static String StatisticsFileName
        {
            get
            {
                string strEnt = Configuration.Config.GetString("Mtn.Library.StatisticsFileName");
                if (strEnt.IsNullOrWhiteSpaceMtn())
                {
                    strEnt = DateTime.Now.ToString("{yyyy-MM-dd}-MtnStatistics.txt");
                }
                return strEnt;
            }
        }
        #endregion

        /*
        #region  Memcached servers

        private static MemcachedClient _getMemcachedClient = null;

        /// <summary>    
        /// <para>Return a list of memcache servers.</para>
        /// </summary>   
        public static MemcachedClient MemcachedClient
        {
            get
            {
                if (_getMemcachedClient == null)
                    _getMemcachedClient = new MemcachedClient();
                return _getMemcachedClient;
            }
        }

        #endregion

        #region  AppFabric servers
        /// <summary>    
        /// <para>Return a list of AppFabric servers.</para>
        /// </summary>   
        public static List<String> AppFabricServerList
        {
            get
            {
                try
                {
                    var retVal = Configuration.Config.GetString("Mtn.Library.AppFabricServerList");
                    return retVal.IsNullOrWhiteSpaceMtn() ? new List<string>() : retVal.SplitMtn(",").ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }
        #endregion
        */
    }
}
