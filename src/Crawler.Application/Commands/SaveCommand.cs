using Crawler.Application.Commands.Messages;
using Crawler.Application.Services.CommandServices;
using Crawler.Application.Services.CommandServices.SaveCommandServices;
using Crawler.Application.Services.ValidationServices;
using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces;
using System.Drawing;

namespace Crawler.Application.Commands;

public class SaveCommand : ICommand
{
    private readonly HtmlNode _root;
    private readonly string _input;
    private readonly int _commandLength;
    private readonly int _whiteSpaces;
    private readonly CustomDictionary<string, Bitmap> _bmpImages;
    private readonly string _htmlName;
    private readonly CommandParsingService _commandParsingService;
    private readonly SaveEncodedDataService _saveEncodedDataService;
    private readonly FilePathValidationService _filePathValidationService;


    // Constructor with DI
    public SaveCommand(string input, HtmlNode root, int commandLength, int whiteSpaces, CustomDictionary<string, Bitmap> bmpImages, string htmlName)
    {
        _input = input;
        _root = root;
        _commandLength = commandLength;
        _whiteSpaces = whiteSpaces;
        _bmpImages = bmpImages;
        _htmlName = htmlName;
        _commandParsingService = new CommandParsingService();
        _filePathValidationService = new FilePathValidationService();
        _saveEncodedDataService = new SaveEncodedDataService();
    }

    public string Execute()
    {
        string filePath = _commandParsingService.GetQuery(_input, _commandLength, _whiteSpaces, out int _);

        _filePathValidationService.EnsureValidFilePath(filePath);

        _saveEncodedDataService.Save(filePath, _bmpImages, _root, _htmlName);

        return CommandMessages.SavedSuccess;
    }
}
