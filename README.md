# Maze Spectre

Juego de laberinto para consola desarrollado en C# con .NET 8 y Spectre.Console. Permite jugar en modo individual contra una inteligencia artificial o en modo local para dos jugadores. Cada partida genera un laberinto aleatorio con trampas y personajes con habilidades limitadas.

## Características

- Generación procedural del laberinto.
- Comprobación de que exista una ruta desde el inicio hasta la salida.
- Modo jugador contra IA y modo jugador contra jugador en el mismo equipo.
- Búsqueda de rutas mediante A* y heurística de Manhattan.
- Verificación de alcanzabilidad mediante búsqueda en profundidad (DFS).
- Tres categorías de trampas: teletransporte, intercambio de posiciones y retroceso.
- Cinco personajes con habilidades especiales y un máximo de tres usos por habilidad.
- Interfaz de terminal construida con Spectre.Console, con tablero táctico, panel de turno, resumen de jugadores, habilidades, leyenda y controles.
- Representación visual diferenciada para muros, caminos, jugadores, salida y las tres categorías de trampas.
- Diseño con colores de alto contraste y paneles compactos para facilitar la lectura durante la partida.

## Requisitos

- .NET SDK 8.0.
- Acceso a NuGet para restaurar el paquete `Spectre.Console`.

## Ejecución

Desde la carpeta que contiene `MazeSpectre.csproj`, ejecuta:

```bash
dotnet restore
dotnet build
dotnet run
```

Para ejecutar el proyecto desde otra ubicación:

```bash
dotnet run --project MazeSpectre.csproj
```

## Controles

Los controles de movimiento son iguales para ambos jugadores:

| Tecla | Acción |
| --- | --- |
| `W` | Mover hacia arriba |
| `A` | Mover hacia la izquierda |
| `S` | Mover hacia abajo |
| `D` | Mover hacia la derecha |

La tecla de habilidad depende del personaje seleccionado:

| Personaje | Tecla | Habilidad |
| --- | --- | --- |
| `P1` | `T` | Teletransportarse a una celda libre aleatoria. |
| `P2` | `T` | Avanzar una celda por la ruta más corta hacia la salida. |
| `P3` | `B` | Intercambiar posiciones con el otro jugador. |
| `P4` | `R` | Aturdir al rival durante dos de sus turnos. |
| `P5` | `P` | Colocar una trampa normal en una celda libre. |

Cada habilidad tiene un máximo de tres usos por partida. Los movimientos que chocan con una pared no desplazan al jugador.

## Trampas

- **Trampa normal:** teletransporta al jugador que la activa a una celda libre aleatoria.
- **Trampa de intercambio:** intercambia las posiciones de los jugadores.
- **Trampa de retroceso:** intenta desplazar al jugador hasta tres celdas por caminos transitables.

Las trampas se consumen al activarse. La colocación evita las paredes, la salida y las posiciones ocupadas por otras trampas.

## Estructura del proyecto

```text
Maze-Spectre-main/
├── .vscode/
│   ├── launch.json
│   └── tasks.json
├── GameManager.cs
├── MazeLogic.cs
├── Player.cs
├── Trap.cs
├── MazeSpectre.csproj
├── Informe del Proyecto.docx
└── README.md
```

- **`GameManager.cs`**: selección de personajes, bucle de turnos, controles, presentación del tablero y condición de victoria.
- **`MazeLogic.cs`**: generación del laberinto, colocación de trampas, búsqueda de caminos y comprobación de solubilidad.
- **`Player.cs`**: habilidades de los personajes y control de sus usos disponibles.
- **`Trap.cs`**: detección y aplicación de los efectos de las trampas.
- **`MazeSpectre.csproj`**: configuración de compilación y dependencias.

## Decisiones técnicas

### Generación y solubilidad

El laberinto se genera mediante un procedimiento aleatorio basado en Prim. Después de generarlo, el programa verifica si existe una ruta desde la entrada hasta la salida. La búsqueda de solubilidad utiliza DFS.

### Búsqueda de caminos

La IA utiliza A*. El coste de cada movimiento es uniforme y la heurística de Manhattan estima la distancia restante hasta la salida. En estas condiciones, A* encuentra una ruta de coste mínimo si existe un camino.

### Interfaz de terminal

Spectre.Console presenta el menú de selección, las tarjetas de personajes y la pantalla de partida. Durante el juego, el tablero se muestra junto a un panel lateral con el turno actual, los jugadores, las habilidades, una leyenda de símbolos y los controles.

La representación utiliza colores consistentes para diferenciar a ambos jugadores y los elementos del escenario. Los muros y caminos tienen tratamientos visuales distintos; la salida y cada tipo de trampa cuentan con identificadores propios. La interfaz se mantiene en la capa de presentación y no sustituye la lógica de movimiento, generación o búsqueda de rutas.

Para una distribución legible, se recomienda ejecutar el juego en una terminal con un ancho aproximado de 100 columnas o superior. Windows Terminal y terminales modernas con soporte ANSI ofrecen la mejor presentación.

## Limitaciones conocidas

- La IA prioriza alcanzar la salida por la ruta más corta; no desarrolla una estrategia competitiva completa contra el jugador humano.
- El retroceso de las trampas se determina mediante una preferencia fija de direcciones, no por el historial real del recorrido.
- La partida está diseñada para una sesión local de consola; no incluye multijugador por red ni persistencia de partidas.

## Informe del proyecto

El documento `Informe del Proyecto.docx` se conserva como material complementario del proyecto.

## Posibles mejoras futuras

- Incorporar niveles de dificultad configurables.
- Ampliar la estrategia de la IA para que considere las trampas y la posición del rival.
- Permitir configurar el tamaño del laberinto y la cantidad de trampas.

Estas mejoras son propuestas de evolución y no funcionalidades incluidas en la versión actual.

## Licencia

El repositorio no declara una licencia de distribución. Antes de reutilizar, modificar o redistribuir el código fuera del ámbito permitido por sus derechos de autor, debe incorporarse una licencia explícita.
