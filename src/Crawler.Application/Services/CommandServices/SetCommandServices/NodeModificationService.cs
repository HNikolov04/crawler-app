using Crawler.Application.Services.TreeServices;
using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using System;

namespace Crawler.Application.Services.CommandServices.SetCommandServices;

public class NodeModificationService
{
    public void ModifyNodes(CustomList<HtmlNode> nodesToSet, CustomStack<string> tokens, string text)
    {
        foreach (HtmlNode node in nodesToSet)
        {
            if (RequiresDeepCopy(node))
            {
                HtmlNode copiedNode = DeepCopy(node);
                SetNodeContent(copiedNode, tokens, text);
            }
            else
            {
                SetNodeContent(node, tokens, text);
            }
        }
    }

    private bool RequiresDeepCopy(HtmlNode node)
    {
        while (node != null)
        {
            if (node.CopiesCount > 0)
            {
                return true;
            }
            node = node.Parent;
        }

        return false;
    }

    private HtmlNode DeepCopy(HtmlNode node)
    {
        HtmlNode parent = node.Parent;

        if (parent == null)
        {
            throw new NullReferenceException();
        }

        HtmlNode deepCopiedNode = CreateDeepCopy(node);
        node.CopiesCount--;

        parent.Children.Remove(node);
        parent.Children.Add(deepCopiedNode);

        return deepCopiedNode;
    }

    private HtmlNode CreateDeepCopy(HtmlNode sourceNode)
    {
        HtmlNode copiedNode = new HtmlNode
        {
            TagType = sourceNode.TagType,
            Attributes = CopyAttributes(sourceNode.Attributes),
            CopiesCount = 0,
            InnerText = sourceNode.InnerText,
            IsSelfClosing = sourceNode.IsSelfClosing,
            Parent = sourceNode.Parent
        };

        foreach (HtmlNode child in sourceNode.Children)
        {
            copiedNode.Children.Add(CreateDeepCopy(child));
        }

        return copiedNode;
    }

    private CustomDictionary<string, string> CopyAttributes(CustomDictionary<string, string> originalAttributes)
    {
        CustomDictionary<string, string> copiedAttributes = new CustomDictionary<string, string>();

        foreach (var kvp in originalAttributes)
        {
            copiedAttributes.Add(kvp.Key, kvp.Value);
        }

        return copiedAttributes;
    }

    private void SetNodeContent(HtmlNode node, CustomStack<string> tokens, string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            node.InnerText = text;
        }
        else
        {
            var treeBuilder = new TreeBuildingService();
            treeBuilder.BuildTreeFromTokens(node, tokens);
            node.InnerText = string.Empty;
        }
    }
}