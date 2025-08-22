using System;
using System.Collections.Generic;
using System.Linq;

namespace ReSieve.Filtering
{
    public enum FilterOperators
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
        Contains,
        DoesNotContains,
        StartsWith,
        EndsWith,
        DoesNotStartsWith,
        DoesNotEndsWith,
        CaseInsensitiveEquals,
        CaseInsensitiveNotEquals,
        CaseInsensitiveContains,
        CaseInsensitiveDoesNotContains,
        CaseInsensitiveStartsWith,
        CaseInsensitiveEndsWith,
        CaseInsensitiveDoesNotStartsWith,
        CaseInsensitiveDoesNotEndsWith,
        In,
        NotIn
    }

    public enum FilterLogicalOperator
    {
        And,
        Or
    }

    public static class FilterOperatorsExtensions
    {
        public readonly static Dictionary<string, FilterOperators> OperatorMap =
            new Dictionary<string, FilterOperators>(StringComparer.Ordinal)
            {
                {">=", FilterOperators.GreaterThanOrEqualTo},
                {"<=", FilterOperators.LessThanOrEqualTo},
                {"==", FilterOperators.Equals},
                {"!=", FilterOperators.NotEquals},
                {"=|", FilterOperators.In},
                {"!=|", FilterOperators.NotIn},
                {">", FilterOperators.GreaterThan},
                {"<", FilterOperators.LessThan},
                {"@=", FilterOperators.Contains},
                {"!@=", FilterOperators.DoesNotContains},
                {"_=", FilterOperators.StartsWith},
                {"_-=", FilterOperators.EndsWith},
                {"!_=", FilterOperators.DoesNotStartsWith},
                {"!_-=", FilterOperators.DoesNotEndsWith},
                {"==*", FilterOperators.CaseInsensitiveEquals},
                {"!=*", FilterOperators.CaseInsensitiveNotEquals},
                {"@=*", FilterOperators.CaseInsensitiveContains},
                {"!@=*", FilterOperators.CaseInsensitiveDoesNotContains},
                {"_=*", FilterOperators.CaseInsensitiveStartsWith},
                {"_-=*", FilterOperators.CaseInsensitiveEndsWith},
                {"!_=*", FilterOperators.CaseInsensitiveDoesNotStartsWith},
                {"!_-=*", FilterOperators.CaseInsensitiveDoesNotEndsWith}
            };

        public static IReadOnlyList<string> AllOperatorStrings => OperatorMap.Keys.ToList();

        public static FilterOperators GetOperatorParsed(string @operator)
        {
            if (OperatorMap.TryGetValue(@operator, out var op))
            {
                return op;
            }

            throw new ArgumentException($"Unknown filter operator '{@operator}'", nameof(@operator));
        }

        public static string ExtractOperator(this string input)
        {
            var op = AllOperatorStrings.OrderByDescending(s => s.Length)
                .FirstOrDefault(o => input.Contains(o, StringComparison.Ordinal));
            if (op == null)
            {
                throw new ArgumentException($"Input must contain a valid operator. Input: '{input}'", nameof(input));
            }

            return op;
        }
    }
}