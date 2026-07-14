namespace RoverRender2D.Export.Tests;

public sealed class ExportModuleTests
{
    [Fact]
    public void Module_has_a_spanish_display_name()
    {
        Assert.Equal("Exportación técnica", ExportModule.DisplayName);
    }
}
