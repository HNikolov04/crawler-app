namespace Crawler.Domain.Entities;

public class MetaData
{
    public string FileName { get; set; }
    public int ContentLength { get; set; }
    public int BitCount { get; set; }
    public int[] FrequencyTable { get; set; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public MetaData(string fileName, int contentLength, int bitCount, int[] frequencyTable)
    {
        FileName = fileName;
        ContentLength = contentLength;
        BitCount = bitCount;
        FrequencyTable = frequencyTable;
    }

    public override string ToString()
    {
        return $"{FileName},{ContentLength},{BitCount},{string.Join(",", FrequencyTable)},";
    }
}