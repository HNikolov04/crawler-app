using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using System.Drawing;
using System.Windows.Forms;

namespace Crawler.Presentation.WindowsForm;

public partial class VisualizationForm : Form
{
    private readonly HtmlNode _root;
    private readonly CustomDictionary<string, Bitmap> _bmpImages;

    public VisualizationForm(HtmlNode root, CustomDictionary<string, Bitmap> bmpImages)
    {
        _root = root;
        _bmpImages = bmpImages;
        InitializeComponent();
    }

    private void VisualizationForm_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        int currentY = 20;
        VisualizeNode(_root, g, 20, ref currentY);
    }

    private void VisualizeNode(HtmlNode node, Graphics g, int xPosition, ref int currentY)
    {
        if (node.TagType.ToLower() == "table")
        {
            DrawTable(node, g, xPosition, ref currentY);
            return;
        }

        if (node.TagType.ToLower() == "img")
        {
            DrawImage(node, g, xPosition, ref currentY);
            return;
        }

        if (node.TagType.ToLower() == "a")
        {
            DrawLink(node, g, xPosition, ref currentY);
            return;
        }

        if (!string.IsNullOrEmpty(node.InnerText))
        {
            using (var font = new Font("Times New Roman", 12, FontStyle.Regular))
            {
                g.DrawString(node.InnerText, font, Brushes.Black, new Point(xPosition, currentY));
            }

            currentY += 40;

            return;
        }

        foreach (var child in node.Children)
        {
            VisualizeNode(child, g, xPosition, ref currentY);
        }
    }

    private void DrawTable(HtmlNode tableNode, Graphics g, int xPosition, ref int currentY)
    {
        int cellHeight = 30;
        int padding = 2;

        int currentRowY = currentY;

        foreach (var row in tableNode.Children)
        {
            int currentX = xPosition;

            foreach (var cell in row.Children)
            {
                using (var font = new Font("Times New Roman", 10, FontStyle.Regular))
                {
                    SizeF textSize = g.MeasureString(cell.InnerText, font);

                    int cellWidth = (int)(textSize.Width + padding * 2);

                    g.DrawRectangle(Pens.DarkSlateGray, currentX, currentRowY, cellWidth, cellHeight);

                    if (!string.IsNullOrEmpty(cell.InnerText))
                    {
                        float textX = currentX + (cellWidth - textSize.Width) / 2;
                        float textY = currentRowY + (cellHeight - textSize.Height) / 2;
                        g.DrawString(cell.InnerText, font, Brushes.Black, textX, textY);
                    }

                    currentX += cellWidth + padding;
                }
            }

            currentRowY += cellHeight + padding;
        }

        currentY = currentRowY + padding;
    }

    private void DrawImage(HtmlNode node, Graphics g, int xPosition, ref int currentY)
    {
        if (node.Attributes.ContainsKey("src"))
        {
            string src = node.Attributes["src"];

            if (_bmpImages.ContainsKey(src))
            {
                Bitmap img = _bmpImages[src];
                g.DrawImage(img, xPosition, currentY, 100, 100);
                currentY += img.Height + 10; 
            }
            else
            {
                using (var font = new Font("Times New Roman", 12, FontStyle.Italic))
                {
                    g.DrawString($"[Image not found: {src}]", font, Brushes.Red, xPosition, currentY);
                }
                currentY += 20;
            }
        }
    }

    private void DrawLink(HtmlNode node, Graphics g, int xPosition, ref int currentY)
    {
        Font linkFont = new Font("Times New Roman", 10, FontStyle.Underline);
        Brush linkBrush = Brushes.Blue;

        if (!string.IsNullOrEmpty(node.InnerText))
        {
            g.DrawString(node.InnerText, linkFont, linkBrush, new Point(xPosition, currentY));
            currentY += 20;
        }
    }
}