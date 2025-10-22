using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace Resieve.Mappings.Interfaces
{
    public interface IResieveMapper
    {
        FrozenDictionary<Type, Dictionary<string, ResievePropertyMap>> PropertyMappings { get; }
    }
}