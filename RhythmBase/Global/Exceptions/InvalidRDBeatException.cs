namespace RhythmBase.Global.Exceptions;

	/// <summary>
	/// Exception thrown when an invalid RD beat is encountered.
	/// </summary>
	public class InvalidRDBeatException : RhythmBaseException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidRDBeatException"/> class.
		/// </summary>
		public InvalidRDBeatException()
		{
			Message = "The beat is invalid, possibly because the beat is not associated with the RDLevel.";
		}
		/// <summary>
		/// Gets the error message that explains the reason for the exception.
		/// </summary>
		public override string Message { get; }
	}
