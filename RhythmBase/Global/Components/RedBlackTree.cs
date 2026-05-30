namespace RhythmBase.Global.Components;

public class RedBlackNode<TKey, TValue>(TKey key, TValue value) where TKey : IComparable<TKey>
{
    public TKey Key = key;
    public TValue Value = value;
    public RedBlackNode<TKey, TValue>? Left = null;
    public RedBlackNode<TKey, TValue>? Right = null;
    public RedBlackNode<TKey, TValue>? Parent = null;
    public bool IsRed = true;
}
/// <summary>
/// Represents a self-balancing binary search tree (red-black tree) that stores key-value pairs.
/// </summary>
/// <typeparam name="TKey">The type of keys in the tree. Must implement <see cref="IComparable{TKey}"/>.</typeparam>
/// <typeparam name="TValue">The type of values in the tree.</typeparam>
public class RedBlackTree<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
{
    private RedBlackNode<TKey, TValue>? _root;
    private int _count;
    /// <summary>
    /// Determines whether the tree contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the tree.</param>
    /// <returns>true if the tree contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(TKey key)
    {
        RedBlackNode<TKey, TValue>? node = FindNode(key);
        return node != null;
    }

    public RedBlackNode<TKey, TValue>? FindNode(TKey key)
    {
        RedBlackNode<TKey, TValue>? current = _root;
        while (current != null)
        {
            int cmp = key.CompareTo(current.Key);
            if (cmp == 0)
                return current;
            current = cmp < 0 ? current.Left : current.Right;
        }
        return null;
    }
    internal RedBlackNode<TKey, TValue>? Floor(TKey key)
    {
        RedBlackNode<TKey, TValue>? current = _root;
        RedBlackNode<TKey, TValue>? floor = null;
        while (current != null)
        {
            int cmp = key.CompareTo(current.Key);
            if (cmp == 0)
                return current;
            if (cmp < 0)
            {
                current = current.Left;
            }
            else
            {
                floor = current;
                current = current.Right;
            }
        }
        return floor;
    }
    internal RedBlackNode<TKey, TValue>? Ceiling(TKey key)
    {
        RedBlackNode<TKey, TValue>? current = _root;
        RedBlackNode<TKey, TValue>? ceiling = null;
        while (current != null)
        {
            int cmp = key.CompareTo(current.Key);
            if (cmp == 0)
                return current;
            if (cmp < 0)
            {
                ceiling = current;
                current = current.Left;
            }
            else
            {
                current = current.Right;
            }
        }
        return ceiling;
    }
    /// <summary>
    /// Inserts a key-value pair into the tree. If the key already exists, updates the value.
    /// </summary>
    /// <param name="key">The key of the element to insert.</param>
    /// <param name="value">The value of the element to insert.</param>
    public void Insert(TKey key, TValue value)
    {
        RedBlackNode<TKey, TValue> newNode = new(key, value);
        RedBlackNode<TKey, TValue>? parent = null;
        RedBlackNode<TKey, TValue>? current = _root;
        while (current != null)
        {
            parent = current;
            int cmp = key.CompareTo(current.Key);
            if (cmp < 0)
                current = current.Left;
            else if (cmp > 0)
                current = current.Right;
            else
            {
                current.Value = value;
                return;
            }
        }
        newNode.Parent = parent;
        if (parent == null)
            _root = newNode;
        else if (key.CompareTo(parent.Key) < 0)
            parent.Left = newNode;
        else
            parent.Right = newNode;
        _count++;
        InsertFixup(newNode);
    }

    private void InsertFixup(RedBlackNode<TKey, TValue> node)
    {
        while (node.Parent != null && node.Parent.IsRed)
        {
            RedBlackNode<TKey, TValue>? grandparent = node.Parent.Parent;
            if (node.Parent == grandparent?.Left)
            {
                RedBlackNode<TKey, TValue>? uncle = grandparent?.Right;
                if (uncle != null && uncle.IsRed)
                {
                    node.Parent.IsRed = false;
                    uncle.IsRed = false;
                    grandparent?.IsRed = true;
                    node = grandparent!;
                }
                else
                {
                    if (node == node.Parent.Right)
                    {
                        node = node.Parent;
                        RotateLeft(node);
                    }
                    node.Parent?.IsRed = false;
                    if (grandparent != null)
                    {
                        grandparent.IsRed = true;
                        RotateRight(grandparent);
                    }
                }
            }
            else
            {
                RedBlackNode<TKey, TValue>? uncle = grandparent?.Left;
                if (uncle != null && uncle.IsRed)
                {
                    node.Parent.IsRed = false;
                    uncle.IsRed = false;
                    grandparent?.IsRed = true;
                    node = grandparent!;
                }
                else
                {
                    if (node == node.Parent.Left)
                    {
                        node = node.Parent;
                        RotateRight(node);
                    }
                    node.Parent?.IsRed = false;
                    if (grandparent != null)
                    {
                        grandparent.IsRed = true;
                        RotateLeft(grandparent);
                    }
                }
            }
        }
        _root?.IsRed = false;
    }

    private void RotateLeft(RedBlackNode<TKey, TValue> node)
    {
        RedBlackNode<TKey, TValue>? right = node.Right;
        if (right == null) return;
        node.Right = right.Left;
        right.Left?.Parent = node;
        right.Parent = node.Parent;
        if (node.Parent == null)
            _root = right;
        else if (node == node.Parent.Left)
            node.Parent.Left = right;
        else
            node.Parent.Right = right;
        right.Left = node;
        node.Parent = right;
    }

    private void RotateRight(RedBlackNode<TKey, TValue> node)
    {
        RedBlackNode<TKey, TValue>? left = node.Left;
        if (left == null) return;
        node.Left = left.Right;
        left.Right?.Parent = node;
        left.Parent = node.Parent;
        if (node.Parent == null)
            _root = left;
        else if (node == node.Parent.Right)
            node.Parent.Right = left;
        else
            node.Parent.Left = left;
        left.Right = node;
        node.Parent = left;
    }

    /// <summary>
    /// Removes the element with the specified key from the tree.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>true if the element was successfully removed; otherwise, false.</returns>
    public bool Remove(TKey key)
    {
        RedBlackNode<TKey, TValue>? node = FindNode(key);
        if (node == null)
            return false;
        DeleteNode(node);
        _count--;
        return true;
    }

    private void DeleteNode(RedBlackNode<TKey, TValue> node)
    {
        RedBlackNode<TKey, TValue>? y = node;
        bool yOriginalIsRed = y.IsRed;
        RedBlackNode<TKey, TValue>? x;
        if (node.Left == null)
        {
            x = node.Right;
            Transplant(node, node.Right);
        }
        else if (node.Right == null)
        {
            x = node.Left;
            Transplant(node, node.Left);
        }
        else
        {
            y = Minimum(node.Right);
            yOriginalIsRed = y.IsRed;
            x = y.Right;
            if (y.Parent == node)
            {
                x?.Parent = y;
            }
            else
            {
                Transplant(y, y.Right);
                y.Right = node.Right;
                y.Right?.Parent = y;
            }
            Transplant(node, y);
            y.Left = node.Left;
            y.Left?.Parent = y;
            y.IsRed = node.IsRed;
        }
        if (!yOriginalIsRed)
            DeleteFixup(x, node.Parent);
    }

    private void DeleteFixup(RedBlackNode<TKey, TValue>? x, RedBlackNode<TKey, TValue>? parent)
    {
        while (x != _root && (x == null || !x.IsRed))
        {
            if (parent == null)
                break;
            if (x == parent.Left)
            {
                RedBlackNode<TKey, TValue>? w = parent.Right;
                if (w != null && w.IsRed)
                {
                    w.IsRed = false;
                    parent.IsRed = true;
                    RotateLeft(parent);
                    w = parent.Right;
                }
                if ((w?.Left == null || !w.Left.IsRed) && (w?.Right == null || !w.Right.IsRed))
                {
                    w?.IsRed = true;
                    x = parent;
                    parent = x.Parent;
                }
                else
                {
                    if (w?.Right == null || !w.Right.IsRed)
                    {
                        if (w?.Left != null)
                            w.Left.IsRed = false;
                        w?.IsRed = true;
                        if (w != null)
                            RotateRight(w);
                        w = parent.Right;
                    }
                    w?.IsRed = parent.IsRed;
                    parent.IsRed = false;
                    if (w?.Right != null)
                        w.Right.IsRed = false;
                    RotateLeft(parent);
                    x = _root;
                }
            }
            else
            {
                RedBlackNode<TKey, TValue>? w = parent.Left;
                if (w != null && w.IsRed)
                {
                    w.IsRed = false;
                    parent.IsRed = true;
                    RotateRight(parent);
                    w = parent.Left;
                }
                if ((w?.Right == null || !w.Right.IsRed) && (w?.Left == null || !w.Left.IsRed))
                {
                    w?.IsRed = true;
                    x = parent;
                    parent = x.Parent;
                }
                else
                {
                    if (w?.Left == null || !w.Left.IsRed)
                    {
                        if (w?.Right != null)
                            w.Right.IsRed = false;
                        w?.IsRed = true;
                        if (w != null)
                            RotateLeft(w);
                        w = parent.Left;
                    }
                    w?.IsRed = parent.IsRed;
                    parent.IsRed = false;
                    if (w?.Left != null)
                        w.Left.IsRed = false;
                    RotateRight(parent);
                    x = _root;
                }
            }
        }
        x?.IsRed = false;
    }

    private void Transplant(RedBlackNode<TKey, TValue> u, RedBlackNode<TKey, TValue>? v)
    {
        if (u.Parent == null)
            _root = v;
        else if (u == u.Parent.Left)
            u.Parent.Left = v;
        else
            u.Parent.Right = v;
        v?.Parent = u.Parent;
    }

    private static RedBlackNode<TKey, TValue> Minimum(RedBlackNode<TKey, TValue> node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value associated with the specified key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key is not found in the tree.</exception>
    public TValue this[TKey key]
    {
        get
        {
            RedBlackNode<TKey, TValue>? node = FindNode(key);
            return node == null ? throw new KeyNotFoundException() : node.Value;
        }
        set => Insert(key, value);
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
    /// <returns>true if the tree contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        RedBlackNode<TKey, TValue>? node = FindNode(key);
        if (node != null)
        {
            value = node.Value;
            return true;
        }
        value = default!;
        return false;
    }

    #region ICollection<KeyValuePair<TKey, TValue>> 实现

    /// <summary>
    /// Gets the number of key-value pairs contained in the tree.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Gets a value indicating whether the tree is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds a key-value pair to the tree.
    /// </summary>
    /// <param name="item">The key-value pair to add.</param>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Insert(item.Key, item.Value);
    }

    /// <summary>
    /// Removes all elements from the tree.
    /// </summary>
    public void Clear()
    {
        _root = null;
        _count = 0;
    }

    /// <summary>
    /// Determines whether the tree contains a specific key-value pair.
    /// </summary>
    /// <param name="item">The key-value pair to locate in the tree.</param>
    /// <returns>true if the tree contains the specified key-value pair; otherwise, false.</returns>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        RedBlackNode<TKey, TValue>? node = FindNode(item.Key);
        return node != null && EqualityComparer<TValue>.Default.Equals(node.Value, item.Value);
    }

    /// <summary>
    /// Copies the elements of the tree to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from the tree.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="ArgumentException">Thrown when the destination array does not have enough space.</exception>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array.Length - arrayIndex < _count)
            throw new ArgumentException("目标数组空间不足。");

        int i = arrayIndex;
        foreach (KeyValuePair<TKey, TValue> kv in this)
        {
            array[i++] = kv;
        }
    }

    /// <summary>
    /// Removes the first occurrence of a specific key-value pair from the tree.
    /// </summary>
    /// <param name="item">The key-value pair to remove.</param>
    /// <returns>true if the key-value pair was successfully removed; otherwise, false.</returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        RedBlackNode<TKey, TValue>? node = FindNode(item.Key);
        if (node == null)
            return false;
        if (!EqualityComparer<TValue>.Default.Equals(node.Value, item.Value))
            return false;
        DeleteNode(node);
        _count--;
        return true;
    }
    /// <summary>
    /// Returns the first key-value pair in the tree that satisfies the specified condition.
    /// </summary>
    /// <param name="predicate">A function to test each key-value pair for a condition.</param>
    /// <returns>The first key-value pair that satisfies the condition, or the default value if no such pair is found.</returns>
    public KeyValuePair<TKey, TValue> FirstOrDefault(Func<KeyValuePair<TKey, TValue>, bool> predicate)
    {
        var first = this._root;
        while (first is not null)
        {
            if (predicate(new KeyValuePair<TKey, TValue>(first.Key, first.Value)))
            {
                if (first.Left is not null)
                    first = first.Left;
                else
                    return new KeyValuePair<TKey, TValue>(first.Key, first.Value);
            }
            else
            {
                if (first.Right is not null)
                    first = first.Right;
                else
                    return default;
            }
        }
        return default;
    }
    /// <summary>
    /// Returns the last key-value pair in the tree that satisfies the specified condition.
    /// </summary>
    /// <param name="predicate">A function to test each key-value pair for a condition.</param>
    /// <returns>The last key-value pair that satisfies the condition, or the default value if no such pair is found.</returns>
    public KeyValuePair<TKey, TValue> LastOrDefault(Func<KeyValuePair<TKey, TValue>, bool> predicate)
    {
        var last = this._root;
        while (last is not null)
        {
            if (predicate(new KeyValuePair<TKey, TValue>(last.Key, last.Value)))
            {
                if (last.Right is not null)
                    last = last.Right;
                else
                    return new KeyValuePair<TKey, TValue>(last.Key, last.Value);
            }
            else
            {
                if (last.Left is not null)
                    last = last.Left;
                else
                    return default;
            }
        }
        return default;
    }
    /// <summary>
    /// Returns the first key-value pair in the tree.
    /// </summary>
    /// <returns>The first key-value pair, or the default value if the tree is empty.</returns>
    public KeyValuePair<TKey, TValue> FirstOrDefault()
    {
        var first = this._root;
        while (first is not null)
        {
            if (first.Left is not null)
                first = first.Left;
            else
                return new KeyValuePair<TKey, TValue>(first.Key, first.Value);
        }
        return default;
    }
    /// <summary>
    /// Returns the last key-value pair in the tree.
    /// </summary>
    /// <returns>The last key-value pair, or the default value if the tree is empty.</returns>
    public KeyValuePair<TKey, TValue> LastOrDefault()
    {
        var last = this._root;
        while (last is not null)
        {
            if (last.Right is not null)
                last = last.Right;
            else
                return new KeyValuePair<TKey, TValue>(last.Key, last.Value);
        }
        return default;
    }
    /// <summary>
    /// Returns an enumerator that iterates through the tree in key order.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the tree.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrderTraversal(_root).GetEnumerator();
    }
    private static IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(RedBlackNode<TKey, TValue>? node)
    {
        if (node == null)
            yield break;
        foreach (KeyValuePair<TKey, TValue> kv in InOrderTraversal(node.Left))
            yield return kv;
        yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        foreach (KeyValuePair<TKey, TValue> kv in InOrderTraversal(node.Right))
            yield return kv;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}
