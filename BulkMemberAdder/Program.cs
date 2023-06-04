using Bogus;
using BulkMemberAdder.Domain;
using Newtonsoft.Json;
using Spectre.Console;
using System.Diagnostics;

namespace BulkMemberAdder;

public class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            if (args.Any())
                return;

            PrintHelper();

            GetSampleFile();

            var path = SetFilePath();
            var memberList = await ImportMembersByPath(path);

            StartProccess(memberList);
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine("Something went wrong here is the detail \n" + ex);
        }
    }

    // print figlet and args
    private static void PrintHelper()
    {
        // figlet
        AnsiConsole.Write(
            new FigletText("Messanger Bulk Member adder")
                .LeftJustified()
                .Color(Color.Red));

        var rule = new Rule("[red]Helper[/]");
        rule.Justification = Justify.Left;
        AnsiConsole.Write(rule);

        //TODO: print args

        var ruleSteps = new Rule("[red]Steps:[/]");
        ruleSteps.Justification = Justify.Left;
        AnsiConsole.Write(ruleSteps);
    }

    // get sample members file
    private static void GetSampleFile()
    {
        if (!AnsiConsole.Confirm("\n Do you want to get sample file?"))
            return;

        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var filePath = Path.Combine(desktopPath, "Members.json");

        using (var writer = new StreamWriter(filePath))
        {
            var fakeData = Member.GenerateFakeMember();

            var user = fakeData.GenerateBetween(0, 10);

            writer.WriteLine(JsonConvert.SerializeObject(user, Formatting.Indented));
        }

        AnsiConsole.Markup($"[red]File saved at[/] [underline]{filePath}[/]\n");

        try { Process.Start("notepad.exe", filePath); } catch { }
    }

    // set members file path
    private static string SetFilePath()
    {
        try
        {
            // get path
            var path = AnsiConsole.Prompt(
                new TextPrompt<string>("\n[grey][[Import json file]][/] Type the path: ")
                );

            if (!File.Exists(path))
            {
                AnsiConsole.Markup("\n[red][[Cannot find the file try again.]][/]");
                return SetFilePath();
            }

            //print path
            var printPath = new TextPath(path)
    .RootStyle(new Style(foreground: Color.Red))
    .SeparatorStyle(new Style(foreground: Color.Green))
    .StemStyle(new Style(foreground: Color.Blue))
    .LeafStyle(new Style(foreground: Color.Yellow));
            AnsiConsole.Write(printPath);

            return path;
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nSomething went wrong while import file \n" + ex);
            AnsiConsole.WriteException(ex);

            return SetFilePath();
        }
    }

    // deserilize and return members from file
    private async static Task<List<Member>> ImportMembersByPath(string path)
    {
        try
        {
            List<Member> members = default;

            await AnsiConsole.Status()
                .StartAsync("\n Read memebers from file...", async ctx =>
                {
                    string jsonString = File.ReadAllText(path);
                    members = JsonConvert.DeserializeObject<List<Member>>(jsonString);
                });

            return members;
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nSomething went wrong while import members from file \n" + ex);
            AnsiConsole.WriteException(ex);

            SetFilePath();
            return await ImportMembersByPath(path);
        }
    }

    // select messanger and start proccess
    private static void StartProccess(List<Member> memberList)
    {
        // select messanger
        var messagner = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nSelect the [green]Messanger[/] and Enter:")
                .AddChoices(new[] {
            MessangerEnum.Eitaa.ToString()
                }));

        Enum.TryParse(messagner, out MessangerEnum messagnerType);
        switch (messagnerType)
        {
            case MessangerEnum.Eitaa:
                //TODO: impelemnt eitaa
                break;
        }
    }
}