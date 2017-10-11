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

            var view  = new View("This is Just a sample Title", list, list, list, ViewType.twoStackOneLong);

            var response = "";

            do
            {
                var height = (view.height / 2) - 1;
                view.SetArea(view.GetCommandHistory(height), TextBoxArea.A);
                view.SetArea(view.GetCommandHistory(height), TextBoxArea.C);
                view.SetArea(view.GetCommandHistory(height), TextBoxArea.B);
                response = view.UpdateScreenAndGetInput();
            } while (response != "quit");
        }
    }
}
