namespace RoverRender2D.Domain.Measurements;

/// <summary>
/// Represents a finite angle expressed in radians.
/// </summary>
public readonly record struct AngleRadians
{
    private const double FullTurn = 2 * Math.PI;

    public AngleRadians(double value)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Angle must be finite.");
        }

        Value = value;
    }

    public double Value { get; }

    public double Degrees => Value * 180 / Math.PI;

    public static AngleRadians FromDegrees(double degrees) => new(degrees * Math.PI / 180);

    /// <summary>
    /// Returns the equivalent angle in the interval (-pi, pi].
    /// </summary>
    public AngleRadians Normalize()
    {
        double normalized = Math.IEEERemainder(Value, FullTurn);
        if (normalized <= -Math.PI)
        {
            normalized += FullTurn;
        }

        return new AngleRadians(normalized);
    }
}
