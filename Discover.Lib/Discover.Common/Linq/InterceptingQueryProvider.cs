using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Discover.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class InterceptingQueryProvider : IQueryProvider
    {
        private IQueryProvider source;
        private List<ExpressionVisitor> expressionVisitors = new List<ExpressionVisitor>();

        public InterceptingQueryProvider(IQueryProvider source)
        {
            this.source = source;
        }

        public InterceptingQueryProvider(IQueryProvider source, IEnumerable<ExpressionVisitor> expressionVisitors)
            : this(source)
        {
            this.expressionVisitors.AddRange(expressionVisitors);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            foreach (var visitor in this.expressionVisitors)
            {
                expression = visitor.Visit(expression);
            }
            return new InterceptingQuery<TElement>(source.CreateQuery<TElement>(expression), this.expressionVisitors);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            foreach (var visitor in this.expressionVisitors)
            {
                expression = visitor.Visit(expression);
            }
            return source.CreateQuery(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            foreach (var visitor in this.expressionVisitors)
            {
                expression = visitor.Visit(expression);
            }
            return source.Execute<TResult>(expression);
        }

        public object Execute(Expression expression)
        {
            foreach (var visitor in this.expressionVisitors)
            {
                expression = visitor.Visit(expression);
            }
            return source.Execute(expression);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InterceptingQuery<T> : IOrderedQueryable<T>
    {
        private IQueryable<T> source;
        private InterceptingQueryProvider provider;

        public InterceptingQuery(IQueryable<T> source, params ExpressionVisitor[] expressionVisitors)
        {
            this.source = source;
            this.provider = new InterceptingQueryProvider(this.source.Provider, expressionVisitors);
        }

        public InterceptingQuery(IQueryable<T> source, IEnumerable<ExpressionVisitor> expressionVisitors)
        {
            this.source = source;
            this.provider = new InterceptingQueryProvider(this.source.Provider, expressionVisitors);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return source.GetEnumerator();
        }

        public Type ElementType
        {
            get { return source.ElementType; }
        }

        public Expression Expression
        {
            get { return source.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return provider; }
        }

        public override string ToString()
        {
            return source.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InterceptingQueryUnwrapperExpressionVisitor : ExpressionVisitor
    {
        private readonly MethodInfo unwrapCallsTo;

        public InterceptingQueryUnwrapperExpressionVisitor(MethodInfo unwrapCallsTo)
        {
            this.unwrapCallsTo = unwrapCallsTo;
        }

        public Expression Modify(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == this.unwrapCallsTo.DeclaringType &&
                node.Method.Name == this.unwrapCallsTo.Name)
            {
                var underlyingQuery = Expression.Lambda(node).Compile().DynamicInvoke();

                var expressionProperty = underlyingQuery.GetType().GetProperty("Expression");

                if (expressionProperty != null)
                {
                    return expressionProperty.GetValue(underlyingQuery, null) as Expression;
                }
            }

            return base.VisitMethodCall(node);
        }
    }
}
