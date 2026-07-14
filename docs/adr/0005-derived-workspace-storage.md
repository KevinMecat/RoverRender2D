# ADR-0005: Fuente inmutable y SQLite para estado derivado

- Estado: Aceptada
- Fecha: 2026-07-13
- Fases: 2–3

## Contexto

La aplicación debe importar desde un medio extraíble no confiable, reanudar operaciones, abrir proyectos sin reprocesar y mantener ediciones/revisiones trazables. Los logs y nubes pueden ser demasiado grandes para duplicarlos dentro de una base relacional.

## Decisión

Crear por misión un workspace local con:

- `source/`: copia validada e inmutable del paquete; nunca se edita en sitio;
- `workspace.db`: SQLite con esquema/migraciones versionados para índice de offsets, calidad, parámetros, checkpoints, poses, restricciones, elementos, revisiones y exportaciones;
- `cache/`: chunks/LOD y artefactos regenerables identificados por hash;
- `exports/`: salidas y reportes asociados a una revisión.

Los payloads crudos masivos permanecen en `telemetry.rvrlog`; SQLite guarda offsets y derivados compactos. Escrituras usan transacciones y temporales/reemplazo atómico donde corresponda. Cada resultado referencia hashes de entrada, parámetros, versión y revisión.

La copia desde microSD verifica espacio, progreso por bytes, cancelación y SHA-256 antes de promover `source/`. La aplicación no escribe en el medio de origen.

## Consecuencias

Positivas:

- consultas, checkpoints y ediciones transaccionales;
- Replay y reprocesamiento selectivo por offsets;
- recuperación clara: fuente, estado durable y caché regenerable están separados;
- el proyecto reabre sin repetir etapas válidas.

Costos:

- se deben diseñar migraciones, integridad y política de invalidación de caché;
- requiere espacio para una copia local y comprobación previa;
- mover/respaldar una misión exige mantener juntos fuente, base y metadatos;
- SQLite no elimina la necesidad de archivos temporales y manejo de corrupción.

## Alternativas descartadas

- **Procesar directamente en microSD:** lento, frágil y podría alterar el único original.
- **Guardar todos los payloads/nube en SQLite:** duplicación, crecimiento y presión de IO innecesarios.
- **Solo JSON/archivos sueltos para derivados:** transacciones, consultas, revisiones y migraciones serían más frágiles.
- **Base central única para todas las misiones:** aumenta el radio de corrupción y dificulta mover/archivar proyectos individuales.
