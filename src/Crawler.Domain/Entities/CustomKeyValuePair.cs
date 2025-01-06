namespace Crawler.Domain.Entities;

public class CustomKeyValuePair<TKey, TValue>
{
    public TKey Key { get; }
    public TValue Value { get; set; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public CustomKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
