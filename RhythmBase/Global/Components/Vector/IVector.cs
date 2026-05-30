using System.Numerics;

namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// Represents a contract for a vortex in the RD system.
	/// </summary>
	/// <remarks>This interface serves as a marker or base for defining vortex-related functionality within the RD
	/// system. Implementing types should provide specific behaviors or properties relevant to the vortex
	/// concept.</remarks>
	public interface IVector
	{
}
	/// <summary>
	/// Represents a vortex interface that supports various mathematical operations.
	/// </summary>
	/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
	/// <typeparam name="TRight">The type of the right operand in addition and subtraction operations.</typeparam>
	/// <typeparam name="TValue">The type of the value in multiplication and division operations.</typeparam>
	public interface IVector<TSelf, TRight, TValue> :
		IVector,
		IEquatable<TSelf>
#if NET7_0_OR_GREATER
		,
		IAdditionOperators<TSelf, TRight, TSelf>,
		ISubtractionOperators<TSelf, TRight, TSelf>,
		IMultiplyOperators<TSelf, TValue, TSelf>,
		IDivisionOperators<TSelf, TValue, TSelf>,
		IEqualityOperators<TSelf, TSelf, bool>
#endif
		where TSelf :
			IEquatable<TSelf>
#if NET7_0_OR_GREATER
		,
			IAdditionOperators<TSelf, TRight, TSelf>,
			ISubtractionOperators<TSelf, TRight, TSelf>,
			IMultiplyOperators<TSelf, TValue, TSelf>,
			IDivisionOperators<TSelf, TValue, TSelf>,
			IEqualityOperators<TSelf, TSelf, bool>
#endif
	{
	}
