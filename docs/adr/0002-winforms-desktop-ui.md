# ADR-0002: Windows Forms para la experiencia de escritorio

- Estado: Aceptada
- Fecha: 2026-07-13
- Fase: 1

## Contexto

El producto está destinado a PCs Windows y el requisito de plataforma especifica C#, .NET 10 y Windows Forms. La UI debe importar/visualizar misiones grandes sin bloquearse y comunicar claramente que los datos son grabados, no telemetría en vivo.

## Decisión

Usar Windows Forms en `RoverRender2D.Desktop` con `net10.0-windows` y plataforma principal x64.

- La UI será una shell minimalista en español: inicio, importación, calidad, procesamiento, mapa, Replay, exportación y configuración.
- Casos de uso, presenters/view models y estados se mantienen fuera de formularios para pruebas sin WinForms.
- IO/procesamiento/exportación son asíncronos, cancelables y reportan progreso; el hilo de UI solo actualiza controles.
- El lienzo consume una abstracción de escena/viewport en `Rendering`, con origen local, culling y LOD.
- SkiaSharp es un backend candidato, no parte irrevocable de esta decisión; su versión/licencia/distribución se evaluarán antes de incorporarlo.
- La UI usa pocos colores, alto contraste, DPI escalable y etiquetas explícitas `Misión grabada`/`Replay`.

## Consecuencias

Positivas:

- cumple la plataforma solicitada y aprovecha herramientas de escritorio instaladas;
- facilita integración con selector de carpetas, unidades y flujo local;
- permite una entrega única sin navegador o servidor.

Costos:

- producto limitado a Windows;
- WinForms no impone por sí solo separación de presentación ni render de millones de puntos;
- DPI, accesibilidad y marshaling al hilo de UI requieren pruebas específicas.

## Alternativas descartadas

- **Web/Electron/servidor local:** aumenta superficie y puede sugerir un producto conectado.
- **WPF/WinUI/Avalonia:** opciones válidas, pero no cumplen la elección explícita de WinForms para este MVP y añadirían migración antes de validar el dominio.
- **Dibujar toda la nube con controles estándar:** no ofrece presupuesto de frame, LOD ni precisión adecuados.
