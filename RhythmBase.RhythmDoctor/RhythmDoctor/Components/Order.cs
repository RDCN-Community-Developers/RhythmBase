using RhythmBase.RhythmDoctor.Converters;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a unique order of rooms identified by their IDs.
/// </summary>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.BuildOrder))]
[System.Text.Json.Serialization.JsonConverter(typeof(OrderConverter))]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct Order : IEnumerable<int>
{
    private readonly int _id;
    private readonly int _length;

    /// <summary>
    /// Initializes a new instance of the <see cref="Order"/> struct with the specified room IDs.
    /// </summary>
    /// <param name="indices">An array of room IDs representing the order. Each ID must be unique.</param>
    /// <exception cref="ArgumentException">Thrown when room IDs are not unique.</exception>
    public Order(params int[] indices)
    {
        _length = indices.Length;
        _id = PermutationToId(indices);
    }
    /// <summary>
    /// The number of rooms in the order.
    /// </summary>
    public int Length => _length;

    /// <summary>
    /// Gets the order of room IDs as an array of numbers.
    /// </summary>
    public readonly int[] Indices => IdToPermutation(_id, _length);

    /// <summary>
    /// Gets the room ID at the specified index in the order.
    /// </summary>
    /// <param name="index">The index of the room ID to retrieve (0 to 3).</param>
    /// <returns>The room ID at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    public readonly byte this[int index] => index < 0 || index >= _length
                ? throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 3.")
                : (byte)Indices[index];

    /// <summary>
    /// Returns a string representation of the order.
    /// </summary>
    /// <returns>A string representing the order.</returns>
    public readonly override string ToString()
    {
        return $"[{string.Join(", ", Indices)}]";
    }
    private static int Factorial(int n)
    {
        int result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }
    private static int PermutationToId(int[] order)
    {
        int n = order.Length;
        int id = 0;
        for (int i = 0; i < n; i++)
        {
            int count = 0;
            for (int j = i + 1; j < n; j++)
                if (order[j] < order[i]) count++;
            id += count * Factorial(n - 1 - i);
        }
        return id;
    }
    private static int[] IdToPermutation(int id, int n)
    {
        List<int> numbers = [.. Enumerable.Range(0, n).Select(i => (int)i)];
        int[] order = new int[n];
        for (int i = 0; i < n; i++)
        {
            int factorial = Factorial(n - 1 - i);
            int index = id / factorial;
            order[i] = numbers[index];
            numbers.RemoveAt(index);
            id %= factorial;
        }
        return order;
    }
    /// <summary>
    /// Returns an enumerator that iterates through the collection of indices as integers.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly IEnumerator<int> GetEnumerator()
    {
        int[] order = Indices;
        for (int i = 0; i < _length; i++)
            yield return order[i];
        yield break;
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    readonly IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
