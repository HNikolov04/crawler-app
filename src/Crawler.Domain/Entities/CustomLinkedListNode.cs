namespace Crawler.Domain.Entities;

public class CustomLinkedListNode<T>
{
    public T Value { get; set; }
    public CustomLinkedListNode<T> Next { get; set; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public CustomLinkedListNode(T value)
    {
        Value = value;
        Next = null;
    }
}