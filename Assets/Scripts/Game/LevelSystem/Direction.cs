using UnityEngine;

namespace MioritzaGame.Game
{
    public enum Direction
    {
        None,
        North,
        South,
        East,
        West,
    }

    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                default: return Direction.None;
            }
        }

        public static Vector2Int ToCellOffset(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return Vector2Int.up;
                case Direction.South: return Vector2Int.down;
                case Direction.East: return Vector2Int.right;
                case Direction.West: return Vector2Int.left;
                default: return Vector2Int.zero;
            }
        }
    }
}
