using System;
using System.Collections.Generic;
using System.Linq;
using SabberStoneSyncchronizer.Model;
using SabberStoneSyncchronizer.Sync;

namespace SabberStoneSyncchronizer
{
	class Program
	{
		static void Main(string[] args)
		{
			//var interpreter = new Interpreter(@"test\", "initialmulli.log");
			//List<PowerGame> games = interpreter.Parse(true, true);
			//Console.WriteLine($"Done parsing! Found {games.Count} game(s) in log.");
			//Console.ReadKey();

			//if (games.Any())
			//{
			//	var game = games.Last();

			//	Console.WriteLine($"Starting a syncronized PowerGame!");

			//	while (game.PowerHistory.Count > 0)
			//	{
			//		var entry = game.PowerHistory.Dequeue();
			//		Console.WriteLine($"Dequeue {entry}.");
			//		entry.Process(game);
			//	}
			//	var realGame = new SyncedGame(game);
			//	realGame.Sync();
			//}
			//Console.ReadKey();
			TestInitialMulli();
		}


		static void TestInitialMulli()
		{
			var interpreter = new Interpreter(@"test\", "initialmulli.log");
			var powerGame = interpreter.Parse(false, false).Last();
			while (powerGame.PowerHistory.Any())
			{
				powerGame.PowerHistory.Dequeue().Process(powerGame);
			}
			Console.WriteLine("Game state extracted, attempting to sync...");
			var game = new SyncedGame(powerGame);
			Console.WriteLine("Sync complete");
			Console.ReadKey();
		}
	}
}
