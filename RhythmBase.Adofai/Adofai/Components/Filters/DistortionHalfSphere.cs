namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Half Sphere</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionHalfSphere : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionHalfSphere;
	/// <summary>
	/// Gets or sets the value of the <b>SphereSize</b>.
	/// </summary>
	[JsonAlias("SphereSize")]
	public float SphereSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Strength</b>.
	/// </summary>
	[JsonAlias("Strength")]
	public float Strength { get; set; }
}