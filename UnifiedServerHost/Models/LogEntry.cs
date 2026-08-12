using System.Windows.Media;

namespace Eudemons.UnifiedServer.Models;

public sealed class LogEntry
{
    public required DateTime Timestamp { get; init; }
    public required string ServerId { get; init; }
    public required string ServerName { get; init; }
    public required string Message { get; init; }
    public required Brush ServerBrush { get; init; }
    public required Brush MessageBrush { get; init; }

    public string TimeText => Timestamp.ToString("HH:mm:ss.fff");
}
