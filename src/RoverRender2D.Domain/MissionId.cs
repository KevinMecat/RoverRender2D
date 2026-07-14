namespace RoverRender2D.Domain;

/// <summary>
/// Identifies one recorded rover mission.
/// </summary>
public readonly record struct MissionId
{
    public MissionId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "A mission identifier cannot be empty.");
        }

        Value = value;
    }

    public Guid Value { get; }

    public static MissionId New() => new(Guid.NewGuid());

    public static bool TryParse(string? value, out MissionId missionId)
    {
        if (Guid.TryParse(value, out Guid parsed) && parsed != Guid.Empty)
        {
            missionId = new MissionId(parsed);
            return true;
        }

        missionId = default;
        return false;
    }

    public override string ToString() => Value.ToString("D");
}
