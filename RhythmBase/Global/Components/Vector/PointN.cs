using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

/// <summary>
/// A point whose horizontal and vertical coordinates are <strong>non-nullable</strong> <see langword="float" />
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct PointN(float x, float y) : IVector<PointN, SizeN, float>, IVector<PointN, SizeNI, float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PointN"/> struct with the specified size.
	/// </summary>
	/// <param name="sz">The size to initialize the point with.</param>
	public PointN(SizeN sz) : this(sz.Width, sz.Height) { }
	/// <summary>
	/// Gets or sets the X coordinate of the point.
	/// </summary>
	public float X { get; set; } = x;
	/// <summary>
	/// Gets or sets the Y coordinate of the point.
	/// </summary>
	public float Y { get; set; } = y;
	/// <summary>
	/// Offsets the point by the specified size.
	/// </summary>
	/// <param name="p">The size to offset the point by.</param>
	public void Offset(SizeN p)
	{
		X += p.Width;
		Y += p.Height;
	}
	/// <summary>
	/// Offsets the point by the specified point.
	/// </summary>
	/// <param name="p">The point to offset the point by.</param>
	public void Offset(PointN p)
	{
		X += p.X;
		Y += p.Y;
	}
	/// <summary>
	/// Offsets the point by the specified horizontal and vertical amounts.
	/// </summary>
	/// <param name="dx">The horizontal amount to offset the point by.</param>
	/// <param name="dy">The vertical amount to offset the point by.</param>
	public void Offset(float dx, float dy)
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
	public static PointN Add(PointN pt, SizeNI sz)
	{
		PointN Add = new(pt.X + sz.Width, pt.Y + sz.Height);
		return Add;
	}
	/// <summary>
	/// Adds the specified size to the point.
	/// </summary>
	/// <param name="pt">The point to add to.</param>
	/// <param name="sz">The size to add.</param>
	/// <returns>A new point that is the result of the addition.</returns>
	public static PointN Add(PointN pt, SizeN sz)
	{
		PointN Add = new(pt.X + sz.Width, pt.Y + sz.Height);
		return Add;
	}
	/// <summary>
	/// Subtracts the specified size from the point.
	/// </summary>
	/// <param name="pt">The point to subtract from.</param>
	/// <param name="sz">The size to subtract.</param>
	/// <returns>A new point that is the result of the subtraction.</returns>
	public static PointN Subtract(PointN pt, SizeNI sz)
	{
		PointN Subtract = new(pt.X - sz.Width, pt.Y - sz.Height);
		return Subtract;
	}
	/// <summary>
	/// Subtracts the specified size from the point.
	/// </summary>
	/// <param name="pt">The point to subtract from.</param>
	/// <param name="sz">The size to subtract.</param>
	/// <returns>A new point that is the result of the subtraction.</returns>
	public static PointN Subtract(PointN pt, SizeN sz)
	{
		PointN Subtract = new(pt.X - sz.Width, pt.Y - sz.Height);
		return Subtract;
	}
	/// <summary>
	/// Multiplies the point by the specified 2x2 matrix.
	/// </summary>
	/// <param name="matrix">The 2x2 matrix to multiply the point by.</param>
	/// <returns>A new point that is the result of the multiplication.</returns>
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
	public readonly PointN Rotate(PointN pivot, float angle) => (this - new SizeN(pivot)).Rotate(angle) + new SizeN(pivot);
	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is PointN e && Equals(e);
	/// <inheritdoc/>
	public readonly override int GetHashCode()
	{
#if NETSTANDARD
		int hash = 17;
		hash = hash * 23 + X.GetHashCode();
		hash = hash * 23 + Y.GetHashCode();
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
	public readonly bool Equals(PointN other) => other.X == X && other.Y == Y;
	/// <inheritdoc/>
	public static PointN operator +(PointN pt, SizeNI sz) => Add(pt, sz);
	/// <inheritdoc/>
	public static PointN operator +(PointN pt, SizeN sz) => Add(pt, sz);
	/// <inheritdoc/>
	public static PointN operator -(PointN pt, SizeNI sz) => Subtract(pt, sz);
	/// <inheritdoc/>
	public static PointN operator -(PointN pt, SizeN sz) => Subtract(pt, sz);
	/// <inheritdoc/>
	public static PointN operator *(PointN pt, float x) => new(pt.X * x, pt.Y * x);
	/// <inheritdoc/>
	public static PointN operator /(PointN pt, float x) => new(pt.X / x, pt.Y / x);
	/// <inheritdoc/>
	public static bool operator ==(PointN left, PointN right) => left.Equals(right);
	/// <inheritdoc/>
	public static bool operator !=(PointN left, PointN right) => !left.Equals(right);
	/// <summary>
	/// Implicitly converts an <see cref="PointN"/> to an <see cref="Point"/>.
	/// </summary>
	/// <param name="p">The <see cref="PointN"/> to convert.</param>
	/// <returns>A new <see cref="Point"/> with the same coordinates.</returns>
	public static implicit operator Point(PointN p) => new(new float?(p.X), new float?(p.Y));
	/// <summary>
	/// Explicitly converts an <see cref="PointN"/> to an <see cref="SizeN"/>.
	/// </summary>
	/// <param name="p">The <see cref="PointN"/> to convert.</param>
	/// <returns>A new <see cref="SizeN"/> with the same dimensions.</returns>
	public static explicit operator SizeN(PointN p) => new(p.X, p.Y);
	private readonly string GetDebuggerDisplay() => ToString();
	/// <summary>
	/// Implicitly converts a tuple containing two <see cref="float"/>.
	/// </summary>
	/// <remarks>This conversion enables concise and readable initialization of <see cref="PointN"/> instances from tuples,
	/// allowing for more flexible assignment and construction patterns.</remarks>
	/// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="float"/>.</param>
	public static implicit operator PointN((float x, float y) tuple) => new(tuple.x, tuple.y);
	/// <summary>
	/// Deconstructs the current instance into its X and Y component values.
	/// </summary>
	/// <param name="x">When this method returns, contains the value of the X component of the current instance.</param>
	/// <param name="y">When this method returns, contains the value of the Y component of the current instance.</param>
	public readonly void Deconstruct(out float x, out float y) { x = X; y = Y; }
}
