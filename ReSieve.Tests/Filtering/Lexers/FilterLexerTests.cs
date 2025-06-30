using ReSieve.Filtering.Lexers;
using ReSieve.Tests.Mocks;

namespace ReSieve.Tests.Filtering.Lexers;

public class FilterLexerTests
{
    [Fact]
    public void Tokenize_NameEqualsAppleFilter_ToExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = "Name==Apple";

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 3 tokens: Identifier, Operator, Identifier
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Name", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("==", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("Apple", tokens[2].Value);
    }
    
    [Fact]
    public void Tokenize_NameEqualsQuotedAppleFilter_ToExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = "Name==\"Apple\"";

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 3 tokens: Identifier, Operator, Identifier
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Name", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("==", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("\"Apple\"", tokens[2].Value);
    }
    
    [Fact]
    public void Tokenize_NameEqualsWhitespaceAndSingleQuoteAppleFilter_ToExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = "Name=='Apple Tree'";

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 3 tokens: Identifier, Operator, Identifier
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Name", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("==", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("'Apple Tree'", tokens[2].Value);
    }
    
    [Fact]
    public void Tokenize_EqualsFilter_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.Equals; // "Name==Bread"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 3 tokens: Identifier, Operator, Identifier
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Name", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("==", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("Bread", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_GreaterThanFilter_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.GreaterThan; // "Weight>1.2"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 3 tokens: Identifier, Operator, Number
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Weight", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal(">", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("1.2", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_NotEqualsFilter_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.NotEquals; // "Price!=10.5"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 3 tokens: Identifier, Operator, Number
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Price", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("!=", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("10.5", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_ContainsFilter_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.Contains; // "Name@=ea"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 3 tokens: Identifier, Operator, Identifier
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Name", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("@=", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("ea", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_CaseInsensitiveStartsWithFilter_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.CaseInsensitiveStartsWith; // "Name_=*br"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 3 tokens: Identifier, Operator, Identifier
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Name", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("_=*", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("br", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_CaseInsensitiveEndsWithFilter_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.CaseInsensitiveEndsWith; // "Name_-=*ad"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 3 tokens: Identifier, Operator, Identifier
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Name", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("_-=*", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("ad", tokens[2].Value);
    }

    [Fact]
    public void Tokenize_AndFilter_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.And; // "Name==Bread,Price>10"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 7 tokens: Identifier, Operator, Identifier, LogicalAnd, Identifier, Operator, Number
        Assert.Equal(7, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Name", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("==", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("Bread", tokens[2].Value);
        Assert.Equal(TokenType.LogicalAnd, tokens[3].Type);
        Assert.Equal(",", tokens[3].Value);
        Assert.Equal(TokenType.Identifier, tokens[4].Type);
        Assert.Equal("Price", tokens[4].Value);
        Assert.Equal(TokenType.Operator, tokens[5].Type);
        Assert.Equal(">", tokens[5].Value);
        Assert.Equal(TokenType.Value, tokens[6].Type);
        Assert.Equal("10", tokens[6].Value);
    }

    [Fact]
    public void Tokenize_OrFilter_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.Or; // "Name==Bread|Price>10"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 7 tokens: Identifier, Operator, Identifier, LogicalOr, Identifier, Operator, Number
        Assert.Equal(7, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Name", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("==", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("Bread", tokens[2].Value);
        Assert.Equal(TokenType.LogicalOr, tokens[3].Type);
        Assert.Equal("|", tokens[3].Value);
        Assert.Equal(TokenType.Identifier, tokens[4].Type);
        Assert.Equal("Price", tokens[4].Value);
        Assert.Equal(TokenType.Operator, tokens[5].Type);
        Assert.Equal(">", tokens[5].Value);
        Assert.Equal(TokenType.Value, tokens[6].Type);
        Assert.Equal("10", tokens[6].Value);
    }
    
    [Fact]
    public void Tokenize_InValues_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.InValues; // "Price=|(1,2)"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 7 tokens: Identifier, Operator, Identifier, OpenParen, Number, LogicalOr, Number, CloseParen
        Assert.Equal(7, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Price", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("=|", tokens[1].Value);
        Assert.Equal(TokenType.OpenParen, tokens[2].Type);
        Assert.Equal(TokenType.Value, tokens[3].Type);
        Assert.Equal("1", tokens[3].Value);
        Assert.Equal(TokenType.LogicalAnd, tokens[4].Type);
        Assert.Equal(TokenType.Value, tokens[5].Type);
        Assert.Equal("2", tokens[5].Value);
        Assert.Equal(TokenType.CloseParen, tokens[6].Type);
    }

    [Fact]
    public void Tokenize_NotInValues_ProducesExpectedTokens()
    {
        // Arrange
        var lexer = new FilterLexer();
        var filter = MockFilters.NotInValues; // "Price!=|(1,2)"

        // Act
        var tokens = lexer.Tokenize(filter).ToList();

        // Assert: Should produce 7 tokens: Identifier, Operator, Identifier, OpenParen, Number, LogicalOr, Number, CloseParen
        Assert.Equal(7, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("Price", tokens[0].Value);
        Assert.Equal(TokenType.Operator, tokens[1].Type);
        Assert.Equal("!=|", tokens[1].Value);
        Assert.Equal(TokenType.OpenParen, tokens[2].Type);
        Assert.Equal(TokenType.Value, tokens[3].Type);
        Assert.Equal("1", tokens[3].Value);
        Assert.Equal(TokenType.LogicalAnd, tokens[4].Type);
        Assert.Equal(TokenType.Value, tokens[5].Type);
        Assert.Equal("2", tokens[5].Value);
        Assert.Equal(TokenType.CloseParen, tokens[6].Type);
    }

    [Fact]
    public void Tokenize_Complex1_ProducesExpectedTokens()
    {
        var lexer = new FilterLexer();
        var filter = MockFilters.Complex1;
        var tokens = lexer.Tokenize(filter).ToList();
        
        // (Category=|(bread,food)|Type==grocery),Price>10,Stock>=5
        Assert.Equal(21, tokens.Count);
        Assert.Equal(TokenType.OpenParen, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("Category", tokens[1].Value);
        
        Assert.Equal(TokenType.Operator, tokens[2].Type);
        Assert.Equal("=|", tokens[2].Value);
        
        Assert.Equal(TokenType.OpenParen, tokens[3].Type);
        Assert.Equal(TokenType.Value, tokens[4].Type);
        Assert.Equal("bread", tokens[4].Value);
        
        Assert.Equal(TokenType.LogicalAnd, tokens[5].Type);
        Assert.Equal(TokenType.Value, tokens[6].Type);
        Assert.Equal("food", tokens[6].Value);
        
        Assert.Equal(TokenType.CloseParen, tokens[7].Type);
        Assert.Equal(TokenType.LogicalOr, tokens[8].Type);
        Assert.Equal(TokenType.Identifier, tokens[9].Type);
        Assert.Equal("Type", tokens[9].Value);
        Assert.Equal(TokenType.Operator, tokens[10].Type);
        Assert.Equal("==", tokens[10].Value);
        Assert.Equal(TokenType.Value, tokens[11].Type);
        Assert.Equal("grocery", tokens[11].Value);
        Assert.Equal(TokenType.CloseParen, tokens[12].Type);
        Assert.Equal(TokenType.LogicalAnd, tokens[13].Type);
        Assert.Equal(TokenType.Identifier, tokens[14].Type);
        Assert.Equal("Price", tokens[14].Value);
        Assert.Equal(TokenType.Operator, tokens[15].Type);
        Assert.Equal(">", tokens[15].Value);
        Assert.Equal(TokenType.Value, tokens[16].Type);
        Assert.Equal("10", tokens[16].Value);
        Assert.Equal(TokenType.LogicalAnd, tokens[17].Type);
        Assert.Equal(TokenType.Identifier, tokens[18].Type);
        Assert.Equal("Stock", tokens[18].Value);
        Assert.Equal(TokenType.Operator, tokens[19].Type);
        Assert.Equal(">=", tokens[19].Value);
        Assert.Equal(TokenType.Value, tokens[20].Type);
        Assert.Equal("5", tokens[20].Value);
    }

    [Fact]
    public void Tokenize_Complex2_ProducesExpectedTokens()
    {
        var lexer = new FilterLexer();
        var filter = MockFilters.Complex2;
        var tokens = lexer.Tokenize(filter).ToList();
        
        // (Price>=10,Rating>=4)|Discount>0
        Assert.Equal(13, tokens.Count);
        Assert.Equal(TokenType.OpenParen, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("Price", tokens[1].Value);
        Assert.Equal(TokenType.Operator, tokens[2].Type);
        Assert.Equal(">=", tokens[2].Value);
        Assert.Equal(TokenType.Value, tokens[3].Type);
        Assert.Equal("10", tokens[3].Value);
        Assert.Equal(TokenType.LogicalAnd, tokens[4].Type);
        Assert.Equal(TokenType.Identifier, tokens[5].Type);
        Assert.Equal("Rating", tokens[5].Value);
        Assert.Equal(TokenType.Operator, tokens[6].Type);
        Assert.Equal(">=", tokens[6].Value);
        Assert.Equal(TokenType.Value, tokens[7].Type);
        Assert.Equal("4", tokens[7].Value);
        Assert.Equal(TokenType.CloseParen, tokens[8].Type);
        Assert.Equal(TokenType.LogicalOr, tokens[9].Type);
        Assert.Equal(TokenType.Identifier, tokens[10].Type);
        Assert.Equal("Discount", tokens[10].Value);
        Assert.Equal(TokenType.Operator, tokens[11].Type);
        Assert.Equal(">", tokens[11].Value);
        Assert.Equal(TokenType.Value, tokens[12].Type);
        Assert.Equal("0", tokens[12].Value);
    }
}