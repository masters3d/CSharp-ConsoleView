using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ConsoleView
{
    [Serializable]
    public class Display
    {
        public string LastShow = " ";
        public List<String> CommandHistory = new List<String>();


        private string lastConsoleTitle = "";
        public void SetConsoleTitle(string title)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            title = rgx.Replace(title, "");

            // Only update if content is different
            if (title == lastConsoleTitle)
            {
                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Title = title;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var space = " ";
                var command = @"printf '\033]0;" + title + space + @"\007'";
                system(command);
            }

            lastConsoleTitle = title;
        }

        public string GetConsoleTitle()
        {
            return lastConsoleTitle;
        }

        // This is used for setting the bash command window size on non WinOS
        [DllImport("libc")]
        private static extern int system(string exec);

        private int lastConsoleHeight = 0;
        private int lastConsoleWidth = 0;

        public void SetConsoleSize(int width, int height)
        {

            // Only update if the height are going to be diffefent
            if ( width == lastConsoleWidth && height == lastConsoleHeight)
            {
                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WindowWidth = width;
                Console.BufferWidth = Console.WindowWidth + 1;
                Console.BufferHeight = Console.WindowHeight = height;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                system(@"printf '\e[8;" + height + ";" + width + "t';");
            }
        }

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

        public string CommandPrompt(string promptText = "<|: ", bool shouldClearScreen = true, ConsoleColor backgroundColor = ConsoleColor.Black, ConsoleColor textColor = ConsoleColor.White )
		{
            Output(promptText, fg: textColor, bg: backgroundColor);
            string result = Console.ReadLine();
            CommandHistory.Insert(0, result);
            if (shouldClearScreen)
            {
                Console.Clear();

            }
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

        // Method adapted from https://rianjs.net/2016/03/line-wrapping-at-word-boundaries-for-console-applications-in-csharp
        public static List<string> WrapText(string paragraph, int width)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(paragraph))
            {
                return result;
            }

            paragraph = paragraph.Replace("\r\n", "↓");
            paragraph = paragraph.Replace('\n', '↓');
            char[] splitOn = { '↓', };
            string[] splitParagraph = paragraph.Split(splitOn);

            foreach (string para in splitParagraph)
            {
                int approxLineCount = para.Length / width;
                var lines = new List<string>();

                if (para.Length == 0)
                {
                    result.Add("".PadRight(width));
                }

                for (var i = 0; i < para.Length;)
                {
                    int grabLimit = Math.Min(width, para.Length - i);
                    string line = para.Substring(i, grabLimit);

                    var isLastChunk = grabLimit + i == para.Length;

                    if (isLastChunk)
                    {
                        i = i + grabLimit;
                        lines.Add(line.PadRight(width));
                    }
                    else
                    {
                        var lastSpace = line.LastIndexOf(" ", StringComparison.Ordinal);
                        lines.Add(line.Substring(0, lastSpace));

                        //Trailing spaces needn't be displayed as the first character on the new line
                        i = i + lastSpace + 1;
                    }
                }
                result.AddRange(lines);
            }
            return result;
        }

        [System.Obsolete("Wrap is deprecated, please use WrapText instead.")]
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

    [Serializable]
    public struct ConsoleChar
    {
       public ConsoleColor BackgroundColor;
       public ConsoleColor TextColor;
       public Char Character;

        public void Output(Display display = null)
        {
            display = display ?? new Display();
            display.Output(Character.ToString(),TextColor, BackgroundColor);
        }
    }

    [Serializable]
    public struct ConsoleText
    {
        public List<ConsoleChar> list;


        public ConsoleText(List<ConsoleChar> list)
        {
            this.list = list;
        }

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

        public void Show(Display display = null) {
            Output(display);
            Console.WriteLine();
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

    [Serializable]
    public struct ConsoleBox
    {
        public int positionX;
        public int positionY;
        public List<ConsoleText> list;

        public ConsoleBox(List<ConsoleText> list, int positionX = 0, int positionY = 0)
        {
            this.positionX = positionX;
            this.positionY = positionY;
            this.list = list;
        }

        public ConsoleBox(string text, int width, int height, ConsoleColor bgColor, ConsoleColor textColor, int positionX = 0, int positionY = 0)
        {
            this.positionX = positionX;
            this.positionY = positionY;

            List<ConsoleText> listToSave = new List<ConsoleText>();
            var wrappedTextList = Display.WrapText(text, width);

            for (var i = 0; i < height; i += 1)
            {
                if (i > wrappedTextList.Count - 1)
                {
                    var consoleText = new ConsoleText();
                    consoleText.Append("".PadRight(width), bgColor, textColor);
                    listToSave.Add(consoleText);
                }
                else
                {
                    var consoleText = new ConsoleText();
                    consoleText.Append(wrappedTextList[i].PadRight(width), bgColor, textColor);
                    listToSave.Add(consoleText);
                }
            }
            this.list = listToSave;

        }
    }
}

