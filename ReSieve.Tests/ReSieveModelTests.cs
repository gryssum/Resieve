using ReSieve.Sorting;

namespace ReSieve.Tests;

public class ReSieveModelTests
{
    [Fact]
    public void SortString_Should_Parse_To_SortTerm_Ascending()
    {
        var model = new ReSieveModel {Sorts = "Name"};
        var sorts = ReSieveSortParser.ParseSorts(model.Sorts);
        Assert.Single(sorts);
        Assert.Equal("Name", sorts[0].Name);
        Assert.False(sorts[0].Descending);
    }

    [Fact]
    public void SortString_Should_Parse_To_SortTerm_Descending()
    {
        var model = new ReSieveModel {Sorts = "-Price"};
        var sorts = ReSieveSortParser.ParseSorts(model.Sorts);
        Assert.Single(sorts);
        Assert.Equal("Price", sorts[0].Name);
        Assert.True(sorts[0].Descending);
    }

    [Fact]
    public void SortString_Should_Parse_MultipleSorts()
    {
        var model = new ReSieveModel {Sorts = "Name,-Price"};
        var sorts = ReSieveSortParser.ParseSorts(model.Sorts);
        Assert.Equal(2, sorts.Count);
        Assert.Equal("Name", sorts[0].Name);
        Assert.False(sorts[0].Descending);
        Assert.Equal("Price", sorts[1].Name);
        Assert.True(sorts[1].Descending);
    }

    [Fact]
    public void SortString_Should_Return_Empty_When_NoSorts()
    {
        var model = new ReSieveModel {Sorts = string.Empty};
        var sorts = ReSieveSortParser.ParseSorts(model.Sorts);
        Assert.Empty(sorts);
    }
}