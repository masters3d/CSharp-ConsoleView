using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleView
{
    public class View
    {
        private Display display = new Display();

        private String Title;
        private TextBox AreaA;
		private TextBox AreaB;
		private TextBox AreaC;
        private ViewType type;

        public int width;
        public int height;

        public int titleHeight = 3;


		// This is used for setting the bash command window size on non WinOS
		[DllImport("libc")]
		private static extern int system(string exec);


        public List<String> GetCommandHistory(int lengthLimit, bool reverse = true)
        {
            var temp = new List<String>();

            for (var i = 0; i < lengthLimit; i += 1)
			{
                var historyCount = display.CommandHistory.Count;
				if (i >= historyCount || historyCount == 0)
				{
					temp.Insert(i, "");
				}
				else
				{
					var each = display.CommandHistory[i];
					temp.Add(each);
				}
			}
            if (reverse)
            {
             temp.Reverse();   
            }
			
        return temp;

        }

        public TextBox GetTextBox(TextBoxArea area)
        {
            switch (area)
            {
                case TextBoxArea.A:
                    return AreaA;
                case TextBoxArea.B:
                    return AreaB;
                case TextBoxArea.C:
                    return AreaC;
                default:
                    return new TextBox(dummyTexBox: true);
            }
        }

        public void SetTitle(string input)
        {
            Title = input;
        }

        public void SetArea(String title, String input, TextBoxArea area)
        {
            var toDisplay1 =  Display.Wrap(title, GetTextBox(area).width);
            var toDisplay2 = Display.Indent(5, Display.Wrap(input, GetTextBox(area).width - 5));

            toDisplay1.AddRange(toDisplay2);

            SetArea(toDisplay1, area);
		}


        public void SetArea(string input, TextBoxArea area)
        {
            var wrappedText = Display.Wrap(input, AreaA.width);
            SetArea(wrappedText, area);
        }

        public void SetArea(string title, List<String> inputListToEdit, TextBoxArea area, int indent = 5, bool spaceBetween = true )
        {
            var inputList = Display.Indent(indent, inputListToEdit);
			if (spaceBetween)
			{
				inputList.Insert(0, "");
			}
            inputList.Insert(0, title);

			SetArea(inputList, area);
		}

        public void SetArea(List<String> inputList, TextBoxArea area)
		{
            TextBox areaToEdit;


            switch (area) 
            {
                case TextBoxArea.A:
                    areaToEdit = AreaA;
					AreaA = new TextBox(areaToEdit.width, areaToEdit.height, inputList, areaToEdit.textColor, areaToEdit.bgColor);
					break;
				case TextBoxArea.B:
					areaToEdit = AreaB;
					AreaB = new TextBox(areaToEdit.width, areaToEdit.height, inputList, areaToEdit.textColor, areaToEdit.bgColor);
					break;
				case TextBoxArea.C:
					areaToEdit = AreaC;
					AreaC = new TextBox(areaToEdit.width, areaToEdit.height, inputList, areaToEdit.textColor, areaToEdit.bgColor);
					break;
                default:
                    areaToEdit = new TextBox(dummyTexBox: true);
                    break;
            }
		}

        public string UpdateScreenAndGetInput()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WindowWidth = this.width;
                Console.BufferWidth = Console.WindowWidth + 1;
                Console.BufferHeight = Console.WindowHeight = this.height;
            }

			if (display.CommandHistory.Count == 0 && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				system(@"printf '\e[8;" + this.height + ";" + this.width + "t';");
			}

            if (type == ViewType.twoStackOneLong) {

                display.Show(Title.PadRight(this.width), ConsoleColor.DarkRed, ConsoleColor.White);

				for (var i = 0; i < AreaA.height; i += 1)
				{
					display.Output(AreaA.processedTextList[i], AreaA.textColor, AreaA.bgColor);
					display.Output(AreaB.processedTextList[i], AreaB.textColor, AreaB.bgColor);
					Console.WriteLine();
				}

				for (var i = 0; i < AreaC.height; i += 1)
				{
					display.Output(AreaC.processedTextList[i], AreaC.textColor, AreaC.bgColor);
					display.Output(AreaB.processedTextList[i + AreaA.height], AreaB.textColor, AreaB.bgColor);
					Console.WriteLine();
				}

				return display.CommandPrompt();
            }

            display.Show("View Set Up not Supported");
            return "Quit";
        }

        public View(string title, List<string> textA, List<string> textB, List<string> textC, ViewType viewType = ViewType.twoStackOneLong )
        {
            if (viewType == ViewType.twoStackOneLong)
            {
                //+--------Title---------------- +
                //+-----------------------------+
                //|               |             |
                //| areaA         |             |
                //|               |             |
                //|               | areaB       |
                //+---------------+             |
                //|               |             |
                //| areaC         |             |
                //|               |             |
                //+---------------+-------------+
                //+--------------Command-------- +


                AreaA = new TextBox(60, 15, textA, ConsoleColor.White, ConsoleColor.DarkGreen);
                AreaB = new TextBox(40, 30, textB, ConsoleColor.White, ConsoleColor.DarkBlue);
                AreaC = new TextBox(60, 15, textC, ConsoleColor.White, ConsoleColor.Black);
                type = viewType;
                Title = title;

                var windowWidth = AreaA.width + AreaB.width;
                var windowHeight = AreaB.height + this.titleHeight;

                width = windowWidth;
                height = windowHeight;

            }
        }
    }
}

public struct TextBox
{
    public int width;
    public int height;
    public List<string> processedTextList;
    public List<String> sourceText;
    public ConsoleColor textColor;
    public ConsoleColor bgColor;

    public TextBox(bool dummyTexBox = true)
    {
        width = height = 10;
        processedTextList = sourceText = new List<string>();
        textColor = bgColor = ConsoleColor.White;

    }

    public TextBox(int width, int height, List<string> wrappedTextList, ConsoleColor textColor, ConsoleColor bgColor)
    {
        var processedText = new List<String>();
        for (var i = 0; i < height; i += 1)
        {
            if(i > wrappedTextList.Count - 1)
            {
                processedText.Add("".PadRight(width));
            } else 
            {
                processedText.Add(wrappedTextList[i].PadRight(width));
            }
        }
        this.width = width;
        this.height = height;
        processedTextList = processedText;
        sourceText = wrappedTextList;
        this.textColor = textColor;
        this.bgColor = bgColor;

    }
}

public enum ViewType
{
    twoStackOneLong
}

public enum TextBoxArea
{
    A, B, C, D
}
