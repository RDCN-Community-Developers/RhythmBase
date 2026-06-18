using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0130
#pragma warning disable CS1591
#if !NET10_0_OR_GREATER
namespace System.Text.Json
{
	internal static class JsonElementExtensions
	{
		extension(JsonElement)
		{
			public static JsonElement Parse(string json)
			{
				using var doc = JsonDocument.Parse(json);
				return doc.RootElement.Clone();
			}
			public static JsonElement Parse(ReadOnlySpan<byte> json)
			{
				using var doc = JsonDocument.Parse(new ReadOnlyMemory<byte>([..json]));
				return doc.RootElement.Clone();
			}
		}
	}
}
#endif
#if NETSTANDARD2_0
namespace System
{
	internal static class EnumExtensions
	{
		extension(Enum)
		{
			public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct, Enum
			{
				return Enum.TryParse(value, ignoreCase, out result);
			}
			public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum
			{
				return (TEnum[])Enum.GetValues(typeof(TEnum));
			}
		}
	}
	internal static class ExceptionExtensions
	{
		extension(ArgumentNullException)
		{
			public static void ThrowIfNull(object? value, string paramName)
			{
				if (value is null)
					throw new ArgumentNullException(paramName);
			}
		}
		extension(ArgumentOutOfRangeException)
		{
			public static void ThrowIfNullOrEmpty(string? value, string paramName)
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentOutOfRangeException(paramName, "Value cannot be null or empty.");
			}
			public static void ThrowIfNullOrWhiteSpace(string? value, string paramName)
			{
				if (string.IsNullOrWhiteSpace(value))
					throw new ArgumentOutOfRangeException(paramName, "Value cannot be null or whitespace.");
			}
			public static void ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
			{
				if (value.CompareTo(other) < 0)
					throw new ArgumentOutOfRangeException(paramName, $"Value must be greater than or equal to {other}.");
			}

		}
		extension(ObjectDisposedException)
		{
			public static void ThrowIf([DoesNotReturnIf(true)] bool condition, object instance)
			{
				if (condition)
					throw new ObjectDisposedException(instance.GetType().FullName);
			}
		}
	}
	internal readonly struct Index(int value, bool fromEnd = false) : IEquatable<Index>
	{
		public static Index End { get; } = new Index(0, false);
		public static Index Start { get; } = new Index(0, true);
		public bool IsFromEnd { get; } = fromEnd;
		public int Value { get; } = value;
		public static Index FromEnd(int value) => new(value, true);
		public static Index FromStart(int value) => new(value, false);
		public bool Equals(Index other) => Value == other.Value && IsFromEnd == other.IsFromEnd;
		public override bool Equals([NotNullWhen(true)] object? value) => value is Index other && Equals(other);
		public override int GetHashCode() => Value.GetHashCode() ^ IsFromEnd.GetHashCode();
		public int GetOffset(int length) => IsFromEnd ? length - Value : Value;
		public override string ToString() => IsFromEnd ? "^" + Value.ToString() : Value.ToString();
		public static implicit operator Index(int value) => new(value);
	}
	internal readonly struct Range(Index start, Index end) : IEquatable<Range>
	{
		public static Range All { get; }
		public Index End { get; } = end;
		public Index Start { get; } = start;
		public static Range EndAt(Index end) => new(new Index(0, false), end);
		public static Range StartAt(Index start) => new(start, new Index(0, true));
		public override bool Equals([NotNullWhen(true)] object? value) => value is Range other && Equals(other);
		public bool Equals(Range other) => Start.Equals(other.Start) && End.Equals(other.End);
		public override int GetHashCode() => Start.GetHashCode() ^ End.GetHashCode();
		public (int Offset, int Length) GetOffsetAndLength(int length)
		{
			int startOffset = Start.GetOffset(length);
			int endOffset = End.GetOffset(length);
			return startOffset > endOffset
					? throw new ArgumentOutOfRangeException(nameof(length), "Start index must be less than or equal to end index.")
					: ((int Offset, int Length))(startOffset, endOffset - startOffset);
		}
		public override string ToString() => $"[{Start}..{End}]";
	}
	internal static class Extensions
	{
		extension(Array)
		{
			public static void Fill<T>(T[] array, T value)
			{
				for (int i = 0; i < array.Length; i++)
					array[i] = value;
			}
		}
	}
	internal static class IntExtensions
	{
		extension(int)
		{
			public static int Parse(ReadOnlySpan<byte> value)
			{
				string str = Text.Encoding.UTF8.GetString(value.ToArray());
				return int.Parse(str);
			}
			public static int Abs(int value) => Math.Abs(value);
			public static int Max(int val1, int val2) => Math.Max(val1, val2);
			public static int Min(int val1, int val2) => Math.Min(val1, val2);
			public static (int div, int rem) DivRem(int value, int divisor)
			{
				int div = value / divisor;
				int rem = value % divisor;
				return (div, rem);
			}
		}
	}
	internal static class FloatExtensions
	{
		extension(float)
		{
			public static float Parse(ReadOnlySpan<byte> value)
			{
				string str = Text.Encoding.UTF8.GetString(value.ToArray());
				return float.Parse(str);
			}
			public static float Floor(float value) => (float)Math.Floor(value);
			public static float Ceiling(float value) => (float)Math.Ceiling(value);
			public static float Abs(float value) => Math.Abs(value);
			public static float Round(float value) => (float)Math.Round(value);
			public static float Pi => (float)Math.PI;
			public static float Max(float val1, float val2) => Math.Max(val1, val2);
			public static float Min(float val1, float val2) => Math.Min(val1, val2);
			public static float Ieee754Remainder(float dividend, float divisor) => (float)Math.IEEERemainder(dividend, divisor);
		}
	}
	internal static class StringExtensions
	{
		extension(string e)
		{
			public static string Join(char separator, params object[] values) => string.Join(separator.ToString(), values);
			public static string Join(char separator, IEnumerable<string> values) => string.Join(separator.ToString(), values);
			public string[] Split(char separator, int count)
			{
				string[] result = new string[count];
				int startIndex = 0;
				for (int i = 0; i < count - 1; i++)
				{
					int endIndex = e.IndexOf(separator, startIndex);
					if (endIndex == -1)
					{
						Array.Resize(ref result, i + 1);
						result[i] = e[startIndex..];
						return result;
					}
					result[i] = e[startIndex..endIndex];
					startIndex = endIndex + 1;
				}
				result[count - 1] = e[startIndex..];
				return result;
			}
			public bool StartsWith(char value) => e.StartsWith(value.ToString());
		}
	}
	internal static class ByteExtensions
	{
		extension(byte)
		{
			public static (byte div, byte rem) DivRem(byte value, byte divisor)
			{
				byte div = (byte)(value / divisor);
				byte rem = (byte)(value % divisor);
				return (div, rem);
			}
		}
	}
}
namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Struct, Inherited = false)]
	public sealed class StackTraceHiddenAttribute : Attribute { }
	namespace CodeAnalysis
	{
		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal class NotNullWhenAttribute(bool returnValue) : Attribute { }
		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class NotNullAttribute : Attribute { }
		[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
		internal sealed class DoesNotReturnIfAttribute(bool parameterValue) : Attribute { }
		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class AllowNullAttribute : Attribute
		{ }
		[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class MemberNotNullAttribute : Attribute
		{
			public MemberNotNullAttribute(string member) => Members = [member];
			public MemberNotNullAttribute(params string[] members) => Members = members;
			public string[] Members { get; }
		}
		[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class MemberNotNullWhenAttribute : Attribute
		{
			public MemberNotNullWhenAttribute(bool returnValue, string member)
			{
				ReturnValue = returnValue;
				Members = [member];
			}
			public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
			{
				ReturnValue = returnValue;
				Members = members;
			}
			public bool ReturnValue { get; }
			public string[] Members { get; }
		}
		[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class MaybeNullWhenAttribute(bool returnValue) : Attribute { }
		[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute { }

		[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
		internal sealed class SetsRequiredMembersAttribute : Attribute { }
	}
}
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class RequiredMemberAttribute : Attribute { }
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
	{
		public string FeatureName { get; } = featureName;
		public bool IsOptional { get; init; }
		public const string RefStructs = nameof(RefStructs);
		public const string RequiredMembers = nameof(RequiredMembers);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static class IsExternalInit
	{
	}
	internal static class RuntimeHelpers
	{
		extension<T>(T[] array)
		{
			public T[] GetSubArray(Range range)
			{
				if (array == null)
					throw new ArgumentNullException(nameof(array));
				var (offset, length) = range.GetOffsetAndLength(array.Length);
				if (length == 0)
					return [];
				if (offset == 0 && length == array.Length)
					return array;
				var dest = new T[length];
				Array.Copy(array, offset, dest, 0, length);
				return dest;
			}
		}
		[Intrinsic]
		public static void InitializeArray(Array array, RuntimeFieldHandle fldHandle)
		{
			typeof(object).Assembly.GetType("System.Runtime.CompilerServices.RuntimeHelpers")
					?.GetMethod("InitializeArray")
					?.Invoke(null, [array, fldHandle]);
		}
		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field, Inherited = false)]
		internal sealed class IntrinsicAttribute : Attribute
		{
		}
	}
	internal sealed class CollectionBuilderAttribute(Type builderType, string methodName) : Attribute
	{
		public Type BuilderType { get; } = builderType;
		public string MethodName { get; } = methodName;
	}
}
namespace System.Text
{
	internal static class EncodingExtensions
	{
		public static string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes) => encoding.GetString(bytes.ToArray());
	}
}
namespace System.IO
{
	internal static class PathExtensions
	{
		extension(Path)
		{
			public static string GetFullPath(string path, string basePath)
			{
				return Path.IsPathRooted(path)
						? path
						: Path.GetFullPath(Path.Combine(Path.GetDirectoryName(basePath) ?? "", path));
			}
		}
	}
}
namespace System.Collections.Generic
{
	internal static class StackExtensions
	{
		public static bool TryPeek<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T result)
		{
			if (stack.Count > 0)
			{
				result = stack.Peek();
				return true;
			}

			result = default;
			return false;
		}

		public static bool TryPop<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T result)
		{
			if (stack.Count > 0)
			{
				result = stack.Pop();
				return true;
			}

			result = default;
			return false;
		}
	}
}
#endif
#pragma warning restore CS1591