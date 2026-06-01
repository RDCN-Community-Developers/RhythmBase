using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

/// <summary>
/// A point whose horizontal and vertical coordinates are <strong>nullable</strong> <see langword="float" />
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct Point(float? x, float? y) : IVector<Point, Size, float?>, IVector<Point, SizeI, float?>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Point"/> struct with the specified size.
	/// </summary>
	/// <param name="sz">The size to initialize the point with.</param>
	public Point(Size sz) : this(sz.Width, sz.Height) { }
	/// <summary>
	/// Gets a value indicating whether this point is empty.
	/// </summary>
	public readonly bool IsEmpty => X == null && Y == null;
	/// <summary>
	/// Gets or sets the X coordinate of the point.
	/// </summary>
	public float? X { get; set; } = x;
	/// <summary>
	/// Gets or sets the Y coordinate of the point.
	/// </summary>
	public float? Y { get; set; } = y;
	/// <summary>
	/// Offsets the point by the specified point.
	/// </summary>
	/// <param name="p">The point to offset by.</param>
	public void Offset(Point p)
	{
		X += p.X;
		Y += p.Y;
	}
	/// <summary>
	/// Offsets the point by the specified amounts.
	/// </summary>
	/// <param name="dx">The amount to offset the X coordinate.</param>
	/// <param name="dy">The amount to offset the Y coordinate.</param>
	public void Offset(float? dx, float? dy)
	{
		X += dx;
		Y += dy;
	}
	/// <summary>
	/// Adds the specified size to the point.
	/// </summary>
	/// <param name="pt">The point to add to.</param>
	/// <param name="sz">The size to add.</param>
	/// <returns>A new point that is the result of the addition.</returns>
	public static Point Add(Point pt, SizeI sz) => new(
	pt.X + sz.Width, pt.Y + sz.Height
	);
	/// <summary>
	/// Adds the specified size to the point.
	/// </summary>
	/// <param name="pt">The point to add to.</param>
	/// <param name="sz">The size to add.</param>
	/// <returns>A new point that is the result of the addition.</returns>
	public static Point Add(Point pt, Size sz) => new(
	pt.X + sz.Width, pt.Y + sz.Height
	);
	/// <summary>
	/// Subtracts the specified size from the point.
	/// </summary>
	/// <param name="pt">The point to subtract from.</param>
	/// <param name="sz">The size to subtract.</param>
	/// <returns>A new point that is the result of the subtraction.</returns>
	public static Point Subtract(Point pt, SizeI sz) => new(
	pt.X - sz.Width, pt.Y - sz.Height
	);
	/// <summary>
	/// Subtracts the specified size from the point.
	/// </summary>
	/// <param name="pt">The point to subtract from.</param>
	/// <param name="sz">The size to subtract.</param>
	/// <returns>A new point that is the result of the subtraction.</returns>
	public static Point Subtract(Point pt, Size sz) => new(
	pt.X - sz.Width, pt.Y - sz.Height
	);
	/// <summary>
	/// Multiplies the point by the specified matrix.
	/// </summary>
	/// <param name="matrix">The matrix to multiply by.</param>
	/// <returns>A new point that is the result of the multiplication.</returns>
	/// <exception cref="Exception">Thrown when the matrix is not a 2x2 matrix.</exception>
	public readonly Point MultipyByMatrix(float[,] matrix)
	{
		if (matrix.Rank == 2 && matrix.Length == 4)
		{
			Point MultipyByMatrix = new(X * matrix[0, 0] + Y * matrix[1, 0], X * matrix[0, 1] + Y * matrix[1, 1]);
			return MultipyByMatrix;
		}
		throw new Exception("Matrix not match, 2*2 matrix expected.");
	}
	/// <summary>
	/// Rotate.
	/// </summary>
	public readonly Point Rotate(float angle)
	{
		float[,] array = new float[2, 2];
		array[0, 0] = (float)Math.Cos((double)angle);
		array[0, 1] = (float)Math.Sin((double)angle);
		array[1, 0] = -(float)Math.Sin((double)angle);
		array[1, 1] = (float)Math.Cos((double)angle);
		return MultipyByMatrix(array);
	}
	/// <summary>
	/// Rotate at a given pivot.
	/// </summary>
	/// <param name="pivot">Given pivot.</param>
	/// <param name="angle">Angle.</param>
	/// <returns></returns>
	public readonly Point Rotate(PointN pivot, float angle) => (this - new SizeN(pivot)).Rotate(angle) + new SizeN(pivot);
	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Point e && Equals(e);
	/// <inheritdoc/>
	public readonly override int GetHashCode()
	{
#if NETSTANDARD
		int hash = 17;
		hash = hash * 31 + (X?.GetHashCode() ?? 0);
		hash = hash * 31 + (Y?.GetHashCode() ?? 0);
		return hash;
#else
		HashCode h = default;
		h.Add(X);
		h.Add(Y);
		return h.ToHashCode();
#endif
	}
	/// <inheritdoc/>
	public readonly override string ToString() => $"[{X?.ToString() ?? "null"}, {Y?.ToString() ?? "null"}]";
	/// <inheritdoc/>
	public readonly bool Equals(Point other) => other.X.NullableEquals(X) && other.Y.NullableEquals(Y);
	/// <inheritdoc/>
	public static Point operator +(Point pt, SizeI sz) => Add(pt, sz);
	/// <inheritdoc/>
	public static Point operator +(Point pt, Size sz) => Add(pt, sz);
	/// <inheritdoc/>
	public static Point operator -(Point pt, SizeI sz) => Subtract(pt, sz);
	/// <inheritdoc/>
	public static Point operator -(Point pt, Size sz) => Subtract(pt, sz);
	/// <inheritdoc/>
	public static Point operator *(Point pt, float? x) => new(pt.X * x, pt.Y * x);
	/// <inheritdoc/>
	public static Point operator /(Point pt, float? x) => new(pt.X / x, pt.Y / x);
	/// <inheritdoc/>
	public static bool operator ==(Point left, Point right) => left.Equals(right);
	/// <inheritdoc/>
	public static bool operator !=(Point left, Point right) => !left.Equals(right);
	/// <summary>
	/// Explicitly converts an <see cref="Point"/> to an <see cref="Size"/>.
	/// </summary>
	/// <param name="p">The <see cref="Point"/> to convert.</param>
	/// <returns>A new <see cref="Size"/> with the same coordinates as the input <see cref="Point"/>.</returns>
	public static explicit operator Size(Point p) => new(p.X, p.Y);
	private readonly string GetDebuggerDisplay() => ToString();
	/// <summary>
	/// Implicitly converts a tuple containing two <see cref="float"/>?.
	/// </summary>
	/// <remarks>This conversion enables concise and readable initialization of <see cref="Point"/> instances from tuples,
	/// allowing for more flexible assignment and construction patterns.</remarks>
	/// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="float"/>?. Either element may be null.</param>
	public static implicit operator Point((float? x, float? y) tuple) => new(tuple.x, tuple.y);
	/// <summary>
	/// Deconstructs the current instance into its X and Y component values.
	/// </summary>
	/// <param name="x">When this method returns, contains the value of the X component of the current instance.</param>
	/// <param name="y">When this method returns, contains the value of the Y component of the current instance.</param>
	public readonly void Deconstruct(out float? x, out float? y) { x = X; y = Y; }
}
