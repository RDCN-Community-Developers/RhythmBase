using RhythmBase.RhythmDoctor.Components.Lang;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Supplies the runtime conditions a sandbox usually would not model (platform, voice language,
/// button count, narration flags, passed levels, ...) and forwards everything else to the
/// underlying environment.
/// </summary>
internal sealed class RuntimeVariables : VariablesBase
{
	private readonly VariablesBase _inner;
	private readonly PlayheadConfig _config;

	internal RuntimeVariables(VariablesBase inner, PlayheadConfig config)
	{
		_inner = inner;
		_config = config;
	}

	internal VariablesBase Inner => _inner;

	public override bool TryGetVariable(string name, out object? value)
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

	public override bool TrySetVariable(string name, object? value) => _inner.TrySetVariable(name, value);

	public override int Rand(int value) => _inner.Rand(value);

	public override bool atLeastRank(Rank rank) => _inner.atLeastRank(rank);

	public override bool atLeastNPerfects(int hitsToCheck, int numberOfPerfects) => _inner.atLeastNPerfects(hitsToCheck, numberOfPerfects);

	/// <summary>Answers <c>passedLevel(Name)</c> from the configured set.</summary>
	public bool passedLevel(string level) => _config.PassedLevels is not null && _config.PassedLevels.Contains(level);
}
