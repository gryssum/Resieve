using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Resieve.Filtering;
using Resieve.Mappings;
using Resieve.Pagination;
using Resieve.Sorting;

namespace Resieve
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddResieve(this IServiceCollection services)
        {
            services.AddScoped<IResieveProcessor, ResieveProcessor>();
            services.AddScoped<IResieveFilterProcessor, ResieveFilterProcessor>();
            services.AddScoped<IResieveSortingProcessor, ResieveSortingProcessor>();
            services.AddScoped<IResievePaginationProcessor, ResievePaginationProcessor>();
            services.AddSingleton<IResieveMapper, ResieveMapper>();
            
            return services;
        }

        public static IServiceCollection AddResieveMappingsFromAssembly(this IServiceCollection services, System.Reflection.Assembly assembly)
        {
            var mappingType = typeof(Mappings.IResieveMapping);
            var types = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && mappingType.IsAssignableFrom(t));
            
            foreach (var type in types)
            {
                services.AddTransient(mappingType, type);
            }
            
            return services;
        }
    }
}