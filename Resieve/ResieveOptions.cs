namespace Resieve;

public class ResieveOptions
{
    public const string Resieve = "Resieve";
    
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 0;
}