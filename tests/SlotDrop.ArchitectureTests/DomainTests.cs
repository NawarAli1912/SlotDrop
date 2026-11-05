using ArchUnitNET.xUnitV3;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace SlotDrop.ArchitectureTests;

public class DomainTests
{
    [Fact]
    public void Domain_should_not_depend_on_other_libraries() =>
        Types().That().ResideInAssembly(SolutionArchitecture.Domain)
            .Should().OnlyDependOnTypesThat().ResideInNamespaceMatching(@"^(SlotDrop\.Domain|System)(\..+)?$")
            .Check(SolutionArchitecture.Architecture);
}
