using RoverRender2D.Domain.Measurements;

namespace RoverRender2D.Domain.Tests;

public sealed class MeasurementTests
{
    [Theory]
    [InlineData(180, Math.PI)]
    [InlineData(90, Math.PI / 2)]
    [InlineData(0, 0)]
    public void Angle_from_degrees_uses_radians(double degrees, double expected)
    {
        AngleRadians angle = AngleRadians.FromDegrees(degrees);

        Assert.Equal(expected, angle.Value, 12);
    }

    [Fact]
    public void Normalize_wraps_angle_to_signed_turn()
    {
        AngleRadians angle = AngleRadians.FromDegrees(450).Normalize();

        Assert.Equal(90, angle.Degrees, 10);
    }

    [Fact]
    public void Distance_rejects_negative_values()
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new DistanceMeters(-0.01));
    }
}
