using System;
using System.Collections.Generic;
using System.Linq;

namespace ReSieve.Models
{
    public interface ISortTerm
    {
        string Name { get; }
        bool Descending { get; }
    }

    public class SortTerm : ISortTerm, IEquatable<SortTerm>
    {
        private SortTerm(string name, bool descending = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Sort property name cannot be null or whitespace.", nameof(name));
            }
            Name = name;
            Descending = descending;
        }

        public bool Equals(SortTerm other)
        {
            return Name == other.Name && Descending == other.Descending;
        }

        public string Name { get; }
        public bool Descending { get; }

        public static List<SortTerm> ParseList(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new List<SortTerm>();
            }

            return input.Split(',')
                .Select(Parse)
                .ToList();
        }
        
        private static SortTerm Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Sort term cannot be null or whitespace.", nameof(input));
            }
            var trimmed = input.Trim();
            var descending = trimmed.StartsWith("-");
            var name = descending ? trimmed.Substring(1) : trimmed;
            return new SortTerm(name, descending);
        }
    }
}