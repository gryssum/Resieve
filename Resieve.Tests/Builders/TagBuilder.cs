using Resieve.Example.Entities;

namespace Resieve.Tests.Builders;

public class TagBuilder
{
    private string _description = "A test tag description.";
    private int _id = 1;
    private string _name = "Test Tag";

    public TagBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public TagBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TagBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public Tag Build()
    {
        return new Tag(_id, _name, _description);
    }
}