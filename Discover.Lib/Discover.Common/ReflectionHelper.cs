using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Discover.Reflection
{
    public static class ReflectionHelper
    {
        public static bool IsNullable(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type UnwrapDynamicProxyType(this Type t)
        {
            while (t.Assembly.IsDynamic) t = t.BaseType;
            return t;
        }

        public static string DisplayName(this Type t)
        {
            return t.GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().Select(dna => dna.DisplayName).FirstOrDefault() ?? t.Name;
        }

        public static string Description(this Type t)
        {
            return t.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().Select(da => da.Description).FirstOrDefault();
        }

        public static string Category(this Type t)
        {
            return t.GetCustomAttributes(typeof(CategoryAttribute), false).Cast<CategoryAttribute>().Select(da => da.Category).FirstOrDefault();
        }
        
        public static IEnumerable<KeyValuePair<string, string>> PropertyDescriptions(this Type t)
        {
            return t.GetProperties().Select(p => new KeyValuePair<string, string>(p.Name, p.Description()));
        }

        public static string DisplayName(this MemberInfo m)
        {
            return m.GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().Select(dna => dna.DisplayName).FirstOrDefault() ??
                m.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Select(da => da.Name).FirstOrDefault();
        }

        public static string Description(this MemberInfo m)
        {
            return m.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().Select(da => da.Description).FirstOrDefault() ??
                m.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Select(da => da.Description).FirstOrDefault();
        }

        public static LambdaExpression GetPropertyAccessExpression(this Type entityType, string propertyName)
        {
            Type dummy;
            return GetPropertyAccessExpression(entityType, propertyName, out dummy);
        }

        public static LambdaExpression GetPropertyAccessExpression(this Type entityType, string propertyName, out Type propertyType)
        {
            var entityProp = entityType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            propertyType = entityProp.PropertyType;

            var argExpr = Expression.Parameter(entityType, "x");
            var expr = Expression.Property(argExpr, entityProp);

            return Expression.Lambda(expr, argExpr);
        }

        public static LambdaExpression GetPropertyAccessExpression(this PropertyInfo propertyInfo)
        {
            var argExpr = Expression.Parameter(propertyInfo.ReflectedType, "x");
            var expr = Expression.Property(argExpr, propertyInfo);

            return Expression.Lambda(expr, argExpr);
        }

        public static IEnumerable<Type> DerivedTypes(this Type t)
        {
            return from t0 in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes())
                   where t.IsAssignableFrom(t0)
                   select t0;
        }

        public static IEnumerable<Type> ConcreteDerivedTypes(this Type t)
        {
            return from t0 in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes())
                   where !t.IsAbstract && t.IsAssignableFrom(t0)
                   select t0;
        }
    }
}
