using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ConsoleView;

namespace ConsoleView
{
    public class View
    {
        public Display display = new Display();

        private String Title;
        private ConsoleColor TitleTextColor = ConsoleColor.White;
        private ConsoleColor TitleBgColor = ConsoleColor.DarkRed;
        private WindowArea AreaA;
		private WindowArea AreaB;
		private WindowArea AreaC;
        private WindowArea AreaD;
        private ViewLayoutType type;

        public int width;
        public int height;


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
                case Area.D:
                    return AreaD;
                default:
                    return new WindowArea(dummyTexBox: true);
            }
        }

        public void SetTitle(string input)
        {
            Title = input;
        }

        public void AddToDrawingArea(ConsoleBox box)
        {

            var toEdit = AreaA.consoleBox;
            var toEditWidth = AreaA.consoleBox[0].GetCount();
            var line = box.positionY;
            var column = box.positionX;
            var boxWidth = box.list[0].GetCount();
            var boxHeight = box.list.Count;

            for (var i = 0; i < boxHeight; i += 1)
            {
                var partA = toEdit[i + line].list.GetRange(0, column);
                var partB = box.list[i].list;
                var partC = toEdit[i + line].list.GetRange(column + boxWidth, toEditWidth - (column + boxWidth));
                var temp = new List<ConsoleChar>();

                temp.AddRange(partA);
                temp.AddRange(partB);
                temp.AddRange(partC);

                toEdit[i + line] = new ConsoleText(temp);

            }


        }

        public void SetArea(String title, String input, Area area)
        {
            var toDisplay1 =  Display.Wrap(title, GetTextBox(area).GetWidth());
            var toDisplay2 = Display.Indent(5, Display.Wrap(input, GetTextBox(area).GetWidth() - 5));

            toDisplay1.AddRange(toDisplay2);

            SetArea(toDisplay1, area);
		}


        public void SetArea(string input, Area area)
        {
            var wrappedText = Display.Wrap(input, AreaA.GetWidth());
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
                    AreaA = new WindowArea(areaToEdit.GetWidth(), areaToEdit.GetHeight(), inputList, areaToEdit.textColor, areaToEdit.bgColor);
					break;
				case Area.B:
					areaToEdit = AreaB;
                    AreaB = new WindowArea(areaToEdit.GetWidth(), areaToEdit.GetHeight(), inputList, areaToEdit.textColor, areaToEdit.bgColor);
					break;
				case Area.C:
					areaToEdit = AreaC;
                    AreaC = new WindowArea(areaToEdit.GetWidth(), areaToEdit.GetHeight(), inputList, areaToEdit.textColor, areaToEdit.bgColor);
					break;
                case Area.D:
                    areaToEdit = AreaD;
                    AreaD = new WindowArea(areaToEdit.GetWidth(), areaToEdit.GetHeight(), inputList, areaToEdit.textColor, areaToEdit.bgColor);
                    break;
                default:
                    areaToEdit = new WindowArea(dummyTexBox: true);
                    break;
            }
		}
        public string UpdateScreenAndGetInput(int __paramLabelRequired__ = 0, bool showingTitle = true, int spaceForPrompt = 1)
        {
            UpdateScreen(showingTitle: showingTitle, spaceForPrompt: spaceForPrompt);
            return display.CommandPrompt();
        }

        // I want to force the caller to use a named paramerter label so please excuse the __paramLabelRequired__
        public void UpdateScreen(int __paramLabelsRequired__ = 0, bool showingTitle = true, int spaceForPrompt = 0)
        {
            int clearance = (showingTitle ? 1 : 0) + spaceForPrompt;

            display.SetConsoleSize(this.width, this.height + clearance);


            if (showingTitle)
            {
                display.Show(Title.PadRight(this.width), TitleTextColor, TitleBgColor);
            } 

            if (type == ViewLayoutType.twoStackOneLong)
            {

                for (var i = 0; i < AreaA.GetHeight(); i += 1)
                {
                    display.Output(AreaA.processedTextList[i], AreaA.textColor, AreaA.bgColor);
                    display.Output(AreaB.processedTextList[i], AreaB.textColor, AreaB.bgColor);
                    Console.WriteLine();
                }

                for (var i = 0; i < AreaC.GetHeight(); i += 1)
                {
                    display.Output(AreaC.processedTextList[i], AreaC.textColor, AreaC.bgColor);
                    display.Output(AreaB.processedTextList[i + AreaA.GetHeight()], AreaB.textColor, AreaB.bgColor);
                    Console.WriteLine();
                }

            }
            else

            if (type == ViewLayoutType.singleView)
            {
                foreach (var each in AreaA.processedTextList)
                {
                    display.Show(each, AreaA.textColor, AreaA.bgColor);
                }
            }
            else

            if (type == ViewLayoutType.drawOnSingleView)
            {
                foreach (var each in AreaA.consoleBox)
                {
                    each.Show(display);
                }
            }
            else
            {
                display.Show("View Set Up not Supported");
            }
        }

        public View(string title = "", List<string> textA = null, List<string> textB = null, List<string> textC = null, List<string> textD = null,  ViewLayoutType viewLayoutType = ViewLayoutType.twoStackOneLong, int width = 100, int height = 30)
        {
            textA = textA ?? new List<string>();
            textB = textB ?? new List<string>();
            textC = textC ?? new List<string>();
            textD = textD ?? new List<string>();

            type = viewLayoutType;
            Title = title;
            this.width = width;
            this.height = height;

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
    private int width;
    private int height;
    public List<string> processedTextList;
    public List<String> sourceText;
    public ConsoleColor textColor;
    public ConsoleColor bgColor;
    public List<ConsoleText> consoleBox;

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }


    static private List<ConsoleText> Process(List<String> list, ConsoleColor textColor, ConsoleColor bgColor)
    {
        var result = new List<ConsoleText>();
        foreach(var each in list)
        {
            var consoleText = new ConsoleText();
            consoleText.Append(each, bgColor, textColor);
            result.Add(consoleText);
        }
        return result;
    }

    public WindowArea(List<ConsoleText> consoleBox)
    {
        consoleBox = consoleBox ?? new List<ConsoleText>();

        this.width = this.height = 0;
        this.consoleBox = consoleBox;
        processedTextList = sourceText = new List<string>();
        textColor = bgColor = ConsoleColor.White;

        if (consoleBox.Count > 0)
        {
            this.width = consoleBox[0].GetCount();
            this.height = consoleBox.Count;
        }

    }

    public WindowArea(bool dummyTexBox = true)
    {
        width = height = 10;
        processedTextList = sourceText = new List<string>();
        textColor = bgColor = ConsoleColor.White;
        this.consoleBox = WindowArea.Process(processedTextList, textColor, bgColor);

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
        this.consoleBox = WindowArea.Process(processedText, textColor, bgColor);
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


