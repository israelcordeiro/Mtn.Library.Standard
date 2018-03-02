// This comes from Matt Warren's sample:
// http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Mtn.Library.Extensions
{
    #region ExpressionVisitor abstract
    /// <summary>
    /// <para>Represents a visitor or rewriter for expression trees.</para>
    /// </summary>
    internal abstract class ExpressionVisitor
    {
        /// <summary>
        /// <para>Dispatches the expression to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <param name="exp">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        public virtual Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        /// <summary>
        /// <para>Visits the children of the MemberBinding.</para>
        /// </summary>
        /// <param name="binding">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        /// <summary>
        /// <para>Visits the children of the ElementInit.</para>
        /// </summary>
        /// <param name="initializer">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }
        /// <summary>
        /// <para>Visits the children of the UnaryExpression.</para>
        /// </summary>
        /// <param name="u">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            if (operand != u.Operand)
            {
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return u;
        }
        /// <summary>
        /// <para>Visits the children of the BinaryExpression.</para>
        /// </summary>
        /// <param name="b">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            Expression conversion = this.Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                    return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }
            return b;
        }
        /// <summary>
        /// <para>Visits the children of the TypeBinaryExpression.</para>
        /// </summary>
        /// <param name="b">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expr = this.Visit(b.Expression);
            if (expr != b.Expression)
            {
                return Expression.TypeIs(expr, b.TypeOperand);
            }
            return b;
        }
        /// <summary>
        /// <para>Visits the ConstantExpression.</para>
        /// </summary>
        /// <param name="c">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }
        /// <summary>
        /// <para>Visits the ConditionalExpression.</para>
        /// </summary>
        /// <param name="c">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }
        /// <summary>
        /// <para>Visits the ParameterExpression.</para>
        /// </summary>
        /// <param name="p">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }
        /// <summary>
        /// <para>Visits the children of the MemberExpression.</para>
        /// </summary>
        /// <param name="m">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression exp = this.Visit(m.Expression);
            if (exp != m.Expression)
            {
                return Expression.MakeMemberAccess(exp, m.Member);
            }
            return m;
        }
        /// <summary>
        /// <para>Visits the children of the MethodCallExpression.</para>
        /// </summary>
        /// <param name="m">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            if (obj != m.Object || args != m.Arguments)
            {
                return Expression.Call(obj, m.Method, args);
            }
            return m;
        }
        /// <summary>
        /// <para>Dispatches the list of expressions to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <param name="original">
        /// <para>The expressions to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression list, if it or any subexpression was modified; otherwise, returns the original expression list.</para>
        /// </returns>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = this.Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return original;
        }
        /// <summary>
        /// <para>Visits the children of the MemberAssignment.</para>
        /// </summary>
        /// <param name="assignment">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = this.Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }
            return assignment;
        }
        /// <summary>
        /// <para>Visits the children of the MemberMemberBinding.</para>
        /// </summary>
        /// <param name="binding">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }
        /// <summary>
        /// <para>Visits the children of the MemberListBinding.</para>
        /// </summary>
        /// <param name="binding">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }
        /// <summary>
        /// <para>Dispatches the list of expressions to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <param name="original">
        /// <para>The expressions to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression list, if it or any subexpression was modified; otherwise, returns the original expression list.</para>
        /// </returns>
        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }
        /// <summary>
        /// <para>Dispatches the list of expressions to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <param name="original">
        /// <para>The expressions to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression list, if it or any subexpression was modified; otherwise, returns the original expression list.</para>
        /// </returns>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }
        /// <summary>
        /// <para>Visits the children of the LambdaExpression.</para>
        /// </summary>
        /// <param name="lambda">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }
        /// <summary>
        /// <para>Visits the children of the NewExpression.</para>
        /// </summary>
        /// <param name="nex">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                    return Expression.New(nex.Constructor, args, nex.Members);
                else
                    return Expression.New(nex.Constructor, args);
            }
            return nex;
        }
        /// <summary>
        /// <para>Visits the children of the MemberInitExpression.</para>
        /// </summary>
        /// <param name="init">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            if (n != init.NewExpression || bindings != init.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }
            return init;
        }
        /// <summary>
        /// <para>Visits the children of the ListInitExpression.</para>
        /// </summary>
        /// <param name="init">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            if (n != init.NewExpression || initializers != init.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }
            return init;
        }
        /// <summary>
        /// <para>Visits the children of the NewArrayExpression.</para>
        /// </summary>
        /// <param name="na">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            if (exprs != na.Expressions)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
                }
                else
                {
                    return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
                }
            }
            return na;
        }
        /// <summary>
        /// <para>Visits the children of the InvocationExpression.</para>
        /// </summary>
        /// <param name="iv">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression expr = this.Visit(iv.Expression);
            if (args != iv.Arguments || expr != iv.Expression)
            {
                return Expression.Invoke(expr, args);
            }
            return iv;
        }
    }
    #endregion
    
    #region ExpressionVisitor
    /// <summary>
    /// <para>This class visits every Parameter expression in an expression tree and calls a delegate to optionally replace the parameter. This is useful where two expression trees need to be merged (and they don't share the same ParameterExpressions).</para>
    /// </summary>
    /// <typeparam name="T">
    /// <para>Entity class.</para>
    /// </typeparam>
    internal class ExpressionVisitor<T> : ExpressionVisitor where T : Expression
    {
        Func<T, Expression> _visitor;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="visitor">
        /// <para>The expression to visit.</para>
        /// </param>
        public ExpressionVisitor(Func<T, Expression> visitor)
        {
            this._visitor = visitor;
        }
        /// <summary>
        /// <para>Dispatches the expression to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <param name="exp">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <param name="visitor">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        public static Expression Visit(Expression exp, Func<T, Expression> visitor)
        {
            return new ExpressionVisitor<T>(visitor).Visit(exp);
        }
        /// <summary>
        /// <para>Dispatches the expression to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <typeparam name="TDelegate">
        /// <para>Delegate class.</para>
        /// </typeparam>
        /// <param name="exp">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <param name="visitor">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        public static Expression<TDelegate> Visit<TDelegate>(Expression<TDelegate> exp, Func<T, Expression> visitor)
        {
            return (Expression<TDelegate>)new ExpressionVisitor<T>(visitor).Visit(exp);
        }
        /// <summary>
        /// <para>Dispatches the expression to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <param name="exp">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        public override Expression Visit(Expression exp)
        {
            if (exp is T && _visitor != null)
                exp = _visitor((T)exp);

            return base.Visit(exp);
        }
    }
    #endregion

    #region ExpressionExtensions

    /// <summary>
    /// <para>The extension for Visitors in Linq.</para>
    /// </summary>
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// <para>Dispatches the expression to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="exp">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <param name="visitor">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        public static Expression VisitMtn<T>(this Expression exp, Func<T, Expression> visitor) where T : Expression
        {
            return ExpressionVisitor<T>.Visit(exp, visitor);
        }
        
        /// <summary>
        /// <para>Dispatches the expression to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <typeparam name="TExp">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="exp">
        /// <para>The expression to visit.</para>
       /// </param>
        /// <param name="visitor">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        public static TExp VisitMtn<T, TExp>(this TExp exp, Func<T, Expression> visitor) where T : Expression 
            where TExp : Expression
        {
            return (TExp)ExpressionVisitor<T>.Visit(exp, visitor);
        }
       
        /// <summary>
        /// <para>Dispatches the expression to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <typeparam name="TDelegate">
        /// <para>Type of Delegate.</para>
        /// </typeparam>
        /// <param name="exp">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <param name="visitor">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        public static Expression<TDelegate> VisitMtn<T, TDelegate>(this Expression<TDelegate> exp, Func<T, Expression> visitor) where T : Expression
        {
            return ExpressionVisitor<T>.Visit<TDelegate>(exp, visitor);
        }
        
        /// <summary>
        /// <para>Dispatches the expression to one of the more specialized visit methods in this class.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <typeparam name="TSource">
        /// <para>Type of IQueryable.</para>
        /// </typeparam>
        /// <param name="source">
        /// <para>IQueryable object.</para>
        /// </param>
        /// <param name="visitor">
        /// <para>The expression to visit.</para>
        /// </param>
        /// <returns>
        /// <para>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</para>
        /// </returns>
        public static IQueryable<TSource> VisitMtn<T, TSource>(this IQueryable<TSource> source, Func<T, Expression> visitor) where T : Expression
        {
            return source.Provider.CreateQuery<TSource>(ExpressionVisitor<T>.Visit(source.Expression, visitor));
        }
    }
    #endregion
}
