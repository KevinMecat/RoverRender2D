using RoverRender2D.Application.Abstractions;
using RoverRender2D.Infrastructure.Time;

namespace RoverRender2D.Infrastructure.Tests;

public sealed class SystemClockTests
{
    [Fact]
    public void UtcNow_uses_the_system_utc_clock()
    {
        DateTimeOffset before = DateTimeOffset.UtcNow;
        IClock clock = new SystemClock();

        DateTimeOffset observed = clock.UtcNow;
        DateTimeOffset after = DateTimeOffset.UtcNow;

        Assert.InRange(observed, before, after);
        Assert.Equal(TimeSpan.Zero, observed.Offset);
    }
}
