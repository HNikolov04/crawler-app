namespace Crawler.Domain.Entities;

public class HtmlContent
{
    public string HtmlName { get; }
    public string HtmlString { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public HtmlContent(string htmlName, string htmlString)
    {
        HtmlName = htmlName;
        HtmlString = htmlString;
    }
}