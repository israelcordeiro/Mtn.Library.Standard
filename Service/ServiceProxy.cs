using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Mtn.Library.Extensions;
using Mtn.Library.Reflection;
using Mtn.Library.Attributes;

namespace Mtn.Library.Service
{
    /// <summary>    
    /// <para>Generates a proxy for the business layers  allowing use many functionality of Mtn , such the cache, among others.</para>
    /// </summary>
    /// <typeparam name="TEntity">
    /// <para>Entity class.</para>
    /// </typeparam>
    public static class ServiceProxy<TEntity>
    {
        #region attributes & properties
        private static readonly Dictionary<string, TEntity> Dict = new Dictionary<string, TEntity>();
        /// <summary>
        /// <para>Returns the object representing the Entity, using the default constructor, ie without input parameters.</para>
        /// </summary>
        public static TEntity Default
        {
            get
            {
                return Create();
            }
        }
        #endregion

        #region Create
        /// <summary>
        /// <para>Returns the object representing the Entity, using the constructor with the input parameters.</para>
        /// </summary>
        /// <param name="parametersContructor">
        /// <para>Array of object to be used as input parameters of a class, use an empty array to the default constructor.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the object representing the Entity, using the constructor with the input parameters.</para>
        /// </returns>
        public static TEntity Create(params object[] parametersContructor)
        {
            var t = typeof(TEntity);

            var key = t.Name + parametersContructor.Count().ToString();

            if (!Dict.ContainsKey(key))
            {
                CheckClassType(t);
                var methods = new List<MethodInfo>();
                var typeBuilder = CreateTypeBuilder();
                var baseConstructorInfo = t.GetConstructor(parametersContructor.Select(objType => objType.GetType()).ToArray());

                var constructorBuilder = typeBuilder.DefineConstructor(
                             MethodAttributes.Public,
                             CallingConventions.Standard,
                             Type.EmptyTypes);

                var ilGenerator = new ILHelper(constructorBuilder.GetILGenerator());
                ilGenerator
                    .LoadThis()                      // Load "this"
                    .DefineConstructor(baseConstructorInfo)    // Call the base constructor
                    .Return();

                AddMethods(methods, t);

                foreach (var method in methods)
                {
                    if (!method.IsVirtual || method.Name == "Equals" || method.Name == "GetHashCode" || method.Name == "ToString" || method.Name == "Finalize" || method.IsFinal)
                        continue;

                    if (method.IsPrivate && !method.IsAssembly)
                        continue;

                    if (!method.IsPublic && (method.Attributes & System.Reflection.MethodAttributes.Family) != System.Reflection.MethodAttributes.Family)
                        continue;

                    try
                    {
                        if (Core.Parameter.IsFulltrust)
                        {
                            var methodBuilder = CreateMethod(method, typeBuilder);
                            typeBuilder.DefineMethodOverride(methodBuilder, method);
                        }   
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {   

                    }
                }

                var typeCon = typeBuilder.CreateTypeInfo();
                var d = (TEntity)Activator.CreateInstance(typeCon);
                var name = t.Name + parametersContructor.Count();
                try
                {
                    if (!Dict.ContainsKey(name))
                        Dict.Add(name, d);
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
                // to debug using ildasm,yes i am so crazy sometimes, lol
                //string assName =  ReflectionUtility.AssemblyBuilder.GetName().Name + ".dll"; //"Mtn.DinamicAssembly.dll";
                //ReflectionUtility.AssemblyBuilder.Save(assName);
            }

            return Dict[key];
        }
        #endregion

        #region CreateMethod
        /// <summary>
        /// <para>Creates the method to override and returns MethodBuilder</para>
        /// </summary>
        /// <param name="methodInfo">Method to override</param>
        /// <param name="typeBuilder">TypeBuilder of original assembly </param>
        /// <returns>
        /// <para>Returns the MethodBuilder.</para>
        /// </returns>
        private static MethodBuilder CreateMethod(MethodInfo methodInfo, TypeBuilder typeBuilder)
        {

            List<MtnBaseAttribute> mtnAtributtes = methodInfo.GetCustomAttributes(typeof(MtnBaseAttribute), false).Cast<MtnBaseAttribute>().OrderBy(k => k.Hierarchy).ToList();
            ParameterInfo[] parameters = methodInfo.GetParameters();

            Type returnType = methodInfo.ReturnType;

            List<Type> argumentTypes = new List<Type>();
            foreach (ParameterInfo parameterInfo in parameters)
                argumentTypes.Add(parameterInfo.ParameterType);

            // Define the method
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, methodInfo.Attributes, returnType, argumentTypes.ToArray());

            if (methodInfo.IsGenericMethod)
            {
                string[] genericParms = methodInfo.GetGenericArguments().Where(i => i.FullName == null).Select(i => i.Name).ToArray();
                methodBuilder.DefineGenericParameters(genericParms);
            }

            ILHelper ilGenerator = new ILHelper(methodBuilder.GetILGenerator());

            DefineLocalVariables(ilGenerator);

            bool invokeMethod = true;
            ilGenerator.MarkLabel("MtnHighPriority");
            foreach (var mtnAttr in mtnAtributtes.Where(a => a.Hierarchy < -100))
            {
                invokeMethod &= mtnAttr.MtnBeforeExecution(methodInfo, ilGenerator);
            }

            ilGenerator.MarkLabel("MtnBeforeCall");

            foreach (var mtnAttr in mtnAtributtes.Where(a => a.Hierarchy >= -100))
            {
                invokeMethod &= mtnAttr.MtnBeforeExecution(methodInfo, ilGenerator);
            }

            if (invokeMethod)
                InvokeMethodProxy(ilGenerator, methodInfo, parameters);

            ilGenerator.MarkLabel("MtnAfterCall");

            foreach (var mtnAttr in mtnAtributtes)
            {
                mtnAttr.MtnAfterExecution(methodInfo, ilGenerator);
            }
            ilGenerator.MarkLabel("MtnAfterAll");
            if (methodInfo.ReturnType != typeof(void))
            {
                ilGenerator
                    .LoadVar("Mtn.ReturnValue")
                    .UnboxAny(methodInfo.ReturnType);
            }

            ilGenerator.Return();
            return methodBuilder;

        }

        #endregion

        #region DefineLocalVariables
        /// <summary>
        /// <para>Define the variables that will be used later by internal methods </para>
        /// </summary>
        /// <param name="il">ILHelper</param>
        private static void DefineLocalVariables(ILHelper il)
        {
            il
                .CreateLocalVar("Mtn.ReturnValue", typeof(object), false);
        }
        #endregion

        #region InvokeMethodProxy
        /// <summary>
        /// <para>Invoke the method </para>
        /// </summary>
        /// <param name="il"></param>
        /// <param name="methodInfo"></param>
        /// <param name="parameters"></param>
        private static void InvokeMethodProxy(ILHelper il, MethodInfo methodInfo, ParameterInfo[] parameters)
        {
            //il.BeginTry();

            for (int i = 0; i <= parameters.Length; i++)
                il.LoadArgument(i);

            il.InvokeMethod(methodInfo);

            if (methodInfo.ReturnType != typeof(void))
                il
                    .Box(methodInfo.ReturnType)
                    .SetVar("Mtn.ReturnValue");
            //il.BeginCatch();
            //il.ReThrow();
            //il.EndTry();

        }
        #endregion

        #region AddMethods
        private static void AddMethods(List<MethodInfo> methods, Type type)
        {
            methods.AddRange(type.GetMethods());

            foreach (var inter in type.GetInterfaces())
                AddMethods(methods, inter);

        }
        #endregion

        #region AddProperties
        private static void AddProperties(List<PropertyInfo> properties, Type type)
        {
            properties.AddRange(type.GetProperties());

            foreach (var inter in type.GetInterfaces())
                AddProperties(properties, inter);
        }
        #endregion

        #region CreateTypeBuilder
        private static TypeBuilder CreateTypeBuilder()
        {
            Type t = typeof(TEntity);

            TypeBuilder typeBuilder = Reflection.Utils.ModuleBuilder.DefineType(t.Name + Guid.NewGuid().ToString(),
                                          TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, t);

            return typeBuilder;

        }
        private static void CheckClassType(Type type)
        {
            if (!type.IsPublic)
                throw new InvalidOperationException("The class : " + type.Name + " must be Public.");

            if (type.IsSealed)
                throw new InvalidOperationException("The class : " + type.Name + " can not be Sealed.");
        }
        #endregion

        //#region ReturnProxyName
        //private static string ReturnProxyName(Type type)
        //{
        //    if (!type.IsGenericType)
        //        return type.Name + "ServiceProxy";

        //    return type.Name;
        //}
        //#endregion
    }

}
