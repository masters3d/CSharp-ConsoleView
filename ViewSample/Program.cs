using System;
using System.Collections.Generic;
using ConsoleView;

namespace ViewSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var template =
    @"
                                                                       
                                                                       
                                                                       
                                                                       
                                                                       
      +---------------------------------------------------------+      
      |                                                         |      
      |                                                         |      
      |     +--------------------+  +--------------------+      |      
      |     |                    |  |                    |      |      
      |     +--------------------+  +--------------------+      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     |                    |  |                    |      |      
      |     +--------------------+  +--------------------+      |      
      |                                                         |      
      +---------------------------------------------------------+       
";

            string fox = @"
                                       
       ((                    ,(/       
       /(((,               /(((/       
       ///(((/           (((////       
       //////(((       (((//////       
       /////(((((((((((((((/////       
       /(((((((((((((((((((((((/       
       ((((/./(((((((((((*.(((((       
       (((/   *(((((((((.   ((((       
     .@(((/   *(((((((((.   ((((@      
    ,@@@&((/ *(((((((((((, /((@@@@     
   /@@@@@@@@(((((((((((((((@@@@@@@@.   
      @@@@@@@@@(((((((((@@@@@@@@@      
    ,@@@@@@@@@@&(((((((@@@@@@@@@@@     
             .@@(((((((@@              
              %@#     @@,              
              #@@@. *@@@.              
               @@@@@@@@&               
                 (@@@*   
                              
";
        string deer = @"
                                       
         /#  %.  %                     
      *  (#./%(#%,                     
      %( (%                            
       ((##                            
    ,(( (((((                          
  .((((((((((((                ((      
  /((((((((((((((.             ((      
        /@@@(((((((((((((((((((((.     
         %@@@((((((((((((((((((((((    
          @@@(((((((((((((((((((((((   
           (((((((((((((((((((((((((   
            *(((((@@@@@@@@@@#(((((((   
              /(((          **((((((   
              /(((          ****(((((  
              /((           ***   (((  
              /((          ,**    /((  
              /((          **     /((  
              /((         **      /((    
";



            var list = new List<String>();

            var response = "";
            View view = new View();
            Random random = new Random();


            do
            {
                var commands = @". Avalible Commands: single, animate, draw, three, quit";

                if (response == "single")
                {
                    view = new View("This is a single view" + commands, viewLayoutType: ViewLayoutType.singleView, display: view.display);
                    view.SetArea(template, Area.A);
                    response = view.UpdateScreenAndGetInput();

                    
                } else if (response == "animate")
                {
                    view = new View("This is a single view" + commands, viewLayoutType: ViewLayoutType.singleView, display: view.display);
                    var randomNumber1 = random.Next(0, (view.width - 80) / 2);
                    var randomNumber2 = random.Next(0, (view.width - 80) / 2);
                                        var deerList = Display.InsertMargins(Display.WrapText(deer, 40), rightmargin: randomNumber1);
                    var foxList = Display.InsertMargins(Display.WrapText(fox, 40), randomNumber2);

                    view.SetArea(Display.Zip(deerList, foxList), Area.A);
                    response = view.UpdateScreenAndGetInput();
                                                         

                }
                else if (response == "draw")
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
