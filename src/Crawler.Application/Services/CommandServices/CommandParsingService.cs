using Crawler.Domain.DataStructures;

namespace Crawler.Application.Services.CommandServices;

public class CommandParsingService
{
    public CustomList<string> GetQueryParts(string input, int commandLength, int whiteSpaces,out int endIndex)
    {
        string query = GetQuery(input, commandLength, whiteSpaces, out int queryEndIndex);

        endIndex = queryEndIndex;

        CustomList<string> queryParts = SplitQuery(query);

        return queryParts;
    }

    public void GetNewContentParts(string input, int startIndex, int whiteSpaces, out CustomStack<string> tokens, out string text)
    {
        tokens = new CustomStack<string>();

        text = string.Empty;

        string newContent = GetNewContent(input ,startIndex, whiteSpaces);

        if (newContent[0] == '<')
        {
            tokens = GetTokensFromContent(newContent);
            return;
        }

        text = newContent;
    }

    public CustomList<string> GetQueryPartsToCopy(string input, int whiteSpaces, int startIndex)
    {
        string query = GetNewContent(input, startIndex, whiteSpaces);

        CustomList<string> queryPartsToCopy = SplitQuery(query);

        return queryPartsToCopy;
    }

    private CustomStack<string> GetTokensFromContent(string newContent)
    {
        CustomList<string> tokensList = new CustomList<string>();

        for (int i = 0; i < newContent.Length; i++)
        {
            if (newContent[i] == '<')
            {
                string tagToken = GetTagToken(newContent, i, out int tagEndIndex);

                i = tagEndIndex - 1;

                tokensList.Add(tagToken);

                continue;
            }

            string textToken = GetTextToken(newContent, i, out int textEndIndex);

            i = textEndIndex - 1;

            tokensList.Add(textToken);
        }

        tokensList.Reverse();

        CustomStack<string> tokens = new CustomStack<string>(tokensList);

        return tokens;
    }

    private string GetTagToken(string line, int startIndex, out int endIndex)
    {
        string token = string.Empty;
        endIndex = startIndex;

        for (int i = startIndex; i < line.Length; i++)
        {
            endIndex++;

            token += line[i];

            if (line[i] == '>')
            {
                break;
            }
        }

        return token;
    }

    private string GetTextToken(string line, int startIndex, out int endIndex)
    {
        string token = string.Empty;
        endIndex = startIndex;

        for (int i = startIndex; i < line.Length; i++)
        {
            if (line[i] == '<')
            {
                break;
            }

            token += line[i];

            endIndex++;
        }

        return token;
    }

    private string GetNewContent(string input, int startIndex, int whiteSpaces)
    {
        string newContent = string.Empty;

        for (int i = startIndex + whiteSpaces + 1; i < input.Length; i++)
        {
            if (input[i] == '"')
            {
                break;
            }

            newContent += input[i];

        }

        return newContent;
    }

    #region HelperMethods

    public string GetQuery(string input, int commandLength, int whiteSpaces ,out int endIndex)
    {
        string query = string.Empty;
        endIndex = 0;

        int i;

        for (i = commandLength + whiteSpaces; i < input.Length; i++)
        {
            if (input[i] == '"')
            {
                break;
            }

            query += input[i];
        }

        endIndex = i;

        return query;
    }

    private CustomList<string> SplitQuery(string query)
    {
        CustomList<string> queryPartsList = new CustomList<string>();

        string queryPart = string.Empty;

        for (int i = 0; i < query.Length; i++)
        {
            if (i + 1 < query.Length && query[i] == '/' && query[i + 1] == '/')
            {
                queryPart += query[i];
                queryPart += query[i + 1];

                queryPartsList.Add(queryPart);

                queryPart = string.Empty;
                continue;
            }

            if (query[i] == '/')
            {
                queryPart = GetQueryPart(query, i + 1, out int endIndex);

                if (queryPart != "")
                {
                    queryPartsList.Add(queryPart);
                }

                queryPart = string.Empty;
                i = endIndex - 1;
            }
        }

        return queryPartsList;
    }

    private string GetQueryPart(string query, int startIndex, out int endIndex)
    {
        string queryPart = string.Empty;
        endIndex = startIndex;

        for (int i = startIndex; i < query.Length; i++)
        {
            if (query[i] == '/')
            {
                break;
            }

            queryPart += query[i];

            endIndex++;
        }

        return queryPart;
    }

    #endregion
}