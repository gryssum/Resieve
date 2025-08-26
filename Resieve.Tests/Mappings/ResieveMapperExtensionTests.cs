using Resieve.Mappings;

namespace Resieve.Tests.Mappings;

public class ResieveMapperExtensionTests
{

    [Fact]
    public void ApplyConfiguration_ShouldCallConfigureAndUpdateMappings()
    {
        var mapper = new ResieveMapper();
        mapper.ApplyConfiguration<TestMapping>();

        var mapping = mapper.PropertyMappings[typeof(TestEntity)];
        Assert.True(mapping.ContainsKey("Id"));
        Assert.True(mapping["Id"].CanFilter);
        Assert.True(mapping["Id"].CanSort);
    }

    [Fact]
    public void ApplyConfigurationsFromAssembly_ShouldApplyAllMappings()
    {
        // Reset static state
        var mapper = new ResieveMapper();
        var assembly = typeof(ResieveMapperExtensionTests).Assembly;
        mapper.ApplyConfigurationsFromAssembly(assembly);

        var mapping = mapper.PropertyMappings[typeof(TestEntity)];
        Assert.True(mapping.ContainsKey("Id"));
        Assert.True(mapping["Id"].CanFilter);
        Assert.True(mapping["Id"].CanSort);
        Assert.True(mapping.ContainsKey("Name"));
        Assert.True(mapping["Name"].CanFilter);
        Assert.True(AnotherTestMapping.Configured);
    }

    [Fact]
    public void ApplyConfigurationsFromAssembly_ShouldIgnoreNonMappings()
    {
        var mapper = new ResieveMapper();
        var assembly = typeof(ResieveMapperExtensionTests).Assembly;
        // Should not throw or add mappings for types not implementing IResieveMapping
        mapper.ApplyConfigurationsFromAssembly(assembly);
        // No exception means pass
    }
    
    
    private class TestMapping : IResieveMapping
    {
        public bool Configured { get; private set; }
        public void Configure(ResieveMapper mapper)
        {
            Configured = true;
            mapper.Property<TestEntity>(x => x.Id).CanFilter().CanSort();
        }
    }

    private class AnotherTestMapping : IResieveMapping
    {
        public static bool Configured { get; private set; }
        public void Configure(ResieveMapper mapper)
        {
            Configured = true;
            mapper.Property<TestEntity>(x => x.Name).CanFilter();
        }
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}