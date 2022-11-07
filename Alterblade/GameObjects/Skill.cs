using Alterblade.GameObjects.Statuses;
using Alterblade.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alterblade.GameObjects
{
	enum SkillTarget
	{
		TARGET,
		NONE,
		ENEMYTEAM,
		ALLYTEAM,
	}

	enum SkillAction
	{
		// General
		DAMAGE,
		DAMAGE_HEAL,
		DAMAGE_RECOIL,
		DAMAGE_IGNORE_DEFENSE,
		SELF_STAT,
		ENEMY_STAT,
		SELF_STAT_CHANCE,
		ENEMY_STAT_CHANCE,
		HEAL_PERCENT,
		HEAL_MISSING,
		DOUBLEHITS,
		TRIPLEHITS,
		MULTIHITS,
		TAUNT,

		// Specials
		CRITBOOST,

		// Status
		THORNS,
		DISABLE,
		DEATH_NOTICE,
		POISON_CHANCE,
	}

	enum SkillType_DEPRECATED
	{

		// Targeted Skills
		T_DAMAGE,
		T_DAMAGEHEAL,
		T_DAMAGERECOIL,
		T_DAMAGE_SELFSTAT,
		T_DAMAGE_ENEMYSTAT,
		T_DAMAGE_SELFSTAT_CHANCE,
		T_DAMAGE_ENEMYSTAT_CHANCE,
		T_DAMAGE_POISON_CHANCE,
		T_ENEMYSTAT,
		T_MULTIHITS,
		T_DOUBLE_HITS,

		// Non-Targeted Skills
		N_SELFSTAT,
		N_HEALSELF,
		N_HEALTEAM,
		N_CRITBOOST,
		N_STATRESET,

		// AOEs
		AOE_DAMAGE,
		AOE_DAMAGE_RANDOMSTAT,
		AOE_DAMAGE_SELFSTAT,
		AOE_DAMAGE_ENEMYSTAT,
		AOE_DAMAGE_SELFSTAT_CHANCE,
		AOE_DAMAGE_ENEMYSTAT_CHANCE,
		AOE_STAT,

		// Specials
		T_DEATHNOTICE,
		T_DEATHBLOSSOM,
		N_ARENAOFVALOR,
		N_TRICKROOM,
		N_MIMIC,
		T_DISABLE,
		T_TAUNT,
		T_GRASPINGTHORNS,

	}

	internal class Skill
	{

		#region Fields

		readonly static Skill none = new Skill("Unknown", 0, 0, 0, new List<SkillAction>(), SkillTarget.NONE, new Dictionary<Stats, int>() { }, false);

		readonly string name;
		readonly int baseDamage;
		readonly float accuracy;
		int skillPoint;

		readonly bool isDamaging;
		readonly bool isUltimate;
		readonly bool isRepeating;

		readonly SkillTarget skillTarget;
		readonly List<SkillAction> skillActions;
		readonly Dictionary<Stats, int> statEffects;

		#endregion

		#region Properties

		public string Name => name;
		public int BaseDamage => baseDamage;
		public int SkillPoint => skillPoint;
		public float Accuracy => accuracy;
		public Dictionary<Stats, int> StatEffects => statEffects;
		public SkillTarget SkillTarget => skillTarget;
		public List<SkillAction> SkillActions => skillActions;
		public bool IsDamaging => isDamaging;
		public bool IsUltimate => isUltimate;
		public bool IsDisabled { get; set; }

		public static Skill None => none;

		#endregion

		#region Constructor

		public Skill(
			string name, int baseDamage, int skillPoint, float accuracy,
			List<SkillAction> skillActions, SkillTarget skillTarget,
			Dictionary<Stats, int> statEffects, bool isUltimate)
		{
			this.name = name;
			this.baseDamage = baseDamage;
			this.skillPoint = skillPoint;
			this.accuracy = accuracy;
			this.skillActions = skillActions;
			this.skillTarget = skillTarget;
			this.statEffects = statEffects;
			this.isUltimate = isUltimate;
			isDamaging = baseDamage > 0;
			isRepeating = skillActions.Contains(SkillAction.DOUBLEHITS) || skillActions.Contains(SkillAction.TRIPLEHITS) || skillActions.Contains(SkillAction.MULTIHITS);
		}

		public Skill(Skill skill)
			: this(skill.name, skill.baseDamage, skill.skillPoint, skill.accuracy, skill.skillActions, skill.skillTarget, skill.statEffects, skill.isUltimate)
		{ }

		#endregion

		public bool Activate(Hero hero, Battle battle)
		{
			if (skillPoint < 1)
			{
				Utils.WriteEmbeddedColorLine("[red]Skill has no more Skill Points![/red]");
				return false;
			}

			if (IsDisabled)
			{
				Utils.WriteEmbeddedColorLine("[red]" + name + " is disabled![/red]");
				return false;
			}

			Hero target = Hero.None;

			if (skillTarget == SkillTarget.TARGET)
			{
				if (battle.OpposingTeam.Count == 1)
					target = battle.OpposingTeam.First();
				else if (hero.PriorityTarget != Hero.None && hero.PriorityTarget.IsAlive)
					target = hero.PriorityTarget;
				else
					target = battle.OpposingTeam[Utils.GetInteger(1, battle.OpposingTeam.Count, "[yellow]Target:[/yellow] ") - 1];
			}

			Utils.WriteEmbeddedColorLine("\n" + hero.Name + " does [cyan]" + name + "[/cyan]!");

			if (skillTarget == SkillTarget.ALLYTEAM)
			{
				for (int i = 0; i < hero.Team.Count; i++)
				{
					target = hero.Team[i];
					if (RollAccuracy(true) || isRepeating)
						DoEffects(hero, target, battle);
				}
			}
			else if (skillTarget == SkillTarget.ENEMYTEAM)
			{
				for (int i = 0; i < battle.OpposingTeam.Count; i++)
				{
					target = battle.OpposingTeam[i];
					if (RollAccuracy(true) || isRepeating)
						DoEffects(hero, target, battle);
				}
			}
			else
			{
				do
				{
					if (RollAccuracy(true) || isRepeating)
						DoEffects(hero, target, battle);
				} while (false);
			}

			skillPoint--;
			return true;
		}

		void RepeatedDamage(Hero hero, Hero target, int repeat)
		{
			int sum = 0;
			int count = 0;
			for (int j = 0; j < repeat; j++)
			{
				if (!RollAccuracy(false)) { continue; }
				if (!target.IsAlive) { break; }

				count++;
				bool willCrit = Utils.RollBoolean(hero.CurrentStats[Stats.CRIT_CHANCE] * 0.01F);
				Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} hits! {1}", count, willCrit ? "[red]It's a critical hit![/red]" : "").ToString());
				int damage = Hero.CalculateDamage(baseDamage, hero, target, willCrit);
				sum += damage;
			}
			Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("A total of {0} damage!", sum).ToString());
			target.TakeDamage(sum, false);
		}

		void DoEffects(Hero hero, Hero target, Battle battle)
		{
			for (int i = 0; i < skillActions.Count; i++)
			{
				switch (skillActions[i])
				{
					case SkillAction.DAMAGE:
					{
						// If skill is an exclusively DAMAGE skill, then it receives a bonus 10% crit chance.
						int temp = hero.CurrentStats[Stats.CRIT_CHANCE];
						if (skillActions.Count == 1) { temp += 10; }
						bool willCrit = Utils.RollBoolean(temp * 0.01F);
						target.TakeDamage(baseDamage, hero.CurrentStats[Stats.ATTACK], true, willCrit);
						break;
					}
					case SkillAction.DAMAGE_HEAL:
					{
						bool willCrit = Utils.RollBoolean(hero.CurrentStats[Stats.CRIT_CHANCE]);
						int damage = Hero.CalculateDamage(baseDamage, hero, target, willCrit);
						target.TakeDamage(damage, true);
						hero.Heal(Convert.ToInt32(damage * 0.5F), true);
						break;
					}
					case SkillAction.DAMAGE_RECOIL:
					{
						bool willCrit = Utils.RollBoolean(hero.CurrentStats[Stats.CRIT_CHANCE]);
						int damage = Hero.CalculateDamage(baseDamage, hero, target, willCrit);
						int recoil = Convert.ToInt32(damage * 0.25F);
						target.TakeDamage(damage, true);
						Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} is hurt by {1} recoil damage.", hero.Name, recoil).ToString());
						hero.TakeDamage(recoil, false);
						break;
					}
					case SkillAction.DAMAGE_IGNORE_DEFENSE:
					{
						bool willCrit = Utils.RollBoolean(hero.CurrentStats[Stats.CRIT_CHANCE]);
						int defense = Math.Clamp(hero.CurrentStats[Stats.ATTACK], 0, hero.BaseStats[Stats.DEFENSE]);
						int damage = Hero.CalculateDamage(baseDamage, willCrit, hero.CurrentStats[Stats.ATTACK], defense, target.BaseStats[Stats.DEFENSE]);
						target.TakeDamage(damage, true);
						break;
					}
					case SkillAction.SELF_STAT:
					{
						foreach (KeyValuePair<Stats, int> stat in StatEffects)
						{
							hero.ModifyStats(stat.Key, stat.Value);
						}
						break;
					}
					case SkillAction.SELF_STAT_CHANCE:
					{
						if (Utils.RollBoolean(0.5F)) { break; }
						foreach (KeyValuePair<Stats, int> stat in StatEffects)
						{
							hero.ModifyStats(stat.Key, stat.Value);
						}
						break;
					}
					case SkillAction.ENEMY_STAT:
					{
						foreach (KeyValuePair<Stats, int> stat in StatEffects)
						{
							target.ModifyStats(stat.Key, stat.Value);
						}
						break;
					}
					case SkillAction.ENEMY_STAT_CHANCE:
					{
						if (Utils.RollBoolean(0.75F)) { break; }
						foreach (KeyValuePair<Stats, int> stat in StatEffects)
						{
							target.ModifyStats(stat.Key, stat.Value);
						}
						break;
					}
					case SkillAction.DOUBLEHITS:
					{
						RepeatedDamage(hero, target, 2);
						break;
					}
					case SkillAction.TRIPLEHITS:
					{
						RepeatedDamage(hero, target, 3);
						break;
					}
					case SkillAction.MULTIHITS:
					{
						RepeatedDamage(hero, target, Utils.Random.Next(2, 5));
						break;
					}
					case SkillAction.HEAL_PERCENT:
					{
						hero.Heal(0.01F * baseDamage, false, true);
						break;
					}
					case SkillAction.HEAL_MISSING:
					{
						hero.Heal(0.01F * baseDamage, true, true);
						break;
					}
					case SkillAction.THORNS:
					{
						int duration = Utils.Random.Next(2, 5);
						Status status = new Status("Thorns", target, hero, this, StatusType.N_DAMAGE_PER_TURN, duration, UpdateType.TURN);
						if (target.AddStatus(status))
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} summoned a cluster of thorns around {1}!", hero.Name, target.Name).ToString());
						break;
					}
					case SkillAction.DEATH_NOTICE:
					{
						Status status = new Status("Death Notice", target, hero, this, StatusType.N_DEATH_NOTICE, 3, UpdateType.TURN);
						if (target.AddStatus(status))
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} became subjected to [cyan]Death Notice[/cyan]!", target.Name).ToString());
						break;
					}
					case SkillAction.DISABLE:
					{
						Status status = new Status("Disable", target, hero, this, StatusType.N_DISABLE, 2, UpdateType.TURN);
						if (target.LastSkillUsed == Skill.None)
						{
							Utils.Error("But it failed...");
							break;
						}
						if (target.AddStatus(status))
						{
							target.LastSkillUsed.IsDisabled = true;
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0}'s [cyan]{1}[/cyan] has been disabled!", target.Name, target.LastSkillUsed.Name).ToString());
						}
						break;
					}
					case SkillAction.CRITBOOST:
					{
						Status status = new Status("Crit Boost", hero, hero, this, StatusType.P_CRITBOOST, 5, UpdateType.POST);
						if (hero.AddStatus(status))
						{
							hero.CurrentStats[Stats.CRIT_CHANCE] += 10;
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0}'s [cyan]Crit Boost[/cyan] is heightened!", hero.Name).ToString());
						}
						break;
					}
					case SkillAction.TAUNT:
					{

						break;
					}
				}
			}
		}

		bool RollAccuracy(bool showText)
		{
			if (accuracy <= 0) return true;
			bool willHit = Utils.RollBoolean(accuracy);
			if (!willHit && showText)
				Utils.WriteEmbeddedColorLine("But it missed...");
			return willHit;
		}
	}
}
