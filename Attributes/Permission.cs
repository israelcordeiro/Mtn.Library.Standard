using System;
using System.Reflection;
using Mtn.Library.Reflection;
using Mtn.Library.Utils;
using Mtn.Library.Extensions;
namespace Mtn.Library.Attributes
{
    /// <summary>
    ///     <para>Performs a check to know if the user has permission.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class Permission : MtnBaseAttribute
    {
        private static Func<object, bool> _mPermissionAction1;
        private static Func<object, object, bool> _mPermissionAction2;
        private static Func<object, object, object, bool> _mPermissionAction3;
        private static Func<object, object, object, object, bool> _mPermissionAction4;
        private static object _mUnallowedValue;

        /// <summary>
        ///     <para>The hierarchy of named attributes.</para>
        /// </summary>
        public override int Hierarchy
        {
            get { return -999998; }
        }

        /// <summary>
        ///     <para>Method that is called to check if the user has permission. Needs to return bool and must have one parameter.</para>
        /// </summary>
        public static Func<object, bool> PermissionAction1
        {
            get
            {
                try
                {
                    if (_mPermissionAction1 == null &&
                        (Parameter.PermissionParameterCount == 1 || Parameter.PermissionParameterCount == 0))
                    {
                        var fnPerm = Parameter.GetPermissionMethod();
                        if (fnPerm!= null)
                            _mPermissionAction1 = (Func<object, bool>) fnPerm;
                        
                    }
                }
                catch(Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
                return _mPermissionAction1;
            }
            set { _mPermissionAction1 = value; }
        }

        /// <summary>
        ///     <para>Method that is called to check if the user has permission. Needs to return bool and must have two parameters.</para>
        /// </summary>
        public static Func<object, object, bool> PermissionAction2
        {
            get
            {
                try
                {
                    if (_mPermissionAction2 == null &&
                        (Parameter.PermissionParameterCount == 2 || Parameter.PermissionParameterCount == 0))
                    {
                        var fnPerm = Parameter.GetPermissionMethod();
                        if (fnPerm != null)
                            _mPermissionAction2 = (Func<object, object, bool>) fnPerm;
                    }
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
                return _mPermissionAction2;
            }
            set { _mPermissionAction2 = value; }
        }

        /// <summary>
        ///     <para>
        ///         Method that is called to check if the user has permission. Needs to return bool and must have three
        ///         parameters.
        ///     </para>
        /// </summary>
        public static Func<object, object, object, bool> PermissionAction3
        {
            get
            {
                try
                {
                    if (_mPermissionAction3 == null &&
                        (Parameter.PermissionParameterCount == 3 || Parameter.PermissionParameterCount == 0))
                    {
                        var fnPerm = Parameter.GetPermissionMethod();
                        if (fnPerm != null)
                            _mPermissionAction3 = (Func<object, object, object, bool>)fnPerm;
                    }
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
                return _mPermissionAction3;
            }
            set { _mPermissionAction3 = value; }
        }

        /// <summary>
        ///     <para>
        ///         Method that is called to check if the user has permission. Needs to return bool and must have four
        ///         parameters.
        ///     </para>
        /// </summary>
        public static Func<object, object, object, object, bool> PermissionAction4
        {
            get
            {
                try
                {
                    if (_mPermissionAction4 == null &&
                        (Parameter.PermissionParameterCount == 4 || Parameter.PermissionParameterCount == 0))
                    {
                        var fnPerm = Parameter.GetPermissionMethod();
                        if (fnPerm != null)
                        _mPermissionAction4 =
                            (Func<object, object, object, object, bool>)fnPerm;
                    }
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
                return _mPermissionAction4;
            }
            set { _mPermissionAction4 = value; }
        }

        /// <summary>
        ///     <para>First value to be passed to the PermissionAction. Default value is int.MinValue.</para>
        /// </summary>
        public object Value1 { get; set; }

        /// <summary>
        ///     <para>Second value to be passed to the PermissionAction. Default value is int.MinValue.</para>
        /// </summary>
        public object Value2 { get; set; }

        /// <summary>
        ///     <para>Third value to be passed to the PermissionAction. Default value is int.MinValue.</para>
        /// </summary>
        public object Value3 { get; set; }

        /// <summary>
        ///     <para>Fourth value to be passed to the PermissionAction. Default value is int.MinValue.</para>
        /// </summary>
        public object Value4 { get; set; }

        /// <summary>
        ///     <para>Mandatory. Represents the return in case of permission is invalid. Cannot be null.</para>
        /// </summary>
        public static object UnallowedResult
        {
            get { return _mUnallowedValue ?? (_mUnallowedValue = Parameter.GetUnauthorizedEntity()); }
            set { _mUnallowedValue = value; }
        }

        /// <summary>
        ///     <para>Returns true if permitted.</para>
        /// </summary>
        /// <param name="value1">
        ///     <para>First parameter to be checked</para>
        /// </param>
        /// <param name="value2">
        ///     <para>Second parameter to be checked</para>
        /// </param>
        /// <param name="value3">
        ///     <para>Third parameter to be checked</para>
        /// </param>
        /// <param name="value4">
        ///     <para>Fourth parameter to be checked</para>
        /// </param>
        /// <returns>
        ///     <para>Returns true if permitted.</para>
        /// </returns>
        public bool HasPermission(object value1, object value2, object value3, object value4)
        {
            var validate = false;
            if (PermissionAction1 != null)
                validate = PermissionAction1(value1);
            else if (PermissionAction2 != null)
                validate = PermissionAction2(value1, value2);
            else if (PermissionAction3 != null)
                validate = PermissionAction3(value1, value2, value3);
            else if (PermissionAction4 != null)
                validate = PermissionAction4(value1, value2, value3, value4);

            return validate;
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
            const BindingFlags bindFlag =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            var name = "MtnAfterPermission" + Guid.NewGuid();
            // for security reasons set to int.MinValue
            if (Value1 == null)
                Value1 = int.MinValue;
            if (Value2 == null)
                Value2 = int.MinValue;
            if (Value3 == null)
                Value3 = int.MinValue;
            if (Value4 == null)
                Value4 = int.MinValue;

            // create local variables
            il
                .CreateLocalVar("Mtn.IsPermitted", typeof (bool), false)
                .CreateLocalVar("Mtn.PermissionParameter1", Value1.GetType(), true)
                .CreateLocalVar("Mtn.PermissionParameter2", Value2.GetType(), true)
                .CreateLocalVar("Mtn.PermissionParameter3", Value3.GetType(), true)
                .CreateLocalVar("Mtn.PermissionParameter4", Value4.GetType(), true);

            // load Value1
            il
                .LoadVar("Mtn.PermissionParameter1")
                .LoadObject(Value1)
                .Box(Value1.GetType())
                .SetVar("Mtn.PermissionParameter1");

            // load Value2
            il
                .LoadObject(Value2)
                .Box(Value2.GetType())
                .SetVar("Mtn.PermissionParameter2");

            // load Value3
            il
                .LoadObject(Value3)
                .Box(Value3.GetType())
                .SetVar("Mtn.PermissionParameter3");

            // load Value4
            il
                .LoadObject(Value4)
                .Box(Value4.GetType())
                .SetVar("Mtn.PermissionParameter4");

            // put all variables in stack
            il
                .LoadVar("Mtn.PermissionParameter1")
                .UnboxAny(typeof (object))
                .LoadVar("Mtn.PermissionParameter2")
                .UnboxAny(typeof (object))
                .LoadVar("Mtn.PermissionParameter3")
                .UnboxAny(typeof (object))
                .LoadVar("Mtn.PermissionParameter4")
                .UnboxAny(typeof (object));

            // invoke method
            il
                .InvokeMethod(typeof (Permission), "HasPermission", bindFlag)
                .SetVar("Mtn.IsPermitted")
                .LoadVar("Mtn.IsPermitted")
                .GotoIfNotNullOrTrue(name);

            // if is invalid returns the default value
            il
                .InvokeMethod(typeof (Permission).GetProperty("UnallowedResult").GetGetMethod())
                .SetVar("Mtn.ReturnValue")
                .LoadVar("Mtn.ReturnValue")
                .GotoIfNotNullOrTrue("MtnAfterAll");

            il.MarkLabel(name);
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
            return true;
        }
    }
}