using System;

namespace _2048
{
    static class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();
            board.Draw();
            while (true)
            {
                Direction d = Board.ParseKey(Console.ReadKey().Key);
                while (d == Direction.NONE)
                    d = Board.ParseKey(Console.ReadKey().Key);
                board.Move(d);

                if (board.Lose)
                {
                    Console.WriteLine("You lose");
                    break;
                }
                else if (board.Win)
                {
                    Console.WriteLine("You win");
                    break;
                }
            }
        }
    }
}
