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
        private ConsoleColor TitleTextColor = ConsoleColor.White;
        private ConsoleColor TitleBgColor = ConsoleColor.DarkRed;
        public int TitleHeight = 2;

        private WindowArea AreaA;
		private WindowArea AreaB;
		private WindowArea AreaC;
        private WindowArea AreaD;
        private ViewLayoutType type;

        public int width;
        public int height;



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

        public WindowArea GetTextBox(Area area)
        {
            switch (area)
            {
                case Area.A:
                    return AreaA;
                case Area.B:
                    return AreaB;
                case Area.C:
                    return AreaC;
                default:
                    return new WindowArea(dummyTexBox: true);
            }
        }

        public void SetTitle(string input)
        {
            Title = input;
        }

        public void SetArea(String title, String input, Area area)
        {
            var toDisplay1 =  Display.Wrap(title, GetTextBox(area).width);
            var toDisplay2 = Display.Indent(5, Display.Wrap(input, GetTextBox(area).width - 5));

            toDisplay1.AddRange(toDisplay2);

            SetArea(toDisplay1, area);
		}


        public void SetArea(string input, Area area)
        {
            var wrappedText = Display.Wrap(input, AreaA.width);
            SetArea(wrappedText, area);
        }

        public void SetArea(string title, List<String> inputListToEdit, Area area, int indent = 5, bool spaceBetween = true )
        {
            var inputList = Display.Indent(indent, inputListToEdit);
			if (spaceBetween)
			{
				inputList.Insert(0, "");
			}
            inputList.Insert(0, title);

			SetArea(inputList, area);
		}

        public void SetArea(List<String> inputList, Area area)
		{
            WindowArea areaToEdit;


            switch (area) 
            {
                case Area.A:
                    areaToEdit = AreaA;
					AreaA = new WindowArea(areaToEdit.width, areaToEdit.height, inputList, areaToEdit.textColor, areaToEdit.bgColor);
					break;
				case Area.B:
					areaToEdit = AreaB;
					AreaB = new WindowArea(areaToEdit.width, areaToEdit.height, inputList, areaToEdit.textColor, areaToEdit.bgColor);
					break;
				case Area.C:
					areaToEdit = AreaC;
					AreaC = new WindowArea(areaToEdit.width, areaToEdit.height, inputList, areaToEdit.textColor, areaToEdit.bgColor);
					break;
                case Area.D:
                    areaToEdit = AreaD;
                    AreaD = new WindowArea(areaToEdit.width, areaToEdit.height, inputList, areaToEdit.textColor, areaToEdit.bgColor);
                    break;
                default:
                    areaToEdit = new WindowArea(dummyTexBox: true);
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

            display.Show(Title.PadRight(this.width), TitleTextColor, TitleBgColor);


            if (type == ViewLayoutType.twoStackOneLong) {

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

            if (type == ViewLayoutType.singleView || type == ViewLayoutType.drawOnSingleView) 
            {
                foreach(var each in AreaA.processedTextList)
                {
                    display.Show(each, AreaA.textColor, AreaA.bgColor);
                }
                return display.CommandPrompt();
            }


            display.Show("View Set Up not Supported");
            return "Quit";
        }

        public View(string title, List<string> textA = null, List<string> textB = null, List<string> textC = null, List<string> textD = null,  ViewLayoutType viewLayoutType = ViewLayoutType.twoStackOneLong, int width = 100, int height = 30)
        {
            textA = textA ?? new List<string>();
            textB = textB ?? new List<string>();
            textC = textC ?? new List<string>();
            textD = textD ?? new List<string>();

            type = viewLayoutType;
            Title = title;
            this.width = width;
            this.height = height + this.TitleHeight;

            if (viewLayoutType == ViewLayoutType.twoStackOneLong)
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

                int sixtyWidth = (int)Math.Ceiling(0.6 * width);
                int fortyWidth = (int)Math.Ceiling(0.4 * width);
                int fiftyHeight = (int) Math.Ceiling(0.5 * height);
                AreaA = new WindowArea(sixtyWidth, fiftyHeight, textA, ConsoleColor.White, ConsoleColor.DarkGreen);
                AreaB = new WindowArea(fortyWidth, height, textB, ConsoleColor.White, ConsoleColor.DarkBlue);
                AreaC = new WindowArea(sixtyWidth, fiftyHeight, textC, ConsoleColor.White, ConsoleColor.Black);
            }

            if (viewLayoutType == ViewLayoutType.singleView || viewLayoutType == ViewLayoutType.drawOnSingleView )
            {
                AreaA = new WindowArea(width, height, textA, ConsoleColor.White, ConsoleColor.DarkGreen);
            }

        }
    }
}

public struct WindowArea
{
    public int width;
    public int height;
    public List<string> processedTextList;
    public List<String> sourceText;
    public ConsoleColor textColor;
    public ConsoleColor bgColor;

    public WindowArea(bool dummyTexBox = true)
    {
        width = height = 10;
        processedTextList = sourceText = new List<string>();
        textColor = bgColor = ConsoleColor.White;
    }

    public WindowArea(int width, int height, List<string> wrappedTextList, ConsoleColor textColor, ConsoleColor bgColor)
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

public enum ViewLayoutType
{
    twoStackOneLong, singleView, drawOnSingleView
}

public enum Area
{
    A, B, C, D
}


