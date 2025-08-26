using System;
using System.Linq;
using System.Reflection;

namespace Resieve.Mappings
{
    public static class ResieveMapperExtensions
    {
        public static ResieveMapper ApplyConfiguration<T>(this ResieveMapper mapper) where T : IResieveMapping, new()
        {
            var configuration = new T();
            configuration.Configure(mapper);
            return mapper;
        }
        
        public static ResieveMapper ApplyConfigurationsFromAssembly(this ResieveMapper mapper, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition))
            {
                // Only accept types that contain a parameterless constructor, are not abstract.
                var noArgConstructor = type.GetConstructor(Type.EmptyTypes);
                if (noArgConstructor is null)
                {
                    continue;
                }

                if (type.GetInterfaces().Any(t => t == typeof(IResieveMapping)))
                {
                    var configuration = (IResieveMapping)noArgConstructor.Invoke(new object?[] { });
                    configuration.Configure(mapper);
                }
            }

            return mapper;
        }

        public static ResieveMapper ApplyConfigurations(this ResieveMapper mapper, System.Collections.Generic.IEnumerable<IResieveMapping> mappings)
        {
            foreach (var mapping in mappings)
            {
                mapping.Configure(mapper);
            }
            return mapper;
        }
    }
    
    public interface IResieveMapping
    {
        void Configure(ResieveMapper mapper);
    }

}