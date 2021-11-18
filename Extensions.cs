using System.Numerics;

namespace _2048
{
    public static class Extensions
    {
        public static Vector2 Vector(this Direction direction)
        {
            return direction switch
            {
                Direction.UP => new Vector2(0, -1),
                Direction.DOWN => new Vector2(0, 1),
                Direction.LEFT => new Vector2(-1, 0),
                Direction.RIGHT => new Vector2(1, 0),
                _ => Vector2.Zero,
            };
        }

        public static bool IsVertical(this Direction direction) => (int)direction % 2 == 1;
        public static bool IsHorizontal(this Direction direction) => (int)direction % 2 == 0;

        public static int GetValue(this int[,] board, Vector2 coords)
        {
            return board[(int)coords.Y, (int)coords.X];
        }
    }
}
