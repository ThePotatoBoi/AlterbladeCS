using System;
using System.Text;

namespace Alterblade.GameObjects.Statuses
{

	enum StatusType
	{
		N_DAMAGE_PER_TURN,
		N_DEATH_NOTICE,
		N_DISABLE,
		P_CRITBOOST
	}

	enum UpdateType
	{
		PRE,
		TURN,
		POST
	}

	internal class Status
	{

		string name;
		Hero hero;
		Hero source;
		int duration;
		Skill sourceSkill;
		StatusType statusType;
		UpdateType updateType;
		bool isNegative;

		public string Name => name;
		public Hero Hero => hero;
		public Hero Source => source;
		public int Duration => duration;
		public UpdateType UpdateType => updateType;
		public bool IsNegative => isNegative;

		public Status(string name, Hero hero, Hero source, Skill sourceSkill, StatusType statusType, int duration, UpdateType updateType)
		{
			this.name = name;
			this.hero = hero;
			this.source = source;
			this.sourceSkill = sourceSkill;
			this.statusType = statusType;
			this.duration = duration;
			this.updateType = updateType;
			isNegative = statusType.ToString()[0] == 'N';
		}

		public void End(bool showText)
		{
			StringBuilder output = new StringBuilder();
			switch (statusType)
			{
				case StatusType.N_DAMAGE_PER_TURN:
				{
					output.AppendFormat("{0} is free from [cyan]{1}[/cyan]!", hero.Name, name);
					break;
				}
				case StatusType.N_DEATH_NOTICE:
				{
					output.AppendFormat("{0}'s [cyan]{1}[/cyan] ended.", hero.Name, name);
					break;
				}
				case StatusType.N_DISABLE:
				{
					for (int i = 0; i < hero.Skills.Count; i++)
					{
						if (hero.Skills[i].IsDisabled)
							hero.Skills[i].IsDisabled = false;
					}
					output.AppendFormat("{0}'s [cyan]{1}[/cyan] ended.", hero.Name, name);
					break;
				}
				case StatusType.P_CRITBOOST:
				{
					hero.CurrentStats[Stats.CRIT_CHANCE] -= 10;
					output.AppendFormat("{0}'s [cyan]{1}[/cyan] petered out ended.", hero.Name, name);
					break;
				}
			}
			if (showText)
				Utils.WriteEmbeddedColorLine( output.ToString() );
			hero.RemoveStatus(this);
		}

		public void Update()
		{
			if (duration-- == 0)
			{
				End(true);
				return;
			}

			StringBuilder output = new StringBuilder();

			switch (statusType)
			{
				case StatusType.N_DAMAGE_PER_TURN:
				{
					output.AppendFormat("{0} is hurt by [cyan]{1}[/cyan]!", hero.Name, name);
					Utils.WriteEmbeddedColorLine(output.ToString());
					hero.TakeDamage(0.08F, false);
					break;
				}
				case StatusType.N_DEATH_NOTICE:
				{
					if (Hero.CurrentStats[Stats.HP] < Convert.ToInt32(0.25F * hero.BaseStats[Stats.HP]))
					{
						output.AppendFormat("{0} is caught by the [cyan]{1}[/cyan]!", hero.Name, name);
						Utils.WriteEmbeddedColorLine(output.ToString());
						hero.RemoveStatus(this);
						hero.TakeDamage(1F, false);
					}
					else
					{
						output.AppendFormat("{0}'s [cyan]{1}[/cyan] is pending...", hero.Name, name);
						Utils.WriteEmbeddedColorLine(output.ToString());
					}
					break;
				}
			}
		}
	}
}
