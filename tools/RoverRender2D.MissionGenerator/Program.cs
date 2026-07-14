using RoverRender2D.Contracts;
using RoverRender2D.Domain;

MissionId missionId = MissionId.New();

Console.WriteLine("RoverRender2D — Generador de misiones sintéticas");
Console.WriteLine($"Misión de ejemplo: {missionId}");
Console.WriteLine($"Versión de contrato: {ContractVersions.MissionManifest}");
Console.WriteLine("Estado: bootstrap listo; todavía no se generan tramas de sensores.");
Console.WriteLine("Los datos que produzca esta herramienta siempre se identificarán como sintéticos.");
