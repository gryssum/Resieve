using System;

namespace ReSieve.Mappings
{
    public interface IReSievePropertyMetadata
    {
        bool CanSort { get; }
        bool CanFilter { get; }
        Type PropertyType { get; }
    }

    public class ReSievePropertyMetadata : IReSievePropertyMetadata
    {
        public bool CanSort { get; set; }
        public bool CanFilter { get; set; }
        public Type PropertyType { get; set; } = typeof(object);
    }
}