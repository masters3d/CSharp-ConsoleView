using System;
using System.Collections.Generic;
using ConsoleView;

namespace ViewSample
{
    class Program
    {
        static void Main(string[] args)
        {

            var list = new List<String>();

            var response = "";
            View view;

            do
            {
                var commands = @". Avalible Commands: single, draw, three, quit";

                if (response == "single")
                {
                    view = new View("This is a single view" + commands, viewLayoutType: ViewLayoutType.singleView);
                    response = view.UpdateScreenAndGetInput();

                    
                } else if (response == "draw")
                {
                    view = new View("This is a single view" + commands, viewLayoutType: ViewLayoutType.drawOnSingleView);

                    var box1 = new ConsoleBox("Camping Site", 10, 2, ConsoleColor.White, ConsoleColor.Red, 10, 2);
                    view.AddToDrawingArea(box1);

                    var box2 = new ConsoleBox("Hello I am another box", 10, 2, ConsoleColor.White, ConsoleColor.Blue, 10, 10);
                    view.AddToDrawingArea(box2);

                    response = view.UpdateScreenAndGetInput();

                } else 
                {
                    view = new View("This view has two stacked areas and one side area " + commands, list, list, list, list, ViewLayoutType.twoStackOneLong);
                    var height = (view.height / 2) - 1;
                    view.SetArea(view.GetCommandHistory(height), Area.A);
                    view.SetArea(view.GetCommandHistory(height), Area.C);
                    view.SetArea(view.GetCommandHistory(height), Area.B);
                    response = view.UpdateScreenAndGetInput();
                }
                view.display.SetConsoleTitle(commands);

            } while (response != "quit");

            view = new View(viewLayoutType: ViewLayoutType.drawOnSingleView);
            view.UpdateScreen();
        }
    }
}
