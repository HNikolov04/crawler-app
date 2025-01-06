using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using System.Drawing;
using System.IO;

namespace Crawler.Application.Services.CommandServices.NewCommandServices;

public class NewFileService
{
    public HtmlWithImages LoadHtmlWithImages(string filePath)
    {
        CustomDictionary<string, Bitmap> bmpImages = new CustomDictionary<string, Bitmap>();

        string htmlString = File.ReadAllText(filePath);
        string htmlName = Path.GetFileName(filePath);
        HtmlContent htmlContent = new HtmlContent(htmlName, htmlString);

        string filePathFolder = Path.GetDirectoryName(filePath);
        var bmpFiles = Directory.GetFiles(filePathFolder, "*.bmp", SearchOption.AllDirectories);

        foreach (var bmpFile in bmpFiles)
        {
            string bmpFileName = Path.GetFileName(bmpFile);
            byte[] bmpBytes = File.ReadAllBytes(bmpFile);

            using (var memoryStream = new MemoryStream(bmpBytes))
            {
                Bitmap bmpImage = new Bitmap(memoryStream);
                bmpImages[bmpFileName] = bmpImage;
            }
        }

        return new HtmlWithImages(htmlContent, bmpImages);
    }
}