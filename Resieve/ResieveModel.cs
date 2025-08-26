namespace Resieve
{
    
    public class ResieveModel
    {
        public string? Filters { get; set; }

        public string? Sorts { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}