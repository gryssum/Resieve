namespace Resieve.Filtering.Lexers
{
    public enum TokenType
    {
        Property,
        Value,
        Operator,
        LogicalAnd, // ,
        LogicalOr, // |
        OpenParen, // (
        CloseParen, // )
        Comma, // , (for value/column grouping)
        Pipe, // | (for value/column grouping)
        Whitespace,
        Unknown
    }
}