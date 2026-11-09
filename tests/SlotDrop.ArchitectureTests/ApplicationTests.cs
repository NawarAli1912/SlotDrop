using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
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

    // By assembly, not namespace: Npgsql's EF extensions (EF.Functions.ILike) live in Microsoft.EntityFrameworkCore.
    // Application may use EF Core itself; which database sits behind it is Infrastructure's business.
    // "Not Infrastructure" needs no rule: Application referencing it closes a cycle, and the build rejects it.
    [Fact]
    public void Application_should_not_depend_on_npgsql() =>
        Types().That().ResideInAssembly(SolutionArchitecture.Application)
            .Should().NotDependOnAnyTypesThat().ResideInAssemblyMatching(@"^Npgsql(\..+)?$")
            .Check(SolutionArchitecture.Architecture);

    // Handlers have no interface (ADR-008), so a handler is recognised by its Handle method.
    [Fact]
    public void Classes_with_a_handle_method_should_be_named_handler() =>
        Classes().That().ResideInAssembly(SolutionArchitecture.Application)
            .And().FollowCustomPredicate(HasHandleMethod, "have a Handle method")
            .Should().HaveNameEndingWith("Handler")
            .Check(SolutionArchitecture.Architecture);

    private static bool HasHandleMethod(Class type) =>
        type.GetMethodMembers().Any(method => method.Name.StartsWith("Handle(", StringComparison.Ordinal));
}
