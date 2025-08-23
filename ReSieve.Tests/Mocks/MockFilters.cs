namespace ReSieve.Tests.Mocks;

public static class MockFilters
{
    // Simple filter strings for each operator
    public new readonly static string Equals = "Name==Bread";
    public readonly static string NotEquals = "Price!=10.5";
    public readonly static string GreaterThan = "Weight>1.2";
    public readonly static string LessThan = "Rating<4.5";
    public readonly static string GreaterThanOrEqualTo = "CreatedAt>=2024-01-01";
    public readonly static string LessThanOrEqualTo = "Id<=100";
    public readonly static string Contains = "Name@=ea";
    public readonly static string StartsWith = "Name_=Br";
    public readonly static string EndsWith = "Name_-=ead";
    public readonly static string DoesNotContains = "Name!@=xyz";
    public readonly static string DoesNotStartsWith = "Name!_=Foo";
    public readonly static string DoesNotEndsWith = "Name!_-=Bar";
    public readonly static string CaseInsensitiveEquals = "Category==*food";
    public readonly static string CaseInsensitiveNotEquals = "Category!=*electronics";
    public readonly static string CaseInsensitiveContains = "Name@=*bread";
    public readonly static string CaseInsensitiveDoesNotContains = "Name!@=*milk";
    public readonly static string CaseInsensitiveStartsWith = "Name_=*br";
    public readonly static string CaseInsensitiveEndsWith = "Name_-=*ad";
    public readonly static string CaseInsensitiveDoesNotStartsWith = "Name!_=*foo";
    public readonly static string CaseInsensitiveDoesNotEndsWith = "Name!_-=*bar";
    public readonly static string And = "Name==Bread,Price>10";
    public readonly static string Or = "Name==Bread|Price>10";

    // Complex filter examples with parenthesis, AND, OR, and value grouping
    public readonly static string
        Complex1 = "(Category==bread|Category==food)|Type==grocery),Price>10,Stock>=5";

    public readonly static string
        Complex2 = "(Price>=10,Rating>=4)|Discount>0";
}