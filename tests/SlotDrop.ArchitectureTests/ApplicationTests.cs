using ArchUnitNET.xUnitV3;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace SlotDrop.ArchitectureTests;

public class ApplicationTests
{
    [Fact]
    public void Application_should_not_depend_on_aspnetcore() =>
        Types().That().ResideInAssembly(SolutionArchitecture.Application)
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(@"^Microsoft\.AspNetCore(\..+)?$")
            .Check(SolutionArchitecture.Architecture);
}
