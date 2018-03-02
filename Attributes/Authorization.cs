using System;
using System.Reflection;
using Mtn.Library.Reflection;
using Mtn.Library.Utils;

namespace Mtn.Library.Attributes
{
    /// <summary>
    ///     <para>Performs a validation to know if the user was  logged or not.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class Authorization : MtnBaseAttribute
    {
        private static object _mUnauthorizedValue;

        private static Func<bool> _mValidateAction;

        /// <summary>
        ///     <para>The hierarchy of named attributes.</para>
        /// </summary>
        public override int Hierarchy
        {
            get { return -999999; }
        }

        /// <summary>
        ///     <para>
        ///         Method that is called to validate if the user was  logged or not. Needs to return bool and have zero
        ///         parameters.
        ///     </para>
        /// </summary>
        public static Func<bool> ValidateAction
        {
            get
            {
                if (_mValidateAction == null)
                    _mValidateAction = Parameter.GetValidateMethod();
                if (_mValidateAction == null)
                {
                }
                return _mValidateAction;
            }
            set { _mValidateAction = value; }
        }

        /// <summary>
        ///     <para>Mandatory. Represents the return in case of authorization be invalid. Cannot be null.</para>
        /// </summary>
        public static object UnauthorizedValue
        {
            get { return _mUnauthorizedValue ?? (_mUnauthorizedValue = Parameter.GetUnauthorizedEntity()); }
            set { _mUnauthorizedValue = value; }
        }

        /// <summary>
        ///     <para>Returns true if authorized .</para>
        /// </summary>
        /// <returns>
        ///     <para>true if authorized.</para>
        /// </returns>
        public bool IsAuthorized()
        {
            var validate = false;

            if (ValidateAction != null)
                validate = ValidateAction();

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

            il
                .CreateLocalVar("Mtn.IsAuthorized", typeof (bool), false)
                .LoadVar("Mtn.IsAuthorized")
                .InvokeMethod(typeof (Authorization), "IsAuthorized", bindFlag)
                .SetVar("Mtn.IsAuthorized")
                .LoadVar("Mtn.IsAuthorized")
                .GotoIfNotNullOrTrue("MtnAfterAuthorization");

            il
                .InvokeMethod(typeof (Authorization).GetProperty("UnauthorizedValue").GetGetMethod())
                .SetVar("Mtn.ReturnValue")
                .LoadVar("Mtn.ReturnValue")
                .GotoIfNotNullOrTrue("MtnAfterAll");

            il.MarkLabel("MtnAfterAuthorization");
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