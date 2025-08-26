using System;
using System.Collections.Generic;

namespace Resieve.Sorting
{
    public static class ResieveSortParser
    {
        public static List<ISortTerm> ParseSorts(string? sorts)
        {
            var result = new List<ISortTerm>();
            if (string.IsNullOrWhiteSpace(sorts))
            {
                return result;
            }

            var sortParts = sorts.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in sortParts)
            {
                var trimmed = part.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    continue;
                }

                var descending = trimmed.StartsWith("-");
                var name = descending ? trimmed.Substring(1) : trimmed;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    result.Add(new SortTerm(name, descending));
                }
            }

            return result;
        }
    }
}