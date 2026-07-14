namespace RoverRender2D.Application;

/// <summary>
/// Contains application-level settings configured by the composition root.
/// </summary>
public sealed class RoverRenderOptions
{
    public const string SectionName = "RoverRender2D";

    public string ApplicationName { get; set; } = "RoverRender2D";

    public string WorkspacePath { get; set; } = string.Empty;
}
