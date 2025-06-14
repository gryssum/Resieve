namespace ReSieve.Services
{
    public interface IReSievePropertyMetadata
    {
        bool CanSort { get; }
        bool CanFilter { get; }
    }

    public class ReSievePropertyMetadata : IReSievePropertyMetadata
    {
        public bool CanSort { get; set; }
        public bool CanFilter { get; set; }
    }
}