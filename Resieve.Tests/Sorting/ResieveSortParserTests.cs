using Resieve.Sorting;

namespace Resieve.Tests.Sorting;

public class ResieveSortParserTests
{
    [Theory]
    [InlineData("Name", new[] {"Name"}, new[] {false})]
    [InlineData("-Age", new[] {"Age"}, new[] {true})]
    [InlineData("Name,-Age", new[] {"Name", "Age"}, new[] {false, true})]
    [InlineData("  Name  ,  -Age  ", new[] {"Name", "Age"}, new[] {false, true})]
    [InlineData("", new string[0], new bool[0])]
    [InlineData(null, new string[0], new bool[0])]
    public void ParseSorts_ParsesCorrectly(string? input, string[] expectedNames, bool[] expectedDesc)
    {
        var result = ResieveSortParser.ParseSorts(input);
        Assert.Equal(expectedNames.Length, result.Count);
        for (var i = 0; i < expectedNames.Length; i++)
        {
            Assert.Equal(expectedNames[i], result[i].Name);
            Assert.Equal(expectedDesc[i], result[i].Descending);
        }
    }

    [Fact]
    public void ParseSorts_IgnoresEmptyParts()
    {
        var result = ResieveSortParser.ParseSorts(",,Name,,");
        Assert.Single(result);
        Assert.Equal("Name", result[0].Name);
        Assert.False(result[0].Descending);
    }
}