using Microsoft.AspNetCore.Http;

namespace SlotDrop.Application;

public static class TenantResults
{
    public static IResult NotFound() => Results.NotFound();
}
