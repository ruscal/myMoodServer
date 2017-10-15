using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Discover.Linq
{
    /// <summary>
    /// This expression visitor modifies expressions which access enumeration properties which are "backed" by an integer property following the naming convention "{property-name}EnumValue" 
    /// </summary>
    /// <remarks>
    /// Use of integer "backing properties" to store enum values is a suggested workaround to address the lack of native support for enum properties under Entity Framework v4 and earlier.
    /// </remarks>
    public class EnumRewriterExpressionVisitor : ExpressionVisitor
    {
        public static readonly EnumRewriterExpressionVisitor SharedInstance = new EnumRewriterExpressionVisitor();

        public string BackingMemberSuffix { get; protected set; }

        public EnumRewriterExpressionVisitor()
            : this("EnumValue")
        {
        }

        public EnumRewriterExpressionVisitor(string backingMemberSuffix)
        {
            BackingMemberSuffix = backingMemberSuffix;
        }

        public Expression Modify(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert && node.Operand.Type.IsEnum)
            {
                return Visit(node.Operand);
            }

            if (node.NodeType == ExpressionType.Quote)
            {
                var res = base.VisitUnary(node);
                return res;
            }

            return base.VisitUnary(node);
        }

        //make projections of enums work
        protected override Expression VisitNew(NewExpression node)
        {
            var arguments = node.Arguments
                .Select<Expression, Expression>((arg, i) =>
                {
                    var newarg = this.Visit(arg);
                    if (arg.Type.IsEnum && newarg.Type == typeof(Int32))
                        return Expression.Convert(newarg, arg.Type); //force an in mem convert from int to enum
                    else
                        return newarg;
                });

            return node.Update(arguments);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            if (node.Member.MemberType == MemberTypes.Property)
            {
                var prop = node.Member as PropertyInfo;

                if (prop.PropertyType.IsEnum)
                {
                    return Expression.Bind(prop, Expression.Convert(this.Visit(node.Expression), prop.PropertyType));
                }
            }

            return base.VisitMemberAssignment(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Type.IsEnum && node.Member.MemberType == MemberTypes.Property)
            {
                var newName = node.Member.Name + BackingMemberSuffix;
                var backingIntegerProperty = node.Expression.Type.GetMember(newName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
                    .FirstOrDefault();

                return backingIntegerProperty != null ?
                    Expression.MakeMemberAccess(node.Expression, backingIntegerProperty) as Expression :
                    Expression.Convert(Expression.MakeMemberAccess(node.Expression, node.Member), typeof(Int32)) as Expression;
            }

            if (node.Type.IsEnum && node.Member.MemberType == MemberTypes.Field && node.Expression is ConstantExpression) //access closure member
            {
                return Expression.Convert(node, typeof(Int32));
            }

            return base.VisitMember(node);
        }
    }
}
