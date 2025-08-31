using System;
using System.Collections.Generic;

namespace Resieve.Mappings.Interfaces
{
    public interface IResieveMapper
    {
        IReadOnlyDictionary<Type, Dictionary<string, ResievePropertyMap>> PropertyMappings { get; }
    }
}