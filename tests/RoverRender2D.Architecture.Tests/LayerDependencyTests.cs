using System.Xml.Linq;

namespace RoverRender2D.Architecture.Tests;

public sealed class LayerDependencyTests
{
    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> AllowedReferences =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            ["RoverRender2D.Application"] = SetOf("RoverRender2D.Domain"),
            ["RoverRender2D.Contracts"] = SetOf(),
            ["RoverRender2D.Desktop"] = SetOf(
                "RoverRender2D.Application",
                "RoverRender2D.Export",
                "RoverRender2D.Infrastructure",
                "RoverRender2D.Processing",
                "RoverRender2D.Rendering"),
            ["RoverRender2D.Domain"] = SetOf(),
            ["RoverRender2D.Export"] = SetOf(
                "RoverRender2D.Application",
                "RoverRender2D.Domain",
                "RoverRender2D.Rendering"),
            ["RoverRender2D.Infrastructure"] = SetOf(
                "RoverRender2D.Application",
                "RoverRender2D.Contracts",
                "RoverRender2D.Domain"),
            ["RoverRender2D.Processing"] = SetOf(
                "RoverRender2D.Application",
                "RoverRender2D.Domain"),
            ["RoverRender2D.Rendering"] = SetOf(
                "RoverRender2D.Application",
                "RoverRender2D.Domain"),
        };

    [Fact]
    public void Production_project_references_point_inward()
    {
        string repositoryRoot = FindRepositoryRoot();

        foreach ((string project, IReadOnlySet<string> allowed) in AllowedReferences)
        {
            string projectFile = Path.Combine(repositoryRoot, "src", project, $"{project}.csproj");
            XDocument document = XDocument.Load(projectFile);
            string[] references = document
                .Descendants("ProjectReference")
                .Select(element => element.Attribute("Include")?.Value)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => Path.GetFileNameWithoutExtension(value!))
                .ToArray();

            string[] forbidden = references.Where(reference => !allowed.Contains(reference)).ToArray();

            Assert.Empty(forbidden);
        }
    }

    [Theory]
    [InlineData("RoverRender2D.Domain")]
    [InlineData("RoverRender2D.Contracts")]
    public void Core_contract_projects_have_no_package_dependencies(string project)
    {
        string repositoryRoot = FindRepositoryRoot();
        string projectFile = Path.Combine(repositoryRoot, "src", project, $"{project}.csproj");
        XDocument document = XDocument.Load(projectFile);

        Assert.Empty(document.Descendants("PackageReference"));
    }

    private static HashSet<string> SetOf(params string[] values) =>
        new(values, StringComparer.Ordinal);

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? current = new(AppContext.BaseDirectory);

        while (current is not null && !File.Exists(Path.Combine(current.FullName, "RoverRender2D.sln")))
        {
            current = current.Parent;
        }

        return current?.FullName
            ?? throw new DirectoryNotFoundException("Could not locate RoverRender2D.sln from the test output directory.");
    }
}
