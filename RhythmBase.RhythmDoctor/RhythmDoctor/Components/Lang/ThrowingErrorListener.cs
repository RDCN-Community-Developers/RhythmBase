using System;
using System.IO;
using Antlr4.Runtime;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// Turns syntax errors reported by ANTLR into <see cref="FormatException"/>s. It serves both
/// parsers (<see cref="IAntlrErrorListener{T}"/> of <see cref="IToken"/>) and lexers
/// (<see cref="IAntlrErrorListener{T}"/> of <see cref="int"/>).
/// </summary>
internal sealed class ThrowingErrorListener : BaseErrorListener, IAntlrErrorListener<int>
{
	/// <summary>The shared instance.</summary>
	public static readonly ThrowingErrorListener Instance = new ThrowingErrorListener();

	/// <inheritdoc/>
	public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
	{
		throw new FormatException($"Syntax error at line {line}:{charPositionInLine}: {msg}");
	}

	/// <inheritdoc/>
	public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
	{
		throw new FormatException($"Syntax error at line {line}:{charPositionInLine}: {msg}");
	}
}
