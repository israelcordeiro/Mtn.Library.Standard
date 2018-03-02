using System;
using System.Reflection;
using Mtn.Library.Reflection;

namespace Mtn.Library.Attributes
{
    /// <summary>
    ///     <para>Abstract attribute for Mtn calls. Must be inherited by the attribute to be called by Service Proxy.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public abstract class MtnBaseAttribute : Attribute
    {
        /// <summary>
        ///     <para>The hierarchy of named attributes.</para>
        /// </summary>
        public virtual int Hierarchy
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        ///     <para>Default call for atributtes by the Service Proxy before the methody body.</para>
        /// </summary>
        /// <param name="method">
        ///     <para>Method .</para>
        /// </param>
        /// <param name="ilGen">
        ///     <para>Il from  assembly to override the method.</para>
        /// </param>
        /// <returns>
        ///     <para>true if can execute method</para>
        /// </returns>
        public virtual bool MtnBeforeExecution(MethodInfo method, ILHelper ilGen)
        {
            return true;
        }

        /// <summary>
        ///     <para>Default call for atributtes by the Service Proxy after the methody body.</para>
        /// </summary>
        /// <param name="method">
        ///     <para>Method .</para>
        /// </param>
        /// <param name="ilGen">
        ///     <para>Il from  assembly to override the method.</para>
        /// </param>
        /// <returns>
        ///     <para>true if can execute method</para>
        /// </returns>
        public virtual bool MtnAfterExecution(MethodInfo method, ILHelper ilGen)
        {
            return true;
        }
    }
}