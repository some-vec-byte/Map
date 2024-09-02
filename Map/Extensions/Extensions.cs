using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Map
{
    public static class Extensions
    {
        public static IServiceCollection AddMapping(this IServiceCollection services)
        {
            services.AddTransient<IMapMethodProvider, MapMethodProvider>();
            services.AddTransient<IMapper, DynamicMapper>();

            return services;
        }


        public delegate TValue ValueProvider<TValue>();
        public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, ValueProvider<TValue> provider)
        {
            TValue value = default;

            if (dict.TryGetValue(key, out value) == false)
            {
                value = provider();
                dict[key] = value;
            }

            return value;
        }
    }
}