using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

/// <summary>
/// A point whose horizontal and vertical coordinates are <strong>nullable</strong> <see langword="integer" />
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct PointI(int? x, int? y) : IVector<PointI, SizeI, int?>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PointI"/> struct with the specified size.
	/// </summary>
	/// <param name="sz">The size to initialize the point with.</param>
	public PointI(SizeI sz) : this(sz.Width, sz.Height) { }
	/// <summary>
	/// Initializes a new instance of the <see cref="PointI"/> struct with the specified nullable size.
	/// </summary>
	/// <param name="sz">The nullable size to initialize the point with.</param>
	public PointI(SizeN sz) : this(
	   (int)Math.Round((double)sz.Width),
	   (int)Math.Round((double)sz.Height))
	{ }
	/// <summary>
	/// Gets a value indicating whether this point is empty.
	/// </summary>
	public readonly bool IsEmpty => X == null && Y == null;
	/// <summary>
	/// Gets or sets the X coordinate of this point.
	/// </summary>
	public int? X { get; set; } = x;
	/// <summary>
	/// Gets or sets the Y coordinate of this point.
	/// </summary>
	public int? Y { get; set; } = y;
	/// <summary>
	/// Offsets this point by the specified point.
	/// </summary>
	/// <param name="p">The point to offset by.</param>
	public void Offset(PointI p)
	{
		X += p.X;
		Y += p.Y;
	}
	/// <summary>
	/// Offsets this point by the specified amounts.
	/// </summary>
	/// <param name="dx">The amount to offset the X coordinate.</param>
	/// <param name="dy">The amount to offset the Y coordinate.</param>
	public void Offset(int? dx, int? dy)
	{
		X += dx;
		Y += dy;
	}
	/// <summary>
	/// Returns a new point that is the ceiling of the specified point.
	/// </summary>
	/// <param name="value">The point to ceiling.</param>
	/// <returns>A new point that is the ceiling of the specified point.</returns>
	public static PointI Ceiling(Point value) => new(
	   value.X == null ? null : (int)Math.Ceiling((double)value.X),
	   value.Y == null ? null : (int)Math.Ceiling((double)value.Y)
	   );
	/// <summary>
	/// Adds the specified size to the specified point.
	/// </summary>
	/// <param name="pt">The point to add to.</param>
	/// <param name="sz">The size to add.</param>
	/// <returns>A new point that is the sum of the specified point and size.</returns>
	public static PointI Add(PointI pt, SizeI sz) => new(
	   pt.X + sz.Width, pt.Y + sz.Height
	   );
	/// <summary>
	/// Returns a new point that is the truncated value of the specified point.
	/// </summary>
	/// <param name="value">The point to truncate.</param>
	/// <returns>A new point that is the truncated value of the specified point.</returns>
	public static PointI Truncate(Point value) => new(
	   value.X == null ? null : (int)Math.Truncate((double)value.X),
	   value.Y == null ? null : (int)Math.Truncate((double)value.Y)
	   );
	/// <summary>
	/// Subtracts the specified size from the specified point.
	/// </summary>
	/// <param name="pt">The point to subtract from.</param>
	/// <param name="sz">The size to subtract.</param>
	/// <returns>A new point that is the difference of the specified point and size.</returns>
	public static PointI Subtract(PointI pt, SizeI sz) => new(
	   pt.X - sz.Width, pt.Y - sz.Height
	   );
	/// <summary>
	/// Returns a new point that is the rounded value of the specified point.
	/// </summary>
	/// <param name="value">The point to round.</param>
	/// <returns>A new point that is the rounded value of the specified point.</returns>
	public static PointI Round(Point value) => new(
	   value.X == null ? null : (int)Math.Round((double)value.X.Value),
	   value.Y == null ? null : (int)Math.Round((double)value.Y.Value)
	   );
	/// <summary>
	/// Multiplies this point by the specified matrix.
	/// </summary>
	/// <param name="matrix">The matrix to multiply by.</param>
	/// <returns>A new point that is the result of the multiplication.</returns>
	/// <exception cref="Exception">Thrown when the matrix is not a 2x2 matrix.</exception>
	public readonly Point MultipyByMatrix(float[,] matrix)
	{
		if (matrix.Rank == 2 && matrix.Length == 4)
		{
			int? num = X;
			float? num2 = (num == null ? null : num) * matrix[0, 0];
			num = Y;
			float? x = num2 + (num == null ? null : num) * matrix[1, 0];
			num = X;
			float? num3 = (num == null ? null : num) * matrix[0, 1];
			num = Y;
			Point MultipyByMatrix = new(x, num3 + (num != null ? num : null) * matrix[1, 1]);
			return MultipyByMatrix;
		}
		throw new Exception("Matrix not match, 2*2 matrix expected.");
	}
	/// <summary>
	/// Rotates this point by the specified angle.
	/// </summary>
	/// <param name="angle">The angle to rotate by.</param>
	/// <returns>A new point that is the result of the rotation.</returns>
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
	public readonly Point Rotate(PointN pivot, float angle) => ((Point)this - new SizeN(pivot)).Rotate(angle) + new SizeN(pivot);
	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is PointI e && Equals(e);
	/// <inheritdoc/>
#if NETSTANDARD
	public readonly override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + (X?.GetHashCode() ?? 0);
		hash = hash * 31 + (Y?.GetHashCode() ?? 0);
		return hash;
	}
#else
	public readonly override int GetHashCode() => HashCode.Combine(X, Y);
#endif
	/// <inheritdoc/>
	public readonly override string ToString() => $"[{(X?.ToString()) ?? "null"},{(Y?.ToString()) ?? "null"}]";
	/// <inheritdoc/>
	public readonly bool Equals(PointI other) => other.X == X && other.Y == Y;
	/// <inheritdoc/>
	public static PointI operator +(PointI pt, SizeI sz) => Add(pt, sz);
	/// <inheritdoc/>
	public static PointI operator -(PointI pt, SizeI sz) => Subtract(pt, sz);
	/// <inheritdoc/>
	public static PointI operator *(PointI pt, int? x) => new(pt.X * x, pt.Y * x);
	/// <inheritdoc/>
	public static PointI operator /(PointI pt, int? x) => new(pt.X / x, pt.Y / x);
	/// <inheritdoc/>
	public static bool operator ==(PointI left, PointI right) => left.Equals(right);
	/// <inheritdoc/>
	public static bool operator !=(PointI left, PointI right) => !left.Equals(right);
	/// <summary>
	/// Implicitly converts an <see cref="PointI"/> to an <see cref="Point"/>.
	/// </summary>
	/// <param name="p">The <see cref="PointI"/> to convert.</param>
	/// <returns>A new <see cref="Point"/> with the same coordinates as the input <see cref="PointI"/>.</returns>
	public static implicit operator Point(PointI p) => new(p.X, p.Y);
	/// <summary>
	/// Explicitly converts an <see cref="PointI"/> to an <see cref="SizeI"/>.
	/// </summary>
	/// <param name="p">The <see cref="PointI"/> to convert.</param>
	/// <returns>A new <see cref="SizeI"/> with the same dimensions as the input <see cref="PointI"/>.</returns>
	public static explicit operator SizeI(PointI p) => new(p);
	private readonly string GetDebuggerDisplay() => ToString();
	/// <summary>
	/// Implicitly converts a tuple containing two <see cref="int"/>?.
	/// </summary>
	/// <remarks>This conversion enables concise and readable initialization of <see cref="PointI"/> instances from tuples,
	/// allowing for more flexible assignment and construction patterns.</remarks>
	/// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="int"/>?. Either element may be null.</param>
	public static implicit operator PointI((int? x, int? y) tuple) => new(tuple.x, tuple.y);
	/// <summary>
	/// Deconstructs the current instance into its X and Y component values.
	/// </summary>
	/// <param name="x">When this method returns, contains the value of the X component of the current instance.</param>
	/// <param name="y">When this method returns, contains the value of the Y component of the current instance.</param>
	public readonly void Deconstruct(out int? x, out int? y) { x = X; y = Y; }
}
