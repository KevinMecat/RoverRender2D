using System.Diagnostics;
using RoverRender2D.Domain.Measurements;
using RoverRender2D.Processing;

const int iterations = 100_000;
var stopwatch = Stopwatch.StartNew();

for (int index = 0; index < iterations; index++)
{
    _ = new AngleRadians(index * 0.001).Normalize();
}

stopwatch.Stop();

Console.WriteLine("RoverRender2D — comprobación de rendimiento de Fase 1");
Console.WriteLine($"Etapas registradas: {ProcessingStageCatalog.Default.Count}");
Console.WriteLine($"Normalizaciones: {iterations:N0} en {stopwatch.Elapsed.TotalMilliseconds:N2} ms");
Console.WriteLine("Nota: esta comprobación no sustituye un benchmark estadístico de las fases posteriores.");
