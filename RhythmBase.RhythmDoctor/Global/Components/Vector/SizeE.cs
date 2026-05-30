using RhythmBase.RhythmDoctor.Components;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// A size whose horizontal and vertical coordinates are <strong>nullable</strong> <seealso cref="T:RhythmBase.Components.Expression" />
	/// </summary>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public struct SizeE(Expression? width, Expression? height) :
		IVector<SizeE, SizeI, Expression>,
		IVector<SizeE, Size, Expression>,
		IVector<SizeE, SizeE, Expression>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct with specified width and height as floats.
		/// </summary>
		/// <param name="width">The width as a float.</param>
		/// <param name="height">The height as a float.</param>
		public SizeE(float width, float height) : this((Expression)width, (Expression)height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct with specified width as an RDExpression and height as a float.
		/// </summary>
		/// <param name="width">The width as an RDExpression.</param>
		/// <param name="height">The height as a float.</param>
		public SizeE(Expression? width, float height) : this(width, (Expression)height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct with specified width as a float and height as an RDExpression.
		/// </summary>
		/// <param name="width">The width as a float.</param>
		/// <param name="height">The height as an RDExpression.</param>
		public SizeE(float width, Expression? height) : this((Expression)width, height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct with specified width as a string and height as a float.
		/// </summary>
		/// <param name="width">The width as a string.</param>
		/// <param name="height">The height as a float.</param>
		public SizeE(string width, float height) : this((Expression)width, (Expression)height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct with specified width as a float and height as a string.
		/// </summary>
		/// <param name="width">The width as a float.</param>
		/// <param name="height">The height as a string.</param>
		public SizeE(float width, string height) : this((Expression)width, (Expression)height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct with specified width and height as strings.
		/// </summary>
		/// <param name="width">The width as a string.</param>
		/// <param name="height">The height as a string.</param>
		public SizeE(string width, string height) : this((Expression)width, (Expression)height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct with specified width as a string and height as an RDExpression.
		/// </summary>
		/// <param name="width">The width as a string.</param>
		/// <param name="height">The height as an RDExpression.</param>
		public SizeE(string width, Expression? height) : this((Expression)width, height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct with specified width as an RDExpression and height as a string.
		/// </summary>
		/// <param name="width">The width as an RDExpression.</param>
		/// <param name="height">The height as a string.</param>
		public SizeE(Expression? width, string height) : this(width, (Expression)height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct from an <see cref="SizeI"/> instance.
		/// </summary>
		/// <param name="p">The <see cref="SizeI"/> instance.</param>
		public SizeE(SizeI p) : this((Expression?)p.Width, (Expression?)p.Height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct from an <see cref="Size"/> instance.
		/// </summary>
		/// <param name="p">The <see cref="Size"/> instance.</param>
		public SizeE(Size p) : this((Expression?)p.Width, (Expression?)p.Height) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct from an <see cref="PointI"/> instance.
		/// </summary>
		/// <param name="p">The <see cref="PointI"/> instance.</param>
		public SizeE(PointI p) : this((Expression?)p.X, (Expression?)p.Y) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct from an <see cref="Point"/> instance.
		/// </summary>
		/// <param name="p">The <see cref="Point"/> instance.</param>
		public SizeE(Point p) : this((Expression?)p.X, (Expression?)p.Y) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeE"/> struct from an <see cref="PointE"/> instance.
		/// </summary>
		/// <param name="p">The <see cref="PointE"/> instance.</param>
		public SizeE(PointE p) : this(p.X, p.Y) { }
		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		public readonly bool IsEmpty => Width == null && Height == null;
		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		public Expression? Width { get; set; } = width;
		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		public Expression? Height { get; set; } = height;
		/// <summary>
		/// Gets the area of the size.
		/// </summary>
		public readonly Expression? Area => Width * Height;
		/// <summary>
		/// Adds two <see cref="SizeE"/> instances and returns the result.
		/// </summary>
		/// <param name="sz1">The first <see cref="SizeE"/> instance.</param>
		/// <param name="sz2">The second <see cref="Size"/> instance.</param>
		/// <returns>The result of the addition.</returns>
		public static SizeE Add(SizeE sz1, Size sz2) => new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		/// <summary>
		/// Adds two <see cref="SizeE"/> instances and returns the result.
		/// </summary>
		/// <param name="sz1">The first <see cref="SizeE"/> instance.</param>
		/// <param name="sz2">The second <see cref="SizeE"/> instance.</param>
		/// <returns>The result of the addition.</returns>
		public static SizeE Add(SizeE sz1, SizeE sz2) => new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		/// <summary>
		/// Subtracts one <see cref="Size"/> instance from another <see cref="SizeE"/> instance and returns the result.
		/// </summary>
		/// <param name="sz1">The first <see cref="SizeE"/> instance.</param>
		/// <param name="sz2">The second <see cref="Size"/> instance.</param>
		/// <returns>The result of the subtraction.</returns>
		public static SizeE Subtract(SizeE sz1, Size sz2) => new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		/// <summary>
		/// Subtracts one <see cref="SizeE"/> instance from another <see cref="SizeE"/> instance and returns the result.
		/// </summary>
		/// <param name="sz1">The first <see cref="SizeE"/> instance.</param>
		/// <param name="sz2">The second <see cref="SizeE"/> instance.</param>
		/// <returns>The result of the subtraction.</returns>
		public static SizeE Subtract(SizeE sz1, SizeE sz2) => new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
    /// <summary>
    /// Performs an implicit conversion from <see cref="Size"/> to <see cref="SizeE"/>.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    /// <returns>An <see cref="SizeE"/> that represents the converted size.</returns>
    public static implicit operator SizeE(Size size) => new(size.Width, size.Height);
    /// <summary>
    /// Implicitly converts an <see cref="SizeI"/> to an <see cref="SizeE"/>.
    /// </summary>
    /// <param name="p">The <see cref="SizeI"/> to convert.</param>
    /// <returns>A new <see cref="SizeE"/> with the same width and height.</returns>
    public static implicit operator SizeE(SizeI p) => new(p.Width, p.Height);
    /// <summary>
    /// Implicitly converts an <see cref="SizeN"/> to an <see cref="SizeE"/>.
    /// </summary>
    /// <param name="size">The <see cref="SizeN"/> to convert.</param>
    /// <returns>A new <see cref="SizeE"/> instance.</returns>
    public static implicit operator SizeE(SizeN size) => new(size.Width, size.Height);
    /// <summary>
    /// Implicitly converts an <see cref="SizeNI"/> to an <see cref="SizeE"/>.
    /// </summary>
    /// <param name="p">The <see cref="SizeNI"/> to convert.</param>
    /// <returns>An <see cref="SizeE"/> with the same width and height as the input.</returns>
    public static implicit operator SizeE(SizeNI p) => new(p.Width, p.Height);
#if NETSTANDARD
		/// <inheritdoc/>
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 31 + (Width?.GetHashCode() ?? 0);
			hash = hash * 31 + (Height?.GetHashCode() ?? 0);
			return hash;
		}
#else
    public readonly override int GetHashCode() => HashCode.Combine(Width, Height);
#endif
		/// <inheritdoc/>
		public readonly override string ToString() => $"[{Width},{Height}]";
		/// <inheritdoc/>
		public readonly bool Equals(SizeE other) => Width == other.Width && Height == other.Height;
		/// <inheritdoc/>
		public readonly PointE ToRDPointE() => new(Width, Height);
		/// <inheritdoc/>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Size e && Equals(e);
		/// <inheritdoc/>
		public static SizeE operator +(SizeE sz1, SizeI sz2) => Add(sz1, sz2);
		/// <inheritdoc/>
		public static SizeE operator +(SizeE sz1, Size sz2) => Add(sz1, sz2);
		/// <inheritdoc/>
		public static SizeE operator +(SizeE sz1, SizeE sz2) => Add(sz1, sz2);
		/// <inheritdoc/>
		public static SizeE operator -(SizeE sz1, SizeI sz2) => Subtract(sz1, sz2);
		/// <inheritdoc/>
		public static SizeE operator -(SizeE sz1, Size sz2) => Subtract(sz1, sz2);
		/// <inheritdoc/>
		public static SizeE operator -(SizeE sz1, SizeE sz2) => Subtract(sz1, sz2);
		/// <inheritdoc/>
		public static SizeE operator *(int left, SizeE right) => new(left * right.Width, left * right.Height);
		/// <inheritdoc/>
		public static SizeE operator *(SizeE left, int right) => new(left.Width * right, left.Height * right);
		/// <inheritdoc/>
		public static SizeE operator *(float left, SizeE right) => new(left * right.Width, left * right.Height);
		/// <inheritdoc/>
		public static SizeE operator *(SizeE left, float right) => new(left.Width * right, left.Height * right);
		/// <inheritdoc/>
		public static SizeE operator *(Expression left, SizeE right) => new(left * right.Width, left * right.Height);
		/// <inheritdoc/>
		public static SizeE operator *(SizeE left, Expression right) => new(left.Width * right, left.Height * right);
		/// <inheritdoc/>
		public static SizeE operator /(SizeE left, float right) => new(left.Width / right, left.Height / right);
		/// <inheritdoc/>
		public static SizeE operator /(SizeE left, Expression right) => new(left.Width / right, left.Height / right);
		/// <inheritdoc/>
		public static bool operator ==(SizeE sz1, SizeE sz2) => sz1.Equals(sz2);
		/// <inheritdoc/>
		public static bool operator !=(SizeE sz1, SizeE sz2) => !sz1.Equals(sz2);
		/// <summary>
		/// Converts an <see cref="SizeE"/> instance to an <see cref="PointE"/> instance explicitly.
		/// </summary>
		/// <param name="size">The <see cref="SizeE"/> instance to convert.</param>
		/// <returns>An <see cref="PointE"/> instance with the same width and height as the <see cref="SizeE"/> instance.</returns>
		public static explicit operator PointE(SizeE size) => new(size.Width, size.Height);
		private readonly string GetDebuggerDisplay() => ToString();
		/// <summary>
		/// Implicitly converts a tuple containing two <see cref="Expression"/>?.
		/// </summary>
		/// <remarks>This conversion enables concise and readable initialization of <see cref="SizeE"/> instances from tuples,
		/// allowing for more flexible assignment and construction patterns.</remarks>
		/// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="Expression"/>?. Either element may be null.</param>
		public static implicit operator SizeE((Expression? width, Expression? height) tuple) => new(tuple.width, tuple.height);
		/// <summary>
		/// Deconstructs the current instance into its Width and Height component values.
		/// </summary>
		/// <param name="width">When this method returns, contains the value of the Width component of the current instance.</param>
		/// <param name="height">When this method returns, contains the value of the Height component of the current instance.</param>
		public readonly void Deconstruct(out Expression? width, out Expression? height) { width = Width; height = Height; }
	}
