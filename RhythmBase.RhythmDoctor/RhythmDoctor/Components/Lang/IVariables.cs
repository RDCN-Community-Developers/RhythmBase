using System.Dynamic;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// The virtual environment used while evaluating expressions.
/// </summary>
/// <remarks>
/// <para>
/// The evaluator owns the 30 fixed slots (<c>i0</c>–<c>i9</c>, <c>f0</c>–<c>f9</c> and
/// <c>b0</c>–<c>b9</c>) itself. Every other identifier is exposed by this environment as a
/// dynamic member.
/// </para>
/// <para>
/// Because the environment is an <see cref="IDynamicMetaObjectProvider"/>, reading and writing
/// variables uses the dynamic member protocol: an implementation returns <see langword="false"/>
/// from <see cref="DynamicObject.TryGetMember"/> for an unknown name, and <see langword="false"/>
/// from <see cref="DynamicObject.TrySetMember"/> for a read-only or unknown name.
/// </para>
/// <para>
/// Implementations are expected to derive from <see cref="VariablesBase"/> (which derives from
/// <see cref="DynamicObject"/> and lets a type declare its variables by overriding
/// <see cref="VariablesBase.TryGetVariable"/> / <see cref="VariablesBase.TrySetVariable"/>).
/// </para>
/// </remarks>
public interface IVariables : IDynamicMetaObjectProvider
{
}
