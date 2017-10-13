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

            do
            {
                var commands = @". Avalible Commands: single, draw, three";


                if (response == "single")
                {
                    var view = new View("This is a single view" + commands, viewLayoutType: ViewLayoutType.singleView);
                    response = view.UpdateScreenAndGetInput();
                    
                } else if (response == "draw")
                {
                    var view = new View("This is a single view" + commands, viewLayoutType: ViewLayoutType.drawOnSingleView);
                    response = view.UpdateScreenAndGetInput();

                } else 
                {
                    var view = new View("This view has two stacked areas and one side area " + commands, list, list, list, list, ViewLayoutType.twoStackOneLong);
                    var height = (view.height / 2) - 1;
                    view.SetArea(view.GetCommandHistory(height), Area.A);
                    view.SetArea(view.GetCommandHistory(height), Area.C);
                    view.SetArea(view.GetCommandHistory(height), Area.B);
                    response = view.UpdateScreenAndGetInput();
                }

            } while (response != "quit");
        }
    }
}
