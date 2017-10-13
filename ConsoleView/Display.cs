using System;
using System.Collections.Generic;

namespace ConsoleView
{
    public class Display
    {
        public string LastShow = " ";
        public List<String> CommandHistory = new List<String>();

        private void WriteLine(string text)
        {
            Console.WriteLine(text);
            LastShow = text;
        }

        private void Write(string text)
        {
            Console.Write(text);
            LastShow = text;
        }

        public void Show(string text, ConsoleColor txColor = ConsoleColor.White, ConsoleColor bg = ConsoleColor.Black)
        {
			Console.BackgroundColor = bg;
			Console.ForegroundColor = txColor;
            WriteLine(text);
            Console.ResetColor();
        }

        public void Show(List<string> textList, ConsoleColor txColor = ConsoleColor.Green, ConsoleColor bg = ConsoleColor.Black)
        {
            string temp = "";
            foreach (string key in textList)
            {
                temp += key;
                temp += Environment.NewLine;
            }
            Show(temp, txColor, bg);
        }

        public void Output(string text, ConsoleColor fg = ConsoleColor.White, ConsoleColor bg = ConsoleColor.Black)
        {
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            Write(text);
            Console.ResetColor();
        }

        public string CommandPrompt(string promptText = "<|: ")
		{
            Output(promptText);
            string result = Console.ReadLine();
            CommandHistory.Insert(0, result);
            Console.Clear();
            return result;

		}

        public static List<String> Indent(int indentAmmount, List<String> list)
        {
			var inputList = new List<String>();

			foreach (var each in list)
			{
				inputList.Add(new string(' ', indentAmmount) + each);
			}

            return inputList;
		}


		public static List<string> Wrap(string str, int maxWidth, int upperMargin = 1, int lowerMargin = 0)
		{
			List<string> output = new List<string>();
			str = new string(' ', upperMargin) + str;
			string lower = new string(' ', lowerMargin);
			while (str.Substring(str.Length - 1) == "\n") str = str.Substring(0, str.Length - 1);
			while (str.Length > maxWidth || str.IndexOf("\n") != -1 || str[0] == '\n')
			{
				int lineBreak = str.IndexOf("\n");
				int lastSpace = str.Substring(0, maxWidth).LastIndexOf(" ");
				int space;
				if (lineBreak == 0)
				{
					output.Add("");
					str = new string(' ', upperMargin) + str.Substring(1);
					lastSpace = str.Substring(0, maxWidth).LastIndexOf(" ");
				}
				if (lineBreak > 0 && lineBreak < maxWidth) space = lineBreak;
				else if (lastSpace > upperMargin && lastSpace < maxWidth) space = lastSpace;
				else space = maxWidth - 1;
				output.Add(str.Substring(0, space).PadRight(maxWidth));
				str = str.Substring(space);
				if (space < str.Length && str[0] != '\n') str = new string(' ', lowerMargin) + str;
			}
			output.Add(str.PadRight(maxWidth));
			return output;
		}
    }

    public struct ConsoleChar
    {
       public ConsoleColor BackgroundColor;
       public ConsoleColor TextColor;
       public Char Character;

        public void Output(Display display = null)
        {
            display = display ?? new Display();
            display.Output(Character.ToString(),BackgroundColor, TextColor);
        }
    }

    public struct ConsoleText
    {
        private List<ConsoleChar> list;

        public int GetCount()
        {
            return list.Count;
        }

        public void Append(string text, ConsoleColor BackgroundColor, ConsoleColor TextColor)
        {
            list = list ?? new List<ConsoleChar>();

            foreach( var character in text)
            {
                list.Add(new ConsoleChar(){Character = character, BackgroundColor = BackgroundColor, TextColor = TextColor});
            }
        }

        public void Output(Display display = null)
        {
            display = display ?? new Display();

            foreach (var each in list)
            {
                each.Output(display);
            }
        }

    }

    public struct ConsoleBox
    {
        public int positionX;
        public int positionY;
        public List<ConsoleText> list;

    }

}

