using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;

namespace Crawler.Application.Services.CommandServices.CopyCommandServices;

public class ShallowCopyService
{
    public void ShallowCopy(CustomList<HtmlNode> targetNodes, CustomList<HtmlNode> sourceNodes)
    {
        HtmlNode sourceNode = sourceNodes[0];

        foreach (var node in targetNodes)
        {
            node.Parent.Children.Remove(node);
            node.Parent.Children.Add(sourceNode);
            sourceNode.CopiesCount++;
        }
    }
}