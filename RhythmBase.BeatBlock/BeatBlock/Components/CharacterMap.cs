namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents a character map for BeatBlock levels.
/// </summary>
public class CharacterMap
{
	/// <summary>
	/// Gets the default character map.
	/// </summary>
	public static CharacterMap Default { get; } = new CharacterMap();
	/// <summary>
	/// Gets the regular character map.
	/// </summary>
	public static readonly CharacterMap Regular = new(CharacterMapType.Regular);
	/// <summary>
	/// Gets the full character map.
	/// </summary>
	public static readonly CharacterMap Full = new(CharacterMapType.Full);
	/// <summary>
	/// Gets the map string.
	/// </summary>
	public string Map { get; }
	/// <summary>
	/// Defines the types of character maps.
	/// </summary>
	public const string RegularMap = " !\"#$%&'()*+,-./"
																 + "0123456789:;<=>?"
																 + "@ABCDEFGHIJKLMNO"
																 + "PQRSTUVWXYZ[\\]^_"
																 + "`abcdefghijklmno"
																 + "pqrstuvwxyz{|}~";
	/// <summary>
	/// Defines the full character map, which includes additional characters beyond the regular map.
	/// </summary>
	public const string FullMap = RegularMap
																 + "¡¢£¤¥¦§¨©ª«¬？®¯"
																 + "°±²³´µ¶·¸¹º»¼½¾¿"
																 + "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏ"
																 + "ÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞß"
																 + "àáâãäåæçèéêëìíîï"
																 + "ðñòóôõö÷øùúûüýþÿΩ";
	private CharacterMap(CharacterMapType type)
	{
		Map = type switch
		{
			CharacterMapType.Regular => RegularMap,
			CharacterMapType.Full => FullMap,
			_ => "",
		};
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="CharacterMap"/> class.
	/// </summary>
	/// <param name="map">The map string.</param>
	public CharacterMap(string map)
	{
		Map = map;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="CharacterMap"/> class with an empty map string.
	/// </summary>
	public CharacterMap() : this(CharacterMapType.Custom)
	{
	}
}
