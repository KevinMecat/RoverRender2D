namespace RoverRender2D.Domain.Measurements;

/// <summary>
/// Represents a finite, non-negative distance expressed in metres.
/// </summary>
public readonly record struct DistanceMeters
{
    public DistanceMeters(double value)
    {
        if (!double.IsFinite(value) || value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Distance must be finite and non-negative.");
        }

        Value = value;
    }

    public double Value { get; }

    public static DistanceMeters Zero => new(0);
}
