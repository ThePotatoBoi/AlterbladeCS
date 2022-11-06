using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Alterblade
{
	internal static class Utils
	{

		public static readonly Random Random = new Random();

		/// <summary>
		/// Console Color Helper provides coloring to individual Console commands!
		/// Credits to @RickStrahl from https://gist.github.com/RickStrahl/52c9ee43bd2723bcdf7bf4d24b029768
		/// </summary>
		#region ColorConsole

		private static Lazy<Regex> colorBlockRegEx = new Lazy<Regex>(
			delegate () { return new Regex("\\[(?<color>.*?)\\](?<text>[^[]*)\\[/\\k<color>\\]", RegexOptions.IgnoreCase); },
			isThreadSafe: true
		);

		public static void WriteLine(string text, ConsoleColor? color = null)
		{
			if (color.HasValue)
			{
				var oldColor = Console.ForegroundColor;
				if (color == oldColor)
					Console.WriteLine(text);
				else
				{
					Console.ForegroundColor = color.Value;
					Console.WriteLine(text);
					Console.ForegroundColor = oldColor;
				}
			}
			else Console.WriteLine(text);
		}

		public static void Write(string text, ConsoleColor? color = null, bool isColorBackground = false)
		{
			if (color.HasValue)
			{
				var oldColor = Console.ForegroundColor;
				var oldBgColor = Console.BackgroundColor;
				if (color == oldColor)
					Console.Write(text);
				else
				{
					if (isColorBackground) { Console.BackgroundColor = color.Value; } else { Console.ForegroundColor = color.Value; }
					Console.Write(text);
					if (isColorBackground) { Console.BackgroundColor = oldBgColor; } else { Console.ForegroundColor = oldColor; }
				}
			}
			else Console.Write(text);
		}

		public static void Write(string text, string color, bool isColorBackground = false)
		{
			if (string.IsNullOrEmpty(color))
			{
				Write(text);
				return;
			}

			if (!Enum.TryParse(color, true, out ConsoleColor col))
				Write(text);
			else
				Write(text, col, isColorBackground);
		}

		public static void Error(string str, bool terminate = false)
		{
			WriteLine(str, ConsoleColor.Red);
			if (terminate) Environment.Exit(0);
		}

		public static void Warn(string str)
		{
			WriteLine(str, ConsoleColor.Yellow);
		}

		public static void WriteEmbeddedColorLine(string text, ConsoleColor? baseTextColor = null)
		{
			WriteEmbeddedColor(text, baseTextColor);
			Console.WriteLine();
		}

		public static void WriteEmbeddedColor(string text, ConsoleColor? baseTextColor = null)
		{
			if (baseTextColor == null) { baseTextColor = Console.ForegroundColor; }

			if (string.IsNullOrEmpty(text))
			{
				Write(string.Empty);
				return;
			}

			int at = text.IndexOf("[");
			int at2 = text.IndexOf("]");
			if (at == -1 || at2 <= at)
			{
				Write(text, baseTextColor);
				return;
			}

			while (true)
			{
				var match = colorBlockRegEx.Value.Match(text);
				if (match.Length < 1)
				{
					Write(text, baseTextColor);
					break;
				}
				bool isBg = false;
				Write(text.Substring(0, match.Index), baseTextColor);
				string highlightText = match.Groups["text"].Value;
				if (highlightText.Length > 0 && highlightText[0] == '~')
				{
					isBg = true;
					highlightText = highlightText.Substring(1);
				}
				string colorVal = match.Groups["color"].Value;
				Write(highlightText, colorVal, isBg);
				text = text.Substring(match.Index + match.Value.Length);
			}

		}

		#endregion

		public static void ClearScreen()
		{
#if DEBUG
			Utils.WriteEmbeddedColorLine("\n[red]~ <Clear Screen> [/red]");
#else
				Console.Clear();
#endif
			Console.WriteLine();
		}

		public static void Delay(int ms)
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(ms));
			while (Console.KeyAvailable)
			{
				Console.ReadKey(true);
			}
		}

		public static int GetInteger(int min, int max, string query = "")
		{
			while (true)
			{
				if (query.Length > 0) { WriteEmbeddedColor(query); }
				int input;
				if (int.TryParse(Console.ReadLine(), out input))
				{
					if (input >= min && input <= max) return input;
				}
				Error("Input is invalid. Please try again.");
			}
		}

		public static bool RollBoolean(float chance)
		{
			return Random.NextDouble() < chance;
		}
	}
}