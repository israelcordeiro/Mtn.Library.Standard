/*
 Thanks to people from http://www.albahari.com/nutshell/linqkit.html and http://tomasp.net/blog/linq-expand.aspx 
 Predicate and some extension we get there 
*/
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mtn.Library.Extensions
{
    /// <summary>
    /// Generates a predicate to "where" and "order by" in Linq.
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Indicates the order of sort in sql.
        /// </summary>
        public enum OrderTypeMtn
        {
            /// <summary>
            /// Ascending.
            /// </summary>
            Ascending,
            /// <summary>
            /// Descending.
            /// </summary>
            Descending
        }
        /// <summary>
        /// Represents the base class to predicate order by Entity.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity class.
        /// </typeparam>
        public class OrderByMtn<TEntity>
        {
            /// <summary>
            /// Represents the predicate.
            /// </summary>
            public OrderByMtn Predicate { get; set; }
            /// <summary>
            /// Constructor
            /// </summary>
            public OrderByMtn()
            {
                this.Predicate = new OrderByMtn();
                this.Predicate.ListOfOrders = new List<OrderByMtn>();
            }
        }
        /// <summary>
        /// Represents the base class to predicate order by Entity.
        /// </summary>
        public class OrderByMtn
        {
            /// <summary>
            /// List of OrderByMtn to use in query.
            /// </summary>
            internal List<OrderByMtn> ListOfOrders { get; set; }
            /// <summary>
            /// Order type (Ascending or Descending).
            /// </summary>
            public OrderTypeMtn OrderType { get; set; }
            /// <summary>
            /// Name of table field.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Represents the order they should be executed.
            /// </summary>
            public int Hierarchy { get; set; }

        }

        /// <summary>
        /// Initialize the predicate with true.
        /// </summary>
        /// <typeparam name="T">
        /// Entity class.
        /// </typeparam>
        /// <returns>
        /// Returns the predicate with true.
        /// </returns>
        public static Expression<Func<T, Boolean>> TrueMtn<T>() { return f => true; }
        /// <summary>
        /// Initialize the predicate with false.
        /// </summary>
        /// <typeparam name="T">
        /// Entity class.
        /// </typeparam>
        /// <returns>
        /// Returns the predicate with false.
        /// </returns>
        public static Expression<Func<T, Boolean>> FalseMtn<T>() { return f => false; }
        /// <summary>
        /// Initialize the predicate order.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity class.
        /// </typeparam>
        /// <returns>
        /// Returns the initial OrderByMtn to use order by predicate.
        /// </returns>
        public static OrderByMtn<TEntity> InitialOrderByMtn<TEntity>() { return new OrderByMtn<TEntity>(); }
        


 
    }
}
