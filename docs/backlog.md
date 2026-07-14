# Backlog de RoverRender2D

Última revisión: **2026-07-13**. El orden favorece incrementos verticales. Un gate solo se cierra con evidencia reproducible; completar tareas aisladas no equivale a completar la fase.

## Estado por fases

| Fase | Resultado esperado | Estado inicial | Gate de salida |
|---|---|---|---|
| 0. Auditoría y repositorio | Entorno, remoto, preguntas y backlog verificados | Parcial: autenticación de `gh` pendiente | Repositorio limpio, remoto correcto, autenticación funcional y diagnóstico actualizado |
| 1. Bootstrap compilable | Solución modular, configuración central, DI/logging, shell WinForms, pruebas y CI | En curso | `dotnet build -c Release` y `dotnet test -c Release` pasan; la aplicación abre sin errores |
| 2. Contrato e importación | Manifiesto y log v1, payloads versionados, streaming, generador sintético e importación segura | Pendiente | Una misión válida importa; la corrupción se detecta y se recupera de forma acotada; memoria no crece linealmente |
| 3. Índice, tiempo y calidad | Índice SQLite, sincronización, métricas, checkpoints y Replay crudo | Pendiente | Una misión sintética de 60 minutos se indexa, cancela y reanuda |
| 4. Trayectoria y fusión | Proyección, filtros, scan matching, estimador 2D y métricas | Pendiente | Ground truth demuestra error medido y mejora frente a fuentes aisladas |
| 5. Mapa y edición | Nube con LOD, elementos, límites, herramientas y revisiones | Pendiente | Una parcela completa se revisa y corrige sin bloquear la UI |
| 6. Replay completo | Timeline, velocidades, eventos, scan actual y prefetch | Pendiente | Replay largo estable, sincronizado y de memoria acotada |
| 7. DXF y PDF | Capas CAD, CRS/unidades, round trip, lámina y reporte | Pendiente | AutoCAD conserva medidas/capas y el PDF no recorta contenido técnico |
| 8. Endurecimiento y release | Benchmarks, memoria, distribución, manual y release | Pendiente | CI verde, instalación limpia y muestra end-to-end con limitaciones documentadas |

## Próximos incrementos

### Fase 0

- [x] Registrar herramientas y versiones sin información sensible.
- [x] Confirmar que el repositorio remoto partió vacío y fue clonado.
- [ ] Completar `gh auth login --hostname github.com --git-protocol https --web` por el propietario.
- [ ] Confirmar remoto, worktree y CI después del primer push.
- [ ] Resolver las preguntas de hardware y publicación listadas abajo.

### Fase 1

- [ ] Crear los proyectos con dependencias dirigidas al dominio y una prueba de arquitectura.
- [ ] Fijar SDK, paquetes, estilo, lock files y builds reproducibles.
- [ ] Configurar host, opciones y logging local rotativo sin datos crudos.
- [ ] Implementar un shell WinForms mínimo en español que comunique claramente el flujo offline.
- [ ] Añadir pruebas mínimas, CI de Windows y prueba de humo documentada.
- [ ] Cerrar los ADR iniciales y verificar que README y changelog coincidan con el producto real.

### Fase 2

- [ ] Congelar el contrato sintético/de referencia v1 descrito en [data-contract.md](data-contract.md).
- [ ] Definir mensajes Protobuf canónicos sin confundirlos con protocolos físicos.
- [ ] Implementar lector/escritor por streaming, CRC, límites y resincronización acotada.
- [ ] Crear `SyntheticMissionGenerator` determinista con corrupción inyectable y ground truth.
- [ ] Descubrir paquetes con profundidad limitada y validar manifiesto, rutas, tamaños y SHA-256.
- [ ] Comprobar espacio, copiar de forma reanudable y mantener la microSD inmutable.
- [ ] Probar paquete válido, versión incompatible, checksum incorrecto, truncamiento, cancelación y retiro del medio.

### Fases 3–8

- [ ] Convertir cada resultado de la tabla en historias pequeñas con criterio de aceptación y dataset.
- [ ] Definir métricas y presupuestos de memoria antes de optimizar algoritmos.
- [ ] Registrar ADR adicionales para fusión, renderizado definitivo y exportación DXF/PDF antes de implementarlos.
- [ ] Mantener trazabilidad entre entrada, parámetros, versión, commit, revisiones y cada exportación.

## Información requerida del propietario

Estas respuestas **no bloquean el bootstrap**. Mientras falten, solo se usarán contratos de referencia, adaptadores `NotImplemented` explícitos y datos sintéticos identificados.

1. **LiDAR:** marca y modelo exactos del LiDAR 360°, frecuencia de giro, resolución angular, alcance y manual del protocolo.
2. **GPS:** modelo exacto, sentencias NMEA o protocolo binario disponibles, frecuencia, campos de calidad, HDOP y tipos de fix.
3. **IMU:** modelo, orientación física en el rover, procedimiento/archivos de calibración y frecuencia.
4. **Encoders:** ¿la versión final tendrá encoders de rueda? Si sí, indicar modelo, resolución, geometría y protocolo.
5. **Muestras:** aportar ejemplos autorizados de una misión y de tramas crudas reales, anonimizados si es necesario, con firmware y configuración.
6. **Volumen:** duración máxima esperada de una misión y capacidad/tamaño típico de la microSD.
7. **Exportación:** versión objetivo de AutoCAD y tamaño de lámina preferido entre A3, A2 y A1.
8. **Referencia espacial:** polígono de geocerca, procedencia y precisión de puntos de control disponibles.
9. **Publicación:** confirmar si el repositorio será público o privado y elegir una licencia (por ejemplo MIT, Apache-2.0, GPL u otra) después de revisar sus implicaciones.

Además, si se pretende integrar HuskyLens o una cámara, se necesita modelo, firmware, modo/protocolo, tasa, sistema de coordenadas de detección, calibración/extrínsecos y muestras sincronizadas. No se asume que esa fuente esté presente en el MVP.

## Criterio transversal de terminado

Cada incremento debe incluir:

- comportamiento implementado y límites explícitos;
- pruebas deterministas y resultado de build en Release;
- cancelación, errores y seguridad de entrada cuando apliquen;
- documentación y ADR actualizados;
- evidencia que distingue datos medidos, estimados, sintéticos y editados;
- un commit pequeño y un PR con riesgos, una vez esté disponible la autenticación.
