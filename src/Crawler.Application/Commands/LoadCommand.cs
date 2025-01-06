using Crawler.Application.Services.CommandServices;
using Crawler.Application.Services.CommandServices.LoadCommandServices;
using Crawler.Application.Services.ValidationServices;
using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces;

namespace Crawler.Application.Commands;

public class LoadCommand : IFileCommand
{
    private readonly string _input;
    private readonly int _commandLength;
    private readonly int _whiteSpaces;
    private readonly CommandParsingService _commandParsingService;
    private readonly LoadCompressedDataService _loadCompressedDataService;
    private readonly FilePathValidationService _filePathValidationService;

    public LoadCommand(string input, int commandLength, int whiteSpaces)
    {
        _input = input;
        _commandLength = commandLength;
        _whiteSpaces = whiteSpaces;
        _commandParsingService = new CommandParsingService();
        _loadCompressedDataService = new LoadCompressedDataService();
        _filePathValidationService = new FilePathValidationService();
    }

    public HtmlWithImages Execute()
    {
        string filePath = _commandParsingService.GetQuery(_input, _commandLength, _whiteSpaces, out int _);

        _filePathValidationService.EnsureValidFilePath(filePath);

        return _loadCompressedDataService.LoadCompressedData(filePath);
    }
}