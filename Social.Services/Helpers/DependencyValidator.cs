using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Social.Services.Helpers
{
    public class DependencyValidator<TEntity> where TEntity : class
    {
        private static readonly Type IEnumerableType = typeof(IEnumerable);
        private static readonly Type StringType = typeof(string);

        public static IEnumerable<KeyValuePair<string, IQueryable<object>>> Dependencies(TEntity entity)
        {
            if (entity == null)
            {
                return Enumerable.Empty<KeyValuePair<string, IQueryable<object>>>();
            }

            var dependents = new List<KeyValuePair<string, IQueryable<object>>>();
            var properties = entity.GetType()
                .GetProperties()
                .Where(p => IEnumerableType.IsAssignableFrom(p.PropertyType) && !StringType.IsAssignableFrom(p.PropertyType));

            foreach (var property in properties)
            {
                var values = property.GetValue(entity) as IQueryable;
                var children = (from object value in values select value);

                dependents.Add(new KeyValuePair<string, IQueryable<object>>(property.Name, children));
            }

            return dependents;
        }
        public static IEnumerable<string> GetDependenciesNames(TEntity entity)
        {
            var IEnumerableType = typeof(System.Collections.IEnumerable);
            Type StringType = typeof(string);
            if (entity == null)
            {
                return Enumerable.Empty<string>();
            }

            var dependents = new List<string>();
            var sdf = entity.GetType()
                    .GetProperties().ToList();
            var properties = entity.GetType()
                .GetProperties()
                .Where(p => IEnumerableType.IsAssignableFrom(p.PropertyType) && !StringType.IsAssignableFrom(p.PropertyType));

            foreach (var property in properties)
            {
                var values = property.GetValue(entity);
                //var children = (from object value in values select value).ToList();
                if (values != null)
                {
                    dependents.Add(property.Name);
                }
            }

            return dependents;
        }
    }
}
