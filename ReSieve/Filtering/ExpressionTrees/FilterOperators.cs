using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ReSieve.Filtering.ExpressionTrees
{
    public static class FilterOperators
    {
        private readonly static Dictionary<string, string> MapWithDescription = new Dictionary<string, string>
        {
            {">=", "Greater than or equal"},
            {"<=", "Less than or equal"},
            {"==", "Equals"},
            {"!=", "Not equals"},
            {">", "Greater than"},
            {"<", "Less than"},
            {"@=", "Contains"},
            {"!@=", "Does not contain"},
            {"_=", "Starts with"},
            {"_-=", "Ends with"},
            {"!_=", "Does not start with"},
            {"!_-=", "Does not end with"},
            {"==*", "Equals (case-insensitive)"},
            {"!=*", "Not equals (case-insensitive)"},
            {"@=*", "Contains (case-insensitive)"},
            {"!@=*", "Does not contain (case-insensitive)"},
            {"_=*", "Starts with (case-insensitive)"},
            {"_-=*", "Ends with (case-insensitive)"},
            {"!_=*", "Does not start with (case-insensitive)"},
            {"!_-=*", "Does not end with (case-insensitive)"}
        };

        public static IReadOnlyList<string> Map => MapWithDescription.Keys.ToList();

        public static Expression GetExpression(string op, Expression prop, Expression constant)
        {
            switch (op)
            {
                case "==": return Expression.Equal(prop, constant);
                case "!=": return Expression.NotEqual(prop, constant);
                case ">": return Expression.GreaterThan(prop, constant);
                case ">=": return Expression.GreaterThanOrEqual(prop, constant);
                case "<": return Expression.LessThan(prop, constant);
                case "<=": return Expression.LessThanOrEqual(prop, constant);
                case "@=": return Expression.Call(prop, typeof(string).GetMethod("Contains", new[] {typeof(string)})!, constant);
                case "!@=": return Expression.Not(Expression.Call(prop, typeof(string).GetMethod("Contains", new[] {typeof(string)})!, constant));
                case "_=": return Expression.Call(prop, typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!, constant);
                case "_-=": return Expression.Call(prop, typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!, constant);
                case "!_=": return Expression.Not(Expression.Call(prop, typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!, constant));
                case "!_-=": return Expression.Not(Expression.Call(prop, typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!, constant));
                case "==*":
                    return Expression.Equal(
                        Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                        Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                    );
                case "!=*":
                    return Expression.NotEqual(
                        Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                        Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                    );
                case "@=*":
                    return Expression.Call(
                        Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                        typeof(string).GetMethod("Contains", new[] {typeof(string)})!,
                        Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                    );
                case "!@=*":
                    return Expression.Not(
                        Expression.Call(
                            Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                            typeof(string).GetMethod("Contains", new[] {typeof(string)})!,
                            Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                        )
                    );
                case "_=*":
                    return Expression.Call(
                        Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                        typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!,
                        Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                    );
                case "_-=*":
                    return Expression.Call(
                        Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                        typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!,
                        Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                    );
                case "!_=*":
                    return Expression.Not(
                        Expression.Call(
                            Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                            typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!,
                            Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                        )
                    );
                case "!_-=*":
                    return Expression.Not(
                        Expression.Call(
                            Expression.Call(prop, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!),
                            typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!,
                            Expression.Call(constant, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!)
                        )
                    );
                default:
                    throw new NotSupportedException($"Operator '{op}' is not supported.");
            }
        }
    }
}