using Crawler.Application.Commands;
using Crawler.Application.Services.TreeServices;
using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces;
using Crawler.Presentation.Interfaces;
using Crawler.Presentation.Messages;
using Crawler.Presentation.WindowsForm;
using System;
using System.Drawing;

namespace Crawler.Presentation.Core;

public class AppController : IAppController
{
    private readonly HtmlParsingService _htmlParsingService;
    private readonly TreeBuildingService _treeBuildingService;
    private CustomDictionary<string, Bitmap> _bmpImages;
    private IFileCommand _fileCommand;
    private ICommand _command;
    private HtmlNode _root;
    private string _htmlName;
    private string _output;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public AppController()
    {
        _htmlParsingService = new HtmlParsingService();
        _treeBuildingService = new TreeBuildingService();
        _bmpImages = new CustomDictionary<string, Bitmap>();
    }

    public string ToLower(string input)
    {
        const int ASCII_SUBTRACTION = 32;
        string loweredInput = null;

        foreach (var letter in input)
        {
            if (letter is >= 'A' and <= 'Z')
            {
                loweredInput += (char)(letter + ASCII_SUBTRACTION);
            }
            else if (letter is >= 'a' and <= 'z')
            {
                loweredInput += letter;
            }
        }

        if (loweredInput is null)
        {
            throw new NullReferenceException();
        }

        return loweredInput;
    }

    public string SplitCommandType(string input)
    {
        string command = string.Empty;

        foreach (var letter in input)
        {
            if (letter == ' ')
            {
                break;
            }

            command += letter;
        }

        return command;
    }

    public void LoadFile(string input)
    {
        const int COMMAND_LENGTH = 4;
        const int WHITE_SPACES = 2;

        _fileCommand = new LoadCommand(input, COMMAND_LENGTH, WHITE_SPACES);

        HtmlWithImages htmlWithImages = _fileCommand.Execute();

        _bmpImages = htmlWithImages.Images;

        _htmlName = htmlWithImages.HtmlContent.HtmlName;

        _root = BuildTree(htmlWithImages.HtmlContent.HtmlString);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(OutputMessages.TreeBuiltSuccessfully);
        Console.ResetColor();
    }

    public void NewFile(string input)
    {
        const int COMMAND_LENGTH = 4;
        const int WHITE_SPACES = 1;

        _fileCommand = new NewCommand(input, COMMAND_LENGTH, WHITE_SPACES);

        HtmlWithImages htmlWithImages = _fileCommand.Execute();

        _bmpImages = htmlWithImages.Images;

        _htmlName = htmlWithImages.HtmlContent.HtmlName;

        _root = BuildTree(htmlWithImages.HtmlContent.HtmlString);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(OutputMessages.TreeBuiltSuccessfully);
        Console.ResetColor();
    }

    private HtmlNode BuildTree(string htmlString)
    {
        CustomStack<string> tokens = _htmlParsingService.TokenizeHtml(htmlString);

        HtmlNode root = _treeBuildingService.BuildTreeFromTokens(null, tokens);

        return root;
    }

    public void PrintNodes(string input)
    {
        const int COMMAND_LENGTH = 5;
        const int WHITE_SPACES = 2;

        _command = new PrintCommand(input, _root, COMMAND_LENGTH, WHITE_SPACES);

        _output = _command.Execute();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(_output);
        Console.ResetColor();
    }

    public void SetNodes(string input)
    {
        const int COMMAND_LENGTH = 3;
        const int WHITE_SPACES = 2;

        _command = new SetCommand(input, _root, COMMAND_LENGTH, WHITE_SPACES);

        _output = _command.Execute();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(_output);
        Console.ResetColor();
    }

    public void CopyNodes(string input)
    {
        const int COMMAND_LENGTH = 4;
        const int WHITE_SPACES = 2;

        _command = new CopyCommand(input, _root, COMMAND_LENGTH, WHITE_SPACES);

        _output = _command.Execute();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(_output);
        Console.ResetColor();
    }

    public void Save(string input)
    {
        const int COMMAND_LENGTH = 4;
        const int WHITE_SPACES = 2;

        _command = new SaveCommand(input, _root, COMMAND_LENGTH, WHITE_SPACES, _bmpImages , _htmlName);

        _output = _command.Execute();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(_output);
        Console.ResetColor();
    }

    public void Visualize()
    {
        VisualizationForm form = new VisualizationForm(_root, _bmpImages);
        form.ShowDialog();
    }

    public bool ChangeFile()
    {
        Clear();
        return true;
    }

    public void Clear()
    {
        Console.Clear();
    }

    public void Exit()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(OutputMessages.ExitingMessage);
        Console.ResetColor();

        Environment.Exit(0);
    }
}