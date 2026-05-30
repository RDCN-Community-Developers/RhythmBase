namespace RhythmBase.Global.Components.Easing;

/// <summary>
/// Specifies that a property should participate in tweening operations, enabling automatic interpolation of its value
/// over time.
/// </summary>
/// <remarks>Apply this attribute to properties that are intended to be animated or smoothly transitioned by a
/// tweening system. This attribute is typically used by frameworks or libraries that detect and process tweenable
/// properties at runtime.</remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class TweenAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the TweenAttribute class.
	/// </summary>
	public TweenAttribute()
	{ }
}
