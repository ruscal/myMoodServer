using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Discover.Linq
{
    /// <summary>
    /// This expression visitor modifies expressions which access a read-only IEnumerable&lt;&gt; property which is "backed" by another property (typically holding a mutable collection) which follows the naming convention "{property-name}Collection"
    /// </summary>
    /// <remarks>
    /// This expression visitor is intended to support the pattern of exposing querying capabilities of a protected/private mutable set of objects (e.g. ICollection&lt;&gt;) via an immutable IEnumerable&lt;&gt; property, 
    /// while still allowing the Entity Framework query provider to generate and optimise its underlying native queries correctly.
    /// </remarks>
    public class ReadOnlyEnumerableRewriterExpressionVisitor : ExpressionVisitor
    {
        public static readonly ReadOnlyEnumerableRewriterExpressionVisitor SharedInstance = new ReadOnlyEnumerableRewriterExpressionVisitor();

        public string BackingMemberSuffix { get; protected set; }

        public ReadOnlyEnumerableRewriterExpressionVisitor()
            : this("Collection")
        {
        }

        public ReadOnlyEnumerableRewriterExpressionVisitor(string backingMemberSuffix)
        {
            BackingMemberSuffix = backingMemberSuffix;
        }

        public Expression Modify(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.MemberType == MemberTypes.Property)
            {
                var propertyInfo = (PropertyInfo)node.Member;

                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                {
                    var newName = node.Member.Name + BackingMemberSuffix;
                    var backingCollectionProperty = node.Expression.Type.GetMember(newName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
                        .FirstOrDefault();

                    if (backingCollectionProperty != null)
                    {
                        return Expression.MakeMemberAccess(node.Expression, backingCollectionProperty);
                    }
                }
            }

            return base.VisitMember(node);
        }
    }
}
