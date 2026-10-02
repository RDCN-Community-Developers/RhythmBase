using System;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// A Rhythm Doctor rank. It is the parameter type of <see cref="IVariables.atLeastRank"/>.
/// </summary>
/// <remarks>
/// The value is a message-free port of the rank used by the level model, kept in the language
/// layer so an environment does not have to depend on the game types.
/// </remarks>
[Flags]
public enum Rank
{
	/// <summary>S rank.</summary>
	S = 1 << 2,
	/// <summary>A rank.</summary>
	A = 2 << 2,
	/// <summary>B rank.</summary>
	B = 3 << 2,
	/// <summary>C rank.</summary>
	C = 4 << 2,
	/// <summary>D rank.</summary>
	D = 5 << 2,
	/// <summary>F rank.</summary>
	F = 6 << 2,

	/// <summary>The plus modifier.</summary>
	Plus = 0b01,
	/// <summary>The minus modifier.</summary>
	Minus = 0b10,

	/// <summary>S+ rank.</summary>
	SPlus = S | Plus,
	/// <summary>S- rank.</summary>
	SMinus = S | Minus,
	/// <summary>A+ rank.</summary>
	APlus = A | Plus,
	/// <summary>A- rank.</summary>
	AMinus = A | Minus,
	/// <summary>B+ rank.</summary>
	BPlus = B | Plus,
	/// <summary>B- rank.</summary>
	BMinus = B | Minus,
	/// <summary>C+ rank.</summary>
	CPlus = C | Plus,
	/// <summary>C- rank.</summary>
	CMinus = C | Minus,
	/// <summary>D+ rank.</summary>
	DPlus = D | Plus,
	/// <summary>D- rank.</summary>
	DMinus = D | Minus,
	/// <summary>F+ rank.</summary>
	FPlus = F | Plus,
	/// <summary>F- rank.</summary>
	FMinus = F | Minus,
}
