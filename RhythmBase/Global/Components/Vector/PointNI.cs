using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

/// <summary>
/// A point whose horizontal and vertical coordinates are <strong>non-nullable</strong> <see langword="integer" />
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct PointNI(int x, int y) : IVector<PointNI, SizeNI, int>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PointNI"/> struct with the specified size.
	/// </summary>
	/// <param name="sz">The size to initialize the point with.</param>
	public PointNI(SizeNI sz) : this(sz.Width, sz.Height) { }
	/// <summary>
	/// Gets or sets the X coordinate of the point.
	/// </summary>
	public int X { get; set; } = x;
	/// <summary>
	/// Gets or sets the Y coordinate of the point.
	/// </summary>
	public int Y { get; set; } = y;
	/// <summary>
	/// Offsets the point by the specified point.
	/// </summary>
	/// <param name="p">The point to offset by.</param>
	public void Offset(PointNI p)
	{
		X += p.X;
		Y += p.Y;
	}
	/// <summary>
	/// Offsets the point by the specified size.
	/// </summary>
	/// <param name="p">The size to offset by.</param>
	public void Offset(SizeNI p)
	{
		X += p.Width;
		Y += p.Height;
	}
	/// <summary>
	/// Offsets the point by the specified horizontal and vertical amounts.
	/// </summary>
	/// <param name="dx">The horizontal offset.</param>
	/// <param name="dy">The vertical offset.</param>
	public void Offset(int dx, int dy)
	{
		X += dx;
		Y += dy;
	}
	/// <summary>
	/// Returns a new point with coordinates rounded up to the nearest integer values.
	/// </summary>
	/// <param name="value">The point to round up.</param>
	/// <returns>A new point with coordinates rounded up.</returns>
	public static PointNI Ceiling(PointN value) => new(
	(int)Math.Ceiling((double)value.X),
	(int)Math.Ceiling((double)value.Y)
	);
	/// <summary>
	/// Adds the specified size to the point.
	/// </summary>
	/// <param name="pt">The point to add to.</param>
	/// <param name="sz">The size to add.</param>
	/// <returns>A new point with the size added.</returns>
	public static PointNI Add(PointNI pt, SizeNI sz) => new(
	pt.X + sz.Width, pt.Y + sz.Height
	);
	/// <summary>
	/// Returns a new point with coordinates truncated to the nearest integer values.
	/// </summary>
	/// <param name="value">The point to truncate.</param>
	/// <returns>A new point with coordinates truncated.</returns>
	public static PointNI Truncate(PointN value) => new(
	(int)Math.Truncate((double)value.X),
	(int)Math.Truncate((double)value.Y)
	);
	/// <summary>
	/// Subtracts the specified size from the point.
	/// </summary>
	/// <param name="pt">The point to subtract from.</param>
	/// <param name="sz">The size to subtract.</param>
	/// <returns>A new point with the size subtracted.</returns>
	public static PointNI Subtract(PointNI pt, SizeNI sz) => new(
	pt.X - sz.Width, pt.Y - sz.Height
	);
	/// <summary>
	/// Returns a new point with coordinates rounded to the nearest integer values.
	/// </summary>
	/// <param name="value">The point to round.</param>
	/// <returns>A new point with coordinates rounded.</returns>
	public static PointNI Round(PointN value) => new(
	(int)Math.Round((double)value.X),
	(int)Math.Round((double)value.Y)
	);
	/// <summary>
	/// Multiplies the point by the specified 2x2 matrix.
	/// </summary>
	/// <param name="matrix">The 2x2 matrix to multiply by.</param>
	/// <returns>A new point resulting from the matrix multiplication.</returns>
	/// <exception cref="Exception">Thrown when the matrix is not a 2x2 matrix.</exception>
	public readonly PointN MultipyByMatrix(float[,] matrix)
	{
		if (matrix.Rank == 2 && matrix.Length == 4)
		{
			PointN MultipyByMatrix = new(X * matrix[0, 0] + Y * matrix[1, 0], X * matrix[0, 1] + Y * matrix[1, 1]);
			return MultipyByMatrix;
		}
		throw new Exception("Matrix not match, 2*2 matrix expected.");
	}
	/// <summary>
	/// Rotate.
	/// </summary>
	public readonly PointN Rotate(float angle)
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
	public readonly PointN Rotate(PointN pivot, float angle) => ((PointN)this - new SizeN(pivot)).Rotate(angle) + new SizeN(pivot);
	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is PointNI e && Equals(e);
	/// <inheritdoc/>
	public readonly override int GetHashCode()
	{
#if NETSTANDARD
		int hash = 17;
		hash = hash * 31 + X.GetHashCode();
		hash = hash * 31 + Y.GetHashCode();
		return hash;
#else
		HashCode h = default;
		h.Add(X);
		h.Add(Y);
		return h.ToHashCode();
#endif
	}
	/// <inheritdoc/>
	public readonly override string ToString() => $"[{X}, {Y}]";
	/// <inheritdoc/>
	public readonly bool Equals(PointNI other) => other.X == X && other.Y == Y;
	/// <inheritdoc/>
	public static PointNI operator +(PointNI pt, SizeNI sz) => Add(pt, sz);
	/// <inheritdoc/>
	public static PointNI operator -(PointNI pt, SizeNI sz) => Subtract(pt, sz);
	/// <inheritdoc/>
	public static PointNI operator *(PointNI pt, int x) => new(pt.X * x, pt.Y * x);
	/// <inheritdoc/>
	public static PointNI operator /(PointNI pt, int x) => new(pt.X / x, pt.Y / x);
	/// <inheritdoc/>
	public static bool operator ==(PointNI left, PointNI right) => left.Equals(right);
	/// <inheritdoc/>
	public static bool operator !=(PointNI left, PointNI right) => !left.Equals(right);
	/// <summary>
	/// Implicitly converts an <see cref="PointNI"/> to an <see cref="PointN"/>.
	/// </summary>
	/// <param name="p">The <see cref="PointNI"/> to convert.</param>
	/// <returns>An <see cref="PointN"/> with the same coordinates.</returns>
	public static implicit operator PointN(PointNI p) => new(p.X, p.Y);
	/// <summary>
	/// Implicitly converts an <see cref="PointNI"/> to an <see cref="PointI"/>.
	/// </summary>
	/// <param name="p">The <see cref="PointNI"/> to convert.</param>
	/// <returns>An <see cref="PointI"/> with the same coordinates.</returns>
	public static implicit operator PointI(PointNI p) => new(new int?(p.X), new int?(p.Y));
	/// <summary>
	/// Explicitly converts an <see cref="PointNI"/> to an <see cref="SizeNI"/>.
	/// </summary>
	/// <param name="p">The <see cref="PointNI"/> to convert.</param>
	/// <returns>An <see cref="SizeNI"/> with the same dimensions.</returns>
	public static explicit operator SizeNI(PointNI p) => new(p.X, p.Y);
	private readonly string GetDebuggerDisplay() => ToString();
	/// <summary>
	/// Implicitly converts a tuple containing two <see cref="int"/>.
	/// </summary>
	/// <remarks>This conversion enables concise and readable initialization of <see cref="PointNI"/> instances from tuples,
	/// allowing for more flexible assignment and construction patterns.</remarks>
	/// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="int"/>.</param>
	public static implicit operator PointNI((int x, int y) tuple) => new(tuple.x, tuple.y);
	/// <summary>
	/// Deconstructs the current instance into its X and Y component values.
	/// </summary>
	/// <param name="x">When this method returns, contains the value of the X component of the current instance.</param>
	/// <param name="y">When this method returns, contains the value of the Y component of the current instance.</param>
	public readonly void Deconstruct(out int x, out int y) { x = X; y = Y; }
}
