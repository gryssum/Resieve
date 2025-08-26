namespace Resieve.Mappings
{
    public interface IResievePropertyMetadata
    {
        bool CanSort { get; }
        bool CanFilter { get; }
    }
   
    public class ResievePropertyMap : IResievePropertyMetadata
    {
        public bool CanFilter { get; set; }
        public bool CanSort { get; set; }
    }
}