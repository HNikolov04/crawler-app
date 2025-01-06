using Crawler.Domain.Entities;
using System.Collections.Generic;

namespace Crawler.Application.Services.TreeServices;

public class TreeSearchingService
{
    public List<HtmlNode> FindNodes(HtmlNode node, List<string> treePathNodes, int index)
    {
        List<HtmlNode> result = new();

        if (index == treePathNodes.Count)
        {
            result.Add(node);
            return result;
        }

        string currentTag = treePathNodes[index];

        if (currentTag == "//" && treePathNodes.Count == 1)
        {
            result.Add(node);
        }
        else if (currentTag == "//")
        {
            result.AddRange(FindNodes(node, treePathNodes, index + 1));
        }
        else if (currentTag == "html")
        {
            result.AddRange(FindNodes(node, treePathNodes, index + 1));
        }
        else if (currentTag == "*")
        {
            foreach (var child in node.Children)
            {
                result.AddRange(FindNodes(child, treePathNodes, index + 1));
            }
        }
        else if (currentTag.Contains("[@"))
        {
            result.AddRange(HandleAttributeQuery(node, currentTag, treePathNodes, index));
        }
        else if (currentTag.Contains("["))
        {
            result.AddRange(HandleIndexedTagQuery(node, currentTag, treePathNodes, index));
        }
        else
        {
            foreach (var child in node.Children)
            {
                if (child.TagType == currentTag)
                {
                    result.AddRange(FindNodes(child, treePathNodes, index + 1));
                }
            }
        }

        return result;
    }

    private List<HtmlNode> HandleAttributeQuery(HtmlNode node, string currentTag, List<string> treePathNodes, int index)
    {
        List<HtmlNode> result = new();

        string tagName = GetTagName(currentTag, 0, out int endIndex);
        string[] attributeParts = GetAttributeParts(currentTag, endIndex);
        string attributeName = attributeParts[0];
        string attributeValue = attributeParts[1];

        foreach (var child in node.Children)
        {
            if (child.TagType == tagName &&
                child.Attributes.ContainsKey(attributeName) &&
                child.Attributes[attributeName] == attributeValue)
            {
                result.AddRange(FindNodes(child, treePathNodes, index + 1));
            }
        }

        return result;
    }

    private List<HtmlNode> HandleIndexedTagQuery(HtmlNode node, string currentTag, List<string> treePathNodes, int index)
    {
        List<HtmlNode> result = new();

        string tagName = GetTagName(currentTag, 0, out int tagNameEndIndex);
        int tagIndex = GetTagIndex(currentTag, tagNameEndIndex);

        int currentIndex = 1;

        foreach (var child in node.Children)
        {
            if (child.TagType != tagName)
            {
                continue;
            }

            if (currentIndex == tagIndex)
            {
                result.AddRange(FindNodes(child, treePathNodes, index + 1));
                break;
            }

            currentIndex++;
        }

        return result;
    }

    private string GetTagName(string currentTag, int startIndex, out int endIndex)
    {
        string tagName = string.Empty;
        int i;
        endIndex = 0;

        for (i = startIndex; i < currentTag.Length; i++)
        {
            if (currentTag[i] == '[')
            {
                break;
            }

            tagName += currentTag[i];
        }

        endIndex = i;

        return tagName;
    }

    private string[] GetAttributeParts(string currentTag, int startIndex)
    {
        string attributeName = string.Empty;
        string attributeValue = string.Empty;

        bool isAttributeName = false;
        bool isAttributeValue = false;

        for (int i = startIndex; i < currentTag.Length; i++)
        {
            if (isAttributeName)
            {
                if (currentTag[i] == '=')
                {
                    isAttributeName = false;
                    isAttributeValue = true;

                    i++;
                    continue;
                }

                attributeName += currentTag[i];
            }
            else if (isAttributeValue)
            {
                if (currentTag[i] == '\'')
                {
                    break;
                }
                attributeValue += currentTag[i];
            }
            else if (currentTag[i] == '@')
            {
                isAttributeName = true;
            }
        }

        return [attributeName, attributeValue];
    }

    private int GetTagIndex(string currentTag, int startIndex)
    {
        string tagIndex = string.Empty;
        bool isTagIndex = false;

        for (int i = startIndex; i < currentTag.Length; i++)
        {
            if (currentTag[i] == '[')
            {
                isTagIndex = true;
                continue;
            }

            if (currentTag[i] == ']')
            {
                break;
            }

            if (isTagIndex)
            {
                tagIndex += currentTag[i];
            }
        }

        return int.Parse(tagIndex);
    }
}