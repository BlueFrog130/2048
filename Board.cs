using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace _2048
{
    /// <summary>
    /// Contains all game logic
    /// </summary>
    public class Board
    {
        private readonly int size;
        private readonly int[,] state;
        private readonly Random random = new Random();

        public bool Win => state.Cast<int>().Any(x => x == 2048);
        public bool Lose => state.Cast<int>().All(x => x > 0) && !CanMerge();

        public Board(int size = 4)
        {
            this.size = size;
            state = new int[size, size];
            FillRandom();
            FillRandom();
        }

        /// <summary>
        /// Draws board state
        /// </summary>
        public void Draw()
        {
            Console.Clear();
            foreach (int i in Enumerable.Range(0, size))
                Console.Write("+----");
            Console.WriteLine("+");
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Console.Write("|");
                    Console.ForegroundColor = GetColor(state[y, x]);
                    Console.Write($"{state[y, x],4}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine("|");
                foreach (int i in Enumerable.Range(0, size))
                    Console.Write("+----");
                Console.WriteLine("+");
            }
        }

        /// <summary>
        /// Fills some random <c>0</c> with a <c>2</c>
        /// </summary>
        public void FillRandom()
        {
            IEnumerable<Vector2> possible = EmptySpaces();
            Vector2 rand = possible.ElementAt(random.Next(0, possible.Count()));
            state[(int)rand.Y, (int)rand.X] = 2;
        }

        /// <summary>
        /// Parses input key to movement direction
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Direction ParseKey(ConsoleKey key)
        {
            return key switch
            {
                ConsoleKey.UpArrow => Direction.UP,
                ConsoleKey.DownArrow => Direction.DOWN,
                ConsoleKey.LeftArrow => Direction.LEFT,
                ConsoleKey.RightArrow => Direction.RIGHT,
                _ => Direction.NONE,
            };
        }

        /// <summary>
        /// Moves all pieces in a direction
        /// Fills random space if something moved
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Direction direction)
        {
            // Keep shifting in that direction until none behind
            bool moved = false;
            for (int i = 0; i < size; i++)
            {
                if (Shift(direction))
                {
                    moved = true;
                    Thread.Sleep(50);
                    // No need to draw on last iteration
                    if (i < size - 1)
                        Draw();
                }
                else
                {
                    break;
                }
            }
            if (moved)
            {
                FillRandom();
                Draw();
            }
        }

        /// <summary>
        /// Gets all <c>0</c> spaces
        /// </summary>
        /// <returns>Empty spaces</returns>
        private IEnumerable<Vector2> EmptySpaces()
        {
            List<Vector2> empty = new List<Vector2>();
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    if (state[y, x] == 0)
                        empty.Add(new Vector2(x, y));
            return empty;
        }

        /// <summary>
        /// Shifts all squares
        /// </summary>
        /// <returns><c>true</c> if at least 1 square was moved</returns>
        private bool Shift(Direction direction)
        {
            bool moved = false;
            if (direction == Direction.DOWN || direction == Direction.RIGHT)
            {
                for (int y = size - 1; y >= 0; y--)
                    for (int x = size - 1; x >= 0; x--)
                        if (MoveSpace(direction, x, y))
                            moved = true;
            }
            else
            {
                for (int y = 0; y < size; y++)
                    for (int x = 0; x < size; x++)
                        if (MoveSpace(direction, x, y))
                            moved = true;
            }

            return moved;
        }

        /// <summary>
        /// Moves space in direction, merges if possible
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns><c>true</c> if merged or moved</returns>
        private bool MoveSpace(Direction direction, int x, int y)
        {
            if (state[y, x] > 0)
            {
                Vector2 current = new Vector2(x, y);
                Vector2 front = current + direction.Vector();
                if (IsValid(front) && state.GetValue(front) == 0)
                {
                    Swap(current, front);
                    return true;
                }
                // Merging
                else if (IsValid(front) && state.GetValue(current) == state.GetValue(front))
                {
                    state[(int)front.Y, (int)front.X] = state.GetValue(front) * 2;
                    state[(int)current.Y, (int)current.X] = 0;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Swaps 2 points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private void Swap(Vector2 p1, Vector2 p2)
        {
            int tmp = state.GetValue(p1);
            state[(int)p1.Y, (int)p1.X] = state.GetValue(p2);
            state[(int)p2.Y, (int)p2.X] = tmp;
        }

        /// <summary>
        /// Checks if this point is valid
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsValid(Vector2 position)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < size && position.Y < size;
        }

        /// <summary>
        /// Checks if any neighbors can be merged
        /// </summary>
        /// <returns><c>True if some neighbor can be merged</c></returns>
        private bool CanMerge()
        {
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(d => d != Direction.NONE))
                    {
                        Vector2 current = new Vector2(x, y);
                        Vector2 neighbor = current + direction.Vector();
                        if (IsValid(neighbor) && state.GetValue(current) == state.GetValue(neighbor))
                            return true;
                    }
            return false;
        }

        /// <summary>
        /// Gets color for a number in the game
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static ConsoleColor GetColor(int number)
        {
            return number switch
            {
                2 => ConsoleColor.DarkCyan,
                4 => ConsoleColor.Cyan,
                8 => ConsoleColor.Green,
                16 => ConsoleColor.DarkYellow,
                32 => ConsoleColor.Yellow,
                64 => ConsoleColor.DarkMagenta,
                128 => ConsoleColor.Magenta,
                256 => ConsoleColor.DarkBlue,
                512 => ConsoleColor.Blue,
                1024 => ConsoleColor.DarkRed,
                2048 => ConsoleColor.Red,
                _ => ConsoleColor.White
            };
        }
    }   
}
