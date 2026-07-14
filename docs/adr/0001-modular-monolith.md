# ADR-0001: Monolito modular con dependencias hacia el dominio

- Estado: Aceptada
- Fecha: 2026-07-13
- Fase: 1

## Contexto

RoverRender2D combina importación segura, procesamiento numérico, renderizado, edición y exportación en una aplicación de escritorio offline. Se necesita separar responsabilidades y probar algoritmos sin UI, pero el MVP no requiere despliegues independientes, red interna ni operación de servicios.

## Decisión

Construir una sola solución/despliegue como monolito modular con proyectos `Domain`, `Application`, `Contracts`, `Infrastructure`, `Processing`, `Rendering`, `Export` y `Desktop`.

- `Domain` contiene reglas y tipos con unidades, sin dependencias exteriores.
- `Application` coordina casos de uso mediante puertos, progreso y cancelación.
- Los módulos exteriores implementan IO, algoritmos, presentación y formatos.
- `Contracts` modela datos no confiables/versionados y no sustituye al dominio.
- Se prohíben ciclos y se validan referencias con pruebas de arquitectura.
- Las herramientas, pruebas y benchmarks consumen los mismos límites públicos sin acoplarse a WinForms.

## Consecuencias

Positivas:

- despliegue y depuración simples para una aplicación offline;
- algoritmos y contratos comprobables sin iniciar la UI;
- dependencias nativas/CAD no contaminan el dominio;
- permite extraer un módulo futuro solo si aparece una necesidad demostrada.

Costos:

- más proyectos y conversiones explícitas;
- disciplina necesaria para evitar que `Desktop` o `Infrastructure` se conviertan en dependencias globales;
- una sola versión de producto, aunque los contratos se versionan por separado.

## Alternativas descartadas

- **Un único proyecto WinForms:** mezcla IO, UI y algoritmos; dificulta pruebas y evolución.
- **Microservicios o servidor local:** agregan red, despliegue, fallos y superficie de seguridad sin beneficio para el flujo offline.
- **Plugins dinámicos desde la microSD:** contradicen el modelo de entrada no confiable; ningún archivo de misión se ejecuta.
