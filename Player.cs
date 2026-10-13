using System;

public class Player
{
    // Propiedades del jugador
    public string Name { get; set; } // Nombre del jugador
    public string PowerDescription { get; set; } // Descripción del poder especial del jugador
    public string Story { get; set; } // Historia o trasfondo del jugador
    
    // Cantidad de usos disponibles para cada poder
    private int teleportUses = 3;
    private int teleportTowardsExitUses = 3;
    private int swapPositionsUses = 3;
    private int stunOtherPlayerUses = 3;
    private int placeRandomTrapUses = 3;

    // Constructor para inicializar el jugador con su nombre, poder y trasfondo
    public Player(string name, string powerDescription, string story)
    {
        Name = name;
        PowerDescription = powerDescription;
        Story = story;
    }

    // Método para verificar si un poder puede ser usado (tiene intentos disponibles)
    public static bool CanUsePower(ref int uses)
    {
        if (uses > 0)
        {
            uses--; // Disminuye la cantidad de usos disponibles
            return true;
        }
        else
        {
            Spectre.Console.AnsiConsole.MarkupLine("[red]No quedan usos disponibles para esta habilidad.[/]");
            return false;
        }
    }

    // Habilidad del personaje P1: teletransporte aleatorio con usos limitados.
    public void Teleport(ref int playerX, ref int playerY, Random rand)
    {
        var candidates = GetTeleportCandidates();
        if (candidates.Count == 0 || !CanUsePower(ref teleportUses))
            return;

        var destination = candidates[rand.Next(candidates.Count)];
        playerX = destination.x;
        playerY = destination.y;
    }

    // Efecto de una trampa: no consume usos de la habilidad del personaje.
    public void TeleportFromTrap(ref int playerX, ref int playerY, Random rand)
    {
        var candidates = GetTeleportCandidates();
        if (candidates.Count == 0)
            return;

        var destination = candidates[rand.Next(candidates.Count)];
        playerX = destination.x;
        playerY = destination.y;
    }

    private static List<(int x, int y)> GetTeleportCandidates()
    {
        var candidates = new List<(int x, int y)>();
        for (int y = 1; y < GameManager.height - 1; y++)
        for (int x = 1; x < GameManager.width - 1; x++)
        {
            if (GameManager.maze[y, x] == 0 &&
                !(x == GameManager.width - 2 && y == GameManager.height - 2))
                candidates.Add((x, y));
        }

        return candidates;
    }

    // Método para teletransportar al jugador hacia la salida utilizando el algoritmo A*
    public void TeleportTowardsExit(ref int playerX, ref int playerY, int goalX, int goalY)
    {
        if (CanUsePower(ref teleportTowardsExitUses))
        {
            var path = MazeLogic.FindPath(playerX, playerY, goalX, goalY);
            if (path != null && path.Count > 0)
            {
                // Mueve al jugador un paso hacia la salida
                playerX = path[0].Item1;
                playerY = path[0].Item2;
            }
        }
    }

    // Método para intercambiar posiciones con otro jugador
    public void SwapPositions(ref int playerX, ref int playerY, ref int otherPlayerX, ref int otherPlayerY)
    {
        if (CanUsePower(ref swapPositionsUses))
        {
            // Se intercambian las coordenadas de ambos jugadores
            int tempX = playerX;
            int tempY = playerY;
            playerX = otherPlayerX;
            playerY = otherPlayerY;
            otherPlayerX = tempX;
            otherPlayerY = tempY;
        }
    }

    // Método para aturdir a otro jugador durante dos turnos
    public void StunOtherPlayer(ref int otherPlayerStunTurns)
    {
        if (CanUsePower(ref stunOtherPlayerUses))
        {
            otherPlayerStunTurns = 2; // Se asigna una penalización de 2 turnos al jugador rival
        }
    }

    // Método para colocar una trampa en una posición aleatoria del laberinto
    public void PlaceRandomTrap(Random rand)
    {
        var candidates = new List<(int x, int y)>();
            for (int y = 1; y < GameManager.height - 1; y++)
            for (int x = 1; x < GameManager.width - 1; x++)
            {
                if (GameManager.maze[y, x] != 0 ||
                    (x == GameManager.width - 2 && y == GameManager.height - 2))
                    continue;
                if (GameManager.traps.Any(t => t.Position == (x, y)) ||
                    GameManager.swapTraps.Any(t => t.Position == (x, y)) ||
                    GameManager.knockbackTraps.Any(t => t.Position == (x, y)))
                    continue;
                candidates.Add((x, y));
            }

            if (candidates.Count == 0)
            {
                Console.WriteLine("No hay posiciones libres para colocar otra trampa.");
                return;
            }

            if (!CanUsePower(ref placeRandomTrapUses))
                return;

            var position = candidates[rand.Next(candidates.Count)];
        GameManager.traps.Add(new Trap(position.x, position.y));
    }
}
