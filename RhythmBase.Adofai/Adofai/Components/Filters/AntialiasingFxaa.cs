namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Antialiasing FXAA</b>.
/// </summary>
[JsonObjectSerializable]
public struct AntialiasingFxaa : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.AntialiasingFxaa;
}