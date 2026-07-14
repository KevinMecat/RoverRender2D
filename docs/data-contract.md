# Contrato de datos de misión v1

Estado: **contrato sintético/de referencia v1**. Es normativo para el generador y las pruebas internas, pero no afirma compatibilidad con ningún logger o sensor físico. Un adaptador físico solo se habilitará después de validar manual, firmware, tramas y muestras reales según [sensor-integration-checklist.md](sensor-integration-checklist.md).

Decisión: [ADR-0003](adr/0003-versioned-mission-contract.md).

## Paquete autocontenido

```text
<mission-id>/
├── mission.json
├── telemetry.rvrlog
├── calibration/
│   ├── lidar.json          opcional según sensores declarados
│   ├── imu.json            opcional
│   └── extrinsics.json     opcional
├── camera/                 opcional, nunca ejecutable
└── checksums.sha256
```

Todo nombre del manifiesto es una ruta relativa normalizada con `/`. Se rechazan rutas absolutas, segmentos vacíos, `.`, `..`, prefijos de dispositivo, ADS de NTFS, caracteres nulos, enlaces/reparse points y cualquier resolución fuera de la raíz. El descubrimiento usa profundidad limitada.

Límites v1:

| Elemento | Límite |
|---|---:|
| `mission.json` antes de deserializar | 1 MiB (`1,048,576` bytes) |
| Payload de un registro | 16 MiB (`16,777,216` bytes) |
| Búsqueda de resincronización por región dañada | 4 MiB (`4,194,304` bytes) |
| Cabecera de archivo | exactamente 44 bytes |
| Cabecera de registro | exactamente 32 bytes |

No se reserva memoria a partir de una longitud hasta comprobar límites, overflow y bytes restantes. Otros límites operativos —cantidad de archivos, longitud de ruta y tamaño total— se definen en la política de importación y se verifican antes de copiar.

## `mission.json`

El archivo es UTF-8 sin BOM, JSON estricto, pequeño y versionado. `schemaVersion` es el entero `1`; una versión mayor desconocida es incompatible. Propiedades desconocidas pueden conservarse para diagnóstico, pero no cambian semántica ni eluden validación.

Campos lógicos mínimos:

| Campo | Requisito |
|---|---|
| `schemaVersion` | `1` |
| `missionId` | UUID canónico; coincide con la cabecera binaria y la carpeta normalizada |
| `rover.id` / `rover.loggerVersion` | Identificadores no vacíos; no implican compatibilidad por nombre |
| `startedUtc` / `endedUtc` | ISO 8601 UTC; fin opcional si fue interrumpida |
| `timeZone` | ID de zona registrado por el logger |
| `clockAnchors` | Correspondencias monotónico↔UTC con calidad/origen |
| `sourceCrs` | `EPSG:4326` para observaciones GPS v1, o valor explícito validable |
| `sensors` | ID, tipo, modelo declarado, frecuencia nominal, unidades y adaptador |
| `files` | Ruta, tamaño, SHA-256 en minúsculas y tipo MIME interno |
| `calibrations` | Rutas/versiones realmente usadas; no valores implícitos |
| `completion` | Cierre limpio, interrumpido o desconocido, con razón opcional |
| `dataOrigin` | `synthetic`, `measured` o `mixed`; las pruebas v1 usan `synthetic` |

Ejemplo mínimo **sintético**, no procedente de hardware:

```json
{
  "schemaVersion": 1,
  "missionId": "00000000-0000-0000-0000-000000000001",
  "rover": {
    "id": "synthetic-rover",
    "loggerVersion": "generator-v1"
  },
  "startedUtc": "2026-01-01T00:00:00Z",
  "endedUtc": "2026-01-01T00:10:00Z",
  "timeZone": "UTC",
  "clockAnchors": [
    {
      "monotonicMicroseconds": 0,
      "utcUnixNanoseconds": 1767225600000000000,
      "source": "synthetic-ground-truth"
    }
  ],
  "sourceCrs": "EPSG:4326",
  "sensors": [
    {
      "id": "synthetic-gps-1",
      "type": "gps",
      "model": "SYNTHETIC",
      "nominalFrequencyHz": 5.0,
      "units": ["degree", "meter"],
      "adapter": "synthetic.v1"
    }
  ],
  "files": [
    {
      "path": "telemetry.rvrlog",
      "sizeBytes": 1234,
      "sha256": "<64 lowercase hexadecimal characters>",
      "mediaType": "application/x-roverrender2d-log"
    }
  ],
  "calibrations": [],
  "completion": {
    "state": "clean"
  },
  "dataOrigin": "synthetic"
}
```

Un hash simbólico como el del ejemplo es inválido en un paquete real. `checksums.sha256` usa líneas `hex-digest *relative/path`, ordenadas por ruta, para `mission.json` y archivos de datos/calibración; no se incluye a sí mismo. No se incluye el hash de `mission.json` dentro del propio manifiesto para evitar una referencia circular.

## Primitivas binarias

- Todos los enteros v1 son **little-endian**.
- Los campos firmados usan complemento a dos.
- `byteOrder=1` significa little-endian; cualquier otro valor es incompatible en v1.
- `missionId` contiene los 16 octetos del UUID en orden canónico RFC 4122, no el orden específico de `Guid.ToByteArray()`.
- `createdUnixNanoseconds` y `utcUnixNanoseconds` son nanosegundos desde Unix epoch UTC en `Int64`.
- `monotonicMicroseconds` es `UInt64` desde el origen monotónico de la misión; nunca es hora civil.
- CRC es CRC-32/ISO-HDLC: polynomial `0x04C11DB7` (reflejado `0xEDB88320`), init/xorout `0xFFFFFFFF`, refin/refout verdaderos. El check de `123456789` es `0xCBF43926`.

## `FileHeader` v1

Tamaño fijo: **44 bytes**.

| Offset | Tamaño | Tipo | Campo | Valor/regla v1 |
|---:|---:|---|---|---|
| 0 | 8 | ASCII | `magic` | `RVR2DLOG` |
| 8 | 2 | `UInt16` | `formatVersion` | `1` |
| 10 | 1 | `UInt8` | `byteOrder` | `1` |
| 11 | 1 | `UInt8` | `flags` | `0`; bits reservados |
| 12 | 16 | bytes | `missionId` | UUID canónico |
| 28 | 8 | `Int64` | `createdUnixNanoseconds` | Fecha válida o `0` si desconocida |
| 36 | 4 | `UInt32` | `headerLength` | `44` |
| 40 | 4 | `UInt32` | `headerCrc32` | CRC de bytes `0..39` |

Magic, versión, byte order, longitud o CRC incorrectos son errores fatales del archivo: no se interpretan registros. `flags != 0` es incompatible en v1 porque podría cambiar el framing.

Vector de cabecera con UUID y fecha en cero. Los primeros 40 bytes son:

```text
52 56 52 32 44 4C 4F 47 01 00 01 00 00 00 00 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
00 00 00 00 2C 00 00 00
```

CRC calculado: `0x837057C3`; almacenado little-endian como `C3 57 70 83`. Es un vector de framing: una misión real no usa UUID cero.

## `RecordEnvelope` v1

Cada registro ocupa `32 + payloadLength + 4` bytes. Los últimos 4 bytes son el CRC del payload y no forman parte de la cabecera fija.

| Offset | Tamaño | Tipo | Campo | Valor/regla v1 |
|---:|---:|---|---|---|
| 0 | 4 | `UInt32` | `syncWord` | `0x32525652`; bytes `52 56 52 32` (`RVR2`) |
| 4 | 1 | `UInt8` | `recordVersion` | `1` |
| 5 | 1 | `UInt8` | `recordType` | Registro de tipos siguiente |
| 6 | 2 | `UInt16` | `flags` | `0`; bits reservados |
| 8 | 4 | `UInt32` | `sequence` | Secuencia ascendente módulo `2^32` |
| 12 | 8 | `UInt64` | `monotonicMicroseconds` | Tiempo monotónico de captura |
| 20 | 8 | `Int64` | `utcUnixNanoseconds` | `0` si no existe ancla UTC por registro |
| 28 | 4 | `UInt32` | `payloadLength` | `0..16,777,216` antes de reservar |
| 32 | N | bytes | `payload` | Protobuf canónico y versionado |
| 32+N | 4 | `UInt32` | `payloadCrc32` | CRC únicamente de los N bytes de payload |

Registro de tipos v1:

| ID | Tipo canónico | Observación |
|---:|---|---|
| 1 | GPS | Fix, calidad y covarianza disponibles en el contrato, no inferidas |
| 2 | LiDAR 2D | Scan canónico; modelo angular debe validarse |
| 3 | IMU | Muestra y estado de calibración explícitos |
| 4 | Odometría de rueda | Opcional; ausencia permitida |
| 5 | Evento del rover/logger | Evento tipado, no texto ejecutable |
| 6 | Marcador visual | Opcional; no implica soporte de una cámara concreta |

Los payloads son mensajes Protobuf v1 propios de RoverRender2D. Un driver traduce del protocolo físico hacia estos mensajes y conserva identificador/origen. Protobuf no reemplaza la validación semántica: se comprueban valores finitos, rangos, unidades, cardinalidades y campos requeridos por cada tipo.

Ejemplo de cabecera de registro: evento tipo 5, secuencia 1, `1,000,000 µs`, UTC desconocida y payload vacío:

```text
52 56 52 32 01 05 00 00 01 00 00 00 40 42 0F 00
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
```

El CRC de payload vacío es `00 00 00 00`. Este vector solo valida el **framing**; el validador semántico de `EventV1` debe rechazar un evento vacío.

## Evolución y campos desconocidos

| Situación | Tratamiento |
|---|---|
| `formatVersion` de archivo desconocida | Fatal: no abrir como v1 |
| `recordVersion` desconocida con longitud/CRC válidos | Omitir registro y reportar error recuperable |
| `recordType` desconocido con longitud/CRC válidos | Omitir, conservar métricas y reportar advertencia recuperable |
| Flags reservados de registro no cero | Omitir el registro y reportar error recuperable |
| Campos Protobuf desconocidos | Ignorar según reglas Protobuf; conservar cuando se reserialice sin transformación |
| Campo conocido inválido | Rechazar payload; continuar si el framing es confiable |
| Salto/duplicado de secuencia | Registrar incidencia de calidad; no renumerar |

Una evolución compatible añade campos opcionales o nuevos tipos. Cambiar unidades, significado, framing o campos obligatorios requiere versión mayor. El escritor nunca emite una versión que el lector activo no pueda validar.

## Corrupción y resincronización

1. Leer exactamente 32 bytes de cabecera sin reservar payload.
2. Validar sync, versión, flags, tipo conocido/desconocido, longitud y overflow.
3. Leer el payload de forma acotada/pooled y sus 4 bytes de CRC.
4. Comparar CRC antes de deserializar.
5. Ante framing inválido o truncamiento, buscar la secuencia little-endian `52 56 52 32` hacia delante hasta un máximo de **4 MiB**.
6. Aceptar un candidato solo si su cabecera es plausible, la longitud cabe y su payload completo pasa CRC.
7. Si no se encuentra candidato dentro del límite, detener ese archivo con un error accionable. Nunca realizar una búsqueda ilimitada ni ocultar bytes perdidos.

Cada recuperación registra offset inicial/final, bytes omitidos, secuencias afectadas y código de error. Un CRC correcto prueba integridad accidental del frame, no autenticidad.

## Validación del paquete

Orden recomendado:

1. inspeccionar entrada y resolver rutas sin seguir enlaces;
2. comprobar tamaño del manifiesto y parsear JSON con límites;
3. validar versión, UUID, tiempos, sensores declarados y lista de archivos;
4. comprobar espacio libre y tamaños declarados con aritmética segura;
5. calcular SHA-256 por streaming y cotejar manifiesto/checksums;
6. copiar a un temporal local, revalidar y promover atómicamente a `source/`;
7. abrir cabecera/log e indexar registros por offset, sin cargar todo el archivo;
8. producir un informe que distinga fatal, recuperable y advertencia.

El origen permanece inmutable. Los índices, checkpoints, parámetros y resultados se guardan fuera de `source/` con hashes que identifican exactamente entrada y configuración.

## Pruebas de conformidad

- Vectores de cabecera y CRC anteriores en todas las implementaciones.
- UUID en orden RFC 4122, timestamps límite y byte order incorrecto.
- Payload en `0`, exactamente 16 MiB y 16 MiB + 1.
- Truncamiento en cada campo, header CRC incorrecto y payload CRC incorrecto.
- Sync falso dentro de un payload corrupto y candidato con longitud maliciosa.
- Resincronización a 4 MiB y fallo a 4 MiB + 1.
- Versiones/tipos/flags desconocidos según la tabla de evolución.
- Paths absolutos, `..`, reparse point, checksum incorrecto y retiro simulado del medio.
- Resultado determinista del generador para semilla/configuración idénticas.
