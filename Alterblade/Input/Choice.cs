using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alterblade.Input
{
	internal class Choice
	{
		string text;
		Callback callback;
		public string Text { get { return text; } }
		public Callback Callback { get { return callback; } }
		public Choice(string text, Callback callback)
		{
			this.text = text;
			this.callback = callback;
		}
	}
}
