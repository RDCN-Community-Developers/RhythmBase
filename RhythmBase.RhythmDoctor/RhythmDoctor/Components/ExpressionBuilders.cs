using System.ComponentModel;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// An interface representing a Rhythm Doctor expression,
	/// which can be serialized into a string format for use in the game.
	/// This interface serves as a base for various types of expressions,
	/// including boolean, numeric, string, and void expressions,
	/// allowing for a unified way to handle and serialize different kinds of
	/// operations and values within the Rhythm Doctor modding framework.
/// </summary>
public interface IRDExpression
	{
		/// <summary>
		/// Serializes the current object to a string representation.
		/// </summary>
		/// <returns>A string that contains the serialized representation of the current object.</returns>
		string Serialize();
	}
	internal interface IOp
	{
		byte Priority { get; }
		bool IsStatic { get; }
		string Serialize();
	}
	internal interface IEndpointOp : IOp { }
	internal interface IConstantOp : IEndpointOp { }
	internal interface IVariableOp : IEndpointOp { }
	internal interface IUnaryOp : IOp { }
	internal interface IBinaryOp : IOp { }
	internal interface IFunctionOp : IOp
	{
		string Name { get; }
		IOp[] Args { get; }
	}
	internal interface IBooleanOp : IOp
	{
		IBooleanOp Simplified();
	}
	internal interface INumericOp : IOp
	{
		INumericOp Simplified();
	}
	internal readonly record struct BooleanValue(bool Value) : IConstantOp, IBooleanOp
	{
		public byte Priority => 0;
		public bool IsStatic => true;
		public IBooleanOp Simplified() => this;
		public override string ToString() => Value.ToString();
		public string Serialize() => Value ? "true" : "false";
	}
	internal readonly record struct NumericValue(float Value) : IConstantOp, INumericOp
	{
		public byte Priority => 0;
		public bool IsStatic => true;
		public INumericOp Simplified() => this;
		public override string ToString() => Value.ToString();
		public string Serialize() => Value.ToString("R");
	}
	internal readonly record struct StringValue(string Value) : IConstantOp
	{
		public byte Priority => 0;
		public bool IsStatic => true;
		public override string ToString() => $"\"{Value}\"";
		public string Serialize() => $"str:{Value}";
	}
	internal readonly record struct BooleanVariable(int Index) : IVariableOp, IBooleanOp
	{
		public byte Priority => 0;
		public bool IsStatic => false;
		public IBooleanOp Simplified() => this;
		public override string ToString() => $"b{Index}";
		public string Serialize() => $"b{Index}";
	}
	internal readonly record struct FloatVariable(int Index) : IVariableOp, INumericOp
	{
		public byte Priority => 0;
		public bool IsStatic => false;
		public INumericOp Simplified() => this;
		public override string ToString() => $"f{Index}";
		public string Serialize() => $"f{Index}";
	}
	internal readonly record struct IntVariable(int Index) : IVariableOp, INumericOp
	{
		public byte Priority => 0;
		public bool IsStatic => false;
		public INumericOp Simplified() => this;
		public override string ToString() => $"i{Index}";
		public string Serialize() => $"i{Index}";
	}
	internal readonly record struct NamedBooleanVariable(string Name) : IVariableOp, IBooleanOp
	{
		public byte Priority => 0;
		public bool IsStatic => false;
		public IBooleanOp Simplified() => this;
		public override string ToString() => Name;
		public string Serialize() => Name;
	}
	internal readonly record struct NamedNumericVariable(string Name) : IVariableOp, INumericOp
	{
		public byte Priority => 0;
		public bool IsStatic => false;
		public INumericOp Simplified() => this;
		public override string ToString() => Name;
		public string Serialize() => Name;
	}
	internal readonly record struct UnaryBooleanOp(IBooleanOp Value, UnaryOperator Op) : IUnaryOp, IBooleanOp
	{
		public byte Priority => (byte)((byte)Op >> 2);
		public bool IsStatic => Value.IsStatic;
		public IBooleanOp Simplified()
		{
			IBooleanOp innerValue = Value.Simplified();
			IBooleanOp value = Op switch
			{
				UnaryOperator.Not => innerValue switch
				{
					BooleanValue boolVal => new BooleanValue(!boolVal.Value),
					_ => this,
				},
				_ => throw new InvalidOperationException(),
			};
			return value;

		}
		public string Serialize()
		{
			var innerValue = Simplified();
			if (innerValue is not UnaryBooleanOp tmp || tmp != this)
			{
				return innerValue.Serialize();
			}
			bool needParens = Value.Priority > 0 && Value.Priority < Priority;
			string opStr = needParens ? $"({Value.Serialize()})" : Value.Serialize();
			return Op switch
			{
				UnaryOperator.Not => $"Not {opStr}",
				_ => throw new InvalidOperationException(),
			};
		}
	}
	internal readonly record struct UnaryNumericOp(INumericOp Value, UnaryOperator Op) : IUnaryOp, INumericOp
	{
		public byte Priority => (byte)((byte)Op >> 2);
		public bool IsStatic => Value.IsStatic;
		public INumericOp Simplified()
		{
			INumericOp innerValue = Value.Simplified();
			INumericOp value = Op switch
			{
				UnaryOperator.Negate => innerValue switch
				{
					NumericValue floatVal => new NumericValue(-floatVal.Value),
					_ => this,
				},
				UnaryOperator.Positive => innerValue,
				_ => throw new InvalidOperationException(),
			};
			return value;
		}
		public string Serialize()
		{
			INumericOp innerValue = Simplified();
			if (innerValue is not UnaryNumericOp tmp || tmp != this)
			{
				return innerValue.Serialize();
			}
			bool needParens = Value.Priority > 0 && Value.Priority < Priority;
			string opStr = needParens ? $"({Value.Serialize()})" : Value.Serialize();
			return Op switch
			{
				UnaryOperator.Negate => $"-{opStr}",
				UnaryOperator.Positive => $"+{opStr}",
				_ => throw new InvalidOperationException(),
			};
		}
	}
	internal readonly record struct BinaryBooleanOp : IBinaryOp, IBooleanOp
	{
		public IOp Left { get; }
		public IOp Right { get; }
		public BinaryOperator Op { get; }
		public byte Priority => (byte)((byte)Op >> 2);
		public bool IsStatic => Left.IsStatic && Right.IsStatic;
		public BinaryBooleanOp(IBooleanOp left, IBooleanOp right, BinaryOperator op)
		{
			Left = left;
			Right = right;
			Op = op;
		}
		public BinaryBooleanOp(INumericOp left, INumericOp right, BinaryOperator op)
		{
			Left = left;
			Right = right;
			Op = op;
		}
		public IBooleanOp Simplified()
		{
			switch (Left, Right)
			{
				case (IBooleanOp leftBool, IBooleanOp rightBool):
					{
						IBooleanOp leftValue = leftBool.Simplified();
						IBooleanOp rightValue = rightBool.Simplified();
						IBooleanOp value = Op switch
						{
							BinaryOperator.Or => (leftValue, rightValue) switch
							{
								(BooleanValue leftVal, BooleanValue rightVal) => new BooleanValue(leftVal.Value || rightVal.Value),
								(BooleanValue leftVal, _) when leftVal.Value => new BooleanValue(true),
								(BooleanValue leftVal, _) when !leftVal.Value => rightValue,
								(_, BooleanValue rightVal) when rightVal.Value => new BooleanValue(true),
								(_, BooleanValue rightVal) when !rightVal.Value => leftValue,
								_ => this,
							},
							BinaryOperator.And => (leftValue, rightValue) switch
							{
								(BooleanValue leftVal, BooleanValue rightVal) => new BooleanValue(leftVal.Value && rightVal.Value),
								(BooleanValue leftVal, _) when leftVal.Value => rightValue,
								(BooleanValue leftVal, _) when !leftVal.Value => new BooleanValue(false),
								(_, BooleanValue rightVal) when rightVal.Value => leftValue,
								(_, BooleanValue rightVal) when !rightVal.Value => new BooleanValue(false),
								_ => this,
							},
							BinaryOperator.Equal => (leftValue, rightValue) switch
							{
								(BooleanValue leftVal, BooleanValue rightVal) => new BooleanValue(leftVal.Value == rightVal.Value),
								(BooleanValue leftVal, _) when leftVal.Value => rightValue,
								(BooleanValue leftVal, _) when !leftVal.Value => new UnaryBooleanOp(rightValue, UnaryOperator.Not),
								(_, BooleanValue rightVal) when rightVal.Value => leftValue,
								(_, BooleanValue rightVal) when !rightVal.Value => new UnaryBooleanOp(leftValue, UnaryOperator.Not),
								_ => this,
							},
							BinaryOperator.NotEqual => (leftValue, rightValue) switch
							{
								(BooleanValue leftVal, BooleanValue rightVal) => new BooleanValue(leftVal.Value == rightVal.Value),
								(BooleanValue leftVal, _) when leftVal.Value => new UnaryBooleanOp(rightValue, UnaryOperator.Not),
								(BooleanValue leftVal, _) when !leftVal.Value => rightValue,
								(_, BooleanValue rightVal) when rightVal.Value => new UnaryBooleanOp(leftValue, UnaryOperator.Not),
								(_, BooleanValue rightVal) when !rightVal.Value => leftValue,
								_ => this,
							},
							_ => throw new InvalidOperationException(),
						};
						return value;
					}
				case (INumericOp leftNum, INumericOp rightNum):
					{
						INumericOp leftValue = leftNum.Simplified();
						INumericOp rightValue = rightNum.Simplified();
						if (!leftValue.IsStatic || !rightValue.IsStatic)
							return this;
						float leftFloat = leftValue switch
						{
							NumericValue floatVal => floatVal.Value,
							_ => throw new InvalidOperationException(),
						};
						float rightFloat = rightValue switch
						{
							NumericValue floatVal => floatVal.Value,
							_ => throw new InvalidOperationException(),
						};
						IBooleanOp value = new BooleanValue(Op switch
						{
							BinaryOperator.Equal => leftFloat == rightFloat,
							BinaryOperator.NotEqual => leftFloat != rightFloat,
							BinaryOperator.GreaterThan => leftFloat > rightFloat,
							BinaryOperator.GreaterThanOrEqual => leftFloat >= rightFloat,
							BinaryOperator.LessThan => leftFloat < rightFloat,
							BinaryOperator.LessThanOrEqual => leftFloat <= rightFloat,
							_ => throw new InvalidOperationException(),
						});
						return value;
					}
				default:
					throw new InvalidOperationException();
			}
		}
		public string Serialize()
		{
			IBooleanOp value = Simplified();
			if (value is not BinaryBooleanOp tmp || tmp != this)
			{
				return value.Serialize();
			}
			bool needParensLeft = Left.Priority > 0 && Left.Priority < Priority;
			bool needParensRight = Right.Priority > 0 && Right.Priority < Priority;
			string leftStr = needParensLeft ? $"({Left.Serialize()})" : Left.Serialize();
			string rightStr = needParensRight ? $"({Right.Serialize()})" : Right.Serialize();
			return Op switch
			{
				BinaryOperator.Or => $"{leftStr} Or {rightStr}",
				BinaryOperator.And => $"{leftStr} And {rightStr}",
				BinaryOperator.Equal => $"{leftStr} == {rightStr}",
				BinaryOperator.NotEqual => $"{leftStr} <> {rightStr}",
				BinaryOperator.GreaterThan => $"{leftStr} > {rightStr}",
				BinaryOperator.GreaterThanOrEqual => $"{leftStr} >= {rightStr}",
				BinaryOperator.LessThan => $"{leftStr} < {rightStr}",
				BinaryOperator.LessThanOrEqual => $"{leftStr} <= {rightStr}",
				_ => throw new InvalidOperationException(),
			};
		}
	}
	internal readonly record struct BinaryNumericOp(INumericOp Left, INumericOp Right, BinaryOperator Op) : IBinaryOp, INumericOp
	{
		public byte Priority => (byte)((byte)Op >> 2);
		public bool IsStatic => Left.IsStatic && Right.IsStatic;
		public INumericOp Simplified()
		{
			INumericOp leftValue = Left.Simplified();
			INumericOp rightValue = Right.Simplified();
			switch (leftValue, rightValue)
			{
				case (_, _) when leftValue.IsStatic && rightValue.IsStatic:
					float leftFloat = leftValue switch
					{
						NumericValue floatVal => floatVal.Value,
						_ => throw new InvalidOperationException(),
					};
					float rightFloat = rightValue switch
					{
						NumericValue floatVal => floatVal.Value,
						_ => throw new InvalidOperationException(),
					};
					INumericOp value = new NumericValue(Op switch
					{
						BinaryOperator.Add => leftFloat + rightFloat,
						BinaryOperator.Subtract => leftFloat - rightFloat,
						BinaryOperator.Multiply => leftFloat * rightFloat,
						BinaryOperator.Divide => leftFloat / rightFloat,
						BinaryOperator.Modulo => leftFloat % rightFloat,
						_ => throw new InvalidOperationException(),
					});
					return value;
				case (NumericValue leftVal1, _) when
					(leftVal1.Value == 0 && Op is BinaryOperator.Add or BinaryOperator.Divide) ||
					(leftVal1.Value == 1 && Op is BinaryOperator.Multiply or BinaryOperator.Subtract):
					return rightValue;
				case (_, NumericValue rightVal1) when
					(rightVal1.Value == 0 && Op is BinaryOperator.Add or BinaryOperator.Divide) ||
					(rightVal1.Value == 1 && Op is BinaryOperator.Multiply or BinaryOperator.Subtract):
					return leftValue;
				case (NumericValue leftVal1, _) when
					(leftVal1.Value == 0 && Op is BinaryOperator.Multiply or BinaryOperator.Divide):
					return new NumericValue(0);
				case (_, NumericValue rightVal1) when
					(rightVal1.Value == 0 && Op is BinaryOperator.Multiply) ||
					(rightVal1.Value == 1 && Op is BinaryOperator.Modulo):
					return new NumericValue(0);
				default:
					return new BinaryNumericOp(leftValue, rightValue, Op);

			}
		}
		public string Serialize()
		{
			INumericOp value = Simplified();
			if (value is not BinaryNumericOp tmp || tmp != this)
			{
				return value.Serialize();
			}
			bool needParensLeft = Left.Priority > 0 && Left.Priority < Priority;
			bool needParensRight =
				Right.Priority > 0 && Right.Priority < Priority ||
				(Op is BinaryOperator.Divide && Right is BinaryNumericOp right && right.Op is not BinaryOperator.Divide) ||
				(Op is BinaryOperator.Modulo && Right is BinaryNumericOp);
			string leftStr = needParensLeft ? $"({Left.Serialize()})" : Left.Serialize();
			string rightStr = needParensRight ? $"({Right.Serialize()})" : Right.Serialize();
			return Op switch
			{
				BinaryOperator.Add => $"{leftStr} + {rightStr}",
				BinaryOperator.Subtract => $"{leftStr} - {rightStr}",
				BinaryOperator.Multiply => $"{leftStr} * {rightStr}",
				BinaryOperator.Divide => $"{leftStr} / {rightStr}",
				BinaryOperator.Modulo => $"{leftStr} % {rightStr}",
				_ => throw new InvalidOperationException(),
			};
		}
	}
	internal readonly record struct FunctionVoidOp(string Name, IOp[] Args, bool IsSelfStatic) : IFunctionOp, IOp
	{
		public byte Priority => 0;
		public bool IsStatic => IsSelfStatic && Args.All(arg => arg.IsStatic);
		public string Serialize()
		{
			string argsStr = string.Join(", ", Args.Select(arg => arg.Serialize()));
			return $"{Name}({argsStr})";
		}
	}
	internal readonly record struct FunctionBooleanOp(string Name, IOp[] Args, bool IsSelfStatic) : IFunctionOp, IBooleanOp
	{
		public byte Priority => 0;
		public bool IsStatic => IsSelfStatic && Args.All(arg => arg.IsStatic);
		public IBooleanOp Simplified() => this;
		public string Serialize()
		{
			string argsStr = string.Join(", ", Args.Select(arg => arg.Serialize()));
			return $"{Name}({argsStr})";
		}
	}
	internal readonly record struct FunctionNumericOp(string Name, IOp[] Args, bool IsSelfStatic) : IFunctionOp, INumericOp
	{
		public byte Priority => 0;
		public bool IsStatic => IsSelfStatic && Args.All(arg => arg.IsStatic);
		public INumericOp Simplified() => this;
		public string Serialize()
		{
			string argsStr = string.Join(", ", Args.Select(arg => arg.Serialize()));
			return $"{Name}({argsStr})";
		}
	}
	/*

	0  value
	1  = += -= *= /= %=
	2  ??
	3  ||
	4  &&
	5  |
	6  ^
	7  &
	8  == !=
	9  < > <= >=
	10 << >>
	11 + - (binary)
	12 * / %
	13 + - ! ~ (unary)
	14 ++ -- . -> [] ()

	 */
	internal enum BinaryOperator : byte
	{
		Or = (3 << 2) | 0,
		And = (4 << 2) | 0,
		Equal = (8 << 2) | 0,
		NotEqual = (8 << 2) | 1,
		GreaterThan = (9 << 2) | 0,
		GreaterThanOrEqual = (9 << 2) | 1,
		LessThan = (9 << 2) | 2,
		LessThanOrEqual = (9 << 2) | 3,
		Add = (11 << 2) | 0,
		Subtract = (11 << 2) | 1,
		Multiply = (12 << 2) | 0,
		Divide = (12 << 2) | 1,
		Modulo = (12 << 2) | 2,
	}
	internal enum UnaryOperator : byte
	{
		Not = (13 << 2) | 0,
		Negate = (13 << 2) | 1,
		Positive = (13 << 2) | 2,
	}
	/// <summary>
	/// Represents a boolean expression that can be evaluated and combined using logical operations.
	/// </summary>
	public struct RDBooleanExpression : IRDExpression
	{
		/// <summary>
		/// Provides indexed access to boolean expression variables for constructing dynamic boolean expressions.
		/// </summary>
		/// <remarks>Use this class to obtain boolean expression variables by their index, enabling the creation of
		/// complex logical expressions where each index corresponds to a unique boolean variable. This is useful in scenarios
		/// where the number or identity of variables is determined at runtime.</remarks>
		public class RDBooleanExpressionBuilder()
		{
			/// <summary>
			/// Gets a new instance of <see cref="RDBooleanExpression"/> that represents the boolean variable at the specified
			/// index.
			/// </summary>
			/// <remarks>The index parameter is used to create a new <see cref="BooleanVariable"/>, which is then
			/// wrapped in an <see cref="RDBooleanExpression"/>. Supplying a negative index may result in an exception.</remarks>
			/// <param name="index">The zero-based index of the boolean variable to retrieve. Must be a non-negative integer.</param>
			/// <returns>A new <see cref="RDBooleanExpression"/> corresponding to the boolean variable at the specified index.</returns>
			public RDBooleanExpression this[int index] => new(new BooleanVariable(index));
		}
		internal IBooleanOp _v;
		internal RDBooleanExpression(IBooleanOp v) => _v = v;
		/// <summary>
		/// Implicitly converts a Boolean value to an instance of RDBooleanExpression.
		/// </summary>
		/// <remarks>This implicit conversion enables Boolean values to be used directly in contexts that require an
		/// RDBooleanExpression, allowing for more concise and expressive query construction.</remarks>
		/// <param name="v">The Boolean value to convert to an RDBooleanExpression.</param>
		public static implicit operator RDBooleanExpression(bool v) => new(new BooleanValue(v));
		/// <summary>
		/// Negates the specified boolean expression by applying the logical NOT operator.
		/// </summary>
		/// <remarks>Use this operator to invert the value of a boolean expression in query construction or evaluation
		/// scenarios.</remarks>
		/// <param name="v">The boolean expression to negate. This parameter cannot be null.</param>
		/// <returns>A new instance of <see cref="RDBooleanExpression"/> that represents the logical negation of the input expression.</returns>
		public static RDBooleanExpression operator !(RDBooleanExpression v) => new(new UnaryBooleanOp(v._v, UnaryOperator.Not));
		/// <summary>
		/// Combines two boolean expressions using a logical OR operation.
		/// </summary>
		/// <remarks>This operator enables the construction of complex boolean logic by combining multiple expressions
		/// in a concise manner. It is particularly useful when building dynamic query conditions or filters.</remarks>
		/// <param name="left">The first boolean expression to combine.</param>
		/// <param name="right">The second boolean expression to combine.</param>
		/// <returns>A new RDBooleanExpression that represents the logical OR of the two input expressions.</returns>
		public static RDBooleanExpression operator |(RDBooleanExpression left, RDBooleanExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.Or));
		/// <summary>
		/// Combines two boolean expressions using a logical AND operation.
		/// </summary>
		/// <remarks>This operator enables chaining of multiple boolean expressions using the AND operator, allowing
		/// for the construction of complex logical conditions.</remarks>
		/// <param name="left">The first boolean expression to combine.</param>
		/// <param name="right">The second boolean expression to combine.</param>
		/// <returns>A new RDBooleanExpression that represents the logical AND of the two input expressions.</returns>
		public static RDBooleanExpression operator &(RDBooleanExpression left, RDBooleanExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.And));
		/// <summary>
		/// Creates a boolean expression that represents the equality comparison of two RDBooleanExpression instances.
		/// </summary>
		/// <remarks>Use this operator to construct an equality comparison between two RDBooleanExpression objects in
		/// a concise and readable manner. Both operands should be properly initialized before comparison.</remarks>
		/// <param name="left">The first RDBooleanExpression to compare.</param>
		/// <param name="right">The second RDBooleanExpression to compare.</param>
		/// <returns>A new RDBooleanExpression that evaluates to <see langword="true"/> if the two expressions are equal; otherwise,
		/// <see langword="false"/>.</returns>
		public static RDBooleanExpression operator ==(RDBooleanExpression left, RDBooleanExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.Equal));
		/// <summary>
		/// Defines the inequality operator for two RDBooleanExpression instances, returning a new expression that evaluates
		/// whether the operands are not equal.
		/// </summary>
		/// <remarks>Use this operator to construct expressions that represent logical inequality between two
		/// RDBooleanExpression instances in a concise and readable manner.</remarks>
		/// <param name="left">The first RDBooleanExpression to compare.</param>
		/// <param name="right">The second RDBooleanExpression to compare.</param>
		/// <returns>A new RDBooleanExpression that evaluates to <see langword="true"/> if the two expressions are not equal;
		/// otherwise, <see langword="false"/>.</returns>
		public static RDBooleanExpression operator !=(RDBooleanExpression left, RDBooleanExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.NotEqual));
    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new NotSupportedException();
    /// <inheritdoc />
		[EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();
    /// <inheritdoc />
    public readonly string Serialize() => _v.Serialize();
	}
	/// <summary>
	/// Represents a numeric expression that can be evaluated and manipulated using various arithmetic operations.
	/// </summary>
	/// <remarks>RDNumericExpression supports implicit conversion from integer and floating-point values, enabling
	/// seamless integration with standard numeric types. It provides operator overloads for common arithmetic and
	/// comparison operations, allowing expressions to be composed and combined in a natural, mathematical syntax. The
	/// expression can be serialized to a string representation for storage or transmission. This structure is typically
	/// used in scenarios where numeric expressions need to be constructed, evaluated, or analyzed dynamically.</remarks>
	public struct RDNumericExpression : IRDExpression
	{
    /// <summary>
    /// Provides indexed access to numeric expressions based on integer variables.
    /// </summary>
    /// <remarks>This class allows access to numeric expressions by index, facilitating the construction of
    /// complex mathematical expressions involving integer values.</remarks>
    public class RDIntegerExpressionBuilder()
		{
			/// <summary>
			/// Gets the numeric expression at the specified zero-based index.
			/// </summary>
			/// <param name="index">The zero-based index of the numeric expression to retrieve.</param>
			/// <returns>A new instance of <see cref="RDNumericExpression"/> representing the numeric expression at the specified index.</returns>
			public RDNumericExpression this[int index] => new(new IntVariable(index));
		}
		/// <summary>
		/// Provides a builder for creating numeric expressions based on floating-point variables.
		/// </summary>
		/// <remarks>This class allows access to numeric expressions by index, facilitating the construction of
		/// complex mathematical expressions involving floating-point values.</remarks>
		public class RDFloatExpressionBuilder()
		{
			/// <summary>
			/// Gets the numeric expression at the specified zero-based index.
			/// </summary>
			/// <remarks>Use this indexer to access numeric expressions by their position in the collection. Ensure that
			/// the index is within the valid range to prevent errors.</remarks>
			/// <param name="index">The zero-based index of the numeric expression to retrieve.</param>
			/// <returns>A new instance of <see cref="RDNumericExpression"/> representing the numeric expression at the specified index.</returns>
			public RDNumericExpression this[int index] => new(new FloatVariable(index));
		}
		internal INumericOp _v;
		internal RDNumericExpression(INumericOp v) => _v = v;
		/// <summary>
		/// Implicitly converts a single-precision floating-point value to an instance of the RDNumericExpression class.
		/// </summary>
		/// <remarks>This implicit conversion enables seamless use of float values in contexts that require
		/// RDNumericExpression instances, allowing for more concise and readable code.</remarks>
		/// <param name="v">The single-precision floating-point value to convert.</param>
		public static implicit operator RDNumericExpression(float v) => new(new NumericValue(v));
		/// <summary>
		/// Implicitly converts a 32-bit integer to an instance of the RDNumericExpression class.
		/// </summary>
		/// <remarks>This implicit conversion enables integer values to be used directly in expressions or assignments
		/// where an RDNumericExpression is expected, improving code readability and convenience.</remarks>
		/// <param name="v">The integer value to convert to an RDNumericExpression.</param>
		public static implicit operator RDNumericExpression(int v) => new(new NumericValue(v));
    /// <summary>
    /// Builds a new RDNumericExpression that represents the positive value of the specified numeric expression by applying
    /// </summary>
    public static RDNumericExpression operator +(RDNumericExpression v) => new(new UnaryNumericOp(v._v, UnaryOperator.Positive));
    /// <summary>
    /// Builds a new RDNumericExpression that represents the negation of the specified numeric expression by applying the unary
    /// </summary>
    public static RDNumericExpression operator -(RDNumericExpression v) => new(new UnaryNumericOp(v._v, UnaryOperator.Negate));
    /// <summary>
    /// Builds a new RDNumericExpression that represents the sum of two RDNumericExpression instances by applying the binary addition operator.
    /// </summary>
    public static RDNumericExpression operator +(RDNumericExpression left, RDNumericExpression right) => new(new BinaryNumericOp(left._v, right._v, BinaryOperator.Add));
    /// <summary>
    /// Builds a new RDNumericExpression that represents the difference between two RDNumericExpression instances by applying the binary subtraction operator.
    /// </summary>
    public static RDNumericExpression operator -(RDNumericExpression left, RDNumericExpression right) => new(new BinaryNumericOp(left._v, right._v, BinaryOperator.Subtract));
    /// <summary>
    /// Builds a new RDNumericExpression that represents the product of two RDNumericExpression instances by applying the binary multiplication operator.
    /// </summary>
    public static RDNumericExpression operator *(RDNumericExpression left, RDNumericExpression right) => new(new BinaryNumericOp(left._v, right._v, BinaryOperator.Multiply));
    /// <summary>
    /// Builds a new RDNumericExpression that represents the quotient of two RDNumericExpression instances by applying the binary division operator.
    /// </summary>
    public static RDNumericExpression operator /(RDNumericExpression left, RDNumericExpression right) => new(new BinaryNumericOp(left._v, right._v, BinaryOperator.Divide));
    /// <summary>
    /// Builds a new RDNumericExpression that represents the remainder of the division of two RDNumericExpression instances by applying the binary modulo operator.
    /// </summary>
    public static RDNumericExpression operator %(RDNumericExpression left, RDNumericExpression right) => new(new BinaryNumericOp(left._v, right._v, BinaryOperator.Modulo));
    /// <summary>
    /// Builds a boolean expression that compares two RDNumericExpression instances for equality.
    /// </summary>
    public static RDBooleanExpression operator ==(RDNumericExpression left, RDNumericExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.Equal));
    /// <summary>
    /// Builds a boolean expression that compares two RDNumericExpression instances for inequality.
    /// </summary>
    public static RDBooleanExpression operator !=(RDNumericExpression left, RDNumericExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.NotEqual));
    /// <summary>
    /// Builds a boolean expression that indicates whether one RDNumericExpression instance is greater than another.
    /// </summary>
    public static RDBooleanExpression operator >(RDNumericExpression left, RDNumericExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.GreaterThan));
    /// <summary>
    /// Builds a boolean expression that indicates whether one RDNumericExpression instance is greater than or equal to another.
    /// </summary>
    public static RDBooleanExpression operator >=(RDNumericExpression left, RDNumericExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.GreaterThanOrEqual));
    /// <summary>
    /// Builds a boolean expression that indicates whether one RDNumericExpression instance is less than another.
    /// </summary>
    public static RDBooleanExpression operator <(RDNumericExpression left, RDNumericExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.LessThan));
    /// <summary>
    /// Builds a boolean expression that indicates whether one RDNumericExpression instance is less than or equal to another.
    /// </summary>
    public static RDBooleanExpression operator <=(RDNumericExpression left, RDNumericExpression right) => new(new BinaryBooleanOp(left._v, right._v, BinaryOperator.LessThanOrEqual));
    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly override bool Equals(object? obj) => throw new NotSupportedException();
    /// <inheritdoc />
		[EditorBrowsable(EditorBrowsableState.Never)]
    public readonly override int GetHashCode() => throw new NotSupportedException();
    /// <inheritdoc />
    public readonly string Serialize() => _v.Serialize();
	}
	/// <summary>
	/// Represents a string-based expression that can be implicitly created from a string or an enumeration value for use
	/// in Rhythm Doctor expression building.
	/// </summary>
	/// <remarks>This struct enables seamless conversion from string literals and enumeration values, allowing for
	/// flexible construction of string expressions. It provides a method to serialize the contained value for further
	/// processing or storage.</remarks>
	public struct RDStringExpression : IRDExpression
	{
		internal StringValue _v;
		internal RDStringExpression(StringValue v) => _v = v;
		/// <summary>
		/// Defines an implicit conversion from a string to an RDStringExpression instance.
		/// </summary>
		/// <remarks>This implicit conversion enables direct assignment of string values to variables or parameters of
		/// type RDStringExpression, simplifying code where string expressions are required. Passing a null string may result
		/// in unexpected behavior.</remarks>
		/// <param name="v">The string value to convert to an RDStringExpression. Cannot be null.</param>
		public static implicit operator RDStringExpression(string v) => new(new StringValue(v));
		/// <summary>
		/// Implicitly converts an enumeration value to an RDStringExpression instance using the enumeration's name as its
		/// string representation.
		/// </summary>
		/// <remarks>This conversion enables seamless use of enumeration values in contexts where an
		/// RDStringExpression is required. The resulting RDStringExpression contains the name of the enumeration value, not
		/// its numeric value.</remarks>
		/// <param name="v">The enumeration value to convert to an RDStringExpression.</param>
		public static implicit operator RDStringExpression(Enum v) => new(new StringValue(v.ToString()));
    /// <inheritdoc />
		public readonly string Serialize() => _v.Serialize();
	}
	/// <summary>
	/// Represents an expression that performs an operation without returning a value.
	/// </summary>
	/// <remarks>Use this struct to encapsulate operations that do not produce a result. It is typically used in
	/// scenarios where a void operation is required within an expression tree or similar construct.</remarks>
	public struct RDVoidExpression : IRDExpression
	{
		internal IOp _v;
		internal RDVoidExpression(FunctionVoidOp op) => _v = op;
    /// <inheritdoc />
		public readonly string Serialize() => _v.Serialize();
	}
/// <summary>
/// Provides static methods and properties for constructing numeric, boolean, and function-based expressions used in
/// the Rhythm Doctor expression system.
/// </summary>
/// <remarks>The RDExpressionBuilder class offers a set of factory methods and predefined expressions to
/// simplify the creation of complex logical and arithmetic expressions. It includes builders for numeric and boolean
/// expressions, as well as commonly used constant expressions. Methods are provided to construct function calls and
/// conditional expressions, enabling dynamic expression composition. This class is intended for scenarios where
/// expressions need to be generated or manipulated programmatically, such as in scripting or rule-based
/// systems.<br/>
	/// Write this at the top of the file to enable static access to the RDExpressionBuilder members without needing to qualify them with the class name:
/// <code>
/// using static RhythmBase.RhythmDoctor.Components.RDExpressionBuilder;
/// </code>
/// </remarks>
public static class RDExpressionBuilder
	{
#pragma warning disable IDE1006 // 命名样式
    /// <summary>
    /// Gets a new instance of the RDFloatExpressionBuilder for constructing numeric expressions.
    /// </summary>
    public static RDNumericExpression.RDFloatExpressionBuilder f => new();
    /// <summary>
    /// Gets a new instance of the RDIntegerExpressionBuilder for constructing numeric expressions.
    /// </summary>
    public static RDNumericExpression.RDIntegerExpressionBuilder i => new();
		/// <summary>
		/// Gets a new instance of the RDBooleanExpressionBuilder for constructing boolean expressions.
		/// </summary>
		public static RDBooleanExpression.RDBooleanExpressionBuilder b => new();
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f0</c>.
    /// </summary>
    public static RDNumericExpression f0 => new(new FloatVariable(0));
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f1</c>.
    /// </summary>
		public static RDNumericExpression f1 => new(new FloatVariable(1));
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f2</c>.
    /// </summary>
		public static RDNumericExpression f2 => new(new FloatVariable(2));
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f3</c>.
    /// </summary>
		public static RDNumericExpression f3 => new(new FloatVariable(3));
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f4</c>.
    /// </summary>
		public static RDNumericExpression f4 => new(new FloatVariable(4));
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f5</c>.
    /// </summary>
		public static RDNumericExpression f5 => new(new FloatVariable(5));
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f6</c>.
    /// </summary>
		public static RDNumericExpression f6 => new(new FloatVariable(6));
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f7</c>.
    /// </summary>
		public static RDNumericExpression f7 => new(new FloatVariable(7));
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f8</c>.
    /// </summary>
		public static RDNumericExpression f8 => new(new FloatVariable(8));
    /// <summary>
    /// Gets a new instance of the float variable indicates <c>f9</c>.
    /// </summary>
		public static RDNumericExpression f9 => new(new FloatVariable(9));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i0</c>.
    /// </summary>
		public static RDNumericExpression i0 => new(new IntVariable(0));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i1</c>.
    /// </summary>
		public static RDNumericExpression i1 => new(new IntVariable(1));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i2</c>.
    /// </summary>
		public static RDNumericExpression i2 => new(new IntVariable(2));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i3</c>.
    /// </summary>
		public static RDNumericExpression i3 => new(new IntVariable(3));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i4</c>.
    /// </summary>
		public static RDNumericExpression i4 => new(new IntVariable(4));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i5</c>.
    /// </summary>
		public static RDNumericExpression i5 => new(new IntVariable(5));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i6</c>.
    /// </summary>
		public static RDNumericExpression i6 => new(new IntVariable(6));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i7</c>.
    /// </summary>
		public static RDNumericExpression i7 => new(new IntVariable(7));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i8</c>.
    /// </summary>
		public static RDNumericExpression i8 => new(new IntVariable(8));
    /// <summary>
    /// Gets a new instance of the integer variable indicates <c>i9</c>.
    /// </summary>
		public static RDNumericExpression i9 => new(new IntVariable(9));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b0</c>.
    /// </summary>
		public static RDBooleanExpression b0 => new(new BooleanVariable(0));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b1</c>.
    /// </summary>
		public static RDBooleanExpression b1 => new(new BooleanVariable(1));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b2</c>.
    /// </summary>
		public static RDBooleanExpression b2 => new(new BooleanVariable(2));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b3</c>.
    /// </summary>
		public static RDBooleanExpression b3 => new(new BooleanVariable(3));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b4</c>.
    /// </summary>
		public static RDBooleanExpression b4 => new(new BooleanVariable(4));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b5</c>.
    /// </summary>
		public static RDBooleanExpression b5 => new(new BooleanVariable(5));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b6</c>.
    /// </summary>
		public static RDBooleanExpression b6 => new(new BooleanVariable(6));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b7</c>.
    /// </summary>
		public static RDBooleanExpression b7 => new(new BooleanVariable(7));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b8</c>.
    /// </summary>
		public static RDBooleanExpression b8 => new(new BooleanVariable(8));
    /// <summary>
    /// Gets a new instance of the boolean variable indicates <c>b9</c>.
    /// </summary>
		public static RDBooleanExpression b9 => new(new BooleanVariable(9));
		/// <summary>
		/// Gets a numeric expression representing the current beats per minute (BPM) value.
		/// </summary>
		public static RDNumericExpression bpm => new(new NamedNumericVariable("bpm"));
#pragma warning restore IDE1006 // 命名样式
		/// <summary>
		/// Creates a new void expression that invokes a function with the specified name and arguments.
		/// </summary>
		/// <remarks>Ensure that all arguments are of supported types. Passing an unsupported expression type will
		/// result in an exception.</remarks>
		/// <param name="name">The name of the function to invoke.</param>
		/// <param name="args">An array of expressions to pass as arguments to the function. Each argument must be a supported expression type:
		/// RDBooleanExpression, RDNumericExpression, or RDStringExpression.</param>
		/// <returns>A void expression representing the invocation of the specified function with the provided arguments.</returns>
		/// <exception cref="InvalidOperationException">Thrown if any argument is not a supported expression type.</exception>
    public static RDVoidExpression CallVoid(string name, params IRDExpression[] args) => new(new FunctionVoidOp(name, [.. args.Select(arg => arg switch {
			RDBooleanExpression exp => exp._v as IOp,
			RDNumericExpression exp => exp._v as IOp,
			RDStringExpression exp => exp._v as IOp,
			_=> throw new InvalidOperationException($"Unsupported expression type: {arg.GetType()} with value {arg}")
		})], false));
		/// <summary>
		/// Creates a boolean expression by invoking a specified boolean function with the provided arguments.
		/// </summary>
		/// <remarks>All arguments must be of supported types. Supplying an unsupported expression type will result in
		/// an exception at runtime.</remarks>
		/// <param name="name">The name of the boolean function to invoke.</param>
		/// <param name="args">An array of expressions to use as arguments for the boolean function. Each argument must be of type
		/// RDBooleanExpression, RDNumericExpression, or RDStringExpression.</param>
		/// <returns>A new RDBooleanExpression representing the result of the boolean function call.</returns>
		/// <exception cref="InvalidOperationException">Thrown if any argument is not of a supported expression type.</exception>
		public static RDBooleanExpression CallBool(string name, params IRDExpression[] args) => new(new FunctionBooleanOp(name, [.. args.Select(arg => arg switch {
			RDBooleanExpression exp => exp._v as IOp,
			RDNumericExpression exp => exp._v as IOp,
			RDStringExpression exp => exp._v as IOp,
			_=> throw new InvalidOperationException($"Unsupported expression type: {arg.GetType()} with value {arg}")
		})], false));
		/// <summary>
		/// Creates a new numeric expression by invoking a specified numeric function with the provided arguments.
		/// </summary>
		/// <remarks>Ensure that all arguments are of supported types (boolean, numeric, or string expressions) to
		/// avoid exceptions.</remarks>
		/// <param name="name">The name of the numeric function to invoke.</param>
		/// <param name="args">An array of expressions to use as arguments for the numeric function. Supported types include boolean, numeric,
		/// and string expressions.</param>
		/// <returns>A new instance of RDNumericExpression representing the result of the numeric function call.</returns>
		/// <exception cref="InvalidOperationException">Thrown if any element in the args array is not a supported expression type.</exception>
		public static RDNumericExpression CallNumeric(string name, params IRDExpression[] args) => new(new FunctionNumericOp(name, [.. args.Select(arg => arg switch {
			RDBooleanExpression exp => exp._v as IOp,
			RDNumericExpression exp => exp._v as IOp,
			RDStringExpression exp => exp._v as IOp,
			_=> throw new InvalidOperationException($"Unsupported expression type: {arg.GetType()} with value {arg}")
		})], false));
		/// <summary>
		/// Evaluates a specified condition and returns one of two numeric expressions depending on whether the condition
		/// evaluates to <see langword="true"/> or <see langword="false"/>.
		/// </summary>
		/// <remarks>Use this method to perform conditional logic within an expression, enabling concise selection
		/// between two numeric values based on a boolean condition.</remarks>
		/// <param name="condition">The condition to evaluate. If the condition is <see langword="true"/>, the method returns the value of <paramref
		/// name="trueExpr"/>; otherwise, it returns the value of <paramref name="falseExpr"/>.</param>
		/// <param name="trueExpr">The numeric expression to return if <paramref name="condition"/> evaluates to <see langword="true"/>.</param>
		/// <param name="falseExpr">The numeric expression to return if <paramref name="condition"/> evaluates to <see langword="false"/>.</param>
		/// <returns>A numeric expression that corresponds to the result of evaluating the specified condition.</returns>
		public static RDNumericExpression IIf(RDBooleanExpression condition, RDNumericExpression trueExpr, RDNumericExpression falseExpr) => CallNumeric("IIf", condition, trueExpr, falseExpr);
		//public static RDExpression Assignment()
		/// <summary>
		/// Generates a random numeric expression with a maximum value specified by the parameter.
		/// </summary>
		/// <remarks>This method utilizes a random number generation algorithm to produce a value that is uniformly
		/// distributed within the specified range.</remarks>
		/// <param name="max">The maximum value that the generated random number can take. Must be a positive numeric expression.</param>
		/// <returns>A numeric expression representing a randomly generated value between 0 and the specified maximum value.</returns>
		public static RDNumericExpression Rand(RDNumericExpression max) => CallNumeric("Rand", max);
#if DEBUG
		/// <summary>
		/// Generates a string representation of the specified IRDExpression, visualizing its structure and components.
		/// </summary>
		/// <remarks>This method is primarily used for debugging purposes to visualize the expression tree. It handles
		/// various types of operations and expressions, providing a clear output for each.</remarks>
		/// <param name="expression">The IRDExpression to be printed, which can be of various types including boolean, numeric, string, or void
		/// expressions.</param>
		/// <returns>A string that represents the hierarchical structure of the expression, formatted for readability.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the provided expression or operation type is unsupported or invalid.</exception>
		public static string PrintTree(IRDExpression expression)
		{
			static string ToString(IOp op)
			{
				static string CombineLines(params string[][] lines)
				{
					if (lines.Length == 0) return "";
					if (lines.Length == 1) return "│\n" + string.Join("\n", lines[0]);
					string header = "";
					string[] results = new string[lines.Max(i => i.Length)];
					for (int i = 0; i < lines.Length - 1; ++i)
					{
						int maxLen = lines[i].Max(x => x.Length);
						if (i == 0)
							header += "├";
						else
							header += "┬";
						header += new string('─', maxLen);
						for (int j = 0; j < lines[i].Length; ++j)
						{
							results[j] += lines[i][j] + new string(' ', maxLen - lines[i][j].Length + 1);
						}
						for (int j = lines[i].Length; j < results.Length; ++j)
						{
							results[j] += new string(' ', maxLen + 1);
						}
					}
					header += "┐";
					for (int i = 0; i < lines[lines.Length - 1].Length; ++i)
					{
						results[i] += lines[lines.Length - 1][i];
					}
					return header + "\n" + string.Join("\n", results);
				}
				if (op is IEndpointOp op1)
					return op1 switch
					{
						BooleanValue boolVal => boolVal.Value.ToString(),
						NumericValue floatVal => floatVal.Value.ToString(),
						StringValue strVal => strVal.Value.ToString(),
						BooleanVariable boolVar => $"b{boolVar.Index}",
						FloatVariable floatVar => $"f{floatVar.Index}",
						IntVariable intVar => $"i{intVar.Index}",
						NamedBooleanVariable namedBoolVar => namedBoolVar.ToString(),
						NamedNumericVariable namedNumVar => namedNumVar.ToString(),
						_ => throw new InvalidOperationException($"Unsupported IOp type: {op.GetType()} with value {op}")
					};
				else if (op is IUnaryOp op2)
				{
					string opStr = op2 switch
					{
						UnaryBooleanOp boolOp => boolOp.Op switch
						{
							UnaryOperator.Not => "Not",
							_ => throw new InvalidOperationException($"Unsupported UnaryOperator for IUnaryOp: {boolOp.Op}")
						},
						UnaryNumericOp numOp => numOp.Op switch
						{
							UnaryOperator.Negate => "-",
							UnaryOperator.Positive => "+",
							_ => throw new InvalidOperationException($"Unsupported UnaryOperator for IUnaryOp: {numOp.Op}")
						},
						_ => throw new InvalidOperationException($"Unsupported IUnaryOp type: {op.GetType()} with value {op}")
					};
					return $"""
						{opStr}
						│
						{op2 switch
					{
						UnaryBooleanOp boolOp => ToString(boolOp.Value),
						UnaryNumericOp numOp => ToString(numOp.Value),
						_ => throw new InvalidOperationException($"Unsupported IUnaryOp type: {op.GetType()} with value {op}")
					}}
						""";
				}
				else if (op is IBinaryOp op3)
				{
					string opStr = op3 switch
					{
						BinaryBooleanOp boolOp => boolOp.Op switch
						{
							BinaryOperator.Or => "Or",
							BinaryOperator.And => "And",
							BinaryOperator.Equal => "==",
							BinaryOperator.NotEqual => "<>",
							BinaryOperator.GreaterThan => ">",
							BinaryOperator.GreaterThanOrEqual => ">=",
							BinaryOperator.LessThan => "<",
							BinaryOperator.LessThanOrEqual => "<=",
							_ => throw new InvalidOperationException($"Unsupported BinaryOperator for IBinaryOp: {boolOp.Op}")
						},
						BinaryNumericOp numOp => numOp.Op switch
						{
							BinaryOperator.Add => "+",
							BinaryOperator.Subtract => "-",
							BinaryOperator.Multiply => "*",
							BinaryOperator.Divide => "/",
							BinaryOperator.Modulo => "%",
							_ => throw new InvalidOperationException($"Unsupported BinaryOperator for IBinaryOp: {numOp.Op}")
						},
						_ => throw new InvalidOperationException($"Unsupported IBinaryOp type: {op.GetType()} with value {op}")
					};
					string strL = op3 switch
					{
						BinaryBooleanOp boolOp => ToString(boolOp.Left),
						BinaryNumericOp numOp => ToString(numOp.Left),
						_ => throw new InvalidOperationException($"Unsupported IBinaryOp type: {op.GetType()} with value {op}")
					};
					string strR = op3 switch
					{
						BinaryBooleanOp boolOp => ToString(boolOp.Right),
						BinaryNumericOp numOp => ToString(numOp.Right),
						_ => throw new InvalidOperationException($"Unsupported IBinaryOp type: {op.GetType()} with value {op}")
					};
					string[] strLsp = strL.Split('\n');
					string[] strRsp = strR.Split('\n');
					return $"""
						{opStr}
						{CombineLines(strLsp, strRsp)}
						""";
				}
				else if (op is IFunctionOp funcOp)
				{
					return $"""
						{funcOp.Name}()
						{CombineLines([.. funcOp.Args.Select(arg => ToString(arg).Split('\n'))])}
						""";
				}
				else
				{
					throw new InvalidOperationException($"Unsupported IOp type: {op.GetType()} with value {op}");
				}
			}
			return ToString(expression switch
			{
				RDBooleanExpression boolExp => boolExp._v,
				RDNumericExpression numExp => numExp._v,
				RDStringExpression strExp => strExp._v,
				RDVoidExpression exp2 => exp2._v,
				_ => throw new InvalidOperationException($"Unsupported IRDExpression type: {expression.GetType()} with value {expression}")
			});
		}
#endif

	}