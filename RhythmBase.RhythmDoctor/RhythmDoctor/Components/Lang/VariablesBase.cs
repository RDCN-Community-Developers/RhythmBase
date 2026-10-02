using System;
using System.Collections.Generic;
using System.Dynamic;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// Base class for an <see cref="IVariables"/> environment that exposes its variables as dynamic
/// members.
/// </summary>
/// <remarks>
/// <para>
/// Deriving from this class gives an implementation both things at once: the evaluator can read
/// variables through <see cref="TryGetVariable"/>/<see cref="TrySetVariable"/>, and the type can
/// be used with <c>dynamic</c>, for example <c>((dynamic)env).score = 10</c>.
/// </para>
/// <para>
/// Return <see langword="false"/> from <see cref="TryGetVariable"/> for an unknown name, and
/// <see langword="false"/> from <see cref="TrySetVariable"/> for a read-only or unknown name.
/// </para>
/// </remarks>
public abstract class VariablesBase : DynamicObject, IVariables
{
	/// <summary>
	/// Tries to read the value of a variable.
	/// </summary>
	/// <param name="name">The variable name.</param>
	/// <param name="value">The value when the variable exists; otherwise <see langword="null"/>.</param>
	/// <returns><see langword="true"/> when the variable exists and is readable.</returns>
	public abstract bool TryGetVariable(string name, out object? value);

	/// <summary>
	/// Tries to assign a variable.
	/// </summary>
	/// <param name="name">The variable name.</param>
	/// <param name="value">The value to assign.</param>
	/// <returns>
	/// <see langword="true"/> when the variable was assigned; <see langword="false"/> when it is
	/// read-only or unknown.
	/// </returns>
	public abstract bool TrySetVariable(string name, object? value);

	/// <summary>
	/// Enumerates the names of every readable variable, for autocomplete and diagnostics.
	/// </summary>
	/// <returns>The variable names.</returns>
	public virtual IEnumerable<string> GetVariableNames() => Array.Empty<string>();

	/// <inheritdoc/>
	public override bool TryGetMember(GetMemberBinder binder, out object? result)
		=> TryGetVariable(binder.Name, out result);

	/// <inheritdoc/>
	public override bool TrySetMember(SetMemberBinder binder, object? value)
		=> TrySetVariable(binder.Name, value);

	/// <inheritdoc/>
	public override IEnumerable<string> GetDynamicMemberNames() => GetVariableNames();
}
