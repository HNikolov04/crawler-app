using Crawler.Application.Services.CommandServices;
using Crawler.Application.Services.CommandServices.PrintCommandServices;
using Crawler.Application.Services.TreeServices;
using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces;
using System;

namespace Crawler.Application.Commands;

public class PrintCommand : ICommand
{
    private readonly string _input;
    private readonly HtmlNode _root;
    private readonly int _commandLength;
    private readonly int _whiteSpaces;

    private readonly TreeSearchingParallelService _treeSearchingParallelService;
    private readonly CommandParsingService _commandParsingService;
    private readonly PrintingService _printingService;

    // ReSharper disable once ConvertToPrimaryConstructor
    public PrintCommand(string input, HtmlNode root, int commandLength, int whiteSpaces)
    {
        _input = input;
        _root = root;
        _commandLength = commandLength;
        _whiteSpaces = whiteSpaces;
        _treeSearchingParallelService = new TreeSearchingParallelService();
        _commandParsingService = new CommandParsingService();
        _printingService = new PrintingService();
    }

    public string Execute()
    {
        CustomList<string> treePathNodes = _commandParsingService.GetQueryParts(_input, _commandLength, _whiteSpaces, out int _);

        if (treePathNodes.Count == 0)
        {
            throw new Exception("No valid query parts found");
        }

        CustomList<HtmlNode> result = _treeSearchingParallelService.ParallelFindNodes(_root, treePathNodes, 0);

        return _printingService.FormatNodesForPrinting(result);
    }
}