using Microsoft.Extensions.Logging;

namespace SlotDrop.Domain;

public static partial class TenantNameRule
{
    public static bool IsValid(string name, ILogger logger)
    {
        var valid = !string.IsNullOrWhiteSpace(name);
        if (!valid)
        {
            LogInvalid(logger);
        }

        return valid;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Tenant name is empty")]
    private static partial void LogInvalid(ILogger logger);
}
