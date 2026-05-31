using System.Text;
using System.Text.Json;

namespace RhythmBase.Global.Converters;

public class NoIndentScope : IDisposable
{
	private readonly Utf8JsonWriter writer;
	private readonly MemoryStream stream;
	private Utf8JsonWriter inlineWriter;
	private MemoryStream inlineStream;
	private readonly MetadataJsonSerializerOptions options;
	public NoIndentScope(System.Text.Encodings.Web.JavaScriptEncoder? encoder, MetadataJsonSerializerOptions options)
	{
		stream = new MemoryStream();
		writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false, Encoder = encoder });
		this.options = options;
	}
	public void WriteNoIndentObjectTo<T>(Utf8JsonWriter instance, T value, Action<Utf8JsonWriter, T, MetadataJsonSerializerOptions> writeValue)
	{
		stream.SetLength(0);
		writeValue(writer, value, options);
		writer.Flush();
		ReadOnlySpan<byte> buffer = stream.GetBuffer().AsSpan(0, (int)stream.Position);
		instance.WriteRawValue(buffer);
		writer.Reset();
	}
	public void WriteNoIndentArrayTo<T>(MetadataJsonSerializerOptions writeSettings, Utf8JsonWriter instance, IEnumerable<T> values, Action<Utf8JsonWriter, T, MetadataJsonSerializerOptions> writeValue)
		=> WriteNoIndentArrayTo<T>(writeSettings.WriteIndented, writeSettings.WriteAligned, instance, values, writeValue);
	public void WriteNoIndentArrayTo<T>(bool writeIndented, bool writeAligned, Utf8JsonWriter instance, IEnumerable<T> values, Action<Utf8JsonWriter, T, MetadataJsonSerializerOptions> writeValue)
	{
		if (!writeIndented)
		{
			foreach(var item in values)
				writeValue(instance, item, options with { WriteIndented = false });
			return;
		}
		stream.SetLength(0);
		byte[] indentBytes = Encoding.UTF8.GetBytes(Environment.NewLine + new string(' ', instance.CurrentDepth * options.JsonSerializerOptions.IndentSize));
		if (indentBytes.Length > 0)
			stream.Write(indentBytes, 0, indentBytes.Length);
		int indentedPosition = (int)stream.Position;
		ReadOnlySpan<byte> buffer;
		static int stringLengthOfNumber(string rawText)
		{
			int dotIndex = rawText.IndexOf('.');
			ushort integerPartLength = (ushort)(dotIndex == -1 ? rawText.Length : dotIndex);
			ushort decimalPartLength = (ushort)(dotIndex == -1 ? 0 : rawText.Length - dotIndex - 1);
			bool hasDot = dotIndex != -1;
			return (integerPartLength << 16) | decimalPartLength | (1 << 31) | (hasDot ? 1 : 0) << 15;
		}
		static int maxLengthOfAlignedNumber(int length1, int length2)
		{
			int integerPartLength1 = length1 & 0x7FFF0000;
			int decimalPartLength1 = length1 & 0x7FFF;
			int hasDot1 = length1 & 0x8000;
			int integerPartLength2 = length2 & 0x7FFF0000;
			int decimalPartLength2 = length2 & 0x7FFF;
			int hasDot2 = length2 & 0x8000;
			return (1 << 31)
					| int.Max(integerPartLength1, integerPartLength2)
					| int.Max(decimalPartLength1, decimalPartLength2)
					| hasDot1
					| hasDot2;
		}
		static int lengthOfNumber(int lengthCombined)
		{
			int integerPartLength = lengthCombined >> 16 & 0x7FFF;
			int decimalPartLength = lengthCombined & 0x7FFF;
			bool hasDot = (lengthCombined & 0x8000) != 0;
			return integerPartLength + decimalPartLength + (hasDot ? 1 : 0);
		}
		static int widthOfString(string text)
		{
			static bool charIsFullWidth(char c)
			{
				return (c >= 0x1100 && c <= 0x115F) ||  // Hangul Jamo
							 (c >= 0x2300 && c <= 0x23FF) ||  // Miscellaneous Technical (APL symbols etc.)
							 (c >= 0x2500 && c <= 0x259F) ||  // Box Drawing + Block Elements
							 (c >= 0x2E80 && c <= 0xA4CF) ||  // CJK Radicals..Yi Radicals
							 (c >= 0xAC00 && c <= 0xD7A3) ||  // Hangul Syllables
							 (c >= 0xF900 && c <= 0xFAFF) ||  // CJK Compatibility Ideographs
							 (c >= 0xFE10 && c <= 0xFE19) ||  // Vertical Forms
							 (c >= 0xFE30 && c <= 0xFE6F) ||  // CJK Compatibility Forms
							 (c >= 0xFF00 && c <= 0xFF60) ||  // Fullwidth Forms
							 (c >= 0xFFE0 && c <= 0xFFE6);    // Fullwidth Currency Symbols
			}
			return text.Sum(i => charIsFullWidth(i) ? 2 : 1);
		}
		if (writeAligned)
		{
			inlineStream ??= new MemoryStream();
			inlineWriter ??= new Utf8JsonWriter(inlineStream, new JsonWriterOptions { Indented = false, Encoder = options.JsonSerializerOptions.Encoder });
			Dictionary<string, int> propertyMaxLengths = [];
			Dictionary<string, string>[] propertyValues = new Dictionary<string, string>[values.Count()];
			JsonElement[] elements = new JsonElement[values.Count()];
			for (int i = 0; i < elements.Length; i++)
			{
				inlineStream.SetLength(0);
				writeValue(inlineWriter, values.ElementAt(i), options);
				inlineWriter.Flush();
				buffer = inlineStream.GetBuffer().AsSpan(0, (int)inlineStream.Position);
				Utf8JsonReader tmpReader = new Utf8JsonReader(buffer);
				elements[i] = JsonElement.ParseValue(ref tmpReader);
				inlineWriter.Reset();
				foreach (JsonProperty property in elements[i].EnumerateObject())
				{
					string rawText = property.Value.GetRawText();
					bool previousHasValue = propertyMaxLengths.TryGetValue(property.Name, out int maxLength);
					bool previousIsNumber = (maxLength & (1 << 31)) != 0;
					bool currentIsNumber = property.Value.ValueKind == JsonValueKind.Number;
					if ((!previousHasValue && currentIsNumber) || (previousHasValue && previousIsNumber && currentIsNumber))
					{
						propertyMaxLengths[property.Name] = maxLengthOfAlignedNumber(stringLengthOfNumber(rawText), maxLength);
						propertyValues[i] ??= [];
					}
					else
					{
						if (previousHasValue && previousIsNumber)
							maxLength = lengthOfNumber(maxLength);
						propertyMaxLengths[property.Name] = int.Max(widthOfString(rawText), maxLength);
						propertyValues[i] ??= [];
					}
					propertyValues[i][property.Name] = rawText;
				}
			}
			StringBuilder sb = new();
			for (int i = 0; i < elements.Length; i++)
			{
				writer.Reset();
				stream.SetLength(indentedPosition);
				sb.Append('{');
				int spaceLeft = 0;
				bool isFirst = true;
				foreach (KeyValuePair<string, int> property in propertyMaxLengths)
				{
					if (propertyValues[i].TryGetValue(property.Key, out string? rawValue))
					{
						if (!isFirst)
							sb.Append(',');
						isFirst = false;
						int rawValueLength;
						string rawText;
						int propertyMaxLength = propertyMaxLengths[property.Key];
						if ((propertyMaxLength & (1 << 31)) != 0)
						{
							rawValueLength = rawValue.Length;
							int integerPartLength = propertyMaxLength >> 16 & 0x7FFF;
							int decimalPartLength = propertyMaxLength & 0x7FFF;
							bool hasDot = (propertyMaxLength & 0x8000) != 0;
							int rawTextDotIndex = rawValue.IndexOf('.');
							if (rawTextDotIndex == -1) rawTextDotIndex = rawValueLength;
							int integerPartSpaceLength = integerPartLength - rawTextDotIndex;
							int decimalPartSpaceLength = decimalPartLength - (rawValueLength - rawTextDotIndex - (hasDot ? 1 : 0));
							rawText = $"{new string(' ', spaceLeft)}\"{property.Key}\":{new string(' ', integerPartSpaceLength)}{rawValue}";
							spaceLeft = decimalPartSpaceLength;
						}
						else
						{
							rawValueLength = widthOfString(rawValue);
							rawText = $"{new string(' ', spaceLeft)}\"{property.Key}\":{rawValue}";
							spaceLeft = property.Value - rawValueLength;
						}
						sb.Append(rawText);
					}
					else
					{
						int length = property.Value;
						spaceLeft += property.Key.Length + 4;
						if ((length & (1 << 31)) == 0)
						{
							spaceLeft += length;
						}
						else
						{
							spaceLeft += lengthOfNumber(length);
						}
					}
				}
				sb.Append('}');
				writer.WriteRawValue(sb.ToString());
				sb.Clear();
				writer.Flush();
				buffer = stream.GetBuffer().AsSpan(0, (int)stream.Position);
				instance.WriteRawValue(buffer);
			}
		}
		else
		{
			foreach (var value in values)
			{
				writer.Reset();
				stream.SetLength(indentedPosition);
				writeValue(writer, value, options);
				writer.Flush();
				buffer = stream.GetBuffer().AsSpan(0, (int)stream.Position);
				instance.WriteRawValue(buffer);
			}
		}
	}
	public void Dispose()
	{
		writer.Dispose();
		stream.Dispose();
	}
}