using ArchUnitNET.xUnitV3;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace SlotDrop.ArchitectureTests;

public class DomainTests
{
    // OnlyDependOnTypesThat() ignores types outside the loaded assemblies, so it let ILogger through in the red commit.
    [Fact]
    public void Domain_should_not_depend_on_other_libraries() =>
        Types().That().ResideInAssembly(SolutionArchitecture.Domain)
            .Should().NotDependOnAnyTypesThat().DoNotResideInNamespaceMatching(@"^(SlotDrop\.Domain|System)(\..+)?$")
            .Check(SolutionArchitecture.Architecture);
}
