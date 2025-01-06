using Crawler.Application.Commands.Messages;
using Crawler.Application.Services.CommandServices;
using Crawler.Application.Services.CommandServices.CopyCommandServices;
using Crawler.Application.Services.TreeServices;
using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces;

namespace Crawler.Application.Commands;

public class CopyCommand : ICommand
{
    private readonly string _input;
    private readonly HtmlNode _root;
    private readonly int _commandLength;
    private readonly int _whiteSpaces;

    private readonly CommandParsingService _commandParsingService;
    private readonly TreeSearchingParallelService _treeSearchingParallelService;
    private readonly ShallowCopyService _shallowCopyService;

    // ReSharper disable once ConvertToPrimaryConstructor
    public CopyCommand(string input, HtmlNode root, int commandLength, int whiteSpaces)
    {
        _input = input;
        _root = root;
        _commandLength = commandLength;
        _whiteSpaces = whiteSpaces;
        _commandParsingService = new CommandParsingService();
        _treeSearchingParallelService = new TreeSearchingParallelService();
        _shallowCopyService = new ShallowCopyService();
    }

    public string Execute()
    {
        CustomList<string> treePathNodes = _commandParsingService.GetQueryParts(_input, _commandLength, _whiteSpaces,out int endIndex);
        CustomList<string> treePathNodesToCopy = _commandParsingService.GetQueryPartsToCopy(_input, _whiteSpaces, endIndex);

        CustomList<HtmlNode> targetNodes = _treeSearchingParallelService.ParallelFindNodes(_root, treePathNodesToCopy, 0);
        CustomList<HtmlNode> sourceNodes = _treeSearchingParallelService.ParallelFindNodes(_root, treePathNodes, 0);

        _shallowCopyService.ShallowCopy(targetNodes, sourceNodes);
        
        return CommandMessages.CopiedSuccess;
    }
}