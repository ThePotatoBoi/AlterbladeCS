using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alterblade.Input
{

    public delegate void Callback();
    internal class Dialogue
	{

		List<Choice> choices;

        public Dialogue(params Choice[] choices)
		{
			this.choices = new List<Choice>();
            foreach (Choice choice in choices)
				this.choices.Add(choice);
		}

        public void Choose()
		{
			StringBuilder output = new StringBuilder();
			output.Append('\n');
			for ( int i = 0; i < choices.Count; i++ )
				output.AppendFormat("  {0} {1}\n", i + 1, choices[i].Text);
			Utils.WriteEmbeddedColorLine(output.ToString());
			int index = Utils.GetInteger(1, choices.Count, "█ Input: ") - 1;
			Utils.ClearScreen();
			choices[index].Callback();
		}
    }
}
