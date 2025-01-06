using Crawler.Domain.DataStructures;
using Crawler.Domain.Entities;
using System;
using System.Drawing;
using System.IO;

namespace Crawler.Application.Services.CommandServices.SaveCommandServices;

public class SaveEncodedDataService
{
    public void Save(string filePath, CustomDictionary<string, Bitmap> bmpImages, HtmlNode root, string htmlName)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            string htmlString = $"<{root.TagType}>{root.OuterText}</{root.TagType}>";
            byte[] htmlBytes = GetBytesFromString(htmlString);
            int[] htmlFrequencyTable = GetBytesFrequencies(htmlBytes);
            HuffmanNode htmlHuffmanRoot = BuildTree(htmlFrequencyTable);
            byte[] compressedHtml = HuffmanEncode(htmlHuffmanRoot, htmlBytes, out int htmlBitCount);

            string htmlMetaData = CreateMetaData(htmlName, compressedHtml.Length, htmlBitCount, htmlFrequencyTable);
            byte[] htmlMetaDataBytes = GetBytesFromString(htmlMetaData);
            fileStream.Write(htmlMetaDataBytes, 0, htmlMetaDataBytes.Length);
            fileStream.Write(compressedHtml, 0, compressedHtml.Length);

            foreach (var bmpFile in bmpImages)
            {
                string bmpFileName = bmpFile.Key;
                byte[] bmpBytes = GetBytesFromBitmap(bmpFile.Value);

                string bmpMetaData = $"{bmpFileName},{bmpBytes.Length},";
                byte[] bmpMetaDataBytes = GetBytesFromString(bmpMetaData);
                fileStream.Write(bmpMetaDataBytes, 0, bmpMetaDataBytes.Length);
                fileStream.Write(bmpBytes, 0, bmpBytes.Length);
            }
        }
    }

    #region HelperMethods

    private string CreateMetaData(string fileName, int contentLength, int bitCount, int[] frequencyTable)
    {
        MetaData metaData = new MetaData(fileName, contentLength, bitCount, frequencyTable);
        return metaData.ToString();
    }

    private byte[] GetBytesFromString(string htmlString)
    {
        byte[] bytes = new byte[htmlString.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)htmlString[i];
        }
        return bytes;
    }

    private int[] GetBytesFrequencies(byte[] bytes)
    {
        int[] bytesFreqs = new int[256];
        for (int c = 0; c < bytes.Length; c++)
        {
            bytesFreqs[bytes[c]]++;
        }
        return bytesFreqs;
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

    private HuffmanNode BuildTree(int[] bytesFreqs)
    {
        var nodes = new CustomLinkedList<HuffmanNode>();

        for (int i = 0; i < bytesFreqs.Length; i++)
        {
            if (bytesFreqs[i] > 0)
            {
                SortedInsert(
                    nodes,
                    new HuffmanNode
                    {
                        Byte = (byte)i,
                        Count = bytesFreqs[i]
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

    private byte[] HuffmanEncode(HuffmanNode root, byte[] bytes, out int bitCount)
    {
        var bools = new CustomLinkedList<bool>();

        foreach (var b in bytes)
        {
            Encode(root, b, bools, bools.Last);
        }

        bitCount = bools.Count;

        return ToByteArray(bools);
    }

    private bool Encode(HuffmanNode node, byte b, CustomLinkedList<bool> bools, CustomLinkedListNode<bool> after)
    {
        if (node == null)
        {
            return false;
        }

        if (node.Byte == b)
        {
            return true;
        }

        if (Encode(node.Left, b, bools, after))
        {
            if (after == null)
            {
                bools.AddFirst(false);
            }
            else
            {
                bools.AddAfter(after, false);
            }

            return true;
        }

        if (Encode(node.Right, b, bools, after))
        {
            if (after == null)
            {
                bools.AddFirst(true);
            }
            else
            {
                bools.AddAfter(after, true);
            }

            return true;
        }

        return false;
    }

    private byte[] ToByteArray(CustomLinkedList<bool> bools)
    {
        var bytes = new byte[(int)Math.Ceiling(bools.Count / 8d)];
        var i = 0;

        foreach (var b in bools)
        {
            if (b)
            {
                bytes[i / 8] |= (byte)(1 << (7 - i % 8));
            }
            i++;
        }

        return bytes;
    }

    public static byte[] GetBytesFromBitmap(Bitmap bitmap)
    {
        using (Bitmap clone = new Bitmap(bitmap))
        using (MemoryStream memoryStream = new MemoryStream())
        {
            clone.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            return memoryStream.ToArray();
        }
    }

    #endregion
}
