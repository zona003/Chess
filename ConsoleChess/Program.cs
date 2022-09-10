using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessClientLib;
using static System.Console;

namespace ConsoleChess
{
    class Program
    {
        public const string HOST = "https://localhost:44325/api/Games";
        public const string USER = "1";

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Start();

        }

        ChessClient client;

        void Start()
        {
            //client = new ChessClient(HOST, USER);
            //WriteLine(client.GetCurrentGame());
            //while (true)
            //{
            //    WriteLine("\nYour Move :");
            //    string move = Console.ReadLine();
            //    if (move == "q") return;
            //    WriteLine(client.SendMove(move));
            //}
        }
    }
}
