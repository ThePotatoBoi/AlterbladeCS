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
			Utils.WriteEmbeddedColorLine(":: [red]Alterblade[/red]: [yellow]Version 0.2 (BETA)[/yellow] ::");
			Dialogue startSelection = new(
				new Choice("Battle Mode", DoBattleMode),
				new Choice("[red]Quit[/red]", delegate () { })
			);
			startSelection.Choose();
		}

		public void DoBattleMode()
		{
			Console.WriteLine("Choose a Battle Mode!");
			Dialogue battleModeSelection = new(
				new Choice("PvP Mode", PVPMode),
				new Choice("2v2 Team Battle", TeamMode),
				new Choice("[red]Return[/red]", Start)
			);
			battleModeSelection.Choose();
		}

		public void DisplayCharacterList()
		{
			Hero hero;
			StringBuilder output = new StringBuilder();
			output.Append("- - - - - - - - - -\nChoose your character!\n- - - - - - - - - -\n");
			for (int i = 0; i < GameConstants.HEROES.Count; i++)
			{
				hero = GameConstants.HEROES[i];
				output.AppendFormat("\n[{0}]: {1} the {2}\n", i + 1, hero.Name, hero.Title);
				output.AppendFormat("[yellow]STATS[/yellow]  : [yellow]HP[/yellow]: {0, -3} | [yellow]ATK[/yellow]: {1, -3} | [yellow]DEF[/yellow]: {2, -3} | [yellow]SPE[/yellow]: {3, -3} | [yellow]CRI[/yellow]: {4}%\n",
					hero.BaseStats[Stats.HP],
					hero.BaseStats[Stats.ATTACK],
					hero.BaseStats[Stats.DEFENSE],
					hero.BaseStats[Stats.SPEED],
					hero.BaseStats[Stats.CRIT_CHANCE]
				);
				output.Append("[yellow]SKILLS[/yellow] :");
				for (int j = 0; j < hero.Skills.Count; j++)
				{
					string color = "cyan";
					if (j != 0) output.Append(',');
					if (hero.Skills[j].IsUltimate) color = "red";
					output.AppendFormat(" [{1}]{0}[/{1}]", hero.Skills[j].Name, color);
				}
			}
			Utils.WriteEmbeddedColorLine(output.ToString());
			Console.WriteLine();
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
