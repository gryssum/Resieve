namespace ReSieve.Filtering.Lexers
{
    public enum TokenType
    {
        Identifier,
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