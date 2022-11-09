﻿using Alterblade.GameObjects;
using Alterblade.GameObjects.Statuses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		List<BattleStatus> battleStatuses = new List<BattleStatus>();

		bool isBattleOver = false;
		int turnCounter = 0;

		public List<Hero> TurningTeam => turningTeam;
		public List<Hero> OpposingTeam => opposingTeam;
		public List<Hero> HeroQueue { get; private set; } = new List<Hero>();
		public List<BattleStatus> BattleStatuses => battleStatuses;

		public HeroQueueSort HeroQueueSort { get; set; } = HeroQueueSort.SPEED;

		public Battle(List<Hero> team1, List<Hero> team2)
		{
			this.team1 = turningTeam = team1;
			this.team2 = opposingTeam = team2;
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
			output.AppendLine("███ BATTLE █████████████████");
			output.Append("\n█ [red]Player 1[/red]\n");
			for (int i = 0; i < team1.Count; i++)
				output.AppendFormat("  {0}\n", team1[i].Name);
			output.Append("\n█ [blue]Player 2[/blue]\n");
			for (int i = 0; i < team2.Count; i++)
				output.AppendFormat("  {0}\n", team2[i].Name);
			Utils.WriteEmbeddedColorLine(output.ToString());
			Utils.Delay(5000);
			Utils.ClearScreen();
		}

		void SortHeroQueue()
		{
			HeroQueue = HeroQueue.OrderBy(delegate (Hero hero) { return Utils.Random.Next(); }).ToList();
			switch (HeroQueueSort)
			{
				case HeroQueueSort.SPEED:
				{
					HeroQueue = HeroQueue.OrderByDescending(delegate (Hero hero) { return hero.CurrentStats[Stats.SPEED]; }).ToList();
					break;
				}
				case HeroQueueSort.SPEED_REVERSED:
				{
					HeroQueue = HeroQueue.OrderBy(delegate (Hero hero) { return hero.CurrentStats[Stats.SPEED]; }).ToList();
					break;
				}
			}
		}

		void Proceed()
		{
			turnCounter++;
			SortHeroQueue();

			for (int i = 0; i < HeroQueue.Count; i++)
			{
				Hero turningHero = HeroQueue[i];
				StringBuilder output = new StringBuilder();

				if (!turningHero.IsAlive) continue;
				if (isBattleOver) break;

				output.AppendLine("███ BATTLE █████████████████\n");
				for (int j = 0; j < team1.Count; j++)
					output.AppendFormat("  {0} {1}\n", j + 1, team1[j].InBattleStatisticsBanner);
				output.Append('\n');
				for (int j = 0; j < team2.Count; j++)
					output.AppendFormat("  {0} {1}\n", j + 1, team2[j].InBattleStatisticsBanner);
				Utils.WriteEmbeddedColorLine(output.ToString());

				turningTeam = team1;
				opposingTeam = team2;

				if (turningHero.Team == team2)
				{
					turningTeam = team2;
					opposingTeam = team1;
				}

				if (turningHero.IsSupressed)
				{
					Utils.Error(turningHero.Name + " is unable to move.");
				}
				else
				{
					Utils.WriteEmbeddedColorLine("███ TURN ███████████████████\n");
					turningHero.LastSkillHit = Skill.None;
					turningHero.LastSkillUsed = Skill.None;
					turningHero.LastHeroAttacker = Hero.None;
					turningHero.DoSkill(this);

					// On-turn Updates
					turningHero.UpdateStatuses();
				}

				// Post-turn Updates
				if (i == HeroQueue.Count - 1)
				{
					Console.WriteLine();
					UpdateBattleStatuses();
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
				Debug.WriteLine(this);
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
				{
					Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} had fallen in battle!", hero.Name).ToString());
					HeroQueue.Remove(hero);
				}
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

		void UpdateBattleStatuses()
		{
			for (int i = 0; i < battleStatuses.Count; i++)
				battleStatuses[i].Update();
		}

		public bool AddBattleStatus(BattleStatus battleStatus)
		{
			for (int i = 0; i < battleStatuses.Count; i++)
			{
				if (battleStatus.Name == battleStatuses[i].Name)
					return false;
			}
			battleStatuses.Add(battleStatus);
			return true;
		}

		public bool RemoveBattleStatus(BattleStatus battleStatus)
		{
			return battleStatuses.Remove(battleStatus);
		}

	}
}
