using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using Discover.Reflection;

namespace Discover.Data.EntityFramework4
{
    [Flags]
    public enum DiscoverMappingConventions
    {
        IntegerBackedEnumProperties = 1,
        NonPublicCollectionProperties = 2,
        DateTimePropertiesAsDateTime2DataType = 4,
        CommonlyIndexedStringPropertiesMaxLengthOf256 = 8,

        All = IntegerBackedEnumProperties | NonPublicCollectionProperties | DateTimePropertiesAsDateTime2DataType | CommonlyIndexedStringPropertiesMaxLengthOf256
    }

    public static class EntityMappingExtensions
    {
        public static void ApplyDiscoverMappingConventionsToTypesDerivedFrom<TEntity>(this DbModelBuilder modelBuilder)
        {
            ApplyDiscoverMappingConventionsToTypesDerivedFrom<TEntity>(modelBuilder, DiscoverMappingConventions.All);
        }

        public static void ApplyDiscoverMappingConventionsToTypesDerivedFrom<TEntity>(this DbModelBuilder modelBuilder, DiscoverMappingConventions conventions)
        {
            // automatically map by convention:
            // - non-public collections 
            // - non-public integer properties which are intended to act as a backing store for public enum properties
            var entityTypes = from t in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes())
                              where t.IsClass && !t.IsAbstract && typeof(TEntity).IsAssignableFrom(t)
                              select t;

            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");
            var mapCollectionsMethod = typeof(EntityMappingExtensions).GetMethod("MapNonPublicCollections");
            var mapEnumsMethod = typeof(EntityMappingExtensions).GetMethod("MapIntegerBackedEnumProperties");
            var mapDateTimeColumns = typeof(EntityMappingExtensions).GetMethod("MapDateTimePropertiesAsDateTime2");
            var mapStringPropsToFixedLength = typeof(EntityMappingExtensions).GetMethod("MapStringPropertiesWithMaxLength");

            foreach (var entityType in entityTypes)
            {
                var entityTypeConfiguration = entityMethod.MakeGenericMethod(entityType).Invoke(modelBuilder, null);

                if (conventions.HasFlag(DiscoverMappingConventions.DateTimePropertiesAsDateTime2DataType))
                {
                    mapDateTimeColumns.MakeGenericMethod(entityType).Invoke(null, new object[] { entityTypeConfiguration });
                }

                if (conventions.HasFlag(DiscoverMappingConventions.NonPublicCollectionProperties))
                {
                    mapCollectionsMethod.MakeGenericMethod(entityType).Invoke(null, new object[] { entityTypeConfiguration });
                }

                if (conventions.HasFlag(DiscoverMappingConventions.IntegerBackedEnumProperties))
                {
                    mapEnumsMethod.MakeGenericMethod(entityType).Invoke(null, new object[] { entityTypeConfiguration });
                }

                if (conventions.HasFlag(DiscoverMappingConventions.CommonlyIndexedStringPropertiesMaxLengthOf256))
                {
                    mapStringPropsToFixedLength.MakeGenericMethod(entityType).Invoke(null, new object[] { entityTypeConfiguration, new string[] { "Code", "Name", "UserName", "EmailAddress", "Email" }, 256 });
                }
            }
        }

        public static void DefineTableNameForAllTypesDerivedFrom<TEntity>(this DbModelBuilder modelBuilder, Func<Type, string> getTableName)
        {
            var entityTypes = from t in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes())
                              where t.IsClass && !t.IsAbstract && typeof(TEntity).IsAssignableFrom(t)
                              select t;

            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");

            foreach (var entityType in entityTypes)
            {
                var tableName = getTableName(entityType);

                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    var entityTypeConfiguration = entityMethod.MakeGenericMethod(entityType).Invoke(modelBuilder, null);

                    entityTypeConfiguration.GetType().GetMethod("ToTable", new Type[] { typeof(string) }).Invoke(entityTypeConfiguration, new object[] { tableName });
                }
            }
        }

        public static ManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType> HasMany<TEntityType, TTargetEntityType>(this EntityTypeConfiguration<TEntityType> mapper, string propertyName)
            where TEntityType : class
            where TTargetEntityType : class
        {
            // NB: this cast will blow up at runtime (i.e. during entity mapper setup) if the nominated property is not of type ICollection<TTargetEntityType>
            return mapper.HasMany((Expression<Func<TEntityType, ICollection<TTargetEntityType>>>)typeof(TEntityType).GetPropertyAccessExpression(propertyName));
        }

        public static ManyToManyNavigationPropertyConfiguration WithMany<TEntityType, TTargetEntityType>(this ManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType> many, string propertyName)
            where TEntityType : class
            where TTargetEntityType : class
        {
            // NB: this cast will blow up at runtime (i.e. during entity mapper setup) if the nominated property is not of type ICollection<TTargetEntityType>
            return many.WithMany((Expression<Func<TTargetEntityType, ICollection<TEntityType>>>)typeof(TTargetEntityType).GetPropertyAccessExpression(propertyName));
        }

        public static PrimitivePropertyConfiguration Property<TEntityType, T>(this EntityTypeConfiguration<TEntityType> mapper, string propertyName)
            where TEntityType : class
            where T : struct
        {
            Type propertyType;
            var expr = typeof(TEntityType).GetPropertyAccessExpression(propertyName, out propertyType);

            return propertyType.IsNullable() ?
                mapper.Property((Expression<Func<TEntityType, Nullable<T>>>)expr) :
                mapper.Property((Expression<Func<TEntityType, T>>)expr);
        }

        public static PrimitivePropertyConfiguration Property<TEntityType>(this EntityTypeConfiguration<TEntityType> mapper, string propertyName)
            where TEntityType : class
        {
            Type propertyType;
            var expr = typeof(TEntityType).GetPropertyAccessExpression(propertyName, out propertyType);

            if (propertyType == typeof(string)) return mapper.Property((Expression<Func<TEntityType, string>>)expr);
            if (propertyType == typeof(byte[])) return mapper.Property((Expression<Func<TEntityType, byte[]>>)expr);

            throw new InvalidOperationException("Type of '" + propertyName + "' must be explicitly declared/provided");
        }

        public static EntityTypeConfiguration<TEntityType> MapCollectionBackedEnumerableProperties<TEntityType>(this EntityTypeConfiguration<TEntityType> mapper)
            where TEntityType : class
        {
            var props = from p in typeof(TEntityType).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                        where p.CanRead && p.PropertyType.IsGenericType && typeof(IEnumerable<>).Equals(p.PropertyType.GetGenericTypeDefinition()) &&
                            !p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any()
                        select p;

            var hasManyMethod = typeof(EntityTypeConfiguration<TEntityType>).GetMethod("HasMany");

            foreach (var prop in props)
            {
                var backingProp = typeof(TEntityType).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .Where(p => p.PropertyType == typeof(int) && p.Name == prop.Name + "Collection")
                    .FirstOrDefault();

                if (backingProp != null)
                {
                    var propertyLambda = backingProp.GetPropertyAccessExpression();

                    hasManyMethod.MakeGenericMethod(prop.PropertyType.GetGenericArguments().First()).Invoke(mapper, new object[] { propertyLambda });
                }
            }

            return mapper;
        }

        public static EntityTypeConfiguration<TEntityType> MapNonPublicCollections<TEntityType>(this EntityTypeConfiguration<TEntityType> mapper)
            where TEntityType : class
        {
            var props = from p in typeof(TEntityType).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                        where p.CanRead && p.CanWrite &&
                            p.PropertyType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition()) &&
                            !p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any()
                        select p;

            var hasManyMethod = typeof(EntityTypeConfiguration<TEntityType>).GetMethod("HasMany");

            foreach (var prop in props)
            {
                var propertyLambda = prop.GetPropertyAccessExpression();

                hasManyMethod.MakeGenericMethod(prop.PropertyType.GetGenericArguments().First()).Invoke(mapper, new object[] { propertyLambda });
            }

            return mapper;
        }

        public static EntityTypeConfiguration<TEntityType> MapIntegerBackedEnumProperties<TEntityType>(this EntityTypeConfiguration<TEntityType> mapper)
            where TEntityType : class
        {
            var props = from p in typeof(TEntityType).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                        where p.CanRead && p.CanWrite && p.PropertyType.IsEnum && !p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any()
                        select p;

            foreach (var prop in props)
            {
                var backingProp = typeof(TEntityType).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .Where(p => p.PropertyType == typeof(int) && p.Name == prop.Name + "EnumValue")
                    .FirstOrDefault();

                if (backingProp != null)
                {
                    mapper.Property((Expression<Func<TEntityType, int>>)backingProp.GetPropertyAccessExpression()).HasColumnName(prop.Name);
                }
            }

            return mapper;
        }

        public static EntityTypeConfiguration<TEntityType> MapDateTimePropertiesAsDateTime2<TEntityType>(this EntityTypeConfiguration<TEntityType> mapper)
            where TEntityType : class
        {
            var props = from p in typeof(TEntityType).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                        where p.CanRead && p.CanWrite && (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)) && !p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any()
                        select p;

            foreach (var prop in props)
            {
                var propConfig = prop.PropertyType.IsNullable() ?
                    mapper.Property((Expression<Func<TEntityType, DateTime?>>)prop.GetPropertyAccessExpression()) :
                    mapper.Property((Expression<Func<TEntityType, DateTime>>)prop.GetPropertyAccessExpression());

                propConfig.HasColumnType("datetime2");
            }

            return mapper;
        }

        public static EntityTypeConfiguration<TEntityType> MapStringPropertiesWithMaxLength<TEntityType>(this EntityTypeConfiguration<TEntityType> mapper, IEnumerable<string> propertyNames, int? maxLength)
            where TEntityType : class
        {
            var props = from p in typeof(TEntityType).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                        where p.CanRead && p.CanWrite && p.PropertyType == typeof(string) && propertyNames.Any(n => p.Name.Equals(n, StringComparison.InvariantCultureIgnoreCase)) && !p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any()
                        select p;

            foreach (var prop in props)
            {
                mapper.Property((Expression<Func<TEntityType, string>>)prop.GetPropertyAccessExpression()).HasMaxLength(maxLength);
            }

            return mapper;
        }
    }
}
