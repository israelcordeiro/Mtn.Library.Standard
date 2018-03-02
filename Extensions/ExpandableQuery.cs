// This is based on the excellent work of Tomas Petricek: http://tomasp.net/blog/linq-expand.aspx
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using Mtn.Library.ExtensionsLinqAux;

namespace Mtn.Library.Extensions
{
    /// <summary>
    /// <para>An IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.</para>
    /// </summary>
    /// <typeparam name="T">
    /// <para>Entity class.</para>
    /// </typeparam>
    /// <remarks>
    /// </remarks>
    internal class ExpandableQuery<T> : IOrderedQueryable<T>
    {
        ExpandableQueryProvider<T> _provider;
        IQueryable<T> _inner;

        internal IQueryable<T> InnerQuery { get { return _inner; } }			// Original query, that we're wrapping

        internal ExpandableQuery(IQueryable<T> inner)
        {
            _inner = inner;
            _provider = new ExpandableQueryProvider<T>(this);
        }

        Expression IQueryable.Expression { get { return _inner.Expression; } }
        Type IQueryable.ElementType { get { return typeof(T); } }
        IQueryProvider IQueryable.Provider { get { return _provider; } }
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>
        /// Enumerator
        /// </returns>
        public IEnumerator<T> GetEnumerator() { return _inner.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _inner.GetEnumerator(); }
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>
        /// String value
        /// </returns>
        public override string ToString() { return _inner.ToString(); }
    }

    /// <summary>
    /// <para>ExpandableQueryProvider.</para>
    /// </summary>
    /// <typeparam name="T">
    /// <para>Entity class.</para>
    /// </typeparam>
    internal class ExpandableQueryProvider<T> : IQueryProvider
    {
         ExpandableQuery<T> _query;

        internal ExpandableQueryProvider(ExpandableQuery<T> query)
        {
            _query = query;
        }

        // The following four methods first call ExpressionExpander to visit the expression tree, then call
        // upon the inner query to do the remaining work.

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new ExpandableQuery<TElement>(_query.InnerQuery.Provider.CreateQuery<TElement>(expression.ExpandMtn()));
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return _query.InnerQuery.Provider.CreateQuery(expression.ExpandMtn());
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return _query.InnerQuery.Provider.Execute<TResult>(expression.ExpandMtn());
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return _query.InnerQuery.Provider.Execute(expression.ExpandMtn());
        }

    }


}
