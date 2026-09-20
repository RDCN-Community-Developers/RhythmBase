using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// A set of built-in sound effect groups that can be used in the game.
/// </summary>
public class SoundCollection : IReadOnlyDictionary<SoundType, Audio?>
{
#pragma warning disable CS1591
	internal protected SoundType[] _keys;
	internal protected Audio?[] _values;
#pragma warning restore CS1591
	/// <summary>
	/// Creates a new instance of the <see cref="SoundCollection"/> class.
	/// </summary>
	/// <param name="types">The array of <see cref="SoundType"/> values that represent the keys of the collection.</param>
	/// <exception cref="ArgumentException">
	/// Thrown when no <see cref="SoundType"/> values are provided or when there are duplicate <see cref="SoundType"/> values in the array.
	/// </exception>
	public SoundCollection(params SoundType[] types)
	{
		if (types.Length == 0)
			throw new ArgumentException("At least one SoundType must be provided.", nameof(types));
		if(types.Distinct().Count() != types.Length)
			throw new ArgumentException("Duplicate SoundType values are not allowed.", nameof(types));
		_keys = types;
		_values = new Audio[types.Length];
	}
	///<inheritdoc/>
	public Audio? this[SoundType key]
	{
		get
		{
			int index = Array.IndexOf(_keys, key);
			if (index >= 0)
				return _values[index];
			throw new KeyNotFoundException($"The given key '{key}' was not present in the collection.");
		}
		set
		{
			int index = Array.IndexOf(_keys, key);
			if (index >= 0)
				_values[index] = value;
			else
				throw new KeyNotFoundException($"The given key '{key}' was not present in the collection.");
		}
	}
	///<inheritdoc/>
	internal ref Audio? First => ref _values[0];
	///<inheritdoc/>
	public IEnumerable<SoundType> Keys => Array.AsReadOnly(_keys);
	///<inheritdoc/>
	public IEnumerable<Audio?> Values => Array.AsReadOnly(_values);
	///<inheritdoc/>
	public int Count => _keys.Length;
	///<inheritdoc/>
	public bool ContainsKey(SoundType key) => _keys.Contains(key);
	///<inheritdoc/>
	public IEnumerator<KeyValuePair<SoundType, Audio?>> GetEnumerator() => _keys.Zip(_values, (k, v) => new KeyValuePair<SoundType, Audio?>(k, v)).GetEnumerator();
	///<inheritdoc/>
	public bool TryGetValue(SoundType key, [MaybeNullWhen(false)] out Audio? value)
	{
		if (_keys.Contains(key))
		{
			value = this[key];
			return true;
		}
		value = default;
		return false;
	}
	/*
			"ClapSoundHold": ["ClapSoundHoldLongEnd", "ClapSoundHoldLongStart", "ClapSoundHoldShortEnd", "ClapSoundHoldShortStart"],
			"PulseSoundHold": ["PulseSoundHoldStart", "PulseSoundHoldShortEnd", "PulseSoundHoldEnd", "PulseSoundHoldStartAlt", "PulseSoundHoldShortEndAlt", "PulseSoundHoldEndAlt"],
			"ClapSoundHoldP2": ["ClapSoundHoldLongEndP2", "ClapSoundHoldLongStartP2", "ClapSoundHoldShortEndP2", "ClapSoundHoldShortStartP2"],
			"PulseSoundHoldP2": ["PulseSoundHoldStartP2", "PulseSoundHoldShortEndP2", "PulseSoundHoldEndP2", "PulseSoundHoldStartAltP2", "PulseSoundHoldShortEndAltP2", "PulseSoundHoldEndAltP2"],
			"FreezeshotSound": ["FreezeshotSoundCueLow", "FreezeshotSoundCueHigh", "FreezeshotSoundRiser", "FreezeshotSoundCymbal"],
			"BurnshotSound": ["BurnshotSoundCueLow", "BurnshotSoundCueHigh", "BurnshotSoundRiser", "BurnshotSoundCymbal"],
			"HoldshotSound": ["HoldshotSoundCue", "HoldshotSoundClapStart", "HoldshotSoundClapShortEnd", "HoldshotSoundClapLongEnd"]

	 */

	///<inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	/// <summary>
	/// The <see cref="SoundCollection"/> class that represents a single audio sound collection.
	/// </summary>
	public class SingleAudioSoundCollection : SoundCollection
	{
		/// <summary>
		/// Creates a new instance of the <see cref="SingleAudioSoundCollection"/> class with a single <see cref="SoundType"/>.
		/// </summary>
		/// <param name="type"></param>
		public SingleAudioSoundCollection(SoundType type) : base(type) { }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the single <see cref="SoundType"/> in this collection.
		/// </summary>
		public Audio? Audio { get => _values[0]; set => _values[0] = value; }
	}
	/// <summary>
	/// The <see cref="SoundCollection"/> class that represents a multiple audio sound collection.
	/// </summary>
	public class ClapSound : SoundCollection
	{
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.ClapSoundHoldLongEnd"/> in this collection.
		/// </summary>
		public Audio? LongEnd { get => _values[0]; set => _values[0] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.ClapSoundHoldLongStart"/> in this collection.
		/// </summary>
		public Audio? LongStart { get => _values[1]; set => _values[1] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.ClapSoundHoldShortEnd"/> in this collection.
		/// </summary>
		public Audio? ShortEnd { get => _values[2]; set => _values[2] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.ClapSoundHoldShortStart"/> in this collection.
		/// </summary>
		public Audio? ShortStart { get => _values[3]; set => _values[3] = value; }
		/// <summary>
		/// Creates a new instance of the <see cref="ClapSound"/> class with the values
		/// <see cref="SoundType.ClapSoundHoldLongEnd"/>,
		/// <see cref="SoundType.ClapSoundHoldLongStart"/>,
		/// <see cref="SoundType.ClapSoundHoldShortEnd"/>,
		/// and <see cref="SoundType.ClapSoundHoldShortStart"/>.
		/// </summary>
		public ClapSound() : base(
			SoundType.ClapSoundHoldLongEnd,
			SoundType.ClapSoundHoldLongStart,
			SoundType.ClapSoundHoldShortEnd,
			SoundType.ClapSoundHoldShortStart
		)
		{ }
	}
	/// <summary>
	/// The <see cref="SoundCollection"/> class that represents a multiple audio sound collection.
	/// </summary>
	public class PulseSound : SoundCollection
	{
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldStart"/> in this collection.
		/// </summary>
		public Audio? Start { get => _values[0]; set => _values[0] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldShortEnd"/> in this collection.
		/// </summary>
		public Audio? ShortEnd { get => _values[1]; set => _values[1] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldEnd"/> in this collection.
		/// </summary>
		public Audio? End { get => _values[2]; set => _values[2] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldStartAlt"/> in this collection.
		/// </summary>
		public Audio? StartAlt { get => _values[3]; set => _values[3] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldShortEndAlt"/> in this collection.
		/// </summary>
		public Audio? ShortEndAlt { get => _values[4]; set => _values[4] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldEndAlt"/> in this collection.
		/// </summary>
		public Audio? EndAlt { get => _values[5]; set => _values[5] = value; }
		/// <summary>
		/// Creates a new instance of the <see cref="PulseSound"/> class with the values
		/// <see cref="SoundType.PulseSoundHoldStart"/>,
		/// <see cref="SoundType.PulseSoundHoldShortEnd"/>,
		/// <see cref="SoundType.PulseSoundHoldEnd"/>,
		/// <see cref="SoundType.PulseSoundHoldStartAlt"/>,
		/// <see cref="SoundType.PulseSoundHoldShortEndAlt"/>,
		/// <see cref="SoundType.PulseSoundHoldEndAlt"/>.
		/// </summary>
		public PulseSound() : base(
			SoundType.PulseSoundHoldStart,
			SoundType.PulseSoundHoldShortEnd,
			SoundType.PulseSoundHoldEnd,
			SoundType.PulseSoundHoldStartAlt,
			SoundType.PulseSoundHoldShortEndAlt,
			SoundType.PulseSoundHoldEndAlt)
		{ }
	}
	/// <summary>
	/// The <see cref="SoundCollection"/> class that represents a multiple audio sound collection.
	/// </summary>
	public class ClapSoundP2 : SoundCollection
	{
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.ClapSoundHoldLongEndP2"/> in this collection.
		/// </summary>
		public Audio? LongEnd { get => _values[0]; set => _values[0] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.ClapSoundHoldLongStartP2"/> in this collection.
		/// </summary>
		public Audio? LongStart { get => _values[1]; set => _values[1] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.ClapSoundHoldShortEndP2"/> in this collection.
		/// </summary>
		public Audio? ShortEnd { get => _values[2]; set => _values[2] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.ClapSoundHoldShortStartP2"/> in this collection.
		/// </summary>
		public Audio? ShortStart { get => _values[3]; set => _values[3] = value; }
		/// <summary>
		/// Creates a new instance of the <see cref="ClapSoundP2"/> class with the values
		/// <see cref="SoundType.ClapSoundHoldLongEndP2"/>,
		/// <see cref="SoundType.ClapSoundHoldLongStartP2"/>,
		/// <see cref="SoundType.ClapSoundHoldShortEndP2"/>,
		/// and <see cref="SoundType.ClapSoundHoldShortStartP2"/>.
		/// </summary>
		public ClapSoundP2() : base(
			SoundType.ClapSoundHoldLongEndP2,
			SoundType.ClapSoundHoldLongStartP2,
			SoundType.ClapSoundHoldShortEndP2,
			SoundType.ClapSoundHoldShortStartP2
		)
		{ }
	}
	/// <summary>
	/// The <see cref="SoundCollection"/> class that represents a multiple audio sound collection.
	/// </summary>
	public class PulseSoundP2 : SoundCollection
	{
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldStartP2"/> in this collection.
		/// </summary>
		public Audio? Start { get => _values[0]; set => _values[0] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldShortEndP2"/> in this collection.
		/// </summary>
		public Audio? ShortEnd { get => _values[1]; set => _values[1] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldEndP2"/> in this collection.
		/// </summary>
		public Audio? End { get => _values[2]; set => _values[2] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldStartAltP2"/> in this collection.
		/// </summary>
		public Audio? StartAlt { get => _values[3]; set => _values[3] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldShortEndAltP2"/> in this collection.
		/// </summary>
		public Audio? ShortEndAlt { get => _values[4]; set => _values[4] = value; }
        /// <summary>
        /// The <see cref="Audio"/> object associated with the <see cref="SoundType.PulseSoundHoldEndAltP2"/> in this collection.
        /// </summary>
        public Audio? EndAlt { get => _values[5]; set => _values[5] = value; }
		/// <summary>
		/// Creates a new instance of the <see cref="PulseSoundP2"/> class with the values
		/// <see cref="SoundType.PulseSoundHoldStartP2"/>,
		/// <see cref="SoundType.PulseSoundHoldShortEndP2"/>,
		/// <see cref="SoundType.PulseSoundHoldEndP2"/>,
		/// <see cref="SoundType.PulseSoundHoldStartAltP2"/>,
		/// <see cref="SoundType.PulseSoundHoldShortEndAltP2"/>,
		/// and <see cref="SoundType.PulseSoundHoldEndAltP2"/>.
		/// </summary>
		public PulseSoundP2() : base(
			SoundType.PulseSoundHoldStartP2,
			SoundType.PulseSoundHoldShortEndP2,
			SoundType.PulseSoundHoldEndP2,
			SoundType.PulseSoundHoldStartAltP2,
			SoundType.PulseSoundHoldShortEndAltP2,
			SoundType.PulseSoundHoldEndAltP2
		)
		{ }
	}
	/// <summary>
	/// The <see cref="SoundCollection"/> class that represents a multiple audio sound collection.
	/// </summary>
	public class FreezeshotSound : SoundCollection
	{
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.FreezeshotSoundCueLow"/> in this collection.
		/// </summary>
		public Audio? CueLow { get => _values[0]; set => _values[0] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.FreezeshotSoundCueHigh"/> in this collection.
		/// </summary>
		public Audio? CueHigh { get => _values[1]; set => _values[1] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.FreezeshotSoundRiser"/> in this collection.
		/// </summary>
		public Audio? Riser { get => _values[2]; set => _values[2] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.FreezeshotSoundCymbal"/> in this collection.
		/// </summary>
		public Audio? Cymbal { get => _values[3]; set => _values[3] = value; }
		/// <summary>
		/// Creates a new instance of the <see cref="FreezeshotSound"/> class with the values
		/// <see cref="SoundType.FreezeshotSoundCueLow"/>,	
		/// <see cref="SoundType.FreezeshotSoundCueHigh"/>,
		/// <see cref="SoundType.FreezeshotSoundRiser"/>, and
		/// <see cref="SoundType.FreezeshotSoundCymbal"/>.
		/// </summary>
		public FreezeshotSound() : base(
			SoundType.FreezeshotSoundCueLow,
			SoundType.FreezeshotSoundCueHigh,
			SoundType.FreezeshotSoundRiser,
			SoundType.FreezeshotSoundCymbal
		)
		{ }
	}
	/// <summary>
	/// The <see cref="SoundCollection"/> class that represents a multiple audio sound collection.
	/// </summary>
	public class BurnshotSound : SoundCollection
	{
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.BurnshotSoundCueLow"/> in this collection.
		/// </summary>
		public Audio? CueLow { get => _values[0]; set => _values[0] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.BurnshotSoundCueHigh"/> in this collection.
		/// </summary>
		public Audio? CueHigh { get => _values[1]; set => _values[1] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.BurnshotSoundRiser"/> in this collection.
		/// </summary>
		public Audio? Riser { get => _values[2]; set => _values[2] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.BurnshotSoundCymbal"/> in this collection.
		/// </summary>
		public Audio? Cymbal { get => _values[3]; set => _values[3] = value; }
		/// <summary>
		/// Creates a new instance of the <see cref="BurnshotSound"/> class with the values
		/// <see cref="SoundType.BurnshotSoundCueLow"/>,
		/// <see cref="SoundType.BurnshotSoundCueHigh"/>,
		/// <see cref="SoundType.BurnshotSoundRiser"/>, and
		/// <see cref="SoundType.BurnshotSoundCymbal"/>.
		/// </summary>
		public BurnshotSound() : base(
			SoundType.BurnshotSoundCueLow,
			SoundType.BurnshotSoundCueHigh,
			SoundType.BurnshotSoundRiser,
			SoundType.BurnshotSoundCymbal
		)
		{ }
	}
	/// <summary>
	/// The <see cref="SoundCollection"/> class that represents a multiple audio sound collection.
	/// </summary>
	public class HoldshotSound : SoundCollection
	{
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.HoldshotSoundCue"/> in this collection.
		/// </summary>
		public Audio? Cue { get => _values[0]; set => _values[0] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.HoldshotSoundClapStart"/> in this collection.
		/// </summary>
		public Audio? ClapStart { get => _values[1]; set => _values[1] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.HoldshotSoundClapShortEnd"/> in this collection.
		/// </summary>
		public Audio? ClapShortEnd { get => _values[2]; set => _values[2] = value; }
		/// <summary>
		/// The <see cref="Audio"/> object associated with the <see cref="SoundType.HoldshotSoundClapLongEnd"/> in this collection.
		/// </summary>
		public Audio? ClapLongEnd { get => _values[3]; set => _values[3] = value; }
		/// <summary>
		/// Creates a new instance of the <see cref="HoldshotSound"/> class with the values
		/// <see cref="SoundType.HoldshotSoundCue"/>,
		/// <see cref="SoundType.HoldshotSoundClapStart"/>,
		/// <see cref="SoundType.HoldshotSoundClapShortEnd"/>,
		/// and <see cref="SoundType.HoldshotSoundClapLongEnd"/>.
		/// </summary>
		public HoldshotSound() : base(
			SoundType.HoldshotSoundCue,
			SoundType.HoldshotSoundClapStart,
			SoundType.HoldshotSoundClapShortEnd,
			SoundType.HoldshotSoundClapLongEnd
		)
		{ }
	}
}
