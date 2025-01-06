using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;

namespace Crawler.Application.Services.CommandServices.PrintCommandServices;

public class PrintingService
{
    public string FormatNodesForPrinting(CustomList<HtmlNode> nodes)
    {
        string output = "";

        if (nodes.Count == 1 && nodes[0].TagType == "html")
        {
            string treeToString = $"<{nodes[0].TagType}>{nodes[0].OuterText}</{nodes[0].TagType}>";
            output += treeToString;
            return output;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            string nodeText = node.Children.Count > 0 ? $"\"{node.OuterText}\"" : $"\"{node.InnerText}\"";
            output += (i == nodes.Count - 1) ? nodeText : $"{nodeText}, ";
        }

        return output;
    }
}