using Crawler.Domain.DataStructures;

namespace Crawler.Domain.Entities;

public class HtmlNode
{
    public string TagType { get; set; }
    public CustomDictionary<string, string> Attributes { get; set; }
    public int CopiesCount { get; set; }
    public string InnerText { get; set; }
    public string OuterText => BuildOuterText(this);
    public bool IsSelfClosing { get; set; }
    public CustomList<HtmlNode> Children { get; set; }
    public HtmlNode Parent { get; set; }

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public HtmlNode()
    {
        IsSelfClosing = false;
        CopiesCount = 0;
        TagType = null;
        Attributes = new CustomDictionary<string, string>();
        InnerText = string.Empty;
        Children = new CustomList<HtmlNode>();
        Parent = null;
    }

    public string BuildOuterText(HtmlNode node)
    {
        string outerText = "";

        if (Children.Count <= 0)
        {
            return outerText;
        }

        if (IsSelfClosing)
        {
            outerText += $"<{TagType}{FormatAttributes(Attributes)}/>";

            return outerText;
        }

        foreach (var child in node.Children)
        {
            outerText += $"<{child.TagType}{FormatAttributes(child.Attributes)}>";
            outerText += child.InnerText;
            outerText += BuildOuterText(child);
            outerText += $"</{child.TagType}>";
        }

        return outerText;
    }

    private string FormatAttributes(CustomDictionary<string, string> attributes)
    {
        if (attributes.Count == 0)
        {
            return "";
        }

        string attributeString = "";

        foreach (var kvp in attributes)
        {
            attributeString += $" {kvp.Key}=\"{kvp.Value}\"";
        }

        return attributeString;
    }
}