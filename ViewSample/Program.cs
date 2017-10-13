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
                if (response == "single")
                {
                    var view = new View("This is a single view", viewLayoutType: ViewLayoutType.singleView);
                    response = view.UpdateScreenAndGetInput();
                    
                } else 
                {
                    var view = new View("This is Just a sample Title", list, list, list, list, ViewLayoutType.twoStackOneLong);
                    var height = (view.height / 2) - 1;
                    view.SetArea(view.GetCommandHistory(height), TextBoxArea.A);
                    view.SetArea(view.GetCommandHistory(height), TextBoxArea.C);
                    view.SetArea(view.GetCommandHistory(height), TextBoxArea.B);
                    response = view.UpdateScreenAndGetInput();
                }

            } while (response != "quit");
        }
    }
}
