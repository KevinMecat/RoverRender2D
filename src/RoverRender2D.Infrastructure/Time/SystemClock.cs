using RoverRender2D.Application.Abstractions;

namespace RoverRender2D.Infrastructure.Time;

/// <summary>
/// Provides the operating system UTC clock.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
