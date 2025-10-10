using System;

namespace Resieve.Sorting
{
    public interface ISortTerm
    {
        string Name { get; }
        bool Descending { get; }
    }

    public class SortTerm : ISortTerm, IEquatable<SortTerm>
    {
        internal SortTerm(string name, bool descending = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Sort property name cannot be null or whitespace.", nameof(name));
            }

            Name = name;
            Descending = descending;
        }

        public bool Equals(SortTerm? other)
        {
            return Name == other?.Name && Descending == other.Descending;
        }

        public string Name { get; }
        public bool Descending { get; }
    }
}