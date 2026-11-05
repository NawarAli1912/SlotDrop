using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

namespace SlotDrop.ArchitectureTests;

internal static class SolutionArchitecture
{
    public static readonly System.Reflection.Assembly Domain = typeof(Domain.AssemblyMarker).Assembly;

    public static readonly System.Reflection.Assembly Application = typeof(Application.AssemblyMarker).Assembly;

    public static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(Domain, Application)
        .Build();
}
