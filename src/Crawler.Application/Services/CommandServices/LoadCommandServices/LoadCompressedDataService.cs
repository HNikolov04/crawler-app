using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using System.Drawing;
using System.IO;

namespace Crawler.Application.Services.CommandServices.LoadCommandServices;

public class LoadCompressedDataService
{
    public HtmlWithImages LoadCompressedData(string filePath)
    {
        byte[] compressedDataBytes = File.ReadAllBytes(filePath);
        CustomDictionary<string, Bitmap> bmpImages = new CustomDictionary<string, Bitmap>();

        int index = 0;
        ReadHtmlMetaData(compressedDataBytes, index, out string fileName, out string contentLength, out string htmlBitCount, out int[] frequencyTable, out int endIndex);

        HuffmanNode root = BuildTree(frequencyTable);
        byte[] htmlCompressedData = ExtractCompressedData(compressedDataBytes, endIndex, int.Parse(contentLength), out endIndex);
        byte[] htmlBytes = HuffmanDecode(htmlCompressedData, root, int.Parse(htmlBitCount));
        string html = GetStringFromBytes(htmlBytes);

        HtmlContent htmlContent = new HtmlContent(fileName, html);

        while (endIndex < compressedDataBytes.Length)
        {
            ReadBmpMetaData(compressedDataBytes, endIndex, out string bmpFileName, out string bmpContentLength, out endIndex);

            byte[] bmpBytes = ExtractCompressedData(compressedDataBytes, endIndex, int.Parse(bmpContentLength), out endIndex);

            using (var memoryStream = new MemoryStream(bmpBytes))
            {
                Bitmap bmpImage = new Bitmap(memoryStream);
                bmpImages[bmpFileName] = bmpImage;
            }
        }

        return new HtmlWithImages(htmlContent, bmpImages);
    }

    #region Helper Methods

    private void ReadHtmlMetaData(byte[] compressedDataBytes, int index, out string fileName, out string contentLength, out string bitCount, out int[] frequencyTable, out int endIndex)
    {
        fileName = GetContent(compressedDataBytes, index, out index);
        contentLength = GetContent(compressedDataBytes, index + 1, out index);
        bitCount = GetContent(compressedDataBytes, index + 1, out index);
        frequencyTable = GetFrequencyTable(compressedDataBytes, index, out index);

        endIndex = index;
    }

    private void ReadBmpMetaData(byte[] compressedDataBytes, int index, out string fileName, out string contentLength, out int endIndex)
    {
        fileName = GetContent(compressedDataBytes, index, out index);
        contentLength = GetContent(compressedDataBytes, index + 1, out index);

        endIndex = index;
    }

    private string GetContent(byte[] compressedDataBytes, int startIndex, out int endIndex)
    {
        string content = string.Empty;

        while (compressedDataBytes[startIndex] != ',')
        {
            content += (char)compressedDataBytes[startIndex];
            startIndex++;
        }

        endIndex = startIndex;
        return content;
    }

    private int[] GetFrequencyTable(byte[] compressedDataBytes, int startIndex, out int endIndex)
    {
        int[] frequencyTable = new int[256];
        int tableIndex = 0;
        int i = startIndex;

        for (; i < compressedDataBytes.Length; i++)
        {
            if (compressedDataBytes[i] == ',')
            {
                continue;
            }

            frequencyTable[tableIndex] = int.Parse(GetContent(compressedDataBytes, i, out int getNumberEndIndex));

            i = getNumberEndIndex;

            if (tableIndex == 255)
            {
                break;
            }

            tableIndex++;
        }

        endIndex = i;
        return frequencyTable;
    }

    private byte[] HuffmanDecode(byte[] compressedBytes, HuffmanNode root, int bitCount)
    {
        var bitList = ExtractBits(compressedBytes, bitCount);

        var decodedBytes = new CustomList<byte>();
        var currentNode = root;

        foreach (var bit in bitList)
        {
            currentNode = bit ? currentNode.Right : currentNode.Left;

            if (currentNode.Left == null && currentNode.Right == null)
            {
                decodedBytes.Add(currentNode.Byte);
                currentNode = root;
            }
        }

        return decodedBytes.ToArray();
    }

    private string GetStringFromBytes(byte[] byteArray)
    {
        char[] chars = new char[byteArray.Length];
        int charIndex = 0;

        foreach (byte b in byteArray)
        {
            chars[charIndex++] = (char)b;
        }

        return new string(chars);
    }

    private HuffmanNode BuildTree(int[] frequencyTable)
    {
        var nodes = new CustomLinkedList<HuffmanNode>();

        for (int i = 0; i < frequencyTable.Length; i++)
        {
            if (frequencyTable[i] > 0)
            {
                SortedInsert(
                    nodes,
                    new HuffmanNode
                    {
                        Byte = (byte)i,
                        Count = frequencyTable[i]
                    });
            }
        }

        while (nodes.Count > 1)
        {
            var left = nodes.First.Value;
            nodes.RemoveFirst();
            var right = nodes.First.Value;
            nodes.RemoveFirst();

            var node = new HuffmanNode()
            {
                Count = left.Count + right.Count,
                Left = left,
                Right = right,
            };

            SortedInsert(nodes, node);
        }

        return nodes.First.Value;
    }

    private void SortedInsert(CustomLinkedList<HuffmanNode> nodes, HuffmanNode newNode)
    {
        var current = nodes.First;
        while (current != null && current.Value.Count < newNode.Count)
        {
            current = current.Next;
        }
        if (current == null)
        {
            nodes.AddLast(newNode);
        }
        else
        {
            nodes.AddBefore(current, newNode);
        }
    }

    private byte[] ExtractCompressedData(byte[] compressedDataBytes, int startIndex, int contentLength, out int endIndex)
    {
        byte[] compressedData = new byte[contentLength];

        for (int i = 0; i < contentLength; i++)
        {
            compressedData[i] = compressedDataBytes[startIndex + 1 + i];
        }

        endIndex = startIndex + 1 + contentLength;
        return compressedData;
    }

    private CustomList<bool> ExtractBits(byte[] bytes, int bitCount)
    {
        var bits = new CustomList<bool>();

        for (int byteIndex = 0; byteIndex < bytes.Length && bitCount > 0; byteIndex++)
        {
            byte currentByte = bytes[byteIndex];

            for (int bitIndex = 7; bitIndex >= 0 && bitCount > 0; bitIndex--)
            {
                bits.Add((currentByte & (1 << bitIndex)) != 0);
                bitCount--;
            }
        }

        return bits;
    }

    #endregion
}