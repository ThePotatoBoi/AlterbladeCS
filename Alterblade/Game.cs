using Alterblade.GameObjects;
using Alterblade.Input;
using Alterblade.Modes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alterblade
{
	internal class Game
	{
		public void Start()
		{
			Utils.ClearScreen();
			Utils.WriteEmbeddedColorLine("██████████████████████████ BETA 0.2 ███████\r\n\r\n  ███ █   ███ ███ ██▄ ██▄ █   ███ ██▄ ███\r\n  █▄█ █    █  █▄  █▄  █▄█ █   █▄█ █ █ █▄ \r\n  █ █ █▄█  █  █▄▄ █ █ █▄█ █▄█ █ █ ███ █▄▄  \r\n\r\n█████████████ by Group 1 BSCS C-103 ███████\r\n\n");
			Utils.WriteEmbeddedColorLine("███ MAIN MENU ██████████████");
			Dialogue startSelection = new Dialogue(
				new Choice("Battle Mode", DoBattleMode),
				new Choice("[red]Quit[/red]", delegate () { })
			);
			startSelection.Choose();
		}

		public void DoBattleMode()
		{
			Utils.WriteEmbeddedColorLine("███ BATTLE MODE ████████████");
			Dialogue battleModeSelection = new(
				new Choice("PvP Mode", PVPMode),
				new Choice("2v2 Team Battle", TeamMode),
				new Choice("[red]Return[/red]", Start)
			);
			battleModeSelection.Choose();
		}

		public Hero SelectHero()
		{
			Hero hero;
			StringBuilder output = new StringBuilder();
			output.Append("███ HERO SELECTION █████████\n\n");
			for (int i = 0; i < GameConstants.HEROES.Count; i++)
			{
				hero = GameConstants.HEROES[i];
				output.AppendFormat("  {0} {1}\n", i + 1, hero.StatisticsBanner);
			}
			output.Append("  0 [red]Next Page[/red]\n");
			Utils.WriteEmbeddedColorLine(output.ToString());
		}

		public void PVPMode()
		{
			List<Hero> team1 = new List<Hero>();
			List<Hero> team2 = new List<Hero>();
			int index;
			Hero hero;
			DisplayCharacterList();
			index = Utils.GetInteger(1, GameConstants.HEROES.Count, "[red]Player 1[/red] Hero: ") - 1;
			hero = GameConstants.HEROES[index];
			team1.Add(new Hero(hero.Name, hero.Title, hero.BaseStats, hero.Skills, team1));
			index = Utils.GetInteger(1, GameConstants.HEROES.Count, "[blue]Player 2[/blue] Hero: ") - 1;
			hero = GameConstants.HEROES[index];
			team2.Add(new Hero(hero.Name, hero.Title, hero.BaseStats, hero.Skills, team2));

			Battle battle = new Battle(team1, team2);
			battle.Start();
			DoBattleMode();
		}

		public void TeamMode()
		{
			List<Hero> team1 = new List<Hero>();
			List<Hero> team2 = new List<Hero>();
			int index;
			Hero hero;

			DisplayCharacterList();

			index = Utils.GetInteger(1, GameConstants.HEROES.Count, "[red]Player 1[/red] First Hero: ") - 1;
			hero = GameConstants.HEROES[index];
			team1.Add(new Hero(hero.Name, hero.Title, hero.BaseStats, hero.Skills, team1));
			index = Utils.GetInteger(1, GameConstants.HEROES.Count, "[blue]Player 2[/blue] First Hero: ") - 1;
			hero = GameConstants.HEROES[index];
			team2.Add(new Hero(hero.Name, hero.Title, hero.BaseStats, hero.Skills, team2));
			index = Utils.GetInteger(1, GameConstants.HEROES.Count, "[red]Player 1[/red] Second Hero: ") - 1;
			hero = GameConstants.HEROES[index];
			team1.Add(new Hero(hero.Name, hero.Title, hero.BaseStats, hero.Skills, team1));
			index = Utils.GetInteger(1, GameConstants.HEROES.Count, "[blue]Player 2[/blue] Second Hero: ") - 1;
			hero = GameConstants.HEROES[index];
			team2.Add(new Hero(hero.Name, hero.Title, hero.BaseStats, hero.Skills, team2));

			Battle battle = new Battle(team1, team2);
			battle.Start();
			DoBattleMode();
		}
	}
}
