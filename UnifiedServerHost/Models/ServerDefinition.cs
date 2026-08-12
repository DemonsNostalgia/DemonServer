namespace Eudemons.UnifiedServer.Models;

public sealed record ServerDefinition(
    string Id,
    string DisplayName,
    string ExecutableName,
    string WorkingDirectoryName,
    string StopCommand,
    string Color,
    int Port,
    int StartOrder);
