namespace NTH.Utilities;

public class NTHException : Exception
{
    public NTHException() : base() { }
    public NTHException(string? message) : base(message) { }
    public NTHException(string? message, Exception? innerException) : base(message, innerException) { }
}

