using ReSieve.Filtering;
using ReSieve.Filtering.Lexers;
using ReSieve.Filtering.TreeBuilder;
using ReSieve.Tests.Mocks;

namespace ReSieve.Tests.Filtering.TreeBuilder;

public class FilterTreeBuilderTests
{
    private readonly FilterLexer _lexer = new FilterLexer();
    private readonly FilterTreeBuilder _treeBuilder = new FilterTreeBuilder();

    [Fact]
    public void BuildTree_NameEqualsAppleFilter_ReturnTree()
    {
        var tokens = _lexer.Tokenize("Name==Apple").ToList();
        Assert.NotEmpty(tokens);


        var node = _treeBuilder.BuildTree(tokens);

        var comparison = Assert.IsType<ComparisonNode>(node);
        Assert.Equal("Name", comparison.PropertyName);
        Assert.Equal(FilterOperators.Equals, comparison.Operator);
        Assert.Equal("Apple", comparison.Value);
    }

    [Fact]
    public void BuildTree_NameAndPriceFilter_ReturnTree()
    {
        var tokens = _lexer.Tokenize("Name==Apple,Price>=10").ToList();
        Assert.NotEmpty(tokens);

        var treeBuilder = new FilterTreeBuilder();
        var node = treeBuilder.BuildTree(tokens);

        var logical = Assert.IsType<LogicalNode>(node);
        Assert.Equal(FilterLogicalOperator.And, logical.Operator);
        Assert.Equal(2, logical.Children.Count);

        var comparison1 = Assert.IsType<ComparisonNode>(logical.Children[0]);
        var comparison2 = Assert.IsType<ComparisonNode>(logical.Children[1]);

        Assert.Equal("Name", comparison1.PropertyName);
        Assert.Equal(FilterOperators.Equals, comparison1.Operator);
        Assert.Equal("Apple", comparison1.Value);

        Assert.Equal("Price", comparison2.PropertyName);
        Assert.Equal(FilterOperators.GreaterThanOrEqualTo, comparison2.Operator);
        Assert.Equal("10", comparison2.Value);
    }

    [Fact]
    public void BuildTree_NameOrPriceFilter_ReturnTree()
    {
        var tokens = _lexer.Tokenize("Name!=Apple|Price<=10").ToList();
        Assert.NotEmpty(tokens);

        var treeBuilder = new FilterTreeBuilder();
        var node = treeBuilder.BuildTree(tokens);

        var logical = Assert.IsType<LogicalNode>(node);
        Assert.Equal(FilterLogicalOperator.Or, logical.Operator);
        Assert.Equal(2, logical.Children.Count);

        var comparison1 = Assert.IsType<ComparisonNode>(logical.Children[0]);
        var comparison2 = Assert.IsType<ComparisonNode>(logical.Children[1]);

        Assert.Equal("Name", comparison1.PropertyName);
        Assert.Equal(FilterOperators.NotEquals, comparison1.Operator);
        Assert.Equal("Apple", comparison1.Value);

        Assert.Equal("Price", comparison2.PropertyName);
        Assert.Equal(FilterOperators.LessThanOrEqualTo, comparison2.Operator);
        Assert.Equal("10", comparison2.Value);
    }

    [Fact]
    public void BuildTree_Or_ReturnTree()
    {
        var tokens = _lexer.Tokenize(MockFilters.Or).ToList();
        Assert.NotEmpty(tokens);
        
        var node = _treeBuilder.BuildTree(tokens);
        var logical = Assert.IsType<LogicalNode>(node);
        Assert.Equal(FilterLogicalOperator.Or, logical.Operator);
        Assert.Equal(2, logical.Children.Count);

        var comparison1 = Assert.IsType<ComparisonNode>(logical.Children[0]);
        var comparison2 = Assert.IsType<ComparisonNode>(logical.Children[1]);

        Assert.Equal("Name", comparison1.PropertyName);
        Assert.Equal(FilterOperators.Equals, comparison1.Operator);
        Assert.Equal("Bread", comparison1.Value);

        Assert.Equal("Price", comparison2.PropertyName);
        Assert.Equal(FilterOperators.GreaterThan, comparison2.Operator);
        Assert.Equal("10", comparison2.Value);
    }
    
    [Fact]
    public void BuildTree_And_ReturnTree()
    {
        var tokens = _lexer.Tokenize(MockFilters.And).ToList();
        Assert.NotEmpty(tokens);
        
        var node = _treeBuilder.BuildTree(tokens);
        var logical = Assert.IsType<LogicalNode>(node);
        Assert.Equal(FilterLogicalOperator.And, logical.Operator);
        Assert.Equal(2, logical.Children.Count);

        var comparison1 = Assert.IsType<ComparisonNode>(logical.Children[0]);
        var comparison2 = Assert.IsType<ComparisonNode>(logical.Children[1]);

        Assert.Equal("Name", comparison1.PropertyName);
        Assert.Equal(FilterOperators.Equals, comparison1.Operator);
        Assert.Equal("Bread", comparison1.Value);

        Assert.Equal("Price", comparison2.PropertyName);
        Assert.Equal(FilterOperators.GreaterThan, comparison2.Operator);
        Assert.Equal("10", comparison2.Value);
    }
    
    [Fact]
    public void BuildTree_InValues_ReturnTree()
    {
        var tokens = _lexer.Tokenize(MockFilters.InValues).ToList();
        Assert.NotEmpty(tokens);
        
        var node = _treeBuilder.BuildTree(tokens);
        var group = Assert.IsType<GroupComparisonNode>(node);
        Assert.Equal("Price", group.PropertyName);
        Assert.Equal(FilterOperators.In, group.Operator);
        Assert.Equal(new List<string> { "1", "2" }, group.Values);
    }

    [Fact]
    public void BuildTree_NotInValues_ReturnTree()
    {
        var tokens = _lexer.Tokenize(MockFilters.NotInValues).ToList();
        Assert.NotEmpty(tokens);
        
        var node = _treeBuilder.BuildTree(tokens);
        var group = Assert.IsType<GroupComparisonNode>(node);
        Assert.Equal("Price", group.PropertyName);
        Assert.Equal(FilterOperators.NotIn, group.Operator);
        Assert.Equal(new List<string> { "1", "2" }, group.Values);
    }

    [Fact]
    public void BuildTree_Complex1_ReturnTree()
    {
        var tokens = _lexer.Tokenize(MockFilters.Complex1).ToList();
        Assert.NotEmpty(tokens);
        
        var node = _treeBuilder.BuildTree(tokens);
        
        var andNode = Assert.IsType<LogicalNode>(node);
        Assert.Equal(FilterLogicalOperator.And, andNode.Operator);
        Assert.Equal(3, andNode.Children.Count);

        // First child: OR node (Category IN [bread,food] OR Type==grocery)
        var orNode = Assert.IsType<LogicalNode>(andNode.Children[0]);
        Assert.Equal(FilterLogicalOperator.Or, orNode.Operator);
        Assert.Equal(2, orNode.Children.Count);
        var group = Assert.IsType<GroupComparisonNode>(orNode.Children[0]);
        Assert.Equal("Category", group.PropertyName);
        Assert.Equal(FilterOperators.In, group.Operator);
        Assert.Equal(new List<string> { "bread", "food" }, group.Values);
        var typeComp = Assert.IsType<ComparisonNode>(orNode.Children[1]);
        Assert.Equal("Type", typeComp.PropertyName);
        Assert.Equal(FilterOperators.Equals, typeComp.Operator);
        Assert.Equal("grocery", typeComp.Value);

        // Second child: Price > 10
        var priceComp = Assert.IsType<ComparisonNode>(andNode.Children[1]);
        Assert.Equal("Price", priceComp.PropertyName);
        Assert.Equal(FilterOperators.GreaterThan, priceComp.Operator);
        Assert.Equal("10", priceComp.Value);

        // Third child: Stock >= 5
        var stockComp = Assert.IsType<ComparisonNode>(andNode.Children[2]);
        Assert.Equal("Stock", stockComp.PropertyName);
        Assert.Equal(FilterOperators.GreaterThanOrEqualTo, stockComp.Operator);
        Assert.Equal("5", stockComp.Value);
    }

    [Fact]
    public void BuildTree_Complex2_ReturnTree()
    {
        var tokens = _lexer.Tokenize(MockFilters.Complex2).ToList();
        Assert.NotEmpty(tokens);
        
        var node = _treeBuilder.BuildTree(tokens);
        
        var orNode = Assert.IsType<LogicalNode>(node);
        Assert.Equal(FilterLogicalOperator.Or, orNode.Operator);
        Assert.Equal(2, orNode.Children.Count);

        // First child: AND node (Price>=10, Rating>=4)
        var andNode = Assert.IsType<LogicalNode>(orNode.Children[0]);
        Assert.Equal(FilterLogicalOperator.And, andNode.Operator);
        Assert.Equal(2, andNode.Children.Count);
        var priceComp = Assert.IsType<ComparisonNode>(andNode.Children[0]);
        Assert.Equal("Price", priceComp.PropertyName);
        Assert.Equal(FilterOperators.GreaterThanOrEqualTo, priceComp.Operator);
        Assert.Equal("10", priceComp.Value);
        var ratingComp = Assert.IsType<ComparisonNode>(andNode.Children[1]);
        Assert.Equal("Rating", ratingComp.PropertyName);
        Assert.Equal(FilterOperators.GreaterThanOrEqualTo, ratingComp.Operator);
        Assert.Equal("4", ratingComp.Value);

        // Second child: Discount > 0
        var discountComp = Assert.IsType<ComparisonNode>(orNode.Children[1]);
        Assert.Equal("Discount", discountComp.PropertyName);
        Assert.Equal(FilterOperators.GreaterThan, discountComp.Operator);
        Assert.Equal("0", discountComp.Value);
    }
}