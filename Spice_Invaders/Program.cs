﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Model;
using Display;
using Storage;

class Program
{
	static void Main()
	{
		try
		{
			// Instantiating the object
			Config config = new Config();
			config.configurateScreen();
			Player joueur = new Player();
			Enemy mechant = new Enemy(0, 0);
			Score score = new Score();
			Playground playground = new Playground();
			DBConnection ConnexionDB = new DBConnection();

			// Assigning the variable
			Console.CursorVisible = false;                      // Not displaying the cursor
			ConsoleKeyInfo keyPressed;                          // Will get the user input
			const int NBRENEMY = 10;                            // Choosing the number of enemy per wave
			int nbrframe = 0;                                   // Calculate the number of frame
			int NBRWAVE = 1;                                    // Calculate the number of wave
			bool IsGameOver = false;                            // Is the game over?
			string username;                                    // Take the name of the player
			bool restart = false;

			mechant.numberofenemy = NBRENEMY;                   // Getting the number of enemy in the enemy class

			List<Ammo> shooted = new List<Ammo>();              // Collection of bullet shooted
			List<Enemy> enemyalive = new List<Enemy>();         // Collection of enemy alive

			// Instantiating enemy
			for (int i = 0; i <= NBRENEMY; ++i)
			{
				enemyalive.Add(new Enemy(i * 4, 1));
			}

			// Game engine
			while (true)
			{
				Console.Clear();    // Clear Screen
				joueur.show();      // Display Player

				// Displaying the score
				score.DisplayScore();

				// If all enemies are dead we create more enemy
				if (enemyalive.Count <= 0)
				{
					++NBRWAVE;
					for (int i = 1; i < 10; ++i)
					{
						enemyalive.Add(new Enemy(i * 4, NBRWAVE));
					}
				}

				// Display and move ammo
				foreach (Ammo ammo in shooted)
				{
					ammo.show();
					ammo.move();
				}

				// Kill ammo
				for (int i = 0; i < shooted.Count; ++i)
				{
					if (shooted[i].y_position <= 1)
					{
						shooted.Remove(shooted[i]);
					}
				}

				// Display and move alien
				foreach (Enemy alien in enemyalive)
				{
					alien.show();
					if (nbrframe % 1 == 0)
					{
						alien.move();
					}
					if (alien._x_position >= Console.WindowWidth - 40 || alien._x_position <= 0)
					{
						for (int i = 0; i < enemyalive.Count(); ++i)
						{
							enemyalive[i].moveDown();
						}
					}
				}

				// Kill dead enemy
				for (int i = 0; i < enemyalive.Count; ++i)
				{
					if (enemyalive[i]._Enemy_lifePoint <= 0)
					{
						score.AddScore(NBRWAVE);
						enemyalive.Remove(enemyalive[i]);
					}
				}

				// Check if an ammo is touching an enemy
				foreach (Ammo ammo in shooted)
				{
					foreach (Enemy alien in enemyalive)
					{
						if ((alien._y_position == ammo.y_position || alien._y_position - 1 == ammo.y_position || alien._y_position + 1 == ammo.y_position) && (alien._x_position - 2 == ammo.x_position || alien._x_position - 3 == ammo.x_position || alien._x_position - 4 == ammo.x_position))
						{
							alien.takeDamage();
							ammo.hastouched = true;
						}
					}
				}

				// Kill ammo if it has touched an enemy
				for (int i = 0; i < shooted.Count; ++i)
				{
					if (shooted[i].hastouched)
					{
						shooted.Remove(shooted[i]);
					}
				}

				// Check if the player is dead
				foreach (Enemy alien in enemyalive)
				{
					if (alien._y_position == joueur.y_position)
					{
						IsGameOver = true;
					}
				}

				// Check if the player has pressed any button
				if (Console.KeyAvailable)                               // If the user press any touch
				{
					keyPressed = Console.ReadKey(false);
					switch (keyPressed.Key)
					{
						case ConsoleKey.LeftArrow:                      // If the user press the left arrow key
							joueur.MovingLeft();
							break;

						case ConsoleKey.RightArrow:                     // If the user press the right arrow key
							joueur.MovingRight();
							break;

						case ConsoleKey.Spacebar:                       // If the user press the space bar
							if (shooted.Count() < 5)                     // Can't shoot if there's more than 5 ammo already on the screen, that way the player can't spam button
							{
								shooted.Add(new Ammo(joueur.x_position, joueur.y_position));
							}
							break;

						case ConsoleKey.Escape:                         // If the user press the touch escape
							Environment.Exit(0);                        // Exit the program
							break;
					}
				}

				// If the player is dead
				if (IsGameOver)
				{
					do
					{
						playground.GameOver();

						Console.SetCursorPosition(30, 10);
						username = Console.ReadLine();
					}
					while (username.Length > 4 || username.Length < 2);

					ConnexionDB.connection();

					ConnexionDB.username = username;
					ConnexionDB.score = score.score;

					ConnexionDB.Add();              // Add the score in the database
					ConnexionDB.Top5();             // Read the 5 five best player score
					ConnexionDB.connectionClose();  // Close the connection to the database

					playground.restart();

					do
					{
						if (Console.KeyAvailable)
						{
							keyPressed = Console.ReadKey(false);
							if (keyPressed.Key == ConsoleKey.Spacebar)
							{
								restart = true;
							}
						}   // If the player press the space bar
					}
					while (!restart);
					IsGameOver = false;

					// Reset all the values

					// Kill all the enemies still alive
					for (int i = 0; i != enemyalive.Count;)
					{
						enemyalive.RemoveAt(0);
					}

					// Reset the score
					score.score = 0;

					// Reset the number of wave
					NBRWAVE = 0;

					// Reset the number of frame
					nbrframe = 0;
				}

				++nbrframe;             // Incrementing the number of frame
				Thread.Sleep(10);       // Making the program wait so that the user have the time to see what happened
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred: {ex.Message}");
			Console.WriteLine(ex.StackTrace);
		}
	}
}
