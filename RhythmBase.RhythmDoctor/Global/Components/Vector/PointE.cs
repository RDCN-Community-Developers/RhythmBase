using RhythmBase.RhythmDoctor.Components;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

/// <summary>
/// A point whose horizontal and vertical coordinates are <strong>nullable</strong> <seealso cref="T:RhythmBase.Components.Expression" />
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct PointE(Expression? x, Expression? y) :
  IVector<PointE, Size, Expression>,
  IVector<PointE, SizeI, Expression>,
  IVector<PointE, SizeE, Expression>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified size.
    /// </summary>
    /// <param name="sz">The size to initialize the point with.</param>
    public PointE(Size sz) : this(sz.Width, sz.Height) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public PointE(float x, float y) : this((Expression)x, (Expression)y) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified x-coordinate and y-coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public PointE(Expression? x, float y) : this(x, (Expression)y) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified x-coordinate and y-coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public PointE(float x, Expression? y) : this((Expression)x, y) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified x-coordinate and y-coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public PointE([AllowNull] string x, float y) : this(x ?? default(Expression?), (Expression)y) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified x-coordinate and y-coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public PointE(float x, [AllowNull] string y) : this((Expression)x, y ?? default(Expression?)) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified x-coordinate and y-coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public PointE([AllowNull] string x, Expression? y) : this(x ?? default(Expression?), y) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified x-coordinate and y-coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public PointE(Expression? x, [AllowNull] string y) : this(x, y ?? default(Expression?)) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified x-coordinate and y-coordinate.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public PointE([AllowNull] string x, [AllowNull] string y) : this(x ?? default(Expression?), y ?? default(Expression?)) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified point.
    /// </summary>
    /// <param name="p">The point to initialize the point with.</param>
    public PointE(PointI p) : this(p.X, p.Y) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointE"/> struct with the specified point.
    /// </summary>
    /// <param name="p">The point to initialize the point with.</param>
    public PointE(Point p) : this(p.X, p.Y) { }
    /// <inheritdoc/>
    public readonly bool IsEmpty => X == null && Y == null;
    /// <inheritdoc/>
    public Expression? X { get; set; } = x;
    /// <inheritdoc/>
    public Expression? Y { get; set; } = y;
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
    /// <param name="dx">The amount to offset the x-coordinate.</param>
    /// <param name="dy">The amount to offset the y-coordinate.</param>
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
    /// <returns>The resulting point.</returns>
    public static PointE Add(PointE pt, SizeI sz) => new(
    pt.X + sz.Width, pt.Y + sz.Height
    );
    /// <inheritdoc cref="Add(PointE, SizeI)"/>
    public static PointE Add(PointE pt, Size sz) => new(
    pt.X + sz.Width, pt.Y + sz.Height
    );
    /// <inheritdoc cref="Add(PointE, SizeI)"/>
    public static PointE Add(PointE pt, SizeE sz) => new(
    pt.X + sz.Width, pt.Y + sz.Height
    );
    /// <summary>
    /// Subtracts the specified size from the point.
    /// </summary>
    /// <param name="pt">The point to subtract from.</param>
    /// <param name="sz">The size to subtract.</param>
    /// <returns>The resulting point.</returns>
    public static PointE Subtract(PointE pt, SizeI sz) => new(
    pt.X - sz.Width, pt.Y - sz.Height
    );
    /// <inheritdoc cref="Subtract(PointE, SizeI)"/>
    public static PointE Subtract(PointE pt, Size sz) => new(
    pt.X - sz.Width, pt.Y - sz.Height
    );
    /// <inheritdoc cref="Subtract(PointE, SizeI)"/>
    public static PointE Subtract(PointE pt, SizeE sz) => new(
    pt.X - sz.Width, pt.Y - sz.Height
    );
    /// <summary>
    /// Implicitly converts an <see cref="PointI"/> to an <see cref="PointE"/>.
    /// </summary>
    /// <param name="p">The <see cref="PointI"/> to convert.</param>
    /// <returns>A new <see cref="PointE"/> with the same coordinates as the input <see cref="PointI"/>.</returns>
    public static implicit operator PointE(PointI p) => new(p.X, p.Y);
    /// <summary>
    /// Implicitly converts an <see cref="PointN"/> to an <see cref="PointE"/>.
    /// </summary>
    /// <param name="p">The <see cref="PointN"/> to convert.</param>
    /// <returns>A new <see cref="PointE"/> with the same coordinates.</returns>
    public static implicit operator PointE(PointN p) => new(p.X, p.Y);
    /// <summary>
    /// Implicitly converts an <see cref="PointNI"/> to an <see cref="PointE"/>.
    /// </summary>
    /// <param name="p">The <see cref="PointNI"/> to convert.</param>
    /// <returns>An <see cref="PointE"/> with the same coordinates.</returns>
    public static implicit operator PointE(PointNI p) => new(p.X, p.Y);
    /// <inheritdoc/>
    public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is PointE e && Equals(e);
    /// <inheritdoc/>
#if NETCOREAPP2_1_OR_GREATER
    public readonly override int GetHashCode() => HashCode.Combine(X, Y);
#else
	public readonly override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + (X?.GetHashCode() ?? 0);
		hash = hash * 31 + (Y?.GetHashCode() ?? 0);
		return hash;
	}
#endif
    /// <inheritdoc/>
    public readonly override string ToString() => $"[{(X?.ExpressionValue) ?? "null"},{(Y?.ExpressionValue) ?? "null"}]";
    /// <inheritdoc/>
    public readonly bool Equals(PointE other) => other.X == X && other.Y == Y;
    /// <summary>
    /// Multiplies the point by the specified matrix.
    /// </summary>
    /// <param name="matrix">The matrix to multiply by.</param>
    /// <returns>The resulting point.</returns>
    /// <exception cref="Exception">Thrown when the matrix is not 2x2.</exception>
    public readonly PointE MultipyByMatrix(Expression[,] matrix)
    {
        if (matrix.Rank == 2 && matrix.Length == 4)
        {
            PointE MultipyByMatrix = new(
              X * matrix[0, 0] + Y * matrix[1, 0],
              X * matrix[0, 1] + Y * matrix[1, 1]);
            return MultipyByMatrix;
        }
        throw new Exception("Matrix not match, 2*2 matrix expected.");
    }
    /// <summary>
    /// Rotate.
    /// </summary>
    public readonly PointE Rotate(float angle)
    {
        Expression[,] array = new Expression[2, 2];
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
    public readonly PointE Rotate(PointE pivot, float angle) => (this - new SizeE(pivot)).Rotate(angle) + new SizeE(pivot);
    /// <inheritdoc/>
    public static PointE operator +(PointE pt, SizeI sz) => Add(pt, sz);
    /// <inheritdoc/>
    public static PointE operator +(PointE pt, Size sz) => Add(pt, sz);
    /// <inheritdoc/>
    public static PointE operator +(PointE pt, SizeE sz) => Add(pt, sz);
    /// <inheritdoc/>
    public static PointE operator -(PointE pt, SizeI sz) => Subtract(pt, sz);
    /// <inheritdoc/>
    public static PointE operator -(PointE pt, Size sz) => Subtract(pt, sz);
    /// <inheritdoc/>
    public static PointE operator -(PointE pt, SizeE sz) => Subtract(pt, sz);
    /// <inheritdoc/>
    public static PointE operator *(PointE pt, Expression x) => new(pt.X * x, pt.Y * x);
    /// <inheritdoc/>
    public static PointE operator /(PointE pt, Expression x) => new(pt.X / x, pt.Y / x);
    /// <inheritdoc/>
    public static bool operator ==(PointE left, PointE right) => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(PointE left, PointE right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts an <see cref="Point"/> to an <see cref="PointE"/>.
    /// </summary>
    /// <param name="p">The <see cref="Point"/> to convert.</param>
    /// <returns>A new <see cref="PointE"/> with the same coordinates as the input <see cref="Point"/>.</returns>
    public static implicit operator PointE(Point p) => new(p.X, p.Y);
    /// <summary>
    /// Converts the specified <see cref="PointE"/> to an <see cref="SizeE"/>.
    /// </summary>
    /// <param name="p">The point to convert.</param>
    /// <returns>An <see cref="SizeE"/> that represents the converted point.</returns>
    public static explicit operator SizeE(PointE p) => new(p);
    private readonly string GetDebuggerDisplay() => ToString();
    /// <summary>
    /// Implicitly converts a tuple containing two <see cref="Expression"/>?.
    /// </summary>
    /// <remarks>This conversion enables concise and readable initialization of <see cref="PointE"/> instances from tuples,
    /// allowing for more flexible assignment and construction patterns.</remarks>
    /// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="Expression"/>?. Either element may be null.</param>
    public static implicit operator PointE((Expression? x, Expression? y) tuple) => new(tuple.x, tuple.y);
    /// <summary>
    /// Deconstructs the current instance into its X and Y component values.
    /// </summary>
    /// <param name="x">When this method returns, contains the value of the X component of the current instance.</param>
    /// <param name="y">When this method returns, contains the value of the Y component of the current instance.</param>
    public readonly void Deconstruct(out Expression? x, out Expression? y) { x = X; y = Y; }
}
