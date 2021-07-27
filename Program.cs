using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace task3_Itra
{
    class Program
    {
        static RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();
        static HMACSHA256 HmacSha256 = new HMACSHA256();
        static Random ComputerMoveGenerator = new Random();
        enum Winner 
        {
            Draw,
            Person,
            Computer,
        }

        static void Main(string[] args)
        {
#if DEBUG 
            args = new string[] { "rock", "paper", "scissors", "lizard", "Spock" };
#endif
            if (!AreArgsValid(args))
            {
                return;
            }

            HmacSha256.Initialize();

            int ost = args.Length % 2;
            int half = args.Length / 2;

            while (true)
            {
                var key = new byte[16];
                RngCsp.GetBytes(key);
                HmacSha256.Key = key;

                var computerMove = ComputerMoveGenerator.Next(0, args.Length);

                var compMove = computerMove.ToString();
                var stringHmac = GetComputerSha(compMove);
                //var compyterMoveInBytes = BitConverter.GetBytes(computerMove);
                //var hmac = HmacSha256.ComputeHash(compyterMoveInBytes);
                //var stringHmac = Encoding.ASCII.GetString(hmac);

                Console.WriteLine($"HMAC: {stringHmac}");
                Console.WriteLine("Available moves: ");

                for (int i = 1; i < args.Length + 1; i++)
                {
                    Console.WriteLine(i.ToString() + " - " + args[i - 1]);
                }

                Console.WriteLine("0" + " - " + "exit");
                Console.Write("Enter your move: ");
                var moyMove = Console.ReadLine();

                int playerMove = 0;

                while (string.IsNullOrWhiteSpace(moyMove) || !int.TryParse(moyMove, out playerMove) || int.Parse(moyMove) > args.Length)
                {
                    Console.Write("Please enter the correct move:  ");
                    moyMove = Console.ReadLine();
                }

                int moy_move = int.Parse(moyMove);
                if (moy_move == 0)
                {
                    break;
                }
                
                Console.WriteLine("Your move: " + args[moy_move - 1]);
                Console.WriteLine("Computre move: " + args[computerMove]);

                var result = (computerMove + 1) - moy_move;
                Win(result, half);
                
                Console.WriteLine("HMAC key: " + string.Join("", HmacSha256.Key.Select(b => b.ToString("X"))));

                Console.Write("Want to play more? Type 'Y'/'y' to continue playing or anything else to leave: ");

                var newGame = Console.ReadLine();

                Console.WriteLine();

                if (!newGame.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Thanks for game!");
                    break;
                }
            }
        }

        static string GetComputerSha(string compMove)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] vs = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(compMove));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < vs.Length; i++)
            {
                builder.Append(vs[i].ToString("X"));
            }
            return builder.ToString();
        }

        private static void Win(int result, int half)
        {
            if (result == 0)
            {
                Console.WriteLine(Winner.Draw);
            }
            else
            {
                if ((result < 0 && Math.Abs(result) <= half) || (result > 0 && Math.Abs(result) > half))
                {
                    Console.WriteLine(Winner.Person + " win");
                }
                else
                {
                    Console.WriteLine(Winner.Computer + " win");
                }

            }
        }
                
        private static bool AreArgsValid(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Number of args should be >= 3");
                Console.WriteLine("Enter: rock, scissors, paper, lizard, Spock or rock, scissors, paper or 1 2 3 etc.");
                return false;
            }

            if (args.Length % 2 == 0)
            {
                Console.WriteLine("Invalid namber of argument");
                Console.WriteLine("Enter: rock, scissors, paper, lizard, Spock or rock, scissors, paper or 1 2 3 etc.");
                return false;
            }

            var notRepeatedArgs = new HashSet<string>(args);
            if (notRepeatedArgs.Count != args.Length)
            {
                Console.WriteLine("All args should be unique values");
                Console.WriteLine("Enter: rock, scissors, paper, lizard, Spock or rock, scissors, paper or 1 2 3 etc.");
                return false;
            }
            return true;
        }
    }
}
