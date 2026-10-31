using System.Collections.Generic;
using RhythmBase.RhythmDoctor.Components.Lang;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Supplies the runtime conditions a sandbox usually would not model (platform, voice language,
/// button count, narration flags, passed levels, ...) and forwards everything else to the
/// underlying environment.
/// </summary>
internal sealed class RuntimeVariables : ExpressionContext
{
	private readonly ExpressionContext _inner;
	private readonly PlayheadConfig _config;

	internal RuntimeVariables(ExpressionContext inner, PlayheadConfig config)
	{
		_inner = inner;
		_config = config;
	}

	internal ExpressionContext Inner => _inner;

	/// <inheritdoc/>
	protected override bool TryGetMemberValue(string name, out object? value)
	{
		switch (name)
		{
			case "platform":
				value = _config.Platform ?? "Windows";
				return true;
			case "voiceLanguage":
				if (_config.Language is { } language)
				{
					value = language.ToString();
					return true;
				}
				break;
			case "twoPlayerMode":
				value = _config.TwoPlayerMode;
				return true;
			case "narration":
				value = _config.NarrationEnabled;
				return true;
			case "buttonPressCount":
				value = _config.ButtonPressCount;
				return true;
			case "windowDanceCanVerticalWrap":
				value = _config.WindowDanceCanVerticalWrap;
				return true;
			case "booth":
				value = _config.Booth;
				return true;
			case "paigeLeaves":
				value = _config.PaigeLeaves;
				return true;
		}
		return _inner.TryGetVariable(name, out value);
	}

	/// <inheritdoc/>
	protected override bool TrySetMemberValue(string name, object? value) => _inner.TrySetVariable(name, value);

	/// <inheritdoc/>
	public override bool TryInvokeFunction(string name, IReadOnlyList<object?> args, out object? result) => _inner.TryInvokeFunction(name, args, out result);

	/// <summary>Answers <c>passedLevel(Name)</c> from the configured set.</summary>
	public bool passedLevel(string level) => _config.PassedLevels is not null && _config.PassedLevels.Contains(level);
}
