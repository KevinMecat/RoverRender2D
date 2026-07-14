namespace RoverRender2D.Application.Abstractions;

/// <summary>
/// Provides time without coupling use cases to the system clock.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
