using Spectre.Console;

namespace ShipItBudget.UI
{
    public class WelcomeScreen
    {
        public static void Show()
        {
            Console.Title = "Budget & Utgifter";

            var appName = new FigletText("Budget & Utgifter")
            {
                Color = Color.Gold1,
                Justification = Justify.Center
            };
            var version = new Text("Version 0.1.0", new Style(Color.Grey))
            {
                Justification = Justify.Center
            };

            AnsiConsole.Write(appName);
            AnsiConsole.Write(version);
            AnsiConsole.WriteLine("\n");

            Spinner spinner = Spinner.Known.Dots10;
            string continueMessage = "Tryck på valfri tangent för att fortsätta";

            // Live ritar om innehållet på samma plats på skärmen. Align.Center centrerar texten.
            AnsiConsole.Live(Align.Center(new Text(continueMessage)))
                .Start(context =>
                {
                    int frameIndex = 0;
                    // KeyAvailable kontrollerar om en tangent väntar utan att blockera, så att spinnern hinner animeras.
                    while (!Console.KeyAvailable)
                    {
                        // % gör att bildrutorna börjar om från början när de tar slut.
                        string spinnerFrame = spinner.Frames[frameIndex % spinner.Frames.Count];
                        context.UpdateTarget(Align.Center(new Text($"{continueMessage} {spinnerFrame}")));
                        frameIndex++;
                        Thread.Sleep(spinner.Interval);
                    }
                    // Läser bort tangenttrycket. Annars ligger det kvar i bufferten och väljer första alternativet i huvudmenyn.
                    Console.ReadKey(intercept: true);
                });
        }
    }
}
