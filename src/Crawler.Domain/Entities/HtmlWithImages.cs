using Crawler.Domain.DataStructures;
using System.Drawing;

namespace Crawler.Domain.Entities;

public class HtmlWithImages
{
    public HtmlContent HtmlContent { get; }
    public CustomDictionary<string, Bitmap> Images { get; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public HtmlWithImages(HtmlContent htmlContent, CustomDictionary<string, Bitmap> images)
    {
        HtmlContent = htmlContent;
        Images = images;
    }
}