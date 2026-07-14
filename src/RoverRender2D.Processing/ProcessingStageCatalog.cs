using System.Collections.ObjectModel;

namespace RoverRender2D.Processing;

/// <summary>
/// Names the stable high-level stages that the desktop shell can present.
/// </summary>
public static class ProcessingStageCatalog
{
    private static readonly ReadOnlyCollection<string> Stages = Array.AsReadOnly(
        [
            "Validación",
            "Indexación",
            "Calidad de sensores",
            "Estimación de trayectoria",
            "Reconstrucción 2D",
        ]);

    public static IReadOnlyList<string> Default => Stages;
}
