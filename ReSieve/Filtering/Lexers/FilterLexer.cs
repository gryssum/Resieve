using System;
using System.Collections.Generic;
using System.Linq;

namespace ReSieve.Filtering.Lexers
{
    public class FilterLexer
    {
        private readonly static string?[] OperatorPatterns = FilterOperatorsExtensions.OperatorMap.Keys
            .OrderByDescending(x => x.Length)
            .ToArray();

        public IEnumerable<Token> Tokenize(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                yield break;
            }

            var i = 0;
            var afterOperator = false;
            var valueGroupDepth = 0;
            while (i < input.Length)
            {
                var c = input[i];
                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                // 1. Parentheses (only for value grouping)
                if (c == '(')
                {
                    yield return new Token(TokenType.OpenParen, "(", i);

                    // If previous char was an operator, we're entering a value group
                    if (i > 0 && IsPartOfOperator(input, i - 1))
                    {
                        valueGroupDepth++;
                    }

                    i++;
                    continue;
                }

                if (c == ')')
                {
                    yield return new Token(TokenType.CloseParen, ")", i);

                    if (valueGroupDepth > 0)
                    {
                        valueGroupDepth--;
                    }

                    i++;
                    continue;
                }

                // 2. Operators (longest match)
                if (TryMatchOperator(input, i, out var op, out var opLength))
                {
                    yield return new Token(TokenType.Operator, op, i);

                    afterOperator = true;
                    i += opLength;
                    continue;
                }

                // 3. Logical connectors (comma)
                if (c == ',')
                {
                    yield return new Token(TokenType.LogicalAnd, ",", i);

                    i++;
                    continue;
                }

                // 3. Logical connectors (pipe)
                if (c == '|')
                {
                    yield return new Token(TokenType.LogicalOr, "|", i);

                    i++;
                    continue;
                }

                // 4. Number
                if (char.IsDigit(c))
                {
                    var start = i;
                    i = ConsumeNumber(input, i);
                    yield return new Token(TokenType.Value, input.Substring(start, i - start), start);

                    afterOperator = false;
                    continue;
                }

                // 5. String (single or double quotes)
                if (IsQuote(c))
                {
                    var start = i;
                    i = ConsumeString(input, i);
                    yield return new Token(TokenType.Value, input.Substring(start, i - start), start);

                    afterOperator = false;
                    continue;
                }

                // 6. Property or Value: consume until next special char or whitespace
                var startIdx = i;
                while (i < input.Length && !char.IsWhiteSpace(input[i]) &&
                       input[i] != '(' && input[i] != ')' && input[i] != ',' && input[i] != '|' &&
                       !TryMatchOperator(input, i, out _, out _))
                {
                    i++;
                }
                
                if (i > startIdx)
                {
                    var value = input.Substring(startIdx, i - startIdx);
                    if (afterOperator || valueGroupDepth > 0)
                    {
                        yield return new Token(TokenType.Value, value, startIdx);

                        afterOperator = false;
                    }
                    else
                    {
                        yield return new Token(TokenType.Property, value, startIdx);
                    }
                }
            }
        }

        private static bool TryMatchOperator(string input, int start, out string op, out int opLength)
        {
            foreach (var opr in OperatorPatterns)
            {
                if (opr != null && input.Length - start >= opr.Length && input.Substring(start, opr.Length) == opr)
                {
                    op = opr;
                    opLength = opr.Length;
                    return true;
                }
            }

            op = string.Empty;
            opLength = 0;
            return false;
        }

        private static bool IsQuote(char c)
        {
            return c == '\'' || c == '"';
        }

        private static int ConsumeNumber(string input, int start)
        {
            var i = start;
            while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '.'))
            {
                i++;
            }

            return i;
        }

        private static int ConsumeString(string input, int start)
        {
            var quote = input[start];
            var i = start + 1;
            while (i < input.Length && input[i] != quote)
            {
                i++;
            }

            if (i < input.Length)
            {
                i++; // skip closing quote
            }


            return i;
        }

        private static bool IsPartOfOperator(string input, int idx)
        {
            foreach (var op in OperatorPatterns)
            {
                if (op != null && idx - op.Length + 1 >= 0 && input.Substring(idx - op.Length + 1, op.Length) == op)
                {
                    return true;
                }
            }

            return false;
        }
    }
}