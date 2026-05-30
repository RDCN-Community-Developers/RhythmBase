using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// An Expression
/// </summary>
public struct Expression
#if NET7_0_OR_GREATER
	: INumber<Expression>
#endif
{
	/// <summary>
	/// Gets the numeric value of the expression.
	/// </summary>
	public float NumericValue { get; }
	/// <summary>
	/// Gets the expression value as a string.
	/// </summary>
	public readonly string ExpressionValue
	{
		get
		{
			bool isNumeric = IsNumeric;
			string ExpressionValue = isNumeric ? NumericValue.ToString() : _exp;
			return ExpressionValue;
		}
	}
	/// <summary>
	/// Gets the evaluated value of the expression.
	/// </summary>
	public readonly float Value => IsNumeric ? NumericValue : Calculate(ExpressionValue);
	private static float Calculate(string exp)
	{
		if (string.IsNullOrWhiteSpace(exp))
			return 0;
		return float.TryParse(exp, out float result) ? result : 0;
	}
#if NET7_0_OR_GREATER
	static Expression INumberBase<Expression>.One => 1;
	static int INumberBase<Expression>.Radix => 10;
#endif
	/// <summary>
	/// Gets the additive identity for the <see cref="Expression"/> type.
	/// </summary>
	public static Expression Zero => 0;
#if NET7_0_OR_GREATER
	static Expression IAdditiveIdentity<Expression, Expression>.AdditiveIdentity => 0;
	static Expression IMultiplicativeIdentity<Expression, Expression>.MultiplicativeIdentity => 1;
#endif
	/// <summary>
	/// Initializes a new instance of the <see cref="Expression"/>
	/// </summary>
	/// <param name="value">The numeric value of the expression.</param>
	public Expression(float value)
	{
		this = default;
		IsNumeric = true;
		NumericValue = value;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="Expression"/> struct with a string value.
	/// </summary>
	/// <param name="value">The string value of the expression.</param>
	public Expression([AllowNull] string value)
	{
		IsNumeric = float.TryParse(value, out float numeric);
		if (IsNumeric)
			NumericValue = numeric;
		else
			_exp = value ?? "";
	}
	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Expression e && Equals(e);
	/// <inheritdoc/>
	public readonly bool Equals(Expression other) => IsNumeric == other.IsNumeric && NumericValue == other.NumericValue || _exp == other._exp;
	/// <inheritdoc/>
#if NETCOREAPP2_1_OR_GREATER
	public readonly override int GetHashCode()
	{
		HashCode hash = default;
		hash.Add(ExpressionValue);
		return hash.ToHashCode();
	}
#else
	public readonly override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + ExpressionValue.GetHashCode();
		return hash;
	}
#endif
	/// <inheritdoc/>
	public readonly override string ToString() => ExpressionValue;
	/// <summary>
	/// Converts a string to a nullable RDExpression.
	/// </summary>
	/// <param name="s">The string to convert.</param>
	/// <returns>A nullable RDExpression if the string is not null or empty; otherwise, null.</returns>
	public static Expression? Nullable(string s) => s != null && s.Length != 0 ? new Expression?(new Expression(s)) : null;

#if NET7_0_OR_GREATER
	readonly int IComparable.CompareTo(object? obj)
	{
		if (obj is Expression other)
		{
			return CompareTo(other);
		}
		throw new ArgumentException("Object is not a RDExpression");
	}
#endif
	/// <inheritdoc/>
	public readonly int CompareTo(Expression other)
	{
		return IsNumeric && other.IsNumeric
			? NumericValue.CompareTo(other.NumericValue)
			: string.Compare(ExpressionValue, other.ExpressionValue, StringComparison.Ordinal);
	}
#if NET7_0_OR_GREATER
	static Expression INumberBase<Expression>.Abs(Expression value)
	{
		return value.IsNumeric ? new Expression(Math.Abs(value.NumericValue)) : value;
	}
	static bool INumberBase<Expression>.IsCanonical(Expression value)
	{
		return true;
	}
	static bool INumberBase<Expression>.IsComplexNumber(Expression value)
	{
		return false;
	}
	static bool INumberBase<Expression>.IsEvenInteger(Expression value)
	{
		return value.IsNumeric && value.NumericValue % 2 == 0;
	}
	static bool INumberBase<Expression>.IsFinite(Expression value)
	{
		return value.IsNumeric && !float.IsInfinity(value.NumericValue);
	}
	static bool INumberBase<Expression>.IsImaginaryNumber(Expression value)
	{
		return false;
	}
	static bool INumberBase<Expression>.IsInfinity(Expression value)
	{
		return value.IsNumeric && float.IsInfinity(value.NumericValue);
	}
	static bool INumberBase<Expression>.IsInteger(Expression value)
	{
		return value.IsNumeric && value.NumericValue == Math.Floor(value.NumericValue);
	}
	static bool INumberBase<Expression>.IsNaN(Expression value)
	{
		return value.IsNumeric && float.IsNaN(value.NumericValue);
	}
	static bool INumberBase<Expression>.IsNegative(Expression value)
	{
		return value.IsNumeric ? value.NumericValue < 0 : Calculate(value.ExpressionValue) < 0;
	}
	static bool INumberBase<Expression>.IsNegativeInfinity(Expression value)
	{
		return value.IsNumeric && float.IsNegativeInfinity(value.NumericValue);
	}
	static bool INumberBase<Expression>.IsNormal(Expression value)
	{
		return value.IsNumeric && !float.IsSubnormal(value.NumericValue);
	}
	static bool INumberBase<Expression>.IsOddInteger(Expression value)
	{
		return value.IsNumeric && value.NumericValue % 2 != 0;
	}
	static bool INumberBase<Expression>.IsPositive(Expression value)
	{
		return value.IsNumeric ? value.NumericValue > 0 : Calculate(value.ExpressionValue) > 0;
	}
	static bool INumberBase<Expression>.IsPositiveInfinity(Expression value)
	{
		return value.IsNumeric && float.IsPositiveInfinity(value.NumericValue);
	}
	static bool INumberBase<Expression>.IsRealNumber(Expression value)
	{
		return value.IsNumeric;
	}
	static bool INumberBase<Expression>.IsSubnormal(Expression value)
	{
		return value.IsNumeric && float.IsSubnormal(value.NumericValue);
	}
	static bool INumberBase<Expression>.IsZero(Expression value)
	{
		return value.IsNumeric ? value.NumericValue == 0 : Calculate(value.ExpressionValue) == 0;
	}
	static Expression INumberBase<Expression>.MaxMagnitude(Expression x, Expression y)
	{
		return x.IsNumeric && y.IsNumeric ? Math.Abs(x.NumericValue) > Math.Abs(y.NumericValue) ? x : y : throw new NotImplementedException();
	}
	static Expression INumberBase<Expression>.MaxMagnitudeNumber(Expression x, Expression y)
	{
		return x.IsNumeric && y.IsNumeric ? Math.Abs(x.NumericValue) > Math.Abs(y.NumericValue) ? x : y : throw new NotImplementedException();
	}
	static Expression INumberBase<Expression>.MinMagnitude(Expression x, Expression y)
	{
		return x.IsNumeric && y.IsNumeric ? Math.Abs(x.NumericValue) < Math.Abs(y.NumericValue) ? x : y : throw new NotImplementedException();
	}
	static Expression INumberBase<Expression>.MinMagnitudeNumber(Expression x, Expression y)
	{
		return x.IsNumeric && y.IsNumeric ? Math.Abs(x.NumericValue) < Math.Abs(y.NumericValue) ? x : y : throw new NotImplementedException();
	}
	static Expression INumberBase<Expression>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
	{
		if (float.TryParse(s, style, provider, out float result))
		{
			return new Expression(result);
		}
		throw new FormatException("Input string was not in a correct format.");
	}
	static Expression INumberBase<Expression>.Parse(string s, NumberStyles style, IFormatProvider? provider)
	{
		if (float.TryParse(s, style, provider, out float result))
		{
			return new Expression(result);
		}
		throw new FormatException("Input string was not in a correct format.");
	}
	static bool INumberBase<Expression>.TryConvertFromChecked<TOther>(TOther value, out Expression result)
	{
		result = new Expression();
		return false;
	}
	static bool INumberBase<Expression>.TryConvertFromSaturating<TOther>(TOther value, out Expression result)
	{
		result = new Expression();
		return false;
	}
	static bool INumberBase<Expression>.TryConvertFromTruncating<TOther>(TOther value, out Expression result)
	{
		result = new Expression();
		return false;
	}
	static bool INumberBase<Expression>.TryConvertToChecked<TOther>(Expression value, out TOther result)
	{
		result = default!;
		return false;
	}
	static bool INumberBase<Expression>.TryConvertToSaturating<TOther>(Expression value, out TOther result)
	{
		result = default!;
		return false;
	}
	static bool INumberBase<Expression>.TryConvertToTruncating<TOther>(Expression value, out TOther result)
	{
		result = default!;
		return false;
	}
	static bool INumberBase<Expression>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Expression result)
	{
		result = new(s.ToString());
		return true;
	}
	static bool INumberBase<Expression>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Expression result)
	{
		result = new(s ?? "0");
		return true;
	}
	readonly bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		return NumericValue.TryFormat(destination, out charsWritten, format, provider);
	}
	readonly string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
	{
		return NumericValue.ToString(format, formatProvider);
	}
	static Expression ISpanParsable<Expression>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
	{
		if (float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out float result))
		{
			return new Expression(result);
		}
		throw new FormatException("Input string was not in a correct format.");
	}
	static bool ISpanParsable<Expression>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Expression result)
	{
		if (float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out float numericResult))
		{
			result = new Expression(numericResult);
			return true;
		}
		result = default;
		return false;
	}
	static Expression IParsable<Expression>.Parse(string s, IFormatProvider? provider)
	{
		if (float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out float result))
		{
			return new Expression(result);
		}
		throw new FormatException("Input string was not in a correct format.");
	}
	static bool IParsable<Expression>.TryParse(string? s, IFormatProvider? provider, out Expression result)
	{
		if (float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out float numericResult))
		{
			result = new Expression(numericResult);
			return true;
		}
		result = default;
		return false;
	}
#endif
	/// <inheritdoc/>
	public static Expression operator +(Expression left, float right) => left.IsNumeric
			? new Expression(left.NumericValue + right)
			: new Expression($"{left.ExpressionValue}+{right}");
	/// <inheritdoc/>
	public static Expression operator +(float left, Expression right) => right.IsNumeric
			? new Expression(left + right.NumericValue)
			: new Expression($"{left}+{right.ExpressionValue}");
	/// <inheritdoc/>
	public static Expression operator +(Expression left, Expression right) => left.IsNumeric && right.IsNumeric
			? new Expression(left.NumericValue + right.NumericValue)
			: new Expression($"{left.ExpressionValue}+{right.ExpressionValue}");
	/// <inheritdoc/>
	public static Expression operator -(Expression left, float right) => left.IsNumeric
			? new Expression(left.NumericValue - right)
			: new Expression($"{left.ExpressionValue}-{right}");
	/// <inheritdoc/>
	public static Expression operator -(float left, Expression right) => right.IsNumeric
			? new Expression(left - right.NumericValue)
			: new Expression($"{left}-{right.ExpressionValue}");
	/// <inheritdoc/>
	public static Expression operator -(Expression left, Expression right) => left.IsNumeric && right.IsNumeric
			? new Expression(left.NumericValue - right.NumericValue)
			: new Expression($"{left.ExpressionValue}-{right.ExpressionValue}");
	/// <inheritdoc/>
	public static Expression operator *(Expression left, float right) => left.IsNumeric
			? new Expression(left.NumericValue * right)
			: new Expression($"({left.ExpressionValue})*{right}");
	/// <inheritdoc/>
	public static Expression operator *(float left, Expression right) => right.IsNumeric
			? new Expression(left * right.NumericValue)
			: new Expression($"{left}*({right.ExpressionValue})");
	/// <inheritdoc/>
	public static Expression operator *(Expression left, Expression right) => left.IsNumeric && right.IsNumeric
			? new Expression(left.NumericValue * right.NumericValue)
			: new Expression($"({left.ExpressionValue})*({right.ExpressionValue})");
	/// <inheritdoc/>
	public static Expression operator /(Expression left, float right) => left.IsNumeric
			? new Expression(left.NumericValue / right)
			: new Expression($"({left.ExpressionValue})/{right}");
	/// <inheritdoc/>
	public static Expression operator /(float left, Expression right) => right.IsNumeric
			? new Expression(left / right.NumericValue)
			: new Expression($"{left}/({right.ExpressionValue})");
	/// <inheritdoc/>
	public static Expression operator /(Expression left, Expression right) => left.IsNumeric && right.IsNumeric
			? new Expression(left.NumericValue / right.NumericValue)
			: new Expression($"({left.ExpressionValue})/({right.ExpressionValue})");
	/// <inheritdoc/>
	public static bool operator ==(Expression left, Expression right) => left.Equals(right);
	/// <inheritdoc/>
	public static bool operator !=(Expression left, Expression right) => !(left == right);
	/// <inheritdoc/>
	public static implicit operator Expression(float v) => new(v);
	/// <inheritdoc/>
	public static implicit operator Expression(string v) => new(v);
#if NET7_0_OR_GREATER
	static bool IComparisonOperators<Expression, Expression, bool>.operator >(Expression left, Expression right)
	{
		return left.CompareTo(right) > 0;
	}
	static bool IComparisonOperators<Expression, Expression, bool>.operator >=(Expression left, Expression right)
	{
		return left.CompareTo(right) >= 0;
	}
	static bool IComparisonOperators<Expression, Expression, bool>.operator <(Expression left, Expression right)
	{
		return left.CompareTo(right) < 0;
	}
	static bool IComparisonOperators<Expression, Expression, bool>.operator <=(Expression left, Expression right)
	{
		return left.CompareTo(right) <= 0;
	}
	static Expression IModulusOperators<Expression, Expression, Expression>.operator %(Expression left, Expression right)
	{
		if (left.IsNumeric && right.IsNumeric)
		{
			return new Expression(left.NumericValue % right.NumericValue);
		}
		throw new NotImplementedException("Modulus operator is not implemented for non-numeric expressions.");
	}
	static Expression IDecrementOperators<Expression>.operator --(Expression value)
	{
		if (value.IsNumeric)
		{
			return new Expression(value.NumericValue - 1);
		}
		throw new NotImplementedException("Decrement operator is not implemented for non-numeric expressions.");
	}
	static Expression IIncrementOperators<Expression>.operator ++(Expression value)
	{
		if (value.IsNumeric)
		{
			return new Expression(value.NumericValue + 1);
		}
		throw new NotImplementedException("Increment operator is not implemented for non-numeric expressions.");
	}
	static Expression IUnaryNegationOperators<Expression, Expression>.operator -(Expression value)
	{
		if (value.IsNumeric)
		{
			return new Expression(-value.NumericValue);
		}
		throw new NotImplementedException("Unary negation operator is not implemented for non-numeric expressions.");
	}
	static Expression IUnaryPlusOperators<Expression, Expression>.operator +(Expression value)
	{
		return value;
	}
#endif
	private readonly string _exp = "";
	/// <summary>
	/// 
	/// </summary>
	public bool IsNumeric { get; private set; } = false;
}
