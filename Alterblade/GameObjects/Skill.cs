﻿using Alterblade.GameObjects.Statuses;
using Alterblade.Modes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Alterblade.GameObjects
{
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
			Dictionary<Stats, int> statEffects, bool isUltimate
		) {
			this.name = name;
			this.baseDamage = baseDamage;
			this.skillPoint = skillPoint;
			this.accuracy = accuracy;
			this.skillActions = skillActions;
			this.skillTarget = skillTarget;
			this.statEffects = statEffects;
			this.isUltimate = isUltimate;
			isDamaging = baseDamage > 0;
			isRepeating = skillActions.Contains(SkillAction.DOUBLEHITS)
				|| skillActions.Contains(SkillAction.TRIPLEHITS)
				|| skillActions.Contains(SkillAction.MULTIHITS);
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
					hero = hero.Team[i];
					DoEffects(hero, target, battle);
				}
			}
			else if (skillTarget == SkillTarget.ENEMYTEAM)
			{
				for (int i = 0; i < battle.OpposingTeam.Count; i++)
				{
					target = battle.OpposingTeam[i];
					if (Utils.RollAccuracy(accuracy, 0, true) || isRepeating)
						DoEffects(hero, target, battle);
				}
			}
			else
			{
				if (Utils.RollAccuracy(accuracy, 0, true) || isRepeating)
					DoEffects(hero, target, battle);
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
				if (!Utils.RollAccuracy(accuracy, 0, true)) { continue; }

				count++;
				bool willCrit = Utils.RollBoolean(hero.CurrentStats[Stats.CRIT_CHANCE] * 0.01F);
				Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} hits! {1}", count, willCrit ? "[red]It's a critical hit![/red]" : "").ToString());
				int damage = Utils.CalculateDamage(baseDamage, willCrit, hero, target);
				sum += damage;
			}
			Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} receives a total of {1} damage!", target.Name, sum).ToString());
			target.TakeDamage(sum, false);
		}

		void DoEffects(Hero hero, Hero target, Battle battle)
		{
			for (int i = 0; i < skillActions.Count; i++)
			{
				bool isSecondary = i > 1;
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
						bool willCrit = Utils.RollBoolean(hero.CurrentStats[Stats.CRIT_CHANCE] * 0.01F);
						int damage = Utils.CalculateDamage(baseDamage, willCrit, hero, target);
						target.TakeDamage(damage, true, willCrit);
						hero.Heal(Convert.ToInt32(damage * 0.5F), true);
						break;
					}
					case SkillAction.DAMAGE_RECOIL:
					{
						bool willCrit = Utils.RollBoolean(hero.CurrentStats[Stats.CRIT_CHANCE] * 0.01F);
						int damage = Utils.CalculateDamage(baseDamage, willCrit, hero, target);
						int recoil = Convert.ToInt32(damage * 0.25F);
						target.TakeDamage(damage, true);
						Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} is hurt by {1} recoil damage.", hero.Name, recoil).ToString());
						hero.TakeDamage(recoil, false);
						break;
					}
					case SkillAction.DAMAGE_IGNORE_TARGET_DEFENSE:
					{
						target.TakeDamage(baseDamage, hero.CurrentStats[Stats.ATTACK], true, true);
						break;
					}
					case SkillAction.DAMAGE_SCALE_WITH_TARGET_ATTACK:
					{
						target.TakeDamage(baseDamage, target.CurrentStats[Stats.ATTACK], true);
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
						hero.Heal(0.01F * -baseDamage, false, true);
						break;
					}
					case SkillAction.HEAL_MISSING:
					{
						hero.Heal(0.01F * -baseDamage, true, true);
						break;
					}
					case SkillAction.THORNS:
					{
						HeroStatus status = new HeroStatus("Thorns", 5, target, hero, this, StatusType.N_DAMAGE_PER_TURN);
						if (target.AddStatus(status, !isSecondary))
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} summoned a cluster of thorns around {1}!", hero.Name, target.Name).ToString());
						break;
					}
					case SkillAction.FEEBLE:
					{
						HeroStatus status = new HeroStatus("Feeble", 3, target, hero, this, StatusType.N_FEEBLE);
						if (target.AddStatus(status, !isSecondary))
						{
							target.IsFeeble = true;
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} was put into [cyan]Feeble[/cyan]!", target.Name).ToString());
						}
						break;
					}
					case SkillAction.DEATH_NOTICE:
					{
						HeroStatus status = new HeroStatus("Death Notice", 3, target, hero, this, StatusType.N_DEATH_NOTICE);
						if (target.AddStatus(status, !isSecondary))
						{
							target.IsFeeble = true;
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} became subjected to [cyan]Death Notice[/cyan]!", target.Name).ToString());
						}
						break;
					}
					case SkillAction.DISABLE:
					{
						HeroStatus status = new HeroStatus("Disable", 2, target, hero, this, StatusType.N_DISABLE);
						if (target.LastSkillUsed == Skill.None)
							Utils.Error("But it failed...");
						else if (target.AddStatus(status, !isSecondary))
						{
							target.LastSkillUsed.IsDisabled = true;
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0}'s [cyan]{1}[/cyan] has been disabled!", target.Name, target.LastSkillUsed.Name).ToString());
						}
						break;
					}
					case SkillAction.CRITBOOST:
					{
						HeroStatus status = new HeroStatus("Crit Boost", 4, hero, hero, this, StatusType.P_CRITBOOST);
						if (hero.AddStatus(status, !isSecondary))
						{
							hero.CurrentStats[Stats.CRIT_CHANCE] += 20;
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0}'s [cyan]Crit Boost[/cyan] is heightened!", hero.Name).ToString());
						}
						break;
					}
					case SkillAction.TAUNT:
					{
						HeroStatus status = new HeroStatus("Taunt", 2, target, hero, this, StatusType.N_TAUNT);
						if (target.AddStatus(status, !isSecondary))
						{
							target.PriorityTarget = hero;
							Utils.WriteEmbeddedColorLine(new StringBuilder().AppendFormat("{0} fell on {1}'s [cyan]{2}[/cyan]!", target.Name, hero.Name, status.Name).ToString());
						}
						break;
					}
					case SkillAction.TRICKROOM:
					{
						BattleStatus status = new BattleStatus("Trick Room", 4, battle, hero, this, BattleStatusType.TRICKROOM);
						if (battle.AddBattleStatus(status))
						{
							StringBuilder output = new StringBuilder();
							battle.HeroQueueSort = HeroQueueSort.SPEED_REVERSED;
							output.AppendFormat("{0} started twisting the dimensions!", hero.Name);
							Utils.WriteEmbeddedColorLine(output.ToString());
						}
						else
							status.End(true);
						break;
					}
					case SkillAction.MIMIC:
					{
						Skill skill = target.LastSkillUsed;
						if (skill == Skill.None)
						{
							Utils.Error("But it failed...");
							break;
						}
						else if (skill.isUltimate)
						{
							Utils.Error("Skill is too powerful to copy!");
							break;
						}
						skill = new Skill(skill);
						skill.skillPoint++;
						hero.DoSkill(battle, skill);
						break;
					}
					case SkillAction.SCORCH:
					{
						BattleStatus status = new BattleStatus("Scorch", 3, battle, hero, this, BattleStatusType.SCORCH);
						if (battle.AddBattleStatus(status))
						{
							Utils.WriteEmbeddedColorLine(
								new StringBuilder().AppendFormat("The battlefield was put on [cyan]{0}[/cyan]!", status.Name).ToString()
							);
						}
						else
							status.End(true);
						break;
					}
				}
				if (isDamaging)
					target.LastSkillHit = this;
			}
		}
	}
}
