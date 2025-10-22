namespace Resieve
{
    public class ResieveModel
    {
        public string? Filters { get; init; }

        public string? Sorts { get; init; }

        public int Page { get; init; } = 1;

        public int PageSize { get; init; } = 10;
    }
}