namespace ReSieve.Filtering.Lexers
{
    public class Token
    {
        public Token(TokenType type, string value, int position)
        {
            Type = type;
            Value = value;
            Position = position;
        }

        public TokenType Type { get; }
        public string Value { get; }
        public int Position { get; }

        public override string ToString()
        {
            return $"{Type}: '{Value}' at {Position}";
        }
    }
}