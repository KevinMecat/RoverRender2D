# ADR-0003: Contrato de misión versionado y log recuperable

- Estado: Aceptada
- Fecha: 2026-07-13
- Fase: 2

## Contexto

Una misión puede contener millones de observaciones y terminar de forma abrupta por energía o retiro del medio. JSON/CSV masivo consume espacio y memoria; un stream sin framing no permite detectar y saltar una región dañada. Aún no hay protocolos físicos confirmados.

## Decisión

Definir un contrato canónico sintético/de referencia v1:

- carpeta autocontenida con `mission.json`, `telemetry.rvrlog`, calibraciones opcionales y `checksums.sha256`;
- manifiesto UTF-8 de máximo 1 MiB, rutas relativas seguras, tamaños y SHA-256;
- cabecera `RVR2DLOG` little-endian fija de 44 bytes y CRC-32/ISO-HDLC;
- registros con cabecera fija de 32 bytes, sync `0x32525652`, secuencia/tiempo, payload Protobuf y CRC del payload;
- payload máximo 16 MiB y resincronización máxima 4 MiB por región dañada;
- tipos v1 GPS, LiDAR, IMU, rueda, evento y marcador visual; una fuente opcional puede estar ausente;
- streaming, límites antes de reservar y política explícita para versiones/tipos/campos desconocidos.

Los adaptadores físicos traducirán un protocolo demostrado al contrato canónico. El nombre de un sensor o un payload sintético no constituye soporte. Los detalles normativos están en [../data-contract.md](../data-contract.md).

## Consecuencias

Positivas:

- indexación y Replay por offsets sin cargar la misión completa;
- corrupción localizada puede detectarse, reportarse y recuperarse de forma acotada;
- manifiesto inspeccionable y payloads compactos/evolucionables;
- generador y pruebas no dependen de hardware disponible.

Costos:

- dos niveles de integridad (CRC por frame y SHA-256 por archivo) y validación semántica adicional;
- Protobuf requiere esquema, versionado y conversión explícita;
- una corrupción mayor que la ventana o una cabecera inválida detiene el archivo;
- cambiar framing/unidades requiere una versión nueva y migración deliberada.

## Alternativas descartadas

- **JSON/CSV por observación:** grande, lento y difícil de recuperar/indexar.
- **Un único blob Protobuf:** no ofrece resincronización ni acceso temporal eficiente.
- **SQLite escrito por el rover como formato fuente:** acopla logger/PC y complica recuperación/compatibilidad; SQLite se reserva para derivados locales.
- **Serialización nativa de objetos .NET:** no es un contrato interoperable y puede ser insegura.
