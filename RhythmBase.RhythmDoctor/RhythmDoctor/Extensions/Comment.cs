using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using static RhythmBase.RhythmDoctor.Extensions.Extensions;

namespace RhythmBase.RhythmDoctor.Events;

public partial record class Comment
{
	/// <summary>
	/// Contains a series of default custom method implementations.
	/// </summary>
	public static class Shared
	{
#pragma warning disable CS1591
		private static Comment FunctionCalling(string name, params object[] @params) => new() { Text = $"()=>{name.ToLowerCamelCase()}({string.Join(',', @params.Select(i => i.ToString()))})" };
		public static Comment TrueCameraMove(int roomId, PointN p, float animationDuration, EaseType ease) => FunctionCalling("TrueCameraMove", (byte)roomId, p.X, p.Y, animationDuration, ease);
		public static Comment Create(Particle particleName, PointN p) => FunctionCalling("Create", $"CustomParticles/{particleName}", p.X, p.Y);
		public static Comment Shockwave(ShockWaveType type, float value) => FunctionCalling("Shockwave", type, value);
		public static Comment WavyRowsAmplitude(byte roomId, float amplitude, float duration) => FunctionCalling("WavyRowsAmplitude", roomId, amplitude, duration);
	}
}