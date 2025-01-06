using Crawler.Presentation.Interfaces;
using Crawler.Presentation.Messages;
using System;

namespace Crawler.Presentation.Core;

public class AppRunner : IAppRunner
{
    private readonly IAppController _controller;
    private bool _hasChangedFile;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public AppRunner()
    {
        _controller = new AppController();
    }

    public void Run()
    {
        Console.WriteLine(OutputMessages.WelcomingMessage);

        while (true)
        {
            try
            {
                HandleInitialOptions();
                HandleCommands();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
    }

    private void HandleInitialOptions()
    {
        Console.WriteLine(OutputMessages.LoadOrNew);

        Console.Write(OutputMessages.ChooseOption);
        string input = Console.ReadLine();
        string command = _controller.ToLower(_controller.SplitCommandType(input));

        switch (command)
        {
            case "load":
                _controller.LoadFile(input);
                break;

            case "new":
                _controller.NewFile(input);
                break;

            case "clear":
                _controller.Clear();
                break;

            default:
                throw new InvalidOperationException(string.Format(OutputMessages.InvalidCommand, command));
        }
    }

    private void HandleCommands()
    {
        while (true)
        {
            if (_hasChangedFile)
            {
                _hasChangedFile = false;
                break;
            }

            Console.WriteLine(OutputMessages.CommandOptions);
            Console.Write(OutputMessages.EnterCommand);

            string input = Console.ReadLine();
            string command = _controller.ToLower(_controller.SplitCommandType(input));

            try
            {
                ExecuteCommand(command, input);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
    }

    private void ExecuteCommand(string command, string input)
    {
        switch (command)
        {
            case "print":
                _controller.PrintNodes(input);
                break;

            case "set":
                _controller.SetNodes(input);
                break;

            case "copy":
                _controller.CopyNodes(input);
                break;

            case "save":
                _controller.Save(input);
                break;

            case "visualize":
                _controller.Visualize();
                break;

            // ReSharper disable once StringLiteralTypo
            case "changefile":
                _hasChangedFile = _controller.ChangeFile();
                break;

            case "clear":
                _controller.Clear();
                break;

            case "exit":
                _controller.Exit();
                break;

            default:
                throw new InvalidOperationException(string.Format(OutputMessages.InvalidCommand, command));
        }
    }
}