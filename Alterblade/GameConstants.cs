using Alterblade.GameObjects;
using System.Collections.Generic;

namespace Alterblade
{
	enum Stats
	{
		HP,
		ATTACK,
		DEFENSE,
		SPEED,
		CRIT_CHANCE,
		RANDOM
	}

	internal static class GameConstants
	{
		public static List<Hero> HEROES = new List<Hero> {

			new Hero(
				"Effelia",
				"Druidess",
				new Dictionary<Stats, int>() {
					{ Stats.HP, 90 * 6 },
					{ Stats.ATTACK, 90 },
					{ Stats.DEFENSE, 85 },
					{ Stats.SPEED, 85 },
					{ Stats.CRIT_CHANCE, 8 }
				},
				new List<Skill>() {
					new Skill( "Pierce", 70, 25, 0.95F,
						new List<SkillAction>() { SkillAction.DAMAGE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Nature's Blessing", 0, 10, 0,
						new List<SkillAction>() { SkillAction.SELF_STAT }, SkillTarget.NONE,
						new Dictionary<Stats, int> { { Stats.ATTACK, 1 }, { Stats.SPEED, 1 } }, false ),
					new Skill( "Garden of Thorns", 0, 5, 0.9F,
						new List<SkillAction>() { SkillAction.THORNS }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Siphon Life", 100, 3, 0.9F,
						new List<SkillAction>() { SkillAction.DAMAGE_HEAL }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, true )
				},
				null
			),

			new Hero(
				"Cadeceous",
				"Plaque Doctor",
				new Dictionary<Stats, int>() {
					{ Stats.HP, 95 * 6 },
					{ Stats.ATTACK, 100 },
					{ Stats.DEFENSE, 80 },
					{ Stats.SPEED, 70 },
					{ Stats.CRIT_CHANCE, 8 }
				},
				new List<Skill>() {
					new Skill( "Incision", 70, 25, 0.9F,
						new List<SkillAction>() { SkillAction.DAMAGE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Noxious Blast", 40, 10, 0.9F,
						new List<SkillAction>() { SkillAction.DAMAGE, SkillAction.ENEMY_STAT }, SkillTarget.TARGET,
						new Dictionary<Stats, int> { { Stats.SPEED, -1 } }, false ),
					new Skill( "Death Notice", 0, 3, 0,
						new List<SkillAction>() { SkillAction.DEATH_NOTICE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Corrosive Rain", 120, 1, 0.95F,
						new List<SkillAction>() { SkillAction.DAMAGE, SkillAction.ENEMY_STAT }, SkillTarget.ENEMYTEAM,
						new Dictionary<Stats, int> { { Stats.RANDOM, -1 } }, true )
				},
				null
			),

			new Hero(
				"Medea",
				"Shadow Elf",
				new Dictionary<Stats, int> {
					{ Stats.HP, 85 * 6 },
					{ Stats.ATTACK, 110 },
					{ Stats.DEFENSE, 70 },
					{ Stats.SPEED, 95 },
					{ Stats.CRIT_CHANCE, 10 }
				},
				new List<Skill> {
					new Skill( "Crippling Shot", 65, 15, 0.9F,
						new List<SkillAction>() { SkillAction.DAMAGE, SkillAction.ENEMY_STAT_CHANCE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { { Stats.ATTACK, -1 } }, false ),
					new Skill( "Bowfaire", 0, 5, 0,
						new List<SkillAction>() { SkillAction.CRITBOOST }, SkillTarget.NONE,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Disable", 0, 5, 0,
						new List<SkillAction>() { SkillAction.DISABLE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Arrow Barrage", 40, 3, 0.85F,
						new List<SkillAction>() { SkillAction.MULTIHITS }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, true )
				},
				null
			)

		};
	}
}
