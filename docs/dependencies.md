# Dependencias de RoverRender2D

## Criterios de selección

Esta matriz se verificó el 13 de julio de 2026 contra el índice, el catálogo y las páginas oficiales de NuGet. Solo se aceptan versiones estables, listadas y compatibles con `net10.0` o `net10.0-windows`.

Las versiones se administran de forma central en `Directory.Packages.props` y se bloquean mediante `packages.lock.json`. Una entrada en esta matriz no autoriza a referenciar el paquete desde todos los proyectos: cada referencia debe existir únicamente en el proyecto que consume su API. `RoverRender2D.Domain` permanece libre de dependencias de infraestructura, Protobuf, UI y almacenamiento.

Las herramientas de compilación, adaptadores y colectores de pruebas deben usar `PrivateAssets="all"` para que no se propaguen como dependencias de producción.

## Fase 1 — Bootstrap compilable

| Paquete | Versión | Licencia | Uso y proyecto previsto | Alternativas evaluadas | Motivo de selección |
|---|---:|---|---|---|---|
| [`Microsoft.Extensions.Hosting`](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/10.0.9) | 10.0.9 | [MIT](https://licenses.nuget.org/MIT) | Composición, ciclo de vida, inyección de dependencias, configuración y logging en `RoverRender2D.Desktop`. Incluye un asset específico para `net10.0`. | Composición manual con `ServiceCollection`; referenciar por separado todas las extensiones de configuración y logging. | Proporciona un punto de composición mantenido por Microsoft y evita construir un ciclo de vida propio para la aplicación. |
| [`Microsoft.Extensions.Options.ConfigurationExtensions`](https://www.nuget.org/packages/Microsoft.Extensions.Options.ConfigurationExtensions/10.0.9) | 10.0.9 | [MIT](https://licenses.nuget.org/MIT) | **Condicional:** referencia directa en el proyecto de composición cuando se enlacen y validen opciones tipadas desde `IConfiguration`. Incluye un asset específico para `net10.0`. | Enlace manual; usar únicamente `Microsoft.Extensions.Configuration.Binder`; depender de la referencia transitiva que actualmente aporta Hosting. | Hace explícita la dependencia si el código llama sus extensiones de configuración y evita depender accidentalmente del grafo transitivo de Hosting. No debe agregarse si todavía no existe ese uso. |
| [`xunit.v3`](https://www.nuget.org/packages/xunit.v3/3.2.2) | 3.2.2 | [Apache-2.0](https://licenses.nuget.org/Apache-2.0) | Framework de pruebas en los proyectos bajo `tests/`. Sus assets `net8.0` son compatibles con `net10.0` y el paquete integra Microsoft Testing Platform. | MSTest; NUnit; `xunit` v2. | Es la línea activa y mantenida de xUnit. Permite comenzar el proyecto nuevo sin adoptar la línea v2 declarada obsoleta en NuGet. |
| [`xunit.runner.visualstudio`](https://www.nuget.org/packages/xunit.runner.visualstudio/3.1.5) | 3.1.5 | [Apache-2.0](https://licenses.nuget.org/Apache-2.0) | Adaptador de descubrimiento y ejecución para Visual Studio/Test Explorer y la ruta VSTest. Es compatible con xUnit v1, v2 y v3 y con .NET 8 o posterior. Debe ser privado. | Ejecutar exclusivamente con Microsoft Testing Platform sin adaptador VSTest. | Conserva descubrimiento en Visual Studio y compatibilidad con el flujo de pruebas del bootstrap. Puede retirarse si el repositorio migra y verifica una ruta exclusivamente MTP. |
| [`Microsoft.NET.Test.Sdk`](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/18.7.0) | 18.7.0 | [MIT](https://licenses.nuget.org/MIT) | Targets y host de pruebas para los proyectos bajo `tests/` en la ruta VSTest usada por `dotnet test`. Sus assets `net8.0` son compatibles con `net10.0`. | Configurar todo el repositorio para usar exclusivamente Microsoft Testing Platform. | Da soporte al flujo inicial de Visual Studio, `dotnet test` y al colector de cobertura. Debe reevaluarse junto con el runner si se adopta MTP puro. |
| [`coverlet.collector`](https://www.nuget.org/packages/coverlet.collector/10.0.1) | 10.0.1 | [MIT](https://licenses.nuget.org/MIT) | Cobertura en los proyectos bajo `tests/`; incluye assets específicos para `net10.0`. Debe ser privado. | `Microsoft.CodeCoverage`; instrumentación con `coverlet.msbuild`; herramientas externas de cobertura. | Integra cobertura multiplataforma mediante `dotnet test --collect:"XPlat Code Coverage"` sin instalar una herramienta global. |

`Microsoft.Extensions.Logging` 10.0.9 no se selecciona como referencia directa para la Fase 1: `Microsoft.Extensions.Hosting` ya incorpora la implementación y sus proveedores. Los proyectos que solo necesiten contratos de logging deben evaluar una referencia directa a `Microsoft.Extensions.Logging.Abstractions`, no al paquete completo.

## Fase 2 — Contrato e importación

| Paquete | Versión | Licencia | Uso y proyecto previsto | Alternativas evaluadas | Motivo de selección |
|---|---:|---|---|---|---|
| [`Google.Protobuf`](https://www.nuget.org/packages/Google.Protobuf/3.35.1) | 3.35.1 | [BSD-3-Clause](https://licenses.nuget.org/BSD-3-Clause) | Runtime para los mensajes versionados del payload binario en `RoverRender2D.Contracts`. Sus assets `net5.0` y `netstandard2.0` son compatibles con `net10.0`. | Contratos binarios escritos a mano; JSON/CSV; MessagePack. | Proporciona evolución de esquema, serialización compacta y generación determinista sin introducir una dependencia nativa grande. JSON queda reservado para el manifiesto pequeño y legible. |
| [`Grpc.Tools`](https://www.nuget.org/packages/Grpc.Tools/2.82.0) | 2.82.0 | [Apache-2.0](https://licenses.nuget.org/Apache-2.0) | Compilador `protoc` e integración MSBuild para generar C# desde `.proto` en `RoverRender2D.Contracts`. Es solo una herramienta de build y debe ser privada. | Instalar `protoc` globalmente; confirmar fuentes generadas; ejecutar scripts manuales de generación. | Fija el compilador dentro del repositorio y hace reproducible la generación en desarrollo y CI. Como RoverRender2D no usa gRPC, los elementos `<Protobuf>` deben declarar `GrpcServices="None"`; no se agrega ningún runtime de gRPC. |
| [`System.IO.Hashing`](https://www.nuget.org/packages/System.IO.Hashing/10.0.9) | 10.0.9 | [MIT](https://licenses.nuget.org/MIT) | CRC32 de cabeceras y registros en la implementación de IO de `RoverRender2D.Infrastructure`. Incluye un asset específico para `net10.0`. | Implementación propia de CRC32; otra biblioteca de hashing. | Evita mantener código criptográfico o de integridad de bajo nivel. CRC32 se usa para recuperación de tramas; SHA-256 del manifiesto continúa usando `System.Security.Cryptography`. |

## Dependencias diferidas

| Paquete | Versión verificada | Licencia | Fase prevista | Motivo para diferir | Alternativas a revisar |
|---|---:|---|---:|---|---|
| [`Microsoft.Data.Sqlite`](https://www.nuget.org/packages/Microsoft.Data.Sqlite/10.0.9) | 10.0.9 | [MIT](https://licenses.nuget.org/MIT) | 3 | La Fase 2 valida e importa por streaming, pero la indexación persistente, los checkpoints y los resultados derivados comienzan en la Fase 3. Agregarlo antes incorporaría también el payload nativo de SQLite sin un consumidor real. | Índices binarios propios; archivos JSON; `Microsoft.Data.Sqlite.Core` con proveedor SQLite elegido explícitamente. Se prefiere SQLite por transacciones, recuperación e índices, pero la decisión final debe documentar también sus dependencias transitivas nativas. |
| [`BenchmarkDotNet`](https://www.nuget.org/packages/BenchmarkDotNet/0.15.8) | 0.15.8 | [MIT](https://licenses.nuget.org/MIT) | 8 | Los benchmarks reproducibles pertenecen al endurecimiento, una vez que existan indexación y algoritmos representativos. Sus assets `net8.0` son compatibles con `net10.0`. | Cronometraje manual con `Stopwatch`; pruebas de rendimiento ad hoc. Se descartan como evidencia principal por calentamiento, JIT y ruido difíciles de controlar. |

Las versiones diferidas se registran para trazabilidad, pero no deben añadirse todavía a los archivos de proyecto ni a la administración central de paquetes si no existe una referencia que las consuma.

## Dependencia rechazada

| Paquete | Última versión estable | Estado oficial | Decisión |
|---|---:|---|---|
| [`xunit`](https://www.nuget.org/packages/xunit/2.9.3) | 2.9.3 | NuGet lo marca como **deprecated**, legado y sin mantenimiento de nuevas funcionalidades; solo recibirá correcciones de seguridad. La propia ficha recomienda `xunit.v3`. | Rechazado para un proyecto nuevo. Se usa `xunit.v3` 3.2.2. `xunit.runner.visualstudio` 3.1.5 permanece válido porque soporta las tres generaciones de xUnit. |

## Reglas de actualización

1. Resolver versiones únicamente desde NuGet oficial y descartar prereleases salvo una decisión documentada en ADR.
2. Confirmar compatibilidad de assets, licencia, deprecación y vulnerabilidades antes de actualizar.
3. Actualizar `Directory.Packages.props` y los archivos de bloqueo en el mismo cambio.
4. Ejecutar restauración bloqueada, build Release y toda la suite de pruebas tras cada actualización.
5. No incorporar una dependencia diferida hasta que el incremento vertical tenga un consumidor y pruebas que justifiquen su costo.
