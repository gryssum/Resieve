using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Resieve.Filtering;
using Resieve.Filtering.ExpressionTrees;
using Resieve.Mappings;
using Resieve.Mappings.Interfaces;
using Resieve.Pagination;
using Resieve.Sorting;

namespace Resieve
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddResieve(this IServiceCollection services)
        {
            services.TryAddSingleton<IResieveMapper, ResieveMapper>();
            services.TryAddScoped<IExpressionTreeBuilder, ExpressionTreeBuilder>();
            services.TryAddScoped<IResievePaginationProcessor, ResievePaginationProcessor>();
            services.TryAddScoped<IResieveSortingProcessor, ResieveSortingProcessor>();
            services.TryAddScoped<IResieveFilterProcessor, ResieveFilterProcessor>();
            services.TryAddScoped<IResieveProcessor, ResieveProcessor>();
            return services;
        }
        
        public static IServiceCollection AddResieveMappingsFromAssembly(this IServiceCollection services, System.Reflection.Assembly assembly)
        {
            var mappingType = typeof(IResieveMapping);
            var types = assembly.GetTypes()
                .Where(t => t is {IsAbstract: false, IsGenericTypeDefinition: false} && mappingType.IsAssignableFrom(t));
            
            foreach (var type in types)
            {
                services.AddTransient(mappingType, type);
            }
            
            return services;
        }
    }
}