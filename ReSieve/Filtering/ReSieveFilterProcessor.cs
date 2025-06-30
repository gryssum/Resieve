using System;
using System.Linq;
using System.Linq.Expressions;
using ReSieve.Filtering.Lexers;
using ReSieve.Mappings;

namespace ReSieve.Filtering
{
    public interface IFilterProcessor
    {
        IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source);
    }

    public class ReSieveFilterProcessor : IFilterProcessor
    {
        private readonly ReSieveMapper _mapper;

        public ReSieveFilterProcessor(ReSieveMapper mapper)
        {
            _mapper = mapper;
        }

        public IQueryable<TEntity> Apply<TEntity>(ReSieveModel reSieveModel, IQueryable<TEntity> source)
        {
            if (string.IsNullOrWhiteSpace(reSieveModel.Filters))
            {
                return source; // No filters to apply
            }

            // 1. Tokenize the filter string
            var lexer = new FilterLexer();
            var tokens = lexer.Tokenize(reSieveModel.Filters).ToList();

            // 2. Build node based tree from tokens
            
            // 3. Build the expression tree
            var propertyToken = tokens.Single(x => x.Type == TokenType.Identifier).Value;
            var foundMapping = _mapper.GetFilterPropertyMetadata<TEntity>(propertyToken);

            var operatorToken = tokens.Single(x => x.Type == TokenType.Operator).Value;
            var valueToken = tokens.Single(x => x.Type == TokenType.Value).Value;

            var param = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(param, propertyToken);
            var constant = CreateTypedConstant(valueToken, foundMapping.PropertyType);
            var body = GetOperatorExpression(operatorToken, property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, param);

            return source.Where(lambda);
        }

        private Expression GetOperatorExpression(string operatorToken, Expression left, Expression right)
        {
            var @operator = FilterOperatorsExtensions.GetOperatorParsed(operatorToken);
            var leftType = left.Type;
            var isString = leftType == typeof(string);
            var isNumeric = leftType == typeof(int) || leftType == typeof(long) || leftType == typeof(float) ||
                            leftType == typeof(double) || leftType == typeof(decimal);

            if (isString)
            {
                return GetStringOperatorExpression(@operator, left, right);
            }

            if (isNumeric)
            {
                return GetNumericOperatorExpression(@operator, left, right, leftType);
            }

            return GetDefaultOperatorExpression(@operator, left, right, leftType);
        }

        private Expression GetStringOperatorExpression(FilterOperators @operator, Expression left, Expression right)
        {
            // Ensure both expressions are of type string
            var leftString = Expression.Convert(left, typeof(string));
            var rightString = Expression.Convert(right, typeof(string));
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
            switch (@operator)
            {
                case FilterOperators.Equals:
                    return Expression.Equal(leftString, rightString);
                case FilterOperators.NotEquals:
                    return Expression.NotEqual(leftString, rightString);
                case FilterOperators.Contains:
                    return Expression.Call(leftString, typeof(string).GetMethod("Contains", new[] {typeof(string)})!, rightString);
                case FilterOperators.DoesNotContains:
                    return Expression.Not(Expression.Call(leftString, typeof(string).GetMethod("Contains", new[] {typeof(string)})!,
                        rightString));
                case FilterOperators.StartsWith:
                    return Expression.Call(leftString, typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!, rightString);
                case FilterOperators.EndsWith:
                    return Expression.Call(leftString, typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!, rightString);
                case FilterOperators.CaseInsensitiveEquals:
                    return Expression.Equal(Expression.Call(leftString, toLowerMethod),
                        Expression.Call(rightString, toLowerMethod)
                    );
                case FilterOperators.CaseInsensitiveContains:
                    return Expression.Call(
                        Expression.Call(leftString, toLowerMethod),
                        typeof(string).GetMethod("Contains", new[] {typeof(string)})!,
                        Expression.Call(rightString, toLowerMethod)
                    );
                case FilterOperators.CaseInsensitiveStartsWith:
                    return Expression.Call(
                        Expression.Call(leftString, toLowerMethod),
                        typeof(string).GetMethod("StartsWith", new[] {typeof(string)})!,
                        Expression.Call(rightString, toLowerMethod)
                    );
                case FilterOperators.CaseInsensitiveEndsWith:
                    return Expression.Call(
                        Expression.Call(leftString, toLowerMethod),
                        typeof(string).GetMethod("EndsWith", new[] {typeof(string)})!,
                        Expression.Call(rightString, toLowerMethod)
                    );
                default:
                    throw new NotSupportedException($"Operator '{@operator}' is not supported for string type.");
            }
        }

        private Expression GetNumericOperatorExpression(FilterOperators @operator, Expression left, Expression right, Type leftType)
        {
            switch (@operator)
            {
                case FilterOperators.Equals:
                    return Expression.Equal(left, Expression.Convert(right, leftType));
                case FilterOperators.NotEquals:
                    return Expression.NotEqual(left, Expression.Convert(right, leftType));
                case FilterOperators.GreaterThan:
                    return Expression.GreaterThan(left, Expression.Convert(right, leftType));
                case FilterOperators.LessThan:
                    return Expression.LessThan(left, Expression.Convert(right, leftType));
                case FilterOperators.GreaterThanOrEqualTo:
                    return Expression.GreaterThanOrEqual(left, Expression.Convert(right, leftType));
                case FilterOperators.LessThanOrEqualTo:
                    return Expression.LessThanOrEqual(left, Expression.Convert(right, leftType));
                default:
                    throw new NotSupportedException($"Operator '{@operator}' is not supported for numeric type.");
            }
        }

        private Expression GetDefaultOperatorExpression(FilterOperators @operator, Expression left, Expression right, Type leftType)
        {
            switch (@operator)
            {
                case FilterOperators.Equals:
                    return Expression.Equal(left, Expression.Convert(right, leftType));
                case FilterOperators.NotEquals:
                    return Expression.NotEqual(left, Expression.Convert(right, leftType));
                default:
                    throw new NotSupportedException($"Operator '{@operator}' is not supported for type '{leftType.Name}'.");
            }
        }

        private Expression CreateTypedConstant(string valueToken, Type targetType)
        {
            if (string.IsNullOrEmpty(valueToken))
            {
                // If the value is empty and the target type is nullable, return null constant
                if (Nullable.GetUnderlyingType(targetType) != null)
                    return Expression.Constant(null, targetType);
            }

            var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            object value;
            if (nonNullableType.IsEnum)
            {
                value = Enum.Parse(nonNullableType, valueToken, ignoreCase: true);
            }
            else
            {
                value = Convert.ChangeType(valueToken, nonNullableType);
            }
            return Expression.Constant(value, targetType);
        }
    }
}