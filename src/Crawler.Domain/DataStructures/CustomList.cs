using System;
using System.Collections;
using System.Collections.Generic;

namespace Crawler.Domain.DataStructures;

public class CustomList<T> : IEnumerable<T>
{
    private T[] _items;
    private int _count;
    private int _capacity;

    public CustomList()
    {
        _capacity = 10;
        _items = new T[_capacity];
        _count = 0;
    }

    public int Count => _count;

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }

            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }

            _items[index] = value;
        }
    }

    public void Add(T item)
    {
        if (_count == _capacity)
        {
            DoubleCapacity();
        }

        _items[_count] = item;
        _count++;
    }

    public bool Remove(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (Equals(_items[i], item))
            {
                RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count)
        {
            throw new IndexOutOfRangeException();
        }

        for (int i = index; i < _count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        _items[--_count] = default;
    }

    public void Reverse()
    {
        for (int i = 0, j = _count - 1; i < j; i++, j--)
        {
            // ReSharper disable once SwapViaDeconstruction
            T temp = _items[i];
            _items[i] = _items[j];
            _items[j] = temp;
        }
    }

    private void DoubleCapacity()
    {
        _capacity *= 2;
        T[] newItems = new T[_capacity];

        for (int i = 0; i < _count; i++)
        {
            newItems[i] = _items[i];
        }

        _items = newItems;
    }

    public void AddRange(IEnumerable<T> collection)
    {
        foreach (T item in collection)
        {
            Add(item);
        }
    }

    public T[] ToArray()
    {
        T[] newArray = new T[_count];
        for (int i = 0; i < _count; i++)
        {
            newArray[i] = _items[i];
        }
        return newArray;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}