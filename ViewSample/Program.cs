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
            View view = new View();

            do
            {
                var commands = @". Avalible Commands: single, draw, three, quit";

                if (response == "single")
                {
                    view = new View("This is a single view" + commands, viewLayoutType: ViewLayoutType.singleView, display: view.display);
                    response = view.UpdateScreenAndGetInput();

                    
                } else if (response == "draw")
                {
                    view = new View("This is a single view" + commands, viewLayoutType: ViewLayoutType.drawOnSingleView, display: view.display);

                    var box1 = new ConsoleBox("Camping Site", 10, 2, ConsoleColor.White, ConsoleColor.Red, 10, 2);
                    view.AddToDrawingArea(box1);

                    var box2 = new ConsoleBox("Hello I am another box", 10, 2, ConsoleColor.White, ConsoleColor.Blue, 10, 10);
                    view.AddToDrawingArea(box2);

                    response = view.UpdateScreenAndGetInput();

                } else if (response == "three")
                {
                    view = new View("This view has two stacked areas and one side area " + commands, list, list, list, list, ViewLayoutType.twoStackOneLong, display: view.display);
                    var height = (view.height / 2) - 1;
                    view.SetArea(view.GetCommandHistory(height), Area.A);
                    view.SetArea(view.GetCommandHistory(height), Area.C);
                    view.SetArea(view.GetCommandHistory(height), Area.B);
                    response = view.UpdateScreenAndGetInput();
                } else {
                    view = new View(commands, display: view.display);
                    view.SetAreaColors(Area.A, ConsoleColor.Yellow, ConsoleColor.Blue);
                    view.SetAreaColors(Area.B, ConsoleColor.Blue, ConsoleColor.White);
                    view.SetAreaColors(Area.C, ConsoleColor.DarkMagenta, ConsoleColor.White);
                    response = view.UpdateScreenAndGetInput();
                }
                view.display.SetConsoleTitle(commands);

            } while (response != "quit");

            view = new View(viewLayoutType: ViewLayoutType.drawOnSingleView);
            view.UpdateScreen(showingTitle: false);
        }
    }
}
