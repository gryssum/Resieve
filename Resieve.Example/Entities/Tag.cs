namespace Resieve.Example.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Constructor for EF
    public Tag() { }

    // Constructor to maintain compatibility with existing code
    public Tag(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}