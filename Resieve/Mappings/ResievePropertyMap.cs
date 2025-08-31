using System;

namespace Resieve.Mappings
{
    public class ResievePropertyMap
    {
        public bool CanFilter { get; set; }
        public bool CanSort { get; set; }
        public Type? CustomFilter { get; set; }
        public Type? CustomSort { get; set; } 
    }
}