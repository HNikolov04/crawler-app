using Crawler.Domain.Entities;
using System;

namespace Crawler.Domain.DataStructures;

public class CustomStack<T>
{
    private StackNode<T>[] _array; 
    private int _size;
    private int _capacity;          
    private const int DefaultCapacity = 4; 

    public int Count => _size;

    public CustomStack()
    {
        _capacity = DefaultCapacity; 
        _array = new StackNode<T>[_capacity]; 
        _size = 0; 
    }

    public CustomStack(CustomList<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items), "The list cannot be null.");

        _capacity = Math.Max(items.Count, DefaultCapacity);
        _array = new StackNode<T>[_capacity]; 
        _size = 0;

        foreach (var item in items)
        {
            Push(item);
        }
    }

    public void Push(T item)
    {
        if (_size == _capacity)
        {
            EnsureCapacity(_size + 1);
        }

        _array[_size] = new StackNode<T> { Value = item };
        _size++;
    }

    public T Pop()
    {
        if (_size == 0)
        {
            throw new InvalidOperationException("Stack is empty.");
        }

        _size--;
        T value = _array[_size].Value;
        _array[_size] = null; 

        return value;
    }

    public T Peek()
    {
        if (_size == 0)
        {
            throw new InvalidOperationException("Stack is empty.");
        }

        return _array[_size - 1].Value;
    }

    private void EnsureCapacity(int min)
    {
        if (_capacity >= min)
        {
            return;
        }

        int newCapacity = _capacity == 0 ? DefaultCapacity : _capacity * 2;
        newCapacity = Math.Max(newCapacity, min);
        Resize(newCapacity);
    }

    private void Resize(int newCapacity)
    {
        var newArray = new StackNode<T>[newCapacity];
        Array.Copy(_array, newArray, _size);
        _array = newArray;
        _capacity = newCapacity;
    }
}
