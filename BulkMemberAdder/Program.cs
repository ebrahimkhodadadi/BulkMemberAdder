﻿using Bogus;
using BulkMemberAdder.Domain;
using BulkMemberAdder.Messangers;
using BulkMemberAdder.Messangers.Eitaa;
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
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();

            if (args is not null)
                if (args.Any())
                    return;

            PrintHelper();

            GetSampleFile();

            var path = SetFilePath();
            var memberList = await ImportMembersByPath(path);

            await StartProccess(memberList);
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

        //var rule = new Rule("[red]Helper[/]");
        //rule.Justification = Justify.Left;
        //AnsiConsole.Write(rule);

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

            var user = fakeData.GenerateBetween(5, 10);

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
                AnsiConsole.Markup("[red][[Cannot find the file try again.]][/]\n");
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
            Console.WriteLine("\nSomething went wrong while import file \n");
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
            Console.WriteLine("\nSomething went wrong while import members from file \n");
            AnsiConsole.WriteException(ex);

            SetFilePath();
            return await ImportMembersByPath(path);
        }
    }

    // select messanger and start proccess
    private static async Task StartProccess(List<Member> memberList)
    {
        // select messanger
        var messagner = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\nSelect the [green]Messanger[/] and Enter:")
                .AddChoices(new[] {
            MessangerEnum.Eitaa.ToString()
                }));

        Enum.TryParse(messagner, out MessangerEnum messagnerType);

        IMessangerService messangerService = default;

        switch (messagnerType)
        {
            case MessangerEnum.Eitaa:
                messangerService = new EitaaService();
                break;
        }

        // start service
        try { await messangerService.Start(memberList); }
        catch { messangerService.Stop(); }

        await ExistHelper();
    }

    private static async Task ExistHelper()
    {
        if (AnsiConsole.Confirm("\nJob finished want to try again?"))
            await Main(null);

        Console.Clear();

        AnsiConsole.Write(
            new FigletText("Good Luck!")
                .Centered()
                .Color(Color.Green));

        Environment.Exit(0);
    }
}