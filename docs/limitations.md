# Limitaciones

Este documento forma parte del resultado técnico y debe acompañar demostraciones y exportaciones relevantes.

## Estado actual

- RoverRender2D está en desarrollo; no existe una release estable ni un instalador validado.
- El contrato v1 es sintético/de referencia. No se ha declarado compatible ningún LiDAR, GPS, IMU, encoder, HuskyLens o cámara físico.
- Faltan muestras reales, manuales, calibraciones y características exactas del rover. Los adaptadores físicos deben permanecer no implementados hasta completar la [lista de integración](sensor-integration-checklist.md).
- UTM 16N (`EPSG:32616`) es un CRS candidato para la ubicación general, no una selección confirmada para todas las misiones.
- Los objetivos de memoria, precisión, FPS y tiempo son metas iniciales; requieren benchmarks y datasets representativos.

## Alcance del producto

- El procesamiento es diferido y totalmente offline. No hay telemetría en vivo, nube, servidor web, Starlink ni control del rover.
- El producto genera una **base planimétrica 2D revisable**. No es un levantamiento topográfico/catastral certificado ni prueba de límites legales.
- No realiza diseño hidráulico final ni calcula automáticamente caudales, bombas, diámetros o pérdidas de carga.
- No exporta DWG nativo. La exportación prevista es DXF, PDF y formatos auxiliares documentados.
- No produce una reconstrucción 3D o curvas de nivel fiables sin altimetría suficiente y validada.

## Sensores y algoritmos

- La precisión final está limitada por calidad/visibilidad GPS, geometría LiDAR, calibración extrínseca, sincronización, movimiento, vegetación y observabilidad de la parcela.
- Filas repetitivas de cultivo pueden causar mínimos falsos en scan matching. La confianza y los rechazos deben mostrarse; no garantizan eliminar toda asociación incorrecta.
- Curso GPS no equivale siempre a orientación del rover. Bajo copa, multipath y pérdida de fix pueden degradar la trayectoria.
- IMU y encoders son opcionales; no se sintetizan lecturas para ocultar su ausencia.
- La detección de árboles/obstáculos es una hipótesis con confianza. Maleza, oclusión y resolución del sensor pueden cambiar falsos positivos y negativos.
- Un círculo ajustado no prueba que un grupo de puntos sea un tronco. La clasificación requiere revisión y, si se pretende automatizar, un dataset validado.
- La fusión y optimización reducen error estimado, pero no convierten observaciones de baja calidad en verdad de terreno.

## Coordenadas y exportación

- Distancias y áreas solo son válidas después de confirmar CRS, datum, zona, unidades y origen.
- Una geocerca proporcionada, puntos de control o un polígono editado deben indicar procedencia y precisión. Una envolvente de nube nunca se declara límite legal.
- DXF/PDF pueden variar entre visores. La compatibilidad debe probarse contra la versión exacta de AutoCAD y tamaño de hoja elegidos por el propietario.
- Las normas de dibujo citadas orientan principios de presentación; la implementación no sustituye una revisión profesional ni reproduce contenido protegido de normas.
- Una escala solicitada que no cabe debe cambiarse con confirmación; el contenido no puede recortarse silenciosamente.

## Datos, seguridad y recuperación

- CRC detecta corrupción accidental; no autentica el origen. SHA-256 verifica igualdad con el manifiesto, no la confiabilidad del autor.
- La resincronización binaria está acotada y puede detenerse si el daño supera el límite; algunos registros podrían ser irrecuperables.
- Retirar una microSD o interrumpir energía puede impedir una importación completa. El original no se repara ni modifica automáticamente.
- La aplicación valida entradas, pero ningún parser complejo se considera invulnerable. Se mantienen límites, actualización de dependencias y pruebas con entradas malformadas.
- Los datos permanecen locales por diseño, pero una exportación o copia manual puede revelar coordenadas. La anonimización requiere una acción explícita y verificación.

## Interpretación de resultados

Cada reporte/exportación debe diferenciar:

- **medido:** capturado por una fuente identificada;
- **estimado:** producido por un algoritmo con parámetros/confianza;
- **sintético:** generado para desarrollo/pruebas;
- **editado:** corrección manual con revisión y autoría.

Si la procedencia, calibración, CRS o incertidumbre esencial es desconocida, el sistema debe advertirlo y bloquear únicamente las salidas cuya interpretación sería engañosa. No debe inventar valores para completar una lámina.

## Decisiones pendientes

La versión de AutoCAD, hoja preferida, sensores exactos, encoders, geocerca/puntos de control, duración máxima de misión, visibilidad del repositorio y licencia están pendientes. Consulta [backlog.md](backlog.md). No hay licencia incluida hasta que el propietario la elija.
