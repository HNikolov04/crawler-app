namespace Crawler.Domain.Entities;

public class HuffmanNode
{
    public byte Byte { get; set; }
    public int Count { get; set; }
    public HuffmanNode Left { get; set; }
    public HuffmanNode Right { get; set; }
}