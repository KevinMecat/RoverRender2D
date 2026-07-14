# Contribuir a RoverRender2D

Gracias por contribuir. El objetivo es construir una herramienta offline, reproducible y segura para producir una base planimétrica 2D revisable; no una plataforma de telemetría ni un sistema certificado.

## Preparación

Se requiere Windows x64 y el SDK de .NET 10. Para la aplicación de escritorio se recomienda Visual Studio con **Desarrollo de escritorio de .NET**. Antes de empezar, lee [AGENTS.md](AGENTS.md), [docs/architecture.md](docs/architecture.md) y los ADR aplicables.

## Flujo de trabajo

1. Parte de una rama actualizada y compilable.
2. Crea una rama `feat/<descripcion>`, `fix/<descripcion>`, `docs/<descripcion>` o `chore/<descripcion>`.
3. Implementa un incremento pequeño con sus pruebas y documentación.
4. Ejecuta formato, compilación y pruebas en Release.
5. Usa un commit convencional, por ejemplo `feat(import): validate mission package integrity`.
6. Abre un PR con resumen, evidencia de validación, riesgos, compatibilidad y limitaciones.

No uses force push sobre ramas compartidas ni comandos que descarten cambios ajenos.

## Estilo y diseño

- Código y API en inglés; experiencia de usuario y documentación general en español.
- Habilita nullability y evita advertencias nuevas.
- Usa tipos explícitos para unidades, tiempo y coordenadas.
- Mantén el dominio libre de IO, UI y dependencias de infraestructura.
- Separa los contratos de disco de las entidades validadas del dominio.
- Conserva operaciones largas asíncronas, cancelables, observables y acotadas en memoria.
- Evita una dependencia nueva si la plataforma resuelve el problema. Toda dependencia relevante requiere evaluación de versión, licencia, mantenimiento y alternativas.

## Pruebas

Ejecuta desde la raíz:

```powershell
dotnet format --verify-no-changes
dotnet build -c Release
dotnet test -c Release
```

Las correcciones de errores deben incluir una prueba de regresión. Los algoritmos deben usar datasets sintéticos deterministas con ground truth y métricas, no umbrales elegidos para hacer pasar una sola muestra. Las pruebas con hardware o archivos reales deben identificar modelo, firmware, configuración y procedencia autorizada.

## Datos y seguridad

- Nunca confirmes tokens, secretos, rutas privadas innecesarias ni datos personales.
- No confirmes misiones o coordenadas reales sin permiso y revisión de privacidad.
- Marca de forma inequívoca todo dataset sintético.
- Trata los paquetes de misión como no confiables: valida rutas, tamaños, versiones, CRC y hashes antes de consumirlos.
- No escribas en la microSD de origen.

## Pull requests

Un PR está listo para revisión cuando:

- [ ] el alcance es único y está explicado;
- [ ] compila en Release sin advertencias nuevas;
- [ ] las pruebas relevantes pasan y se adjunta el resultado;
- [ ] se probaron cancelación, errores y entradas límite cuando aplican;
- [ ] se actualizó documentación, changelog o ADR cuando corresponde;
- [ ] no se afirma compatibilidad de hardware sin evidencia;
- [ ] no se añadieron datos sensibles ni una licencia no autorizada.

Los cambios de contrato, CRS, persistencia, algoritmo central, UI principal o formato de exportación requieren ADR antes de consolidarse.

## Versionado

El proyecto seguirá SemVer cuando comiencen las releases. Los cambios visibles se registran en [CHANGELOG.md](CHANGELOG.md). El contrato de misión se versiona de forma independiente y rechaza incompatibilidades mayores de manera explícita.

El repositorio no incluye licencia por ahora; la elección corresponde al propietario.
