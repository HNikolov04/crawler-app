using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using System;
using System.Threading;

namespace Crawler.Application.Services.TreeServices;

public class TreeSearchingParallelService
{
    private readonly Semaphore _semaphore;
    private readonly int _processorCount;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public TreeSearchingParallelService()
    {
        _processorCount = Environment.ProcessorCount;
        _semaphore = new Semaphore(Environment.ProcessorCount, Environment.ProcessorCount);
    }

    public CustomList<HtmlNode> ParallelFindNodes(HtmlNode node, CustomList<string> treePathNodes, int index)
    {
        var finalResults = new CustomList<HtmlNode>();

        if (index == treePathNodes.Count)
        {
            finalResults.Add(node);
            return finalResults;
        }

        string currentTag = treePathNodes[index];

        if (currentTag == "//" && treePathNodes.Count == 1)
        {
            finalResults.Add(node);
        }
        else if (currentTag == "//" || currentTag == "html")
        {
            foreach (var result in ParallelFindNodes(node, treePathNodes, index + 1))
            {
                finalResults.Add(result);
            }
        }
        else if (currentTag == "*")
        {
            if (node.Children.Count >= 4)
            {
                var threads = new CustomList<Thread>();
                var childResults = new CustomList<CustomList<HtmlNode>>();

                int numberOfThreads = Math.Min(_processorCount, node.Children.Count);
                int childrenPerThread = node.Children.Count / numberOfThreads;
                int remainingChildren = node.Children.Count % numberOfThreads;

                int childIndex = 0;

                for (int i = 0; i < numberOfThreads; i++)
                {
                    var childSubset = new CustomList<HtmlNode>();
                    int end = childIndex + childrenPerThread + (i < remainingChildren ? 1 : 0);

                    for (int j = childIndex; j < end; j++)
                    {
                        childSubset.Add(node.Children[j]);
                    }

                    childIndex = end;

                    _semaphore.WaitOne();

                    var thread = new Thread(() =>
                    {
                        var localResults = new CustomList<HtmlNode>();
                        try
                        {
                            foreach (var child in childSubset)
                            {
                                foreach (var result in ParallelFindNodes(child, treePathNodes, index + 1))
                                {
                                    localResults.Add(result);
                                }
                            }
                        }
                        finally
                        {
                            _semaphore.Release();
                        }

                        lock (childResults)
                        {
                            childResults.Add(localResults);
                        }
                    });

                    threads.Add(thread);
                    thread.Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }

                foreach (var resultList in childResults)
                {
                    foreach (var result in resultList)
                    {
                        finalResults.Add(result);
                    }
                }
            }
            else
            {
                foreach (var child in node.Children)
                {
                    foreach (var result in ParallelFindNodes(child, treePathNodes, index + 1))
                    {
                        finalResults.Add(result);
                    }
                }
            }
        }
        else if (currentTag.Contains("[@"))
        {
            var attributeResults = HandleAttributeQuery(node, currentTag, treePathNodes, index);
            foreach (var result in attributeResults)
            {
                finalResults.Add(result);
            }
        }
        else if (currentTag.Contains("["))
        {
            var indexedResults = HandleIndexedTagQuery(node, currentTag, treePathNodes, index);
            foreach (var result in indexedResults)
            {
                finalResults.Add(result);
            }
        }
        else
        {
            if (node.Children.Count >= 4)
            {
                var threads = new CustomList<Thread>();
                var childResults = new CustomList<CustomList<HtmlNode>>();

                foreach (var child in node.Children)
                {
                    var thread = new Thread(() =>
                    {
                        if (child.TagType == currentTag)
                        {
                            var localResults = ParallelFindNodes(child, treePathNodes, index + 1);
                            lock (childResults)
                            {
                                childResults.Add(localResults);
                            }
                        }
                    });

                    threads.Add(thread);
                    thread.Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }

                foreach (var resultList in childResults)
                {
                    foreach (var result in resultList)
                    {
                        finalResults.Add(result);
                    }
                }
            }
            else
            {
                foreach (var child in node.Children)
                {
                    if (child.TagType == currentTag)
                    {
                        foreach (var result in ParallelFindNodes(child, treePathNodes, index + 1))
                        {
                            finalResults.Add(result);
                        }
                    }
                }
            }
        }

        return finalResults;
    }

    private CustomList<HtmlNode> HandleAttributeQuery(HtmlNode node, string currentTag, CustomList<string> treePathNodes, int index)
    {
        CustomList<HtmlNode> result = new CustomList<HtmlNode>();

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
                result.AddRange(ParallelFindNodes(child, treePathNodes, index + 1));
            }
        }

        return result;
    }

    private CustomList<HtmlNode> HandleIndexedTagQuery(HtmlNode node, string currentTag, CustomList<string> treePathNodes, int index)
    {
        CustomList<HtmlNode> result = new CustomList<HtmlNode>();

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
                result.AddRange(ParallelFindNodes(child, treePathNodes, index + 1));
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