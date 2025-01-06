using Crawler.Application.Commands.Messages;
using Crawler.Application.Services.CommandServices;
using Crawler.Application.Services.CommandServices.SetCommandServices;
using Crawler.Application.Services.TreeServices;
using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using Crawler.Domain.Interfaces;

namespace Crawler.Application.Commands;

public class SetCommand : ICommand
{
    private readonly string _input;
    private readonly HtmlNode _root;
    private readonly int _commandLength;
    private readonly int _whiteSpaces;

    private readonly NodeModificationService _nodeModificationService;
    private readonly CommandParsingService _commandParsingService;
    private readonly TreeSearchingParallelService _treeSearchingParallelService;

    // ReSharper disable once ConvertToPrimaryConstructor
    public SetCommand(string input, HtmlNode root, int commandLength, int whiteSpaces)
    {
        _input = input;
        _root = root;
        _commandLength = commandLength;
        _whiteSpaces = whiteSpaces;
        _nodeModificationService = new NodeModificationService();
        _commandParsingService = new CommandParsingService();
        _treeSearchingParallelService = new TreeSearchingParallelService();
    }

    public string Execute()
    {
        CustomList<string> treePathNodes = _commandParsingService.GetQueryParts(_input, _commandLength, _whiteSpaces, out int endIndex);
        _commandParsingService.GetNewContentParts(_input, endIndex, _whiteSpaces,out CustomStack<string> tokens, out string text);

        CustomList<HtmlNode> nodesToSet = _treeSearchingParallelService.ParallelFindNodes(_root, treePathNodes, 0);

        _nodeModificationService.ModifyNodes(nodesToSet, tokens, text);

        return CommandMessages.ChangesSetSuccess;
    }
}