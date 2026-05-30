using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

/// <summary>
/// A size whose horizontal and vertical coordinates are <strong>non-nullable</strong> <see langword="float" />
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct SizeN(float width, float height) : IVector<SizeN, SizeN, float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SizeN"/> struct with the specified point.
	/// </summary>
	/// <param name="pt">The point to initialize the size with.</param>
	public SizeN(PointN pt) : this(pt.X, pt.Y) { }
	/// <summary>
	/// Gets or sets the width of the size.
	/// </summary>
	public float Width { get; set; } = width;
	/// <summary>
	/// Gets or sets the height of the size.
	/// </summary>
	public float Height { get; set; } = height;
	/// <summary>
	/// Gets the area of the size.
	/// </summary>
	public readonly float Area => Width * Height;
	/// <summary>
	/// Adds two sizes together.
	/// </summary>
	/// <param name="sz1">The first size.</param>
	/// <param name="sz2">The second size.</param>
	/// <returns>The result of adding the two sizes.</returns>
	public static SizeN Add(SizeN sz1, SizeN sz2) => new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
	/// <summary>
	/// Subtracts one size from another.
	/// </summary>
	/// <param name="sz1">The size to subtract from.</param>
	/// <param name="sz2">The size to subtract.</param>
	/// <returns>The result of subtracting the second size from the first size.</returns>
	public static SizeN Subtract(SizeN sz1, SizeN sz2) => new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
	/// <inheritdoc/>
#if NETSTANDARD
	public readonly override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 23 + Width.GetHashCode();
		hash = hash * 23 + Height.GetHashCode();
		return hash;
	}
#else
	public readonly override int GetHashCode() => HashCode.Combine(Width, Height);
#endif
	/// <inheritdoc/>
	public readonly override string ToString() => $"[{Width},{Height}]";
	/// <inheritdoc/>
	public readonly bool Equals(SizeN other) => Width == other.Width && Height == other.Height;
	/// <summary>
	/// Converts the current size to an integer size.
	/// </summary>
	/// <returns>A new <see cref="SizeNI"/> instance with the width and height rounded to the nearest integer.</returns>
	public readonly SizeNI ToSizeI() => new((int)Math.Round((double)Width), (int)Math.Round((double)Height));
	/// <summary>
	/// Converts the current size to a point.
	/// </summary>
	/// <returns>A new <see cref="PointN"/> instance with the width as the X coordinate and the height as the Y coordinate.</returns>
	public readonly PointN ToPoint() => new(Width, Height);
	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is SizeN e && Equals(e);
	/// <inheritdoc/>
	public static SizeN operator +(SizeN sz1, SizeN sz2) => Add(sz1, sz2);
	/// <inheritdoc/>
	public static SizeN operator -(SizeN sz1, SizeN sz2) => Subtract(sz1, sz2);
	/// <inheritdoc/>
	public static SizeN operator *(float left, SizeN right) => new(left * right.Width, left * right.Height);
	/// <inheritdoc/>
	public static SizeN operator *(SizeN left, float right) => new(left.Width * right, left.Height * right);
	/// <inheritdoc/>
	public static SizeN operator /(SizeN left, float right) => new(left.Width / right, left.Height / right);
	/// <inheritdoc/>
	public static bool operator ==(SizeN sz1, SizeN sz2) => sz1.Equals(sz2);
	/// <inheritdoc/>
	public static bool operator !=(SizeN sz1, SizeN sz2) => !sz1.Equals(sz2);
	/// <summary>
	/// Implicitly converts an <see cref="SizeN"/> to an <see cref="Size"/>.
	/// </summary>
	/// <param name="size">The <see cref="SizeN"/> to convert.</param>
	/// <returns>A new <see cref="Size"/> instance.</returns>
	public static implicit operator Size(SizeN size) => new(new float?(size.Width), new float?(size.Height));
	/// <summary>
	/// Explicitly converts an <see cref="SizeN"/> to an <see cref="PointN"/>.
	/// </summary>
	/// <param name="size">The <see cref="SizeN"/> to convert.</param>
	/// <returns>A new <see cref="PointN"/> instance.</returns>
	public static explicit operator PointN(SizeN size) => new(size.Width, size.Height);
	private readonly string GetDebuggerDisplay() => ToString();
	/// <summary>
	/// Implicitly converts a tuple containing two <see cref="float"/>.
	/// </summary>
	/// <remarks>This conversion enables concise and readable initialization of <see cref="SizeN"/> instances from tuples,
	/// allowing for more flexible assignment and construction patterns.</remarks>
	/// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="float"/>.</param>
	public static implicit operator SizeN((float width, float height) tuple) => new(tuple.width, tuple.height);
	/// <summary>
	/// Deconstructs the current instance into its Width and Height component values.
	/// </summary>
	/// <param name="width">When this method returns, contains the value of the Width component of the current instance.</param>
	/// <param name="height">When this method returns, contains the value of the Height component of the current instance.</param>
	public readonly void Deconstruct(out float width, out float height) { width = Width; height = Height; }
}
