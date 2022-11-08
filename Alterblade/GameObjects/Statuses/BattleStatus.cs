using Alterblade.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alterblade.GameObjects.Statuses
{
	internal class BattleStatus : Status
	{

		BattleStatusType battleStatusType;
		Battle battle;

		public BattleStatusType BattleStatusType => battleStatusType;

		public BattleStatus(string name, int duration, Battle battle, Hero source, Skill sourceSkill, BattleStatusType battleStatusType, UpdateType updateType)
			: base(name, duration, source, sourceSkill, updateType)
		{
			this.battleStatusType = battleStatusType;
			this.battle = battle;
		}

		public override bool End(bool showText)
		{
			StringBuilder output = new StringBuilder();
			switch (battleStatusType)
			{
				case BattleStatusType.TRICKROOM:
				{
					battle.HeroQueueSort = HeroQueueSort.SPEED;
					output.AppendFormat("The twisting dimensions turned to normal.");
					break;
				}
			}
			if (showText)
				Utils.WriteEmbeddedColorLine(output.ToString());
			return true;
		}

		public override bool Update()
		{
			if (duration-- == 0)
			{
				End(true);
				return false;
			}
			switch(battleStatusType)
			{
				case BattleStatusType.TRICKROOM:
				{
					Utils.WriteEmbeddedColorLine("The dimensions are twisting disorderly.");
					battle.HeroQueueSort = HeroQueueSort.SPEED_REVERSED;
					break;
				}
			}
			return true;
		}
	}
}
