﻿using Alterblade.GameObjects;
using Alterblade.GameObjects.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Alterblade.Modes
{
	internal class Battle
	{

		readonly List<Hero> team1;
		readonly List<Hero> team2;

		List<Hero> turningTeam;
		List<Hero> opposingTeam;

		readonly bool isTeamBattle = false;
		bool isBattleOver = false;
		int turnCounter = 0;

		public List<Hero> TurningTeam => turningTeam;
		public List<Hero> OpposingTeam => opposingTeam;

		public List<Hero> HeroQueue = new List<Hero>();
		public bool ReverseHeroQueue { get; set; }

		public Battle(List<Hero> team1, List<Hero> team2)
		{
			this.team1 = turningTeam = team1;
			this.team2 = opposingTeam = team2;
			if (team1.Count > 1) isTeamBattle = true;
			HeroQueue.AddRange(team1);
			HeroQueue.AddRange(team2);
		}

		public void Start()
		{
			DisplayHeader();
			while (!isBattleOver) Proceed();
		}

		void DisplayHeader()
		{
			Utils.ClearScreen();
			StringBuilder output = new StringBuilder();
			output.AppendLine("- - - - - - - - - -");
			if ( isTeamBattle )
			{
				output.AppendFormat(
					"[red]{0}[/red] and [red]{1}[/red] vs [blue]{2}[/blue] and [blue]{3}[/blue]!\n",
					team1[0].Name, team1[1].Name, team2[0].Name, team2[1].Name
				);
			}
			else
			{
				output.AppendFormat(
					"[red]{0}[/red] vs [blue]{1}[/blue]!\n",
					team1[0].Name, team2[0].Name
				);
			}
			output.Append("- - - - - - - - - -");
			Utils.WriteEmbeddedColorLine(output.ToString());
			Utils.Delay(2500);
			Utils.ClearScreen();
		}

		void Proceed()
		{
			turnCounter++;

			// Handles SPEED ties so sorting them with come out as fairly random.
			HeroQueue = HeroQueue.OrderBy(delegate (Hero hero) { return Utils.Random.Next(); }).ToList();
			// Sorts the `HeroQueue` by each hero's SPEED in descending order.
			HeroQueue = ReverseHeroQueue
				? HeroQueue.OrderBy(delegate (Hero hero) { return hero.CurrentStats[Stats.SPEED]; }).ToList()
				: HeroQueue.OrderByDescending(delegate (Hero hero) { return hero.CurrentStats[Stats.SPEED]; }).ToList();

			string turningPlayer;

			for (int i = 0; i < HeroQueue.Count; i++)
			{

				Hero turningHero = HeroQueue[i];

				if (!turningHero.IsAlive) continue;
				if (isBattleOver) break;

				Utils.WriteLine("- - - - - - - - - -");
				Utils.WriteEmbeddedColorLine("Turn [yellow]" + turnCounter + "[/yellow]!");
				Utils.WriteLine("- - - - - - - - - -");

				// Pre-turn Updates
				if (i == 0)
				{
					HeroQueue[i].UpdateStatuses(UpdateType.PRE);
				}

				turningTeam = team1;
				opposingTeam = team2;
				turningPlayer = "[red]Player 1[/red]";
				// opposingPlayer = "[blue]Player 2[/blue]";

				if (turningHero.Team == team2)
				{
					turningTeam = team2;
					opposingTeam = team1;
					turningPlayer = "[blue]Player 2[/blue]";
					// opposingPlayer = "[red]Player 1[/red]";
				}

				Utils.WriteEmbeddedColorLine("- [ [red]Player 1[/red] ] - - - - - - - - -");
				for (int j = 0; j < team1.Count; j++)
				{
					Console.Write("[" + (j + 1) + "] " + team1[j].Name);
					team1[j].DisplayStats();
				}

				Utils.WriteEmbeddedColorLine("- [ [blue]Player 2[/blue] ] - - - - - - - - -");
				for (int j = 0; j < team2.Count; j++)
				{
					Console.Write("[" + (j + 1) + "] " + team2[j].Name);
					team2[j].DisplayStats();
				}

				if (turningHero.IsSupressed)
				{
					Utils.WriteLine(turningHero.Name + " is unable to move.");
				}
				else
				{
					Utils.WriteEmbeddedColorLine("[ " + turningPlayer + " ]: " + turningHero.Name + "'s Turn!");
					turningHero.DoSkill(this);

					// On-turn Updates
					turningHero.UpdateStatuses(UpdateType.TURN);
				}

				// Post-turn Updates
				if (i == HeroQueue.Count - 1)
				{
					HeroQueue[i].UpdateStatuses(UpdateType.POST);
				}

				Utils.Delay(1000);

				UpdateTeamStates();

				if (!isBattleOver)
				{
					Utils.Delay(1000);
					Utils.WriteEmbeddedColor("Press [yellow]<Enter>[/yellow] to continue...");
					while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
					Console.WriteLine();
				}

				Utils.ClearScreen();

			}

		}

		void ConcludeGame(string winningPlayer, string losingPlayer)
		{
			Utils.WriteEmbeddedColorLine("\n" + losingPlayer + "'s heroes had fallen. " + winningPlayer + " claims victory!");
			Utils.Delay(5000);
			Utils.WriteEmbeddedColor("\nPress any key to continue...");
			Console.ReadKey();
		}

		void UpdateTeamStates()
		{
			for (int i = 0; i < HeroQueue.Count; i++)
			{
				Hero hero = HeroQueue[i];
				if (!hero.IsAlive)
					Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} had fallen in battle!", hero.Name).ToString());
			}
			if (team1.Count <= 0)
			{
				isBattleOver = true;
				ConcludeGame("[ [blue]Player 2[/blue] ]", "[ [red]Player 1[/red] ]");
			}
			else if (team2.Count <= 0)
			{
				isBattleOver = true;
				ConcludeGame("[ [red]Player 1[/red] ]", "[ [blue]Player 2[/blue] ]");
			}
		}

	}
}
