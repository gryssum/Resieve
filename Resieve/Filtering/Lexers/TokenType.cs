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
        CloseParen // )
    }
}