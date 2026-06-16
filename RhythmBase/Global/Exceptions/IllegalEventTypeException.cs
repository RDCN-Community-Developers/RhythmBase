namespace RhythmBase.Global.Exceptions;

/// <summary>
/// Exception thrown when an illegal event type is encountered.
/// </summary>
public class IllegalEventTypeException : InvalidOperationException
{
	/// <summary>
	/// Gets the error message that explains the reason for the exception.
	/// </summary>
	public override string Message => $"Illegal type: \"{IllegalTypeName}\"{(string.IsNullOrEmpty(ExtraMessage) ? "." : $", {ExtraMessage}")}";
	/// <summary>
	/// Gets the extra message that provides additional information about the exception.
	/// </summary>
	public string ExtraMessage { get; }
	/// <summary>
	/// Gets the name of the illegal type that caused the exception.
	/// </summary>
	public string IllegalTypeName { get; }
	/// <summary>
	/// Initializes a new instance of the <see cref="IllegalEventTypeException"/> class with the specified type.
	/// </summary>
	/// <param name="type">The illegal type that caused the exception.</param>
	public IllegalEventTypeException(Type type) : this(type, string.Empty)
	{
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="IllegalEventTypeException"/> class with the specified type name.
	/// </summary>
	/// <param name="type">The name of the illegal type that caused the exception.</param>
	public IllegalEventTypeException(string type) : this(type, string.Empty)
	{
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="IllegalEventTypeException"/> class with the specified type and extra message.
	/// </summary>
	/// <param name="type">The illegal type that caused the exception.</param>
	/// <param name="extraMessage">The extra message that provides additional information about the exception.</param>
	public IllegalEventTypeException(Type type, string extraMessage)
	{
		IllegalTypeName = type.Name;
		ExtraMessage = extraMessage;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="IllegalEventTypeException"/> class with the specified type name and extra message.
	/// </summary>
	/// <param name="type">The name of the illegal type that caused the exception.</param>
	/// <param name="extraMessage">The extra message that provides additional information about the exception.</param>
	public IllegalEventTypeException(string type, string extraMessage)
	{
		IllegalTypeName = type;
		ExtraMessage = extraMessage;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="IllegalEventTypeException"/> class with the specified type, extra message, and inner exception.
	/// </summary>
	/// <param name="type">The illegal type that caused the exception.</param>
	/// <param name="extraMessage">The extra message that provides additional information about the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception.</param>
	public IllegalEventTypeException(Type type, string extraMessage, Exception innerException) : base("", innerException)
	{
		IllegalTypeName = type.Name;
		ExtraMessage = extraMessage;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="IllegalEventTypeException"/> class with the specified type name, extra message, and inner exception.
	/// </summary>
	/// <param name="type">The name of the illegal type that caused the exception.</param>
	/// <param name="extraMessage">The extra message that provides additional information about the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception.</param>
	public IllegalEventTypeException(string type, string extraMessage, Exception innerException) : base("", innerException)
	{
		IllegalTypeName = type;
		ExtraMessage = extraMessage;
	}
}
