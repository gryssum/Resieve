namespace Resieve
{
    public record ResieveModel
    {
        public string? Filters { get; init; }

        public string? Sorts { get; init; }

        public int Page { get; init; }

        public int? PageSize { get; init; }
    }
    
    public record PaginatedResponse<T>(T Items, int PageNumber, int PageSize, int TotalCount);
}