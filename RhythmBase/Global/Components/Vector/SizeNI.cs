using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// A size whose horizontal and vertical coordinates are <strong>non-nullable</strong> <see langword="integer" />
	/// </summary>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public struct SizeNI(int width, int height) : IVector<SizeNI, SizeNI, int>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeNI"/> struct with the specified point.
		/// </summary>
		/// <param name="pt">The point to initialize the size with.</param>
		public SizeNI(PointNI pt) : this(pt.X, pt.Y) { }
		/// <summary>
		/// Gets or sets the width of the size.
		/// </summary>
		public int Width { get; set; } = width;
		/// <summary>
		/// Gets or sets the height of the size.
		/// </summary>
		public int Height { get; set; } = height;
		/// <summary>
		/// Gets the area of the size.
		/// </summary>
		public readonly int Area => Width * Height;
		/// <summary>
		/// Gets the screen size.
		/// </summary>
		public static SizeNI Screen => new(352, 198);
		/// <summary>
		/// Adds two sizes together.
		/// </summary>
		/// <param name="sz1">The first size.</param>
		/// <param name="sz2">The second size.</param>
		/// <returns>The sum of the two sizes.</returns>
		public static SizeNI Add(SizeNI sz1, SizeNI sz2) => new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		/// <summary>
		/// Truncates the specified size to integer values.
		/// </summary>
		/// <param name="value">The size to truncate.</param>
		/// <returns>The truncated size.</returns>
		public static SizeNI Truncate(SizeN value) => new((int)value.Width, (int)value.Height);
		/// <summary>
		/// Subtracts one size from another.
		/// </summary>
		/// <param name="sz1">The first size.</param>
		/// <param name="sz2">The second size.</param>
		/// <returns>The difference between the two sizes.</returns>
		public static SizeNI Subtract(SizeNI sz1, SizeNI sz2) => new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		/// <summary>
		/// Rounds up the specified size to the nearest integer values.
		/// </summary>
		/// <param name="value">The size to round up.</param>
		/// <returns>The rounded-up size.</returns>
		public static SizeNI Ceiling(SizeN value) => new(
			(int)Math.Ceiling((double)value.Width),
			(int)Math.Ceiling((double)value.Height));
		/// <summary>
		/// Rounds the specified size to the nearest integer values.
		/// </summary>
		/// <param name="value">The size to round.</param>
		/// <returns>The rounded size.</returns>
		public static SizeNI Round(SizeN value) => new(
			(int)Math.Round((double)value.Width),
			(int)Math.Round((double)value.Height));
		/// <summary>
		/// Converts the current size to a point.
		/// </summary>
		/// <returns>A <see cref="PointNI"/> with the same width and height as the current size.</returns>
		public readonly PointNI ToPoint() => new(Width, Height);
		/// <inheritdoc/>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is SizeNI e && Equals(e);
		/// <inheritdoc/>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 31 + Width.GetHashCode();
			hash = hash * 31 + Height.GetHashCode();
			return hash;
		}
#else
		public readonly override int GetHashCode() => HashCode.Combine(Width, Height);
#endif
		/// <inheritdoc/>
		public readonly override string ToString() => $"[{Width},{Height}]";
		/// <inheritdoc/>
		public readonly bool Equals(SizeNI other) => Width == other.Width && Height == other.Height;
		/// <inheritdoc/>
		public static SizeNI operator +(SizeNI sz1, SizeNI sz2) => Add(sz1, sz2);
		/// <inheritdoc/>
		public static SizeNI operator -(SizeNI sz1, SizeNI sz2) => Subtract(sz1, sz2);
		/// <inheritdoc/>
		public static SizeN operator *(float left, SizeNI right) => new(left * right.Width, left * right.Height);
		/// <inheritdoc/>
		public static SizeN operator *(SizeNI left, float right) => new(left.Width * right, left.Height * right);
		/// <inheritdoc/>
		public static SizeNI operator *(int left, SizeNI right) => new(left * right.Width, left * right.Height);
		/// <inheritdoc/>
		public static SizeNI operator *(SizeNI left, int right) => new(left.Width * right, left.Height * right);
		/// <inheritdoc/>
		public static SizeN operator /(SizeNI left, float right) => new(left.Width / right, left.Height / right);
		/// <inheritdoc/>
		public static SizeNI operator /(SizeNI left, int right) => new(
			left.Width / right,
			left.Height / right);
		/// <inheritdoc/>
		public static bool operator ==(SizeNI sz1, SizeNI sz2) => sz1.Equals(sz2);
		/// <inheritdoc/>
		public static bool operator !=(SizeNI sz1, SizeNI sz2) => !sz1.Equals(sz2);
		/// <summary>
		/// Implicitly converts an <see cref="SizeNI"/> to an <see cref="SizeN"/>.
		/// </summary>
		/// <param name="p">The <see cref="SizeNI"/> to convert.</param>
		/// <returns>An <see cref="SizeN"/> with the same width and height as the input.</returns>
		public static implicit operator SizeN(SizeNI p) => new(p.Width, p.Height);
		/// <summary>
		/// Implicitly converts an <see cref="SizeNI"/> to an <see cref="SizeI"/>.
		/// </summary>
		/// <param name="p">The <see cref="SizeNI"/> to convert.</param>
		/// <returns>An <see cref="SizeI"/> with the same width and height as the input.</returns>
		public static implicit operator SizeI(SizeNI p) => new(p.Width, p.Height);
		/// <summary>
		/// Explicitly converts an <see cref="SizeNI"/> to an <see cref="PointNI"/>.
		/// </summary>
		/// <param name="size">The <see cref="SizeNI"/> to convert.</param>
		/// <returns>An <see cref="PointNI"/> with the same width and height as the input.</returns>
		public static explicit operator PointNI(SizeNI size) => new(size.Width, size.Height);
		private readonly string GetDebuggerDisplay() => ToString();
		/// <summary>
		/// Implicitly converts a tuple containing two <see cref="int"/>.
		/// </summary>
		/// <remarks>This conversion enables concise and readable initialization of <see cref="SizeNI"/> instances from tuples,
		/// allowing for more flexible assignment and construction patterns.</remarks>
		/// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="int"/>.</param>
		public static implicit operator SizeNI((int width, int height) tuple) => new(tuple.width, tuple.height);
		/// <summary>
		/// Deconstructs the current instance into its Width and Height component values.
		/// </summary>
		/// <param name="width">When this method returns, contains the value of the Width component of the current instance.</param>
		/// <param name="height">When this method returns, contains the value of the Height component of the current instance.</param>
		public readonly void Deconstruct(out int width, out int height) { width = Width; height = Height; }
	}
