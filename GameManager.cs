 using System;
using System.Collections.Generic;
using System.Threading;
using Spectre.Console;
using System.Text;

class GameManager
{
    public static int width = 15;
    public static int height = 15;
    public static int[,] maze = new int[height, width];
    public static List<Trap> traps = new List<Trap>();
    public static List<Trap> swapTraps = new List<Trap>();
   
public static List<Trap> knockbackTraps = new List<Trap>();

    public static int player1StunTurns = 0;
    public static int player2StunTurns = 0;
    public static bool isAIPlayer2 = false;

   public static List<Player> players = new List<Player>
{
    new Player("P1", "Puede teletransportarse a una posición aleatoria del laberinto. Tecla: T", "Guerrero desconocido de un lugar lejano a las afuera del reino"),
    new Player("P2", "Puede moverse un paso más cerca de la salida usando la teletransportación. Tecla: T", "Arquero preciso perteneciente a la artillería del reino que busca más aventura como forma de mejorar su vida"),
    new Player("P3", "Puede intercambiar su posición con la del otro jugador. Tecla: B", "Asesino que deserta de su clan se esconde en uno de los gremios mercenarios y terminó por giros del destino uniéndose a esta peligrosa aventura"),
    new Player("P4", "Puede aturdir al otro jugador durante 2 turnos. Tecla: R", "Mago de gran prestigio en busca de lo desconocido y recompensas para continuar con su investigación"),
    new Player("P5", "Puede colocar trampas aleatorias por el tablero. Tecla: P", "Guerrero desconocido de un lugar lejano a las afuera del reino")
};

    static Player selectedPlayer1 = new Player("Placeholder", "Placeholder", "Placeholder");
    static Player selectedPlayer2 = new Player("Placeholder", "Placeholder", "Placeholder");

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        ShowPlayerSelectionMenu();
        ShowAdventureStory();
        player1StunTurns = 0;
        player2StunTurns = 0;
        do
        {
            MazeLogic.InitializeMaze();// Inicializa el laberinto
           
            MazeLogic.GenerateMaze(1, 1); // Genera el laberinto mediante Prim
        } while (!MazeLogic.IsSolvable()); // Asegura que el laberinto sea solucionable

        MazeLogic.PlaceTraps(Random.Shared); // Coloca las tres categorías de trampas sin solapamientos
        StartGame(); // Inicia el juego
    }
     // Permite seleccionar los jugadores antes de empezar el juego
static void ShowPlayerSelectionMenu()
{
  //  PlaySound(@"E:\maze en consola\sounds\sound_main_menu.wav");
    AnsiConsole.Clear();
    AnsiConsole.Write(new Rule("[bold deepskyblue1]MAZE SPECTRE[/]").RuleStyle("grey").Centered());
    AnsiConsole.Write(new FigletText("SELECCION").Centered().Color(Color.Cyan1));
    AnsiConsole.MarkupLine("[grey]Configura la partida antes de entrar al laberinto.[/]");
    AnsiConsole.WriteLine();

    selectedPlayer1 = AnsiConsole.Prompt(
        new SelectionPrompt<Player>()
            .Title("[bold deepskyblue1]Selecciona el personaje del jugador 1[/]")
            .UseConverter(player => $"{player.Name} - {player.PowerDescription}")
            .AddChoices(players));

    AnsiConsole.Write(new Panel($"[bold deepskyblue1]{Markup.Escape(selectedPlayer1.Name)}[/]\n{Markup.Escape(selectedPlayer1.PowerDescription)}")
        .Border(BoxBorder.Rounded).BorderColor(Color.DeepSkyBlue1).Header("[bold]JUGADOR 1[/]"));

    // Pregunta si el segundo jugador es IA
    isAIPlayer2 = AnsiConsole.Confirm("¿Deseas jugar contra la IA?");

    if (!isAIPlayer2)
    {
        selectedPlayer2 = AnsiConsole.Prompt(
            new SelectionPrompt<Player>()
                .Title("[bold deepskyblue1]Selecciona el personaje del jugador 2[/]")
                .UseConverter(player => $"{player.Name} - {player.PowerDescription}")
                .AddChoices(players));

        while (selectedPlayer1 == selectedPlayer2)
        {
            AnsiConsole.MarkupLine("[red]Selecciona dos personajes diferentes.[/]");
            selectedPlayer2 = AnsiConsole.Prompt(
                new SelectionPrompt<Player>()
                    .Title("[bold deepskyblue1]Selecciona el personaje del jugador 2[/]")
                    .UseConverter(player => $"{player.Name} - {player.PowerDescription}")
                    .AddChoices(players));
        }
AnsiConsole.Write(new Panel($"[bold orange1]{Markup.Escape(selectedPlayer2.Name)}[/]\n{Markup.Escape(selectedPlayer2.PowerDescription)}")
            .Border(BoxBorder.Rounded).BorderColor(Color.Orange1).Header("[bold]JUGADOR 2[/]"));
    }
    else
    {
        selectedPlayer2 = new Player("IA", "La IA avanza un paso por turno siguiendo la ruta más corta.", "Un agente automatizado que intenta alcanzar la salida del laberinto.");
        AnsiConsole.Write(new Panel($"[bold orange1]{Markup.Escape(selectedPlayer2.Name)}[/]\n{Markup.Escape(selectedPlayer2.PowerDescription)}")
            .Border(BoxBorder.Rounded).BorderColor(Color.Orange1).Header("[bold]JUGADOR 2 / IA[/]"));
    }
}

    static void ShowAdventureStory() // Muestra una historia antes de iniciar el juego
    {
        AnsiConsole.Write(new Rule("[bold deepskyblue1]INTRODUCCIÓN[/]").RuleStyle("grey").Centered());

        string story = $"{selectedPlayer1.Name} y {selectedPlayer2.Name} se embarcaron en una emocionante aventura. \n\n" +
                       $"{selectedPlayer1.Story}\n\n{selectedPlayer2.Story}\n\n" +
                       "Juntos, enfrentaron los peligros del laberinto y desafiaron las trampas que encontraban a su paso, en busca de la salida y la gloria.";

        string[] storyLines = story.Split('\n');
//efecto narrador
        foreach (string line in storyLines)
        {
            foreach (char c in line)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write(line.Substring(line.IndexOf(c)));
                    break;
                }
                Console.Write(c);
                Thread.Sleep(50);
            }
            Console.WriteLine();
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) continue;
            Thread.Sleep(500);
        }

        Console.WriteLine("\n\nLa partida está a punto de comenzar.");
        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) return;
        Thread.Sleep(2000);
    }

    static void ShowVictoryStory(Player winner)  // Muestra una historia de victoria al finalizar el juego
    {
        string victoryStory = $"Después de muchos desafíos, {winner.Name} finalmente encontró la salida del laberinto. \n\n" +
                              $"{winner.Story}\n\n" +
                              "Con valor y determinación, sobrevivió a todas las trampas y peligros del laberinto, " +
                              "emergiendo como un verdadero héroe. La gloria y las riquezas de la victoria pertenecen a este ganador. La victoria ha sido alcanzada.";

        string[] victoryLines = victoryStory.Split('\n');

        foreach (string line in victoryLines)
        {
            foreach (char c in line)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write(line.Substring(line.IndexOf(c)));
                    break;
                }
                Console.Write(c);
                Thread.Sleep(50);
            }
            Console.WriteLine();
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) continue;
            Thread.Sleep(500);
        }

        Console.WriteLine("\n\nFin de la partida.");
        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter) return;
    }

   static void StartGame() // Maneja el bucle principal del juego y la lógica de los turnos
{
    Random rand = new Random();
    int player1X, player1Y, player2X, player2Y;

    // Definir la distancia mínima a la salida
    int minDistanceToExit = 3;

    do // coloca l jugador uno en una posicion aleatoria
    {
        player1X = rand.Next(1, width - 1);
        player1Y = rand.Next(1, height - 1);
    } while (maze[player1Y, player1X] != 0 || IsNearExit(player1X, player1Y, minDistanceToExit));

    do  // coloca 2 jugador uno en una posicion aleatoria
    {
        player2X = rand.Next(1, width - 1);
        player2Y = rand.Next(1, height - 1);
    } while (maze[player2Y, player2X] != 0 || (player2X == player1X && player2Y == player1Y) || IsNearExit(player2X, player2Y, minDistanceToExit));

    int turns = 0; //contador de turnos
    while (true)
    {
        Console.Clear();
        PrintMazeWithPlayers(player1X, player1Y, player2X, player2Y, turns);

        if (turns % 2 == 0) // Turno del Jugador 1
        {
            if (player1StunTurns > 0)
            {
                player1StunTurns--;
                AnsiConsole.MarkupLine("[yellow]El jugador 1 pierde este turno por aturdimiento.[/]");
                turns++;
                continue;
            }
            {
                var key = Console.ReadKey(true).Key;

                ExecutePlayerAction(key, selectedPlayer1, ref player1X, ref player1Y, ref player2X, ref player2Y, ref player2StunTurns, rand);
                Trap.ActivateTrap(ref player1X, ref player1Y, ref player2X, ref player2Y, rand, selectedPlayer1); // Actualizar esta línea
            }
        }
        else // Turno del Jugador 2
        {
            if (!isAIPlayer2) // Jugador 2 humano
            {
                if (player2StunTurns > 0)
                {
                    player2StunTurns--;
                    AnsiConsole.MarkupLine("[yellow]El jugador 2 pierde este turno por aturdimiento.[/]");
                    turns++;
                    continue;
                }
                {
                    var key = Console.ReadKey(true).Key;

                    ExecutePlayerAction(key, selectedPlayer2, ref player2X, ref player2Y, ref player1X, ref player1Y, ref player1StunTurns, rand);
                    Trap.ActivateTrap(ref player2X, ref player2Y, ref player1X, ref player1Y, rand, selectedPlayer2); // Actualizar esta línea
                }
            }
            else if (turns % 2 != 0) // Jugador 2 controlado por la IA
            {
                if (player2StunTurns == 0)
                {
                   
                    // Movimiento de la IA 
                    if (isAIPlayer2)
                    {
                        MazeLogic.MoveAIPlayerTowardsExit(ref player2X, ref player2Y);              
                         Trap.ActivateTrap(ref player2X, ref player2Y, ref player1X, ref player1Y, rand, selectedPlayer2); // Actualizar esta línea
                   
                    }
                }
            }
        }

        if (player1X == width - 2 && player1Y == height - 2)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow bold]******[/]");
            AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
            AnsiConsole.MarkupLine("[yellow bold]*[/] [green bold] Jugador 1 gana. [/][yellow bold]*[/]");
            AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
            AnsiConsole.MarkupLine("[yellow bold]******[/]");
            AnsiConsole.Write(new Rule("[bold green]VICTORIA[/]").RuleStyle("green").Centered());
            AnsiConsole.Markup("\n[bold green]El jugador 1 ha encontrado la salida del laberinto.[/]\n");
            AnsiConsole.Markup("[bold cyan]Victoria por alcanzar la salida.[/]");
            ShowVictoryStory(selectedPlayer1);
            break;
        }

        if (player2X == width - 2 && player2Y == height - 2)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[yellow bold]******[/]");
            AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
            AnsiConsole.MarkupLine("[yellow bold]*[/] [green bold] Jugador 2 gana. [/][yellow bold]*[/]");
            AnsiConsole.MarkupLine("[yellow bold]*                            *[/]");
            AnsiConsole.MarkupLine("[yellow bold]******[/]");
            AnsiConsole.Write(new Rule("[bold green]VICTORIA[/]").RuleStyle("green").Centered());
            AnsiConsole.Markup("\n[bold green]El jugador 2 ha encontrado la salida del laberinto.[/]\n");
            AnsiConsole.Markup("[bold cyan]Victoria por alcanzar la salida.[/]");
            ShowVictoryStory(selectedPlayer2);
            break;
        }

        turns++;
    }
}
    // Ejecuta la habilidad asignada al personaje; las teclas de movimiento siguen disponibles para todos.
    static void ExecutePlayerAction(
        ConsoleKey key,
        Player player,
        ref int playerX,
        ref int playerY,
        ref int otherPlayerX,
        ref int otherPlayerY,
        ref int otherPlayerStunTurns,
        Random random)
    {
        bool abilityUsed = false;

        switch (player.Name)
        {
            case "P1" when key == ConsoleKey.T:
                player.Teleport(ref playerX, ref playerY, random);
                abilityUsed = true;
                break;
            case "P2" when key == ConsoleKey.T:
                player.TeleportTowardsExit(ref playerX, ref playerY, width - 2, height - 2);
                abilityUsed = true;
                break;
            case "P3" when key == ConsoleKey.B:
                player.SwapPositions(ref playerX, ref playerY, ref otherPlayerX, ref otherPlayerY);
                abilityUsed = true;
                break;
            case "P4" when key == ConsoleKey.R:
                player.StunOtherPlayer(ref otherPlayerStunTurns);
                abilityUsed = true;
                break;
            case "P5" when key == ConsoleKey.P:
                player.PlaceRandomTrap(random);
                abilityUsed = true;
                break;
        }

        if (abilityUsed)
            return;

        switch (key)
        {
            case ConsoleKey.W when playerY > 0 && maze[playerY - 1, playerX] == 0:
                playerY--;
                break;
            case ConsoleKey.S when playerY < height - 1 && maze[playerY + 1, playerX] == 0:
                playerY++;
                break;
            case ConsoleKey.A when playerX > 0 && maze[playerY, playerX - 1] == 0:
                playerX--;
                break;
            case ConsoleKey.D when playerX < width - 1 && maze[playerY, playerX + 1] == 0:
                playerX++;
                break;
        }
    }

 // Determina si una posición está cerca de la salida
static bool IsNearExit(int x, int y, int minDistance) 
   { 
    int exitX = width - 2;
    int exitY = height - 2; 
        return Math.Abs(x - exitX) < minDistance && Math.Abs(y - exitY) < minDistance;
     }
    static void PrintMazeWithPlayers(int player1X, int player1Y, int player2X, int player2Y, int turns)
    {
        AnsiConsole.Clear();

        var activePlayer = turns % 2 == 0 ? selectedPlayer1 : selectedPlayer2;
        var activeColor = turns % 2 == 0 ? "deepskyblue1" : "orange1";
        var header = new Rule($"[bold deepskyblue1]MAZE SPECTRE[/] [grey] / PARTIDA EN CURSO[/]")
            .RuleStyle("grey").Centered();
        AnsiConsole.Write(header);

        var mazeTable = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .Expand()
            .PadLeft(0)
            .PadRight(0);

        for (int x = 0; x < width; x++)
            mazeTable.AddColumn(new TableColumn(string.Empty).NoWrap());

        for (int y = 0; y < height; y++)
        {
            var cells = new List<Markup>();
            for (int x = 0; x < width; x++)
            {
                string cell;
                if (x == player1X && y == player1Y)
                    cell = "[bold black on deepskyblue1] P1 [/]";
                else if (x == player2X && y == player2Y)
                    cell = "[bold black on orange1] P2 [/]";
                else if (x == width - 2 && y == height - 2)
                    cell = "[bold white on green] EX [/]";
                else if (traps.Exists(t => t.Position == (x, y)))
                    cell = "[bold white on purple] T  [/]";
                else if (swapTraps.Exists(t => t.Position == (x, y)))
                    cell = "[bold black on cyan1] X  [/]";
                else if (knockbackTraps.Exists(t => t.Position == (x, y)))
                    cell = "[bold white on red3] K  [/]";
                else if (maze[y, x] == 1)
                    cell = "[grey23 on grey23]    [/]";
                else
                    cell = "[grey50 on grey11] ·  [/]";

                cells.Add(new Markup(cell));
            }
            mazeTable.AddRow(cells.ToArray());
        }

        var mazePanel = new Panel(mazeTable)
            .Header("[bold white]MAPA TÁCTICO[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Grey)
            .Padding(1, 0);

        var status = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .AddColumn(new TableColumn("Jugador").Width(10))
            .AddColumn(new TableColumn("Estado"));

        status.AddRow(
            new Markup("[bold deepskyblue1]P1[/]"),
            new Markup($"[white]{Markup.Escape(selectedPlayer1.Name)}[/]"));
        status.AddRow(
            new Markup("[bold orange1]P2[/]"),
            new Markup($"[white]{Markup.Escape(selectedPlayer2.Name)}{(isAIPlayer2 ? " [IA]" : "")}[/]"));

        var turnPanel = new Panel(
            new Markup($"[bold {activeColor}]{Markup.Escape(activePlayer.Name)}[/]\n[grey]Turno {(turns / 2) + 1}[/]\n\n[white]Es el momento de actuar.[/]"))
            .Header("[bold]TURNO ACTUAL[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(turns % 2 == 0 ? Color.DeepSkyBlue1 : Color.Orange1);

        var abilityTable = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .AddColumn(new TableColumn("Tecla").Width(7))
            .AddColumn(new TableColumn("Habilidad"));

        abilityTable.AddRow($"[bold yellow]{GetPowerKey(selectedPlayer1.Name)}[/]", $"[white]{Markup.Escape(selectedPlayer1.PowerDescription)}[/]");
        abilityTable.AddRow($"[bold yellow]{GetPowerKey(selectedPlayer2.Name)}[/]", $"[white]{Markup.Escape(selectedPlayer2.PowerDescription)}[/]");

        var abilityPanel = new Panel(abilityTable)
            .Header("[bold]HABILIDADES[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Grey);

        var legend = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .AddColumn(new TableColumn("Símbolo").Width(8))
            .AddColumn(new TableColumn("Elemento"));

        legend.AddRow(new Markup("[bold black on deepskyblue1] P1 [/]"), new Markup("Jugador 1"));
        legend.AddRow(new Markup("[bold black on orange1] P2 [/]"), new Markup("Jugador 2"));
        legend.AddRow(new Markup("[bold white on green] EX [/]"), new Markup("Salida"));
        legend.AddRow(new Markup("[bold white on purple] T  [/]"), new Markup("Teletransporte"));
        legend.AddRow(new Markup("[bold black on cyan1] X  [/]"), new Markup("Intercambio"));
        legend.AddRow(new Markup("[bold white on red3] K  [/]"), new Markup("Retroceso"));

        var legendPanel = new Panel(legend)
            .Header("[bold]LEYENDA[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Grey);

        var controls = new Panel(
            new Markup("[bold white]W / A / S / D[/]  Movimiento\n[bold yellow]Habilidad del personaje[/]  Tecla indicada\n[grey]Los movimientos contra muros no se ejecutan.[/]"))
            .Header("[bold]CONTROLES[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Grey);

        var sidePanel = new Rows(turnPanel, new Panel(status).Header("[bold]JUGADORES[/]").Border(BoxBorder.Rounded).BorderColor(Color.Grey), abilityPanel, legendPanel, controls);
        AnsiConsole.Write(new Columns(mazePanel, sidePanel).Padding(1));

        AnsiConsole.MarkupLine($"\n[grey]Trampas activas: {traps.Count + swapTraps.Count + knockbackTraps.Count}    |    Movimiento: W A S D    |    Salir: Ctrl+C[/]");
        AnsiConsole.MarkupLine($"[bold {activeColor}]Turno de {Markup.Escape(activePlayer.Name)}[/] [grey]— pulsa una tecla para realizar una acción.[/]");
    }
 // Retorna la tecla asociada con la habilidad de un jugador específico
   static string GetPowerKey(string playerName)
{
    return playerName switch
    {
        "P1" => "T", // Teletransportarse a una posición aleatoria
        "P2" => "T", // Moverse un paso más cerca de la salida usando la teletransportación
        "P3" => "B", // Intercambiar posición con el otro jugador
        "P4" => "R", // Aturdir al otro jugador
        "P5" => "P", // Colocar trampas aleatorias
        _ => string.Empty,
    };
}
}
