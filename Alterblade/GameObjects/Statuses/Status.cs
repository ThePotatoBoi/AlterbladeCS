using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alterblade.GameObjects.Statuses
{
	internal abstract class Status
	{
		protected string name;
		protected int duration;
		protected Hero source;
		protected Skill sourceSkill;
		protected UpdateType updateType;

		public string Name => name; 
		public int Duration => duration;
		public Hero Source => source;
		public Skill SourceSkill => sourceSkill;
		public UpdateType UpdateType => updateType;

		public Status(string name, int duration, Hero source, Skill sourceSkill, UpdateType updateType)
		{
			this.name = name;
			this.duration = duration;
			this.source = source;
			this.sourceSkill = sourceSkill;
			this.updateType = updateType;
		}
		public abstract bool End(bool showText);
		public abstract  bool Update();
	}
}
