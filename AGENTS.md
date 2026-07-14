# Instrucciones para agentes

Estas reglas aplican a todo el repositorio salvo que un archivo `AGENTS.md` más cercano imponga reglas adicionales.

## Prioridades

1. Preserva el trabajo existente del usuario y de otros agentes. Inspecciona antes de editar y limita cada cambio al objetivo activo.
2. Entrega incrementos verticales que compilen, tengan pruebas y documentación. No sustituyas una implementación solicitada por pseudocódigo.
3. Mantén el producto totalmente offline. La UI y la documentación nunca deben insinuar telemetría en vivo.
4. No inventes compatibilidad, tramas, calibraciones ni rendimiento de sensores físicos. Hasta recibir evidencia real, usa adaptadores explícitamente no implementados y misiones sintéticas identificadas.
5. No presentes resultados como levantamiento certificado, límite legal de finca o diseño hidráulico final.

## Límites arquitectónicos

- Respeta la dirección `Domain <- Application <- adaptadores`. Las capas exteriores pueden depender de puertos internos; el dominio nunca depende de WinForms, SQLite, Protobuf, renderizado o CAD.
- `Contracts` representa datos de disco y transporte. Valida y convierte de forma explícita antes de crear entidades de dominio.
- Mantén IO, UI, procesamiento y exportación cancelables. No bloquees el hilo de interfaz.
- Procesa logs y nubes por streaming con backpressure. No cargues una misión completa en memoria.
- Usa tipos con unidades y convenciones claras. En el núcleo: metros, segundos y radianes; X/Este, Y/Norte y yaw positivo antihorario.
- Conserva entradas crudas inmutables. Las correcciones son revisiones derivadas, trazables y reversibles.
- Centraliza versiones de paquetes y documenta licencia, mantenimiento y razón antes de añadir una dependencia. No introduzcas dependencias nativas grandes sin ADR.

## Seguridad de datos

- Trata la microSD, el manifiesto y cada payload como entrada hostil.
- Rechaza rutas absolutas, `..`, enlaces inesperados, longitudes fuera de rango, overflow y reservas desproporcionadas.
- Prohíbe `BinaryFormatter` y la deserialización polimórfica insegura.
- Calcula hashes y CRC por streaming; escribe resultados mediante temporal y reemplazo atómico.
- No ejecutes contenido de una misión, no escribas en la microSD y no registres payloads crudos completos.
- Nunca publiques coordenadas o misiones reales sin autorización; ofrece anonimización.

## Calidad del código

- Código y nombres técnicos en inglés; interfaz y documentación de usuario en español.
- Habilita nullability, análisis estático y advertencias como errores en CI.
- Prefiere funciones pequeñas, dependencias explícitas y resultados tipados sobre estado global o excepciones usadas como control de flujo.
- Propaga `CancellationToken` y reporta progreso con métricas útiles.
- El resultado debe ser determinista para las mismas entradas, configuración y versión.
- Añade códigos de error estables y mensajes accionables; no expongas solamente un stack trace.

## Validación mínima

Antes de declarar terminado un cambio de código, ejecuta lo aplicable:

```powershell
dotnet format --verify-no-changes
dotnet build -c Release
dotnet test -c Release
```

Para contratos, parsing, proyección, geometría y algoritmos, añade vectores deterministas y casos de límites. Para DXF/PDF, valida estructura y round trip; para UI, prueba que cancelación y operaciones largas no bloqueen el hilo.

No ocultes pruebas fallidas ni afirmes que una validación se ejecutó si no fue posible. Documenta el comando, resultado y motivo de cualquier omisión.

## Git y documentación

- Usa ramas `feat/`, `fix/`, `docs/` o `chore/` y Conventional Commits.
- Nunca uses `git reset --hard`, `git clean -fd`, `git checkout -- .`, force push ni reescritura destructiva sin permiso explícito.
- Mantén commits pequeños; no mezcles refactors ajenos con una funcionalidad.
- Actualiza `README.md`, `CHANGELOG.md`, el backlog y los ADR cuando cambien comportamiento, contrato o decisiones.
- No agregues una licencia hasta que el propietario la seleccione.
