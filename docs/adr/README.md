# Registros de decisiones de arquitectura

Un ADR registra una decisión que cambia límites, contrato, datos o comportamiento difícil de revertir. Se conserva aunque sea reemplazado; una decisión nueva la marca como `Reemplazada por ADR-xxxx`.

| ADR | Estado | Decisión |
|---|---|---|
| [0001](0001-modular-monolith.md) | Aceptada | Monolito modular con dependencias hacia el dominio |
| [0002](0002-winforms-desktop-ui.md) | Aceptada | UI Windows Forms offline y lógica de presentación separada |
| [0003](0003-versioned-mission-contract.md) | Aceptada | Paquete de misión v1 y log secuencial recuperable |
| [0004](0004-coordinate-reference-systems.md) | Aceptada | WGS 84 original, CRS proyectado explícito y marco local |
| [0005](0005-derived-workspace-storage.md) | Aceptada | Fuente inmutable y SQLite para estado derivado |

Estado permitido: `Propuesta`, `Aceptada`, `Rechazada`, `Obsoleta` o `Reemplazada`.
