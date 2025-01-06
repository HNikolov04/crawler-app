using System;
using System.Collections.Generic;
using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;

namespace Crawler.Application.Services.TreeServices;

public class TreeBuildingService
{
    public HtmlNode BuildTreeFromTokens(HtmlNode node, CustomStack<string> tokens)
    {
        HtmlNode root = node;
        HtmlNode currentNode = node;

        while (tokens.Count > 0)
        {
            string token = tokens.Pop();

            if (IsOpeningTag(token))
            {
                string tagName = ParseTagName(token);
                CustomDictionary<string, string> attributes = ParseAttributes(token);

                HtmlNode newNode = new HtmlNode
                {
                    TagType = tagName,
                    Attributes = attributes,
                    Parent = currentNode,
                    IsSelfClosing = IsSelfClosingTag(tagName)
                };

                if (currentNode != null)
                {
                    currentNode.Children.Add(newNode);
                }
                else
                {
                    root = newNode;
                }

                if (!newNode.IsSelfClosing)
                {
                    currentNode = newNode;
                }
            }
            else if (IsClosingTag(token))
            {
                if (currentNode != null && currentNode.Parent != null)
                {
                    currentNode = currentNode.Parent;
                }
            }
            else
            {
                if (currentNode != null)
                {
                    if (currentNode != null)
                    {
                        if (!string.IsNullOrEmpty(currentNode.InnerText))
                        {
                            currentNode.InnerText += Environment.NewLine;
                        }
                        currentNode.InnerText += token;
                    }
                }
            }
        }

        return root;
    }

    #region BuildTreeHelperMethods

    private bool IsOpeningTag(string token)
    {
        return token[0] == '<' && token[1] != '/';
    }

    private bool IsClosingTag(string token)
    {
        return token[0] == '<' && token[1] == '/';
    }

    private bool IsSelfClosingTag(string tag)
    {
        List<string> _validSelfClosingTags = ["img", "br", "input"];
        return _validSelfClosingTags.Contains(tag);
    }

    private static string ParseTagName(string token)
    {
        string tagName = string.Empty;

        for (int i = 0; i < token.Length; i++)
        {

            if (token[i] == '<' || token[i] == '>')
            {
                continue;
            }

            if (token[i] == ' ')
            {
                break;
            }

            tagName += token[i];
        }

        return tagName;
    }

    private CustomDictionary<string, string> ParseAttributes(string token)
    {
        CustomDictionary<string, string> attributes = new CustomDictionary<string, string>();

        bool isTagNameSkipped = false;
        bool isAttributeKey = true;
        bool isAttributeValue = false;

        string attributeKey = string.Empty;
        string attributeValue = string.Empty;

        for (int i = 0; i < token.Length; i++)
        {
            if (token[i] == ' ' && !isTagNameSkipped)
            {
                isTagNameSkipped = true;
                continue;
            }

            if (!isTagNameSkipped)
            {
                continue;
            }

            if ((token[i] == ' ' || token[i] == '>' || (token[i] == '/' && token[i + 1] == '>')) && isAttributeValue)
            {
                attributes.Add(attributeKey, attributeValue);

                attributeKey = string.Empty;
                attributeValue = string.Empty;

                isAttributeValue = false;
                isAttributeKey = true;
            }

            if (token[i] == '\'' || token[i] == '"')
            {
                continue;
            }

            if (token[i] == '=')
            {
                isAttributeValue = true;
                isAttributeKey = false;
                continue;
            }

            if (isAttributeKey)
            {
                attributeKey += token[i];
            }

            if (isAttributeValue)
            {
                attributeValue += token[i];
            }
        }

        return attributes;
    }

    #endregion
}