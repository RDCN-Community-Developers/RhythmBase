using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

/// <summary>
/// A size whose horizontal and vertical coordinates are <strong>nullable</strong> <see langword="float" />
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct Size(float? width, float? height) : IVector<Size, Size, float?>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Size"/> struct with the specified point.
	/// </summary>
	/// <param name="pt">The point to initialize the size with.</param>
	public Size(Point pt) : this(pt.X, pt.Y) { }
	/// <summary>
	/// Gets a value indicating whether this size is empty (both width and height are null).
	/// </summary>
	public readonly bool IsEmpty => Width == null && Height == null;
	/// <summary>
	/// Gets or sets the width of the size.
	/// </summary>
	public float? Width { get; set; } = width;
	/// <summary>
	/// Gets or sets the height of the size.
	/// </summary>
	public float? Height { get; set; } = height;
	/// <summary>
	/// Gets the area of the size.
	/// </summary>
	public readonly float? Area => Width * Height;
	/// <summary>
	/// Adds two sizes together.
	/// </summary>
	/// <param name="sz1">The first size.</param>
	/// <param name="sz2">The second size.</param>
	/// <returns>The sum of the two sizes.</returns>
	public static Size Add(Size sz1, Size sz2) => new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
	/// <summary>
	/// Subtracts one size from another.
	/// </summary>
	/// <param name="sz1">The first size.</param>
	/// <param name="sz2">The second size.</param>
	/// <returns>The difference between the two sizes.</returns>
	public static Size Subtract(Size sz1, Size sz2) => new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
	/// <inheritdoc/>
#if NETSTANDARD
	public readonly override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 23 + (Width?.GetHashCode() ?? 0);
		hash = hash * 23 + (Height?.GetHashCode() ?? 0);
		return hash;
	}
#else
	public readonly override int GetHashCode() => HashCode.Combine(Width, Height);
#endif
	/// <inheritdoc/>
	public readonly override string ToString() => $"[{Width},{Height}]";
	/// <inheritdoc/>
	public readonly bool Equals(Size other) => Width == other.Width && Height == other.Height;
	/// <summary>
	/// Converts this size to an <see cref="SizeI"/>.
	/// </summary>
	/// <returns>An <see cref="SizeI"/> that represents this size.</returns>
	public readonly SizeI ToSize() => new((int?)Width, (int?)Height);
	/// <summary>
	/// Converts this size to an <see cref="Point"/>.
	/// </summary>
	/// <returns>An <see cref="Point"/> that represents this size.</returns>
	public readonly Point ToPointF() => new(Width, Height);
	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Size e && Equals(e);
	/// <inheritdoc/>
	public static Size operator +(Size sz1, Size sz2) => Add(sz1, sz2);
	/// <inheritdoc/>
	public static Size operator -(Size sz1, Size sz2) => Subtract(sz1, sz2);
	/// <inheritdoc/>
	public static Size operator *(float left, Size right) => new(left * right.Width, left * right.Height);
	/// <inheritdoc/>
	public static Size operator *(Size left, float? right) => new(left.Width * right, left.Height * right);
	/// <inheritdoc/>
	public static Size operator /(Size left, float? right) => new(left.Width / right, left.Height / right);
	/// <inheritdoc/>
	public static bool operator ==(Size sz1, Size sz2) => sz1.Equals(sz2);
	/// <inheritdoc/>
	public static bool operator !=(Size sz1, Size sz2) => !sz1.Equals(sz2);

	/// <summary>
	/// Performs an explicit conversion from <see cref="Size"/> to <see cref="Point"/>.
	/// </summary>
	/// <param name="size">The size to convert.</param>
	/// <returns>An <see cref="Point"/> that represents the converted size.</returns>
	public static explicit operator Point(Size size) => new(size.Width, size.Height);
	private readonly string GetDebuggerDisplay() => ToString();
	/// <summary>
	/// Implicitly converts a tuple containing two <see cref="float"/>?.
	/// </summary>
	/// <remarks>This conversion enables concise and readable initialization of <see cref="Size"/> instances from tuples,
	/// allowing for more flexible assignment and construction patterns.</remarks>
	/// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="float"/>?. Either element may be null.</param>
	public static implicit operator Size((float? width, float? height) tuple) => new(tuple.width, tuple.height);
	/// <summary>
	/// Deconstructs the current instance into its Width and Height component values.
	/// </summary>
	/// <param name="width">When this method returns, contains the value of the Width component of the current instance.</param>
	/// <param name="height">When this method returns, contains the value of the Height component of the current instance.</param>
	public readonly void Deconstruct(out float? width, out float? height) { width = Width; height = Height; }
}
