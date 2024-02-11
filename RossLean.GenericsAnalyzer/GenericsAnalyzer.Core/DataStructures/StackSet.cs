using System.Collections;
using System.Collections.Generic;

namespace RossLean.GenericsAnalyzer.Core.DataStructures;

public class StackSet<T> : IEnumerable<T>
{
    private readonly Stack<T> stack;
    private readonly HashSet<T> set;

    public int Count => set.Count;
    public bool IsEmpty => Count == 0;

    public StackSet()
    {
        stack = new Stack<T>();
        set = new HashSet<T>();
    }
    public StackSet(int capacity)
    {
        stack = new Stack<T>(capacity);
        set = new HashSet<T>();
    }
    public StackSet(IEqualityComparer<T> comparer)
    {
        stack = new Stack<T>();
        set = new HashSet<T>(comparer);
    }
    public StackSet(int capacity, IEqualityComparer<T> comparer)
    {
        stack = new Stack<T>(capacity);
        set = new HashSet<T>(comparer);
    }

    public bool Contains(T item) => set.Contains(item);

    public bool Push(T item)
    {
        if (!set.Add(item))
            return false;

        stack.Push(item);
        return true;
    }
    public T Pop()
    {
        var item = stack.Pop();
        set.Remove(item);
        return item;
    }

    public void Clear()
    {
        set.Clear();
        stack.Clear();
    }

    public void CopyTo(T[] array, int arrayIndex) => stack.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => stack.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
