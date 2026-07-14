# ADR-0004: WGS 84 original, CRS proyectado explícito y marco local

- Estado: Aceptada
- Fecha: 2026-07-13
- Fases: 1–4

## Contexto

GPS entrega coordenadas geográficas, mientras que distancias, fusión, renderizado y CAD necesitan unidades métricas. Coordenadas UTM grandes pueden degradar cálculos de pantalla si se convierten prematuramente a precisión reducida. La ubicación general sugiere una zona, pero faltan coordenadas/puntos de control confirmados.

## Decisión

- Conservar cada observación GPS original en WGS 84 junto con calidad y procedencia.
- Transformar a un CRS proyectado configurable y validado antes de medir o exportar.
- Tratar `EPSG:32616` (WGS 84 / UTM 16N) solo como candidato para Finca Ramírez, no como valor silencioso.
- Usar metros, `X=Este`, `Y=Norte`, yaw cero al Este y positivo antihorario; el núcleo usa radianes.
- Crear un marco local restando un origen proyectado `(E0,N0)` conservado en doble precisión para render/cálculo estable.
- Incluir CRS, datum, zona, unidades y origen local en proyectos, reportes y exportaciones.

Consulta [../coordinate-systems.md](../coordinate-systems.md) para fórmulas y pruebas.

## Consecuencias

Positivas:

- medidas y exportaciones tienen semántica explícita;
- se conserva la observación original y puede reprocesarse;
- viewport estable sin perder el offset georreferenciado;
- errores de grados/metros y ejes pueden prevenirse con tipos.

Costos:

- requiere biblioteca/procedimiento de proyección evaluado y vectores conocidos;
- proyectos sin CRS confirmado no pueden producir distancias georreferenciadas confiables;
- transformaciones, covarianzas y extrínsecos exigen metadatos y pruebas adicionales.

## Alternativas descartadas

- **Operar directamente en latitud/longitud:** grados no son una métrica cartesiana uniforme.
- **Hardcodear UTM 16N:** una misión fuera de zona o con otro datum produciría errores silenciosos.
- **Descartar UTM y guardar solo coordenadas locales:** rompe trazabilidad y exportación georreferenciada.
- **Usar coordenadas proyectadas grandes directamente en float de pantalla:** aumenta pérdida de precisión visual.
