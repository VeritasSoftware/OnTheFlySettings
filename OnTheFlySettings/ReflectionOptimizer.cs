using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace OnTheFlySettings
{
    public static class ReflectionOptimizer
    {
        // Cache PropertyInfo arrays per type
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        // Cache compiled getters per property
        private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> GetterCache =
            new ConcurrentDictionary<PropertyInfo, Func<object, object>>();

        // Cache compiled setters per property
        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> SetterCache =
            new ConcurrentDictionary<PropertyInfo, Action<object, object>>();

        /// <summary>
        /// Get cached PropertyInfo array for a type.
        /// </summary>
        public static PropertyInfo[] GetPropertiesCached(Type type)
        {
            return PropertyCache.GetOrAdd(type, t =>
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }

        /// <summary>
        /// Get a fast compiled getter delegate for a property.
        /// </summary>
        public static Func<object, object> GetFastGetter(PropertyInfo prop)
        {
            return GetterCache.GetOrAdd(prop, p =>
            {
                var instance = Expression.Parameter(typeof(object), "instance");
                var castInstance = Expression.Convert(instance, p.DeclaringType!);
                var propertyAccess = Expression.Property(castInstance, p);
                var castResult = Expression.Convert(propertyAccess, typeof(object));
                return Expression.Lambda<Func<object, object>>(castResult, instance).Compile();
            });
        }

        /// <summary>
        /// Get a fast compiled setter delegate for a property.
        /// </summary>
        public static Action<object, object> GetFastSetter(PropertyInfo prop)
        {
            return SetterCache.GetOrAdd(prop, p =>
            {
                var instance = Expression.Parameter(typeof(object), "instance");
                var value = Expression.Parameter(typeof(object), "value");
                var castInstance = Expression.Convert(instance, p.DeclaringType!);
                var castValue = Expression.Convert(value, p.PropertyType);
                var propertyAccess = Expression.Property(castInstance, p);
                var assign = Expression.Assign(propertyAccess, castValue);
                return Expression.Lambda<Action<object, object>>(assign, instance, value).Compile();
            });
        }
    }
}
