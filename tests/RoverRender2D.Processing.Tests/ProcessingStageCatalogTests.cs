using RoverRender2D.Processing;

namespace RoverRender2D.Processing.Tests;

public sealed class ProcessingStageCatalogTests
{
    [Fact]
    public void Default_stages_describe_an_offline_pipeline()
    {
        Assert.Collection(
            ProcessingStageCatalog.Default,
            stage => Assert.Equal("Validación", stage),
            stage => Assert.Equal("Indexación", stage),
            stage => Assert.Equal("Calidad de sensores", stage),
            stage => Assert.Equal("Estimación de trayectoria", stage),
            stage => Assert.Equal("Reconstrucción 2D", stage));
    }
}
