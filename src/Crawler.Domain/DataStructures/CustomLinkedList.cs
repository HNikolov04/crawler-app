using Crawler.Domain.Entities;
using System.Collections.Generic;

namespace Crawler.Domain.DataStructures;

public class CustomLinkedList<T>
{
    private CustomLinkedListNode<T> _head;
    private CustomLinkedListNode<T> _tail;
    private int _count;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public CustomLinkedList()
    {
        _head = null;
        _tail = null;
        _count = 0;
    }

    public CustomLinkedListNode<T> First => _head;
    public CustomLinkedListNode<T> Last => _tail;
    public int Count => _count;

    public void AddLast(T value)
    {
        CustomLinkedListNode<T> newNode = new CustomLinkedListNode<T>(value);

        if (_tail == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            _tail.Next = newNode;
            _tail = newNode;
        }

        _count++;
    }

    public void AddFirst(T value)
    {
        CustomLinkedListNode<T> newNode = new CustomLinkedListNode<T>(value);

        if (_head == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            newNode.Next = _head;
            _head = newNode;
        }

        _count++;
    }

    public bool AddBefore(CustomLinkedListNode<T> existingNode, T value)
    {
        if (existingNode == null) return false;

        if (existingNode == _head)
        {
            AddFirst(value);
            return true;
        }

        CustomLinkedListNode<T> current = _head;
        while (current != null && current.Next != existingNode)
        {
            current = current.Next;
        }

        if (current == null) return false;

        CustomLinkedListNode<T> newNode = new CustomLinkedListNode<T>(value);
        current.Next = newNode;
        newNode.Next = existingNode;

        _count++;
        return true;
    }

    public bool AddAfter(CustomLinkedListNode<T> existingNode, T value)
    {
        if (existingNode == null) return false;

        CustomLinkedListNode<T> newNode = new CustomLinkedListNode<T>(value);

        if (existingNode == _tail)
        {
            existingNode.Next = newNode;
            _tail = newNode;
        }
        else
        {
            newNode.Next = existingNode.Next;
            existingNode.Next = newNode;
        }

        _count++;
        return true;
    }

    public bool RemoveFirst()
    {
        if (_head == null) return false;

        _head = _head.Next;
        if (_head == null)
        {
            _tail = null;
        }

        _count--;
        return true;
    }

    public bool Remove(T value)
    {
        CustomLinkedListNode<T> current = _head;
        CustomLinkedListNode<T> previous = null;

        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, value))
            {
                if (previous == null) 
                {
                    _head = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }

                if (current.Next == null) 
                {
                    _tail = previous;
                }

                _count--;
                return true;
            }

            previous = current;
            current = current.Next;
        }

        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        CustomLinkedListNode<T> current = _head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }
}