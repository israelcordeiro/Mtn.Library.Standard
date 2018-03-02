using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

using System.IO;

using System.Data;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq.Expressions;
using System.Globalization;
using System.Runtime.Serialization;

using Mtn.Library.Extensions;
namespace Mtn.Library.ExtensionsLinqAux
{
    /// <summary>
    /// Linq to Sql extensions
    /// </summary>
    public static class Linq2Sql
    {
        #region Linq2SQL

        #region Expand
       
        /// <summary>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="query">
        /// <para>Query.</para>
        /// </param>
        /// <returns>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </returns>
        public static IQueryable<T> AsExpandableMtn<T>(this IQueryable<T> query)
        {
            if (query is ExpandableQuery<T>)
                return (ExpandableQuery<T>)query;
            return new ExpandableQuery<T>(query);
        }
        /// <summary>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </summary>
        /// <typeparam name="TDelegate">
        /// <para>Type of Delegate.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <returns>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </returns>
        public static Expression<TDelegate> ExpandMtn<TDelegate>(this Expression<TDelegate> expr)
        {
            return (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
        }

        /// <summary>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </summary>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <returns>
        /// <para>Returns an IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
        /// </returns>
        public static Expression ExpandMtn(this Expression expr)
        {
            return new ExpressionExpander().Visit(expr);
        }
        #endregion

        #region Invoke

        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// </summary>
        /// <typeparam name="TResult">
        /// <para>Result class.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<TResult>(this Expression<Func<TResult>> expr)
        {
            return expr.Compile().Invoke();
        }
       
        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// See http://tomasp.net/blog/linq-expand.aspx for information on how it's used.
        /// </summary>
        /// <typeparam name="T1">
        /// <para>The type of the parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="TResult">
        /// <para>The type of the return value of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <param name="arg1">
        /// <para>The parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1)
        {
            return expr.Compile().Invoke(arg1);
        }
       
        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// See http://tomasp.net/blog/linq-expand.aspx for information on how it's used.
        /// </summary>
        /// <typeparam name="T1">
        /// <para>The type of the first parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T2">
        /// <para>The type of the second parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="TResult">
        /// <para>The type of the return value of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <param name="arg1">
        /// <para>The first parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg2">
        /// <para>The second parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2)
        {
            return expr.Compile().Invoke(arg1, arg2);
        }
        
        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// See http://tomasp.net/blog/linq-expand.aspx for information on how it's used.
        /// </summary>
        /// <typeparam name="T1">
        /// <para>The type of the first parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T2">
        /// <para>The type of the second parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T3">
        /// <para>The type of the third parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="TResult">
        /// <para>The type of the return value of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <param name="arg1">
        /// <para>The first parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg2">
        /// <para>The second parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg3">
        /// <para>The third parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3);
        }
       
        /// <summary>
        /// <para>Extends the Expression class to invoke the lambda expression while making it still possible to translate query to T-SQL</para>
        /// See http://tomasp.net/blog/linq-expand.aspx for information on how it's used.
        /// </summary>
        /// <typeparam name="T1">
        /// <para>The type of the first parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T2">
        /// <para>The type of the second parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T3">
        /// <para>The type of the third parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="T4">
        /// <para>The type of the fourth parameter of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <typeparam name="TResult">
        /// <para>The type of the return value of the method that this delegate encapsulates.</para>
        /// </typeparam>
        /// <param name="expr">
        /// <para>Lambda expression.</para>
        /// </param>
        /// <param name="arg1">
        /// <para>The first parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg2">
        /// <para>The second parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg3">
        /// <para>The third parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <param name="arg4">
        /// <para>The fourth parameter of the method that this delegate encapsulates.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a value of the type specified by the TResult parameter.</para>
        /// </returns>
        internal static TResult InvokeMtn<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
        }

        #endregion

        #region ForEach

        /// <summary>
        /// <para>Through each element in source and performs the action method as parameter passing this element.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Type of IEnumerable.</para>
        /// </typeparam>
        /// <param name="source">
        /// <para>IEnumerable to be traversed.</para>
        /// </param>
        /// <param name="action">
        /// <para>Action to be executed.</para>
        /// </param>
        public static void ForEachMtn<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
        #endregion

        
        #region DumpCSV

        /// <summary>
        /// <para>Creates a *.csv file from an IQueryable query, dumping out the 'simple' properties/fields.</para>
        /// </summary>
        /// <param name="query">
        /// <para>Represents a SELECT query to execute.</para>
        /// </param>
        /// <param name="fileName">
        /// <para>The name of the file to create.</para>
        /// </param>
        /// <param name="deleteFile">
        /// <para>Whether or not to delete the file specified by <paramref name="fileName"/> if it exists.</para>
        /// </param>
        /// <param name="delimiter">
        /// <para>Another delimiter to replace the default (,) .</para>
        /// </param>
        /// <param name="createHeader">
        /// <para>Whether or not insert header (property name) on file.</para>
        /// </param>
        /// <param name="encoding"></param>
        /// <remarks>
        /// <para>If the <paramref name="query"/> contains any properties that are entity sets (i.e. rows from a FK relationship) the values will not be dumped to the file.</para>
        /// <para>This method is useful for debugging purposes or when used in other utilities such as LINQPad.</para>
        /// </remarks>
        public static void DumpCsvMtn(this IQueryable query, String fileName, Boolean deleteFile = true, String delimiter = ",", Boolean createHeader = true, Encoding encoding = null)
        {
        
            if (delimiter.IsNullOrEmptyMtn(true))
                delimiter = ",";

            if (File.Exists(fileName) && deleteFile)
            {
                File.Delete(fileName);
            }

            if (encoding == null)
                encoding = new UTF8Encoding();

            using (var output = new FileStream(fileName, FileMode.CreateNew))
            {
                using (var writer = new StreamWriter(output, encoding))
                {
                    var firstRow = true;

                    PropertyInfo[] properties = null;
                    Type type = null;

                    foreach (var r in query)
                    {
                        if (type == null)
                        {
                            type = r.GetType();
                            properties = type.GetProperties();
                        }

                        var firstCol = true;


                        if (createHeader)
                        {
                            if (firstRow)
                            {
                                foreach (var p in properties)
                                {
                                    if (!firstCol)
                                        writer.Write(",");
                                    else { firstCol = false; }

                                    writer.Write(p.Name);
                                }
                                writer.WriteLine();
                            }
                        }
                        firstRow = false;
                        firstCol = true;

                        foreach (var p in properties)
                        {
                            if (!firstCol)
                                writer.Write(delimiter);
                            else { firstCol = false; }
                            DumpValueMtn(p.GetValue(r, null), writer, delimiter);
                        }
                        writer.WriteLine();
                    }
                }
            }
        }
        #endregion

        #region DumpValue

        private static void DumpValueMtn(object v, StreamWriter writer, string delimiter)
        {
            if (v != null)
            {
                switch (Type.GetTypeCode(v.GetType()))
                {
                    // csv encode the value
                    case TypeCode.String:
                        string value = (string)v;
                        if (value.Contains(delimiter) || value.Contains('"') || value.Contains("\n"))
                        {
                            value = value.Replace("\"", "\"\"");

                            if (value.Length > 31735)
                            {
                                value = value.Substring(0, 31732) + "...";
                            }
                            writer.Write("\"" + value + "\"");
                        }
                        else
                        {
                            writer.Write(value);
                        }
                        break;

                    default:
                        writer.Write(v);
                        break;
                }
            }
        }
        #endregion

        #region IsAnonymous
       
        private static Boolean IsAnonymousMtn(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                       && type.IsGenericType && type.Name.Contains("AnonymousType")
                       && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                       && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;

        }
        #endregion

        #region Clone

        /// <summary>
        /// <para>Return a clone from the original entity.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="entity">
        /// <para>Entity to be cloned.</para>
        /// </param>
        /// <param name="nullableProperties">
        /// <para>Delimited string by comma with the names of properties to be set to null.</para>
        /// </param>
        /// <returns>
        /// <para>Return a clone from the original entity.</para>
        /// </returns>
        public static TEntity CloneMtn<TEntity>(this TEntity entity, String nullableProperties = null)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(TEntity));
            using (var ms = new System.IO.MemoryStream())
            {
                ser.WriteObject(ms, entity);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                TEntity retVal = (TEntity)ser.ReadObject(ms);

                if (nullableProperties.IsNullOrEmptyMtn(true) == false)
                {
                    foreach (string name in nullableProperties.SplitMtn(","))
                    {
                        retVal.GetType().GetProperty(name).SetValue(retVal, null, null);
                    }
                }

                return retVal;
            }
        }
        #endregion

        
        #endregion
    }
}
