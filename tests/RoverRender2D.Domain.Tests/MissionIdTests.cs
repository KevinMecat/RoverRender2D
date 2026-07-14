using RoverRender2D.Domain;

namespace RoverRender2D.Domain.Tests;

public sealed class MissionIdTests
{
    [Fact]
    public void Constructor_rejects_empty_identifier()
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new MissionId(Guid.Empty));
    }

    [Fact]
    public void TryParse_round_trips_valid_identifier()
    {
        Guid value = Guid.NewGuid();

        bool parsed = MissionId.TryParse(value.ToString("D"), out MissionId missionId);

        Assert.True(parsed);
        Assert.Equal(value, missionId.Value);
        Assert.Equal(value.ToString("D"), missionId.ToString());
    }
}
