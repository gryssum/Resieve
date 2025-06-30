using System;
using System.Collections.Generic;
using System.Linq;
using ReSieve.Filtering.Lexers;

namespace ReSieve.Filtering.TreeBuilder
{
    public class FilterTreeBuilder
    {
        public FilterNode BuildTree(List<Token> tokens)
        {
            if (tokens == null || tokens.Count == 0)
            {
                throw new ArgumentException("No tokens provided");
            }

            var nodes = new List<FilterNode>();
            FilterLogicalOperator? logicalOp = null;
            var i = 0;
            while (i < tokens.Count)
            {
                FilterNode node;
                if (tokens[i].Type == TokenType.OpenParen && (i == 0 || tokens[i - 1].Type != TokenType.Operator))
                {
                    // Logical grouping: parse subexpression inside parentheses, skipping value group parens
                    var closeIdx = FindMatchingLogicalParen(tokens, i);
                    var innerTokens = tokens.Skip(i + 1).Take(closeIdx - i - 1).ToList();
                    node = BuildTree(innerTokens);
                    i = closeIdx + 1;
                }
                else
                {
                    node = ParseComparisonNode(tokens, ref i);
                }

                nodes.Add(node);

                if (i < tokens.Count && (tokens[i].Type == TokenType.LogicalAnd || tokens[i].Type == TokenType.LogicalOr))
                {
                    var op = tokens[i].Type == TokenType.LogicalAnd ? FilterLogicalOperator.And : FilterLogicalOperator.Or;
                    if (logicalOp == null)
                    {
                        logicalOp = op;
                    }
                    else if (logicalOp != op)
                    {
                        throw new ArgumentException("Mixed logical operators not supported");
                    }

                    i++; // move past logical token
                }
            }

            if (nodes.Count == 1)
            {
                return nodes[0];
            }

            if (nodes.Count > 1 && logicalOp != null)
            {
                return new LogicalNode(logicalOp.Value, nodes);
            }

            throw new ArgumentException("Invalid or empty filter expression");
        }

        // This method skips over value group parens (after an operator)
        private static int FindMatchingLogicalParen(List<Token> tokens, int openIdx)
        {
            int depth = 0;
            for (int i = openIdx; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.OpenParen)
                {
                    // If this is a value group (immediately after an operator), skip to its close
                    if (i > 0 && tokens[i - 1].Type == TokenType.Operator)
                    {
                        int valueGroupClose = FindMatchingValueGroupParen(tokens, i);
                        i = valueGroupClose;
                        continue;
                    }
                    depth++;
                }
                else if (tokens[i].Type == TokenType.CloseParen)
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }
            throw new ArgumentException("Unmatched parenthesis in grouping");
        }

        private static int FindMatchingValueGroupParen(List<Token> tokens, int openIdx)
        {
            int depth = 0;
            for (int i = openIdx; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.OpenParen) depth++;
                else if (tokens[i].Type == TokenType.CloseParen) depth--;
                if (depth == 0) return i;
            }

            throw new ArgumentException("Unmatched parenthesis in value group");
        }

        private static FilterNode ParseComparisonNode(List<Token> tokens, ref int i)
        {
            // Expect: Identifier Operator Value | Identifier Operator (Value[,|Value]*)
            if (!IsComparisonStart(tokens, i))
            {
                throw new ArgumentException($"Expected property/operator at token {i}");
            }

            var property = tokens[i].Value;
            var op = FilterOperatorsExtensions.GetOperatorParsed(tokens[i + 1].Value);

            // Handle value group in parentheses
            if (tokens[i + 2].Type == TokenType.OpenParen)
            {
                var values = ParseValueGroup(tokens, i + 2, out var nextIndex);
                i = nextIndex;
                if (op == FilterOperators.In || op == FilterOperators.NotIn)
                {
                    if (values.Count < 1)
                    {
                        throw new ArgumentException("IN/NOT IN must have at least one value");
                    }

                    return new GroupComparisonNode(property, op, values);
                }

                if (values.Count == 1)
                {
                    return new ComparisonNode(property, op, values[0]);
                }

                throw new ArgumentException($"Operator {op} does not support grouped values. Use IN/NOT IN for value groups.");
            }

            // Handle single value (no parentheses)
            if (tokens[i + 2].Type == TokenType.Value)
            {
                var value = tokens[i + 2].Value;
                i += 3;
                return new ComparisonNode(property, op, value);
            }

            throw new ArgumentException($"Expected value or value group after operator at token {i}");
        }

        private static bool IsComparisonStart(List<Token> tokens, int i)
        {
            return i + 2 < tokens.Count && tokens[i].Type == TokenType.Identifier && tokens[i + 1].Type == TokenType.Operator;
        }

        private static List<string> ParseValueGroup(List<Token> tokens, int openParenIdx, out int nextIndex)
        {
            var closeIdx = tokens.FindIndex(openParenIdx, t => t.Type == TokenType.CloseParen);
            if (closeIdx == -1)
            {
                throw new ArgumentException("Unmatched parenthesis in value group");
            }

            var valueTokens = tokens.Skip(openParenIdx + 1).Take(closeIdx - (openParenIdx + 1)).ToList();
            var sepType = valueTokens.Any(t => t.Type == TokenType.LogicalOr) ? TokenType.LogicalOr : TokenType.LogicalAnd;
            var valueGroups = SplitTokens(valueTokens, sepType);
            var values = valueGroups.Select(group => group.Single(t => t.Type == TokenType.Value).Value).ToList();
            nextIndex = closeIdx + 1;
            return values;
        }

        private static List<List<Token>> SplitTokens(List<Token> tokens, TokenType splitType)
        {
            var groups = new List<List<Token>>();
            var current = new List<Token>();
            foreach (var token in tokens)
            {
                if (token.Type == splitType)
                {
                    if (current.Count > 0)
                    {
                        groups.Add(current);
                        current = new List<Token>();
                    }
                }
                else
                {
                    current.Add(token);
                }
            }

            if (current.Count > 0)
            {
                groups.Add(current);
            }

            return groups;
        }
    }
}