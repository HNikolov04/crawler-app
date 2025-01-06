using Crawler.Domain.DataStructures;

namespace Crawler.Application.Services.TreeServices;

public class HtmlParsingService
{
    public CustomStack<string> TokenizeHtml(string htmlString)
    {
        CustomList<string> lines = CreateLines(htmlString);

        CustomList<string> tokensList = new CustomList<string>();

        foreach (var line in lines)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\t' || line[i] == '\r' || line[i] == '\n' || line[i] == ' ')
                {
                    continue;
                }

                if (line[i] == '<')
                {
                    string tagToken = GetTagToken(line, i, out int tagEndIndex);

                    i = tagEndIndex - 1;

                    tokensList.Add(tagToken);

                    continue;
                }

                string textToken = GetTextToken(line, i, out int textEndIndex);

                i = textEndIndex - 1;

                tokensList.Add(textToken);
            }
        }

        tokensList.Reverse();

        return new CustomStack<string>(tokensList);
    }

    #region TokenizeStringHelperMethods

    private CustomList<string> CreateLines(string text)
    {
        CustomList<string> lines = new CustomList<string>();
        string line = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (i == text.Length - 1)
            {
                line += text[i];
                lines.Add(line);
                break;
            }

            if (text[i] == '\r' && text[i + 1] == '\n')
            {
                lines.Add(line);
                line = "";
                i++;
                continue;
            }

            line += text[i];
        }

        return lines;
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

    #endregion
}