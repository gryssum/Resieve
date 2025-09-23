using System.Linq;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<IResieveMapper, ResieveMapper>();
            services.AddScoped<IExpressionTreeBuilder, ExpressionTreeBuilder>();
            services.AddScoped<IResievePaginationProcessor, ResievePaginationProcessor>();
            services.AddScoped<IResieveSortingProcessor, ResieveSortingProcessor>();
            services.AddScoped<IResieveFilterProcessor, ResieveFilterProcessor>();
            services.AddScoped<IResieveProcessor, ResieveProcessor>();
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