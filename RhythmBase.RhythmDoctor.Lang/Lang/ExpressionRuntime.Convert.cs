using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RhythmBase.RhythmDoctor.Components.Lang;

internal sealed partial class ExpressionRuntime
{

	/// <summary>Converts a runtime value to a float.</summary>
	internal static float ToFloat(object? value)
	{
		switch (value)
		{
			case float f:
				return f;
			case int i:
				return i;
			case bool b:
				return b ? 1f : 0f;
			case string s:
				return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed) ? parsed : 0f;
			default:
				return 0f;
		}
	}

	/// <summary>Converts a runtime value to an integer.</summary>
	internal static int ToInt(object? value)
	{
		switch (value)
		{
			case int i:
				return i;
			case float f:
				return (int)Math.Round(f);
			case bool b:
				return b ? 1 : 0;
			case string s:
				return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) ? parsed : 0;
			default:
				return 0;
		}
	}

	/// <summary>Converts a runtime value to a boolean.</summary>
	internal static bool ToBool(object? value)
	{
		switch (value)
		{
			case bool b:
				return b;
			case int i:
				return i != 0;
			case float f:
				return f != 0f;
			case string s:
				return string.Equals(s, "true", StringComparison.OrdinalIgnoreCase);
			default:
				return false;
		}
	}

	/// <summary>Converts a runtime value to an <see cref="ExpressionContext.RDRank"/>.</summary>
	internal static ExpressionContext.RDRank ToRank(object? value)
	{
		switch (ToStr(value))
		{
			case "S+":
				return ExpressionContext.RDRank.SPlus;
			case "S":
				return ExpressionContext.RDRank.S;
			case "A":
				return ExpressionContext.RDRank.A;
			case "B":
				return ExpressionContext.RDRank.B;
			case "C":
				return ExpressionContext.RDRank.C;
			case "D":
				return ExpressionContext.RDRank.D;
			case "F":
				return ExpressionContext.RDRank.F;
			default:
				throw new FormatException($"Invalid rank '{ToStr(value)}'.");
		}
	}

	/// <summary>Converts a runtime value to a string.</summary>
	internal static string ToStr(object? value)
	{
		switch (value)
		{
			case null:
				return "";
			case string s:
				return s;
			case float f:
				return f.ToString(CultureInfo.InvariantCulture);
			case int i:
				return i.ToString(CultureInfo.InvariantCulture);
			case bool b:
				return b ? "true" : "false";
			default:
				return value.ToString() ?? "";
		}
	}
}
