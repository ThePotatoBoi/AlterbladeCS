using Alterblade.GameObjects;
using Alterblade.Input;
using Alterblade.Modes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
				"Mode: ",
				new Choice("Battle Mode", DoBattleMode),
				new Choice("[red]Quit[/red]", delegate () { })
			);
			startSelection.Choose();
		}

		public void DoBattleMode()
		{
			Utils.WriteEmbeddedColorLine("███ BATTLE MODE ████████████");
			Dialogue battleModeSelection = new(
				"Battle Mode: ",
				new Choice("PvP Mode", delegate() { HeroSelection(1); } ),
				new Choice("2v2 Team Battle", delegate () { HeroSelection(2); } ),
				new Choice("[red]Return[/red]", Start)
			);
			battleModeSelection.Choose();
		}

		public Hero HeroSelector(string query)
		{
			int count = 3;
			int from = 0;
			while (true)
			{
				StringBuilder output = new StringBuilder();
				output.Append("███ HERO SELECTION █████████\n\n");
				int index = 0;
				for (int i = 0; i < GameConstants.HEROES.Count; i++)
				{
					if (i >= from && i < from + count)
					{
						index++;
						Hero hero = GameConstants.HEROES[i];
						output.AppendFormat("  {0} {1}\n", index, hero.StatisticsBanner);
					}
				}
				output.Append("  0 [red]Next Page[/red]\n");
				Utils.WriteEmbeddedColorLine(output.ToString());

				int input = Utils.GetInteger(0, 3, query);
				Utils.ClearScreen();
				if (input == 0)
				{
					if (from + 3 > GameConstants.HEROES.Count)
					{
						from = 0;
						continue;
					}
					from += count;
					continue;
				}
				return GameConstants.HEROES[from + input - 1];
			}
		}

		public void HeroSelection(int heroCountPerTeam)
		{
			List<Hero> team1 = new List<Hero>();
			List<Hero> team2 = new List<Hero>();

			for (int i = 0; i < heroCountPerTeam * 2; i++)
			{
				if (i % 2 == 0)
				{
					Hero hero = HeroSelector("[red]Player 1[/red] Hero: ");
					team1.Add(new Hero(hero.Name, hero.Title, hero.BaseStats, hero.Skills, team1));
				}
				else
				{
					Hero hero = HeroSelector("[blue]Player 2[/blue] Hero: ");
					team2.Add(new Hero(hero.Name, hero.Title, hero.BaseStats, hero.Skills, team2));
				}
			}

			Battle battle = new Battle(team1, team2);
			battle.Start();
			DoBattleMode();
		}
	}
}
