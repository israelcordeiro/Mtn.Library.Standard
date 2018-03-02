using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
#if ! UseSystemExtensions 
using Mtn.Library.Extensions;
#endif

namespace Mtn.Library.Reflection
{
    /// <summary>
    /// ReflectionUtility used internal.    
    /// </summary>
    public static class Utils
    {
         private static ModuleBuilder _mModuleBuilder;
        private static readonly object MObjLock = new object();
        private static readonly object MObjLockAsm = new object();
        private static AssemblyBuilder _mAssemblyBuilder;
        
        private static readonly Dictionary<string, Assembly> AssemblyContainer =
            new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly Dictionary<string, dynamic> AssemblyClassInstanceContainer =
            new Dictionary<string, dynamic>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly Dictionary<string, Type> AssemblyTypeInstanceContainer =
            new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly Dictionary<string, MethodInfo> AssemblyMethodContainer =
            new Dictionary<string, MethodInfo>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        ///Returns the AssemblyBuilder for internal functions.
        /// </summary>
        public static AssemblyBuilder AssemblyBuilder
        {
            get
            {
                if (_mAssemblyBuilder == null)
                {
                    lock (MObjLockAsm)
                    {
                        string name = "Mtn.Library.DynamicAssembly." + Guid.NewGuid().ToString();
                        _mAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name),
                                                                                     AssemblyBuilderAccess.Run);
                    }
                }
                return _mAssemblyBuilder;
            }
        }

        /// <summary>
        ///Returns the ModuleBuilder for internal functions.
        /// </summary>
        public static ModuleBuilder ModuleBuilder
        {
            get
            {
                if (_mModuleBuilder != null)
                    return _mModuleBuilder;
                else
                {
                    lock (MObjLock)
                    {
                        string name = "Mtn.Library.DynamicAssembly.DynamicModule." + Guid.NewGuid().ToString();
                        _mModuleBuilder = AssemblyBuilder.DefineDynamicModule(name);
                    }
                }
                return _mModuleBuilder;
            }
        }


        /// <summary>
        /// Gets the assembly.
        /// </summary>
        /// <param name="assemblyName">The name.</param>
        /// <param name="className">Name of the instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="isField">if set to <c>true</c> [is field].</param>
        /// <param name="cache">if set to <c>true</c> [cache].</param>
        /// <param name="isStatic">if set to <c>true</c> [is static].</param>
        /// <param name="parms">The parms.</param>
        /// <param name="rethrow">if set to <c>true</c> [rethrow].</param>
        /// <returns></returns>
        public static dynamic ExecuteAssemblyMethod(String assemblyName, String className, String methodName = "",
                                                    String propertyName = "", Boolean isField = false, Boolean cache = true,
                                                    Boolean isStatic = false, Object[] parms = null, Boolean rethrow = false)
        {
            var assembly = AssemblyContainer.ContainsKey(assemblyName) ? AssemblyContainer[assemblyName] : null;
            var type = AssemblyTypeInstanceContainer.ContainsKey(assemblyName + className)
                           ? AssemblyTypeInstanceContainer[assemblyName + className]
                           : null;
            var instance = AssemblyClassInstanceContainer.ContainsKey(assemblyName + className)
                               ? AssemblyClassInstanceContainer[assemblyName + className]
                               : null;
            var method = AssemblyMethodContainer.ContainsKey(assemblyName + className + methodName)
                             ? AssemblyMethodContainer[assemblyName + className + methodName]
                             : null;

            dynamic dynResponse = null;

            if (!cache && assembly != null)
            {
                assembly = null;
                AssemblyContainer.Remove(assemblyName);
                if (type != null)
                    AssemblyTypeInstanceContainer.Remove(assemblyName + className);

                if (method != null)
                    AssemblyMethodContainer.Remove(assemblyName + className + methodName);

                if (instance != null)
                    AssemblyClassInstanceContainer.Remove(assemblyName + className);
            }

            if (assembly == null)
            {
                try
                {
                    assembly = Thread.GetDomain().Load(assemblyName);
                    AssemblyContainer.Add(assemblyName, assembly);
                }
                catch
                {
                    try
                    {
                        assembly = Thread.GetDomain().Load(assemblyName + ".dll");
                    }
                    catch
                    {
                        if (rethrow)
                            throw;
                        else
                            return null;
                    }
                }
            }
            try
            {
                if (type == null)
                {
                    type = assembly.GetType(className);
                    AssemblyTypeInstanceContainer.Add(assemblyName + className, type);
                }
                Type[] types = null;
                if (parms != null && parms.Length > 0)
                {
                    types = new Type[parms.Length];
                    for (var i = 0; i < parms.Length; i++)
                    {
                        types[i] = parms[i].GetType();
                    }
                }
                else
                {
                    types = Type.EmptyTypes;
                }

                if (!methodName.IsNullOrEmptyMtn() && type != null && method == null)
                {
                    if (isStatic)
                    {
                        if (propertyName.IsNullOrEmptyMtn())
                            return type.InvokeMember(methodName,
                                                     BindingFlags.InvokeMethod | BindingFlags.Static |
                                                     BindingFlags.Public |
                                                     BindingFlags.Instance | BindingFlags.DeclaredOnly, null, null,
                                                     parms);
                        else
                        {
                            if (isField)
                            {
                                if (instance == null)
                                {
                                    var prop = type.GetField(propertyName);
                                    instance = prop.GetValue(null);
                                    AssemblyClassInstanceContainer.Add(assemblyName + className, instance);
                                }
                                method = type.GetMethod(methodName, types);
                                AssemblyMethodContainer.Add(assemblyName + className + methodName, method);
                            }
                            else
                            {
                                var prop = type.GetProperty(propertyName);
                                type = prop.GetType();
                            }

                        }
                    }
                    else
                    {
                        if (instance == null)
                            instance = Activator.CreateInstance(type);
                        AssemblyClassInstanceContainer.Add(assemblyName + className, instance);

                        method = type.GetMethod(methodName, types);
                        AssemblyMethodContainer.Add(assemblyName + className + methodName, method);
                    }
                }
                if (method != null)
                    dynResponse = method.Invoke(instance, parms);
            }
            catch
            {
                if (rethrow)
                    throw;
                else
                    return null;
            }
            return dynResponse;
        }
    }
}
