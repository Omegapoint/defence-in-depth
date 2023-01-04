using AutoMapper;
using Defence.In.Depth;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

public class AutoMapperConfigurationTests
{
    [Fact]
    public void AssertConfigurationIsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        config.AssertConfigurationIsValid();
    }    
}