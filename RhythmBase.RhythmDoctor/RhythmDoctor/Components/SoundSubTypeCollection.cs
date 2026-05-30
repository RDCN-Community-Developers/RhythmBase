using System.Collections;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a read-only collection of <see cref="SoundSubType"/> objects.
/// </summary>
/// <remarks>This collection provides a read-only view of the underlying list of <see cref="SoundSubType"/>
/// objects. Attempting to modify the collection, such as adding or removing items, will result in a <see
/// cref="NotImplementedException"/>.</remarks>
public class SoundSubTypeCollection : ICollection<SoundSubType>
{
	internal List<SoundSubType> _sounds = [];
	/// <inheritdoc/>
	public int Count => _sounds.Count;
	/// <inheritdoc/>
	public bool IsReadOnly => true;
	/// <inheritdoc/>
	public void Add(SoundSubType item) => _sounds.Add(item);
	/// <inheritdoc/>
	public void Clear() => _sounds.Clear();
	/// <inheritdoc/>
	public bool Contains(SoundSubType item) => _sounds.Contains(item);
	/// <inheritdoc/>
	public void CopyTo(SoundSubType[] array, int arrayIndex) => _sounds.CopyTo(array, arrayIndex);
	/// <inheritdoc/>
	public IEnumerator<SoundSubType> GetEnumerator() => _sounds.GetEnumerator();
	/// <inheritdoc/>
	public bool Remove(SoundSubType item) => _sounds.Remove(item);
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<SoundSubType>)this).GetEnumerator();
}
