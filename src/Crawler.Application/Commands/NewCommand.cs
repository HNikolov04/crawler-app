using Crawler.Application.Services.CommandServices;
using Crawler.Application.Services.CommandServices.NewCommandServices;
using Crawler.Application.Services.ValidationServices;
using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces;

namespace Crawler.Application.Commands;

public class NewCommand : IFileCommand
{
    private readonly string _input;
    private readonly int _commandLength;
    private readonly int _whiteSpaces;
    private readonly CommandParsingService _commandParsingService;
    private readonly FilePathValidationService _filePathValidationService;
    private readonly NewFileService _newFileService;

    // ReSharper disable once ConvertToPrimaryConstructor
    public NewCommand(string input, int commandLength, int whiteSpaces)
    {
        _input = input;
        _commandLength = commandLength;
        _whiteSpaces = whiteSpaces;
        _filePathValidationService = new FilePathValidationService();
        _commandParsingService = new CommandParsingService();
        _newFileService = new NewFileService();
    }

    public HtmlWithImages Execute()
    {
        string filePath = _commandParsingService.GetQuery(_input, _commandLength, _whiteSpaces, out int _);

        _filePathValidationService.EnsureValidFilePath(filePath);

        return _newFileService.LoadHtmlWithImages(filePath);
    }
}