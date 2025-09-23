using NSubstitute;
using Resieve.Filtering.ExpressionTrees;
using Resieve.Filtering.Lexers;
using Resieve.Mappings;
using Resieve.Tests.Mocks;

namespace Resieve.Tests.Filtering.ExpressionTrees;

public class ExpressionTreeBuilderTests
{
    private IExpressionTreeBuilder CreateExpressionTreeBuilder()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        return new ExpressionTreeBuilder(serviceProvider);
    }

    [Fact]
    public void BuildFromTokens_SingleCondition_ReturnsCorrectExpression()
    {
        // Arrange
        var tokens = new List<Token>
        {
            new Token(TokenType.Property, "Name", 0),
            new Token(TokenType.Operator, "==", 1),
            new Token(TokenType.Value, "Apple", 2)
        };

        // Act
        var expression = CreateExpressionTreeBuilder().BuildFromTokens<Product>(tokens, new Dictionary<string, ResievePropertyMap>());

        // Assert
        Assert.NotNull(expression);
        var compiled = expression.Compile();
        var result = compiled(new Product(1, "Apple", 1.99m, 0.5, 4.5f, DateTime.Now, Guid.NewGuid(), ProductCategory.Food, true, new List<Tag>()));
        Assert.True(result);
    }

    [Fact]
    public void BuildFromTokens_LogicalAnd_ReturnsCorrectExpression()
    {
        // Arrange
        var tokens = new List<Token>
        {
            new Token(TokenType.Property, "Name", 0),
            new Token(TokenType.Operator, "==", 1),
            new Token(TokenType.Value, "Apple", 2),
            new Token(TokenType.LogicalAnd, ",", 3),
            new Token(TokenType.Property, "Price", 4),
            new Token(TokenType.Operator, ">", 5),
            new Token(TokenType.Value, "10", 6)
        };

        // Act
        var expression = CreateExpressionTreeBuilder().BuildFromTokens<Product>(tokens, new Dictionary<string, ResievePropertyMap>());

        // Assert
        Assert.NotNull(expression);
        var compiled = expression.Compile();
        var result = compiled(new Product(2, "Apple", 10.99m, 0.5, 4.5f, DateTime.Now, Guid.NewGuid(), ProductCategory.Food, true, new List<Tag>()));
        Assert.True(result);
    }

    [Fact]
    public void BuildFromTokens_LogicalOr_ReturnsCorrectExpression()
    {
        // Arrange
        var tokens = new List<Token>
        {
            new Token(TokenType.Property, "Name", 0),
            new Token(TokenType.Operator, "==", 1),
            new Token(TokenType.Value, "Apple", 2),
            new Token(TokenType.LogicalOr, "|", 3),
            new Token(TokenType.Property, "Name", 4),
            new Token(TokenType.Operator, "==", 5),
            new Token(TokenType.Value, "Banana", 6)
        };

        // Act
        var expression = CreateExpressionTreeBuilder().BuildFromTokens<Product>(tokens, new Dictionary<string, ResievePropertyMap>());

        // Assert
        Assert.NotNull(expression);
        var compiled = expression.Compile();
        var result = compiled(new Product(3, "Banana", 0.99m, 0.3, 4.0f, DateTime.Now, Guid.NewGuid(), ProductCategory.Food, true, new List<Tag>()));
        Assert.True(result);
    }

    [Fact]
    public void BuildFromTokens_EmptyTokens_ThrowsException()
    {
        // Arrange
        var tokens = new List<Token>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => CreateExpressionTreeBuilder().BuildFromTokens<Product>(tokens, new Dictionary<string, ResievePropertyMap>()));
    }
}