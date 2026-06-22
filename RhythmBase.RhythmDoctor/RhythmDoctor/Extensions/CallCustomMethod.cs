using RhythmBase.Global.Components.Easing;
using System.Reflection;
using static RhythmBase.RhythmDoctor.Extensions.Extensions;

namespace RhythmBase.RhythmDoctor.Events;

public partial record class CallCustomMethod
{
	/// <summary>
	/// Contains a series of default custom method implementations.
	/// </summary>
	public static class Shared
	{
#pragma warning disable CS1591
		public static CallCustomMethod PropertyAssignment(string propertyName, bool value) => new() { MethodName = $"{propertyName.ToLowerCamelCase()} = {value}" };
		public static CallCustomMethod RoomPropertyAssignment(byte room, string propertyName, bool value) => new() { MethodName = $"room[{room}].{propertyName.ToLowerCamelCase()} = {value}" };
		public static CallCustomMethod FunctionCalling(string functionName, params object[] @params) => new() { MethodName = $"{functionName}({ArgumentCombining(@params)})" };
		private static string ArgumentCombining(params object[] @params) => $"{string.Join(", ", @params.Select(i =>
																								 i is string || i.GetType().IsEnum
																									 ? $"str:{i}"
																									 : i.ToString() ?? ""))}";
		public static CallCustomMethod RoomFunctionCalling(byte room, string functionName, params object[] @params) => new() { MethodName = $"room[{room}].{functionName}({ArgumentCombining(@params)})" };
		private static CallCustomMethod VfxFunctionCalling(string functionName, params object[] @params) => new() { MethodName = $"vfx.{functionName}({ArgumentCombining(@params)})" };
		public static CallCustomMethod BombBeats(bool value) => PropertyAssignment(nameof(BombBeats), value);
		public static CallCustomMethod SetScoreboardLights(bool mode, string text) => FunctionCalling(nameof(SetScoreboardLights), mode, text);
		public static CallCustomMethod InvisibleChars(bool value) => PropertyAssignment(nameof(InvisibleChars), value);
		public static CallCustomMethod InvisibleHeart(bool value) => PropertyAssignment(nameof(InvisibleHeart), value);
		public static CallCustomMethod NoHitFlashBorder(bool value) => PropertyAssignment(nameof(NoHitFlashBorder), value);
		public static CallCustomMethod NoHitStrips(bool value) => PropertyAssignment(nameof(NoHitStrips), value);
		public static CallCustomMethod SetOneshotType(int rowId, ShockWaveType wavetype) => FunctionCalling(nameof(SetOneshotType), rowId, wavetype);
		public static CallCustomMethod WobblyLines(bool value) => PropertyAssignment(nameof(WobblyLines), value);
		public static CallCustomMethod ShockwaveSizeMultiplier(bool value) => PropertyAssignment(nameof(ShockwaveSizeMultiplier), value);
		public static CallCustomMethod ShockwaveDistortionMultiplier(bool value) => PropertyAssignment(nameof(ShockwaveDistortionMultiplier), value);
		public static CallCustomMethod ShockwaveDurationMultiplier(bool value) => PropertyAssignment(nameof(ShockwaveDurationMultiplier), value);
		public static CallCustomMethod MistakeOrHeal(float damageOrHeal) => FunctionCalling(nameof(MistakeOrHeal), damageOrHeal);
		public static CallCustomMethod MistakeOrHealP1(float damageOrHeal) => FunctionCalling(nameof(MistakeOrHealP1), damageOrHeal);
		public static CallCustomMethod MistakeOrHealP2(float damageOrHeal) => FunctionCalling(nameof(MistakeOrHealP2), damageOrHeal);
		public static CallCustomMethod MistakeOrHealSilent(float damageOrHeal) => FunctionCalling(nameof(MistakeOrHealSilent), damageOrHeal);
		public static CallCustomMethod MistakeOrHealP1Silent(float damageOrHeal) => FunctionCalling(nameof(MistakeOrHealP1Silent), damageOrHeal);
		public static CallCustomMethod MistakeOrHealP2Silent(float damageOrHeal) => FunctionCalling(nameof(MistakeOrHealP2Silent), damageOrHeal);
		public static CallCustomMethod SetMistakeWeight(int rowId, float weight) => FunctionCalling(nameof(SetMistakeWeight), rowId, weight);
		public static CallCustomMethod DamageHeart(int rowId, float damage) => FunctionCalling(nameof(DamageHeart), rowId, damage);
		public static CallCustomMethod HealHeart(int rowId, float damage) => FunctionCalling(nameof(HealHeart), rowId, damage);
		public static CallCustomMethod WavyRowsAmplitude(byte roomId, float amplitude) => RoomPropertyAssignment(roomId, nameof(WavyRowsAmplitude), amplitude != 0f);
		public static CallCustomMethod WavyRowsFrequency(byte roomId, float frequency) => RoomPropertyAssignment(roomId, nameof(WavyRowsFrequency), frequency != 0f);
		public static CallCustomMethod SetShakeIntensityOnHit(byte roomId, int number, int strength) => RoomFunctionCalling(roomId, nameof(SetShakeIntensityOnHit), number, strength);
		public static CallCustomMethod ShowPlayerHand(byte roomId, bool isPlayer1, bool isShortArm, bool isInstant) => FunctionCalling(nameof(ShowPlayerHand), roomId, isPlayer1, isShortArm, isInstant);
		public static CallCustomMethod TintHandsWithInts(byte roomId, float r, float g, float b, float a) => FunctionCalling(nameof(TintHandsWithInts), roomId, r, g, b, a);
		public static CallCustomMethod SetHandsBorderColor(byte roomId, float r, float g, float b, float a, int style) => FunctionCalling(nameof(SetHandsBorderColor), roomId, r, g, b, a, style);
		public static CallCustomMethod SetAllHandsBorderColor(float r, float g, float b, float a, int style) => FunctionCalling(nameof(SetAllHandsBorderColor), r, g, b, a, style);
		public static CallCustomMethod SetHandToP1(int room, bool rightHand) => FunctionCalling(nameof(SetHandToP1), room, rightHand);
		public static CallCustomMethod SetHandToP2(int room, bool rightHand) => FunctionCalling(nameof(SetHandToP2), room, rightHand);
		public static CallCustomMethod SetHandToIan(int room, bool rightHand) => FunctionCalling(nameof(SetHandToIan), room, rightHand);
		public static CallCustomMethod SetHandToPaige(int room, bool rightHand) => FunctionCalling(nameof(SetHandToPaige), room, rightHand);
		public static CallCustomMethod SetShadowRow(int mimickerRowId, int mimickedRowId) => FunctionCalling(nameof(SetShadowRow), mimickerRowId, mimickedRowId);
		public static CallCustomMethod UnsetShadowRow(int mimickerRowId, int mimickedRowId) => FunctionCalling(nameof(UnsetShadowRow), mimickerRowId, mimickedRowId);
		public static CallCustomMethod ShakeCam(int number, int strength, int roomId) => VfxFunctionCalling(nameof(ShakeCam), number, strength, roomId);
		public static CallCustomMethod StopShakeCam(int roomId) => VfxFunctionCalling(nameof(StopShakeCam), roomId);
		public static CallCustomMethod ShakeCamSmooth(int duration, int strength, int roomId) => VfxFunctionCalling(nameof(ShakeCamSmooth), duration, strength, roomId);
		public static CallCustomMethod ShakeCamRotate(int duration, int strength, int roomId) => VfxFunctionCalling(nameof(ShakeCamRotate), duration, strength, roomId);
		public static CallCustomMethod SetKaleidoscopeColor(int roomId, float r1, float g1, float b1, float r2, float g2, float b2) => FunctionCalling(nameof(SetKaleidoscopeColor), roomId, r1, g1, b1, r2, g2, b2);
		public static CallCustomMethod SyncKaleidoscopes(int targetRoomId, int otherRoomId) => FunctionCalling(nameof(SyncKaleidoscopes), targetRoomId, otherRoomId);
		public static CallCustomMethod SetVignetteAlpha(float alpha, int roomId) => VfxFunctionCalling(nameof(SetVignetteAlpha), alpha, roomId);
		public static CallCustomMethod NoOneshotShadows(bool value) => PropertyAssignment(nameof(NoOneshotShadows), value);
		public static CallCustomMethod EnableRowReflections(int roomId) => FunctionCalling(nameof(EnableRowReflections), roomId);
		public static CallCustomMethod DisableRowReflections(int roomId) => FunctionCalling(nameof(DisableRowReflections), roomId);
		public static CallCustomMethod ChangeCharacter(string name, int roomId) => FunctionCalling(nameof(ChangeCharacter), name, roomId);
		public static CallCustomMethod ChangeCharacter(GameCharacter name, int roomId) => FunctionCalling(nameof(ChangeCharacter), name, roomId);
		public static CallCustomMethod ChangeCharacterSmooth(string name, int roomId) => FunctionCalling(nameof(ChangeCharacterSmooth), name, roomId);
		public static CallCustomMethod ChangeCharacterSmooth(GameCharacter name, int roomId) => FunctionCalling(nameof(ChangeCharacterSmooth), name, roomId);
		public static CallCustomMethod SmoothShake(bool value) => PropertyAssignment(nameof(SmoothShake), value);
		public static CallCustomMethod RotateShake(bool value) => PropertyAssignment(nameof(RotateShake), value);
		public static CallCustomMethod DisableRowChangeWarningFlashes(bool value) => PropertyAssignment(nameof(DisableRowChangeWarningFlashes), value);
		public static CallCustomMethod StatusSignWidth(float value) => PropertyAssignment(nameof(StatusSignWidth), value != 0f);
		public static CallCustomMethod SkippableRankScreen(bool value) => PropertyAssignment(nameof(SkippableRankScreen), value);
		public static CallCustomMethod MissesToCrackHeart(int value) => PropertyAssignment(nameof(MissesToCrackHeart), value != 0);
		public static CallCustomMethod SkipRankText(bool value) => PropertyAssignment(nameof(SkipRankText), value);
		public static CallCustomMethod AlternativeMatrix(bool value) => PropertyAssignment(nameof(AlternativeMatrix), value);
		public static CallCustomMethod ToggleSingleRowReflections(byte room, byte row, bool value) => FunctionCalling(nameof(ToggleSingleRowReflections), room, row, value);
		public static CallCustomMethod SetScrollSpeed(byte roomId, float speed, float duration, EaseType ease) => RoomFunctionCalling(roomId, nameof(SetScrollSpeed), speed, duration, ease);
		public static CallCustomMethod SetScrollOffset(byte roomId, float cameraOffset, float duration, EaseType ease) => RoomFunctionCalling(roomId, nameof(SetScrollOffset), cameraOffset, duration, ease);
		public static CallCustomMethod DarkenedRollerdisco(byte roomId, float value) => RoomFunctionCalling(roomId, nameof(DarkenedRollerdisco), value);
		public static CallCustomMethod CurrentSongVol(float targetVolume, float fadeTimeSeconds) => FunctionCalling(nameof(CurrentSongVol), targetVolume, fadeTimeSeconds);
		public static CallCustomMethod PreviousSongVol(float targetVolume, float fadeTimeSeconds) => FunctionCalling(nameof(PreviousSongVol), targetVolume, fadeTimeSeconds);
		public static CallCustomMethod EditTree(byte room, string property, float value, float beats, EaseType ease) => RoomFunctionCalling(room, nameof(EditTree), property, value, beats, ease);
		public static IEnumerable<CallCustomMethod> EditTree(byte room, ProceduralTree treeProperties, float beats, EaseType ease)
		{
			List<CallCustomMethod> l = [];
			foreach (FieldInfo p in typeof(ProceduralTree).GetFields())
			{
				float? value = (float?)p.GetValue(treeProperties);
				if (value != null)
					l.Add(EditTree(room, p.Name.ToLowerCamelCase(), (float)value, beats, ease));
			}
			return l;
		}
		public static CallCustomMethod EditTreeColor(byte room, bool location, string color, float beats, EaseType ease) => RoomFunctionCalling(room, nameof(EditTreeColor), location, color, beats, ease);
		public struct ProceduralTree
		{
			/// <summary>
			/// Enables or disables the procedural tree effect.
			/// </summary>
			public float?
				BrachedPerlteration,
				BranchesPerDivision,
				IterationsPerSecond,
				Thickness,
				TargetLength,
				MaxDeviation,
				Angle,
				CamAngle,
				CamDistance,
				CamDegreesPerSecond,
				CamSpeed,
				PulseIntensity,
				PulseRate,
				PulseWavelength;
		}
	}
}
