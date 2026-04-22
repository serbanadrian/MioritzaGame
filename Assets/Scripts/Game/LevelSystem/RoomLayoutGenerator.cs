using System.Collections.Generic;
using MioritzaGame.Constants;
using UnityEngine;

namespace MioritzaGame.Game
{
    internal static class RoomLayoutGenerator
    {
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        
        public static List<Vector2Int> Generate(int roomCount)
        {
            var cells = new List<Vector2Int>(roomCount);
            if (roomCount <= 0) return cells;

            var occupied = new HashSet<Vector2Int>();
            cells.Add(Vector2Int.zero);
            occupied.Add(Vector2Int.zero);

            while (cells.Count < roomCount)
            {
                var placed = false;
                for (var attempt = 0; attempt < MagicNumbers.MAX_ATTEMPTS_PER_ROOM; attempt++)
                {
                    var origin = cells[Random.Range(0, cells.Count)];
                    var direction = Directions[Random.Range(0, Directions.Length)];
                    var candidate = origin + direction;
                    if (occupied.Add(candidate) == true)
                    {
                        cells.Add(candidate);
                        placed = true;
                        break;
                    }
                }
                
                if (placed == false) 
                    break;
            }

            return cells;
        }
    }
}
