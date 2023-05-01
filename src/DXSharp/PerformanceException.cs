#region Using Directives
#endregion

namespace DXSharp;

/// <summary>
/// Type of exception thrown when something happens which can be
/// deterimental to performance.
/// </summary>
/// <remarks>
/// <b>NOTE:</b> Generally, PerformanceExceptions will only ever be thrown by
/// the Debug assembly of DXSharp except in some rare situations where something 
/// may halt the program indefinitely or cause other serious problems.
/// </remarks>
public class PerformanceException: Exception
{
	/// <summary>
	/// Creates a new PerformanceException with the specified exception message
	/// </summary>
	/// <param name="message">Exception message</param>
	public PerformanceException( string? message ) : base( message ) { }

	/// <summary>
	/// Creates a new PerformanceException with the specified exception message
	/// and inner exception info
	/// </summary>
	/// <param name="message">Exception message</param>
	/// <param name="innerException">Inner exception</param>
	public PerformanceException( string? message, Exception innerException ) : base( message, innerException ) { }
}