namespace Windows.Win32 ;

/// <summary>Contract for COM interface with an IID.</summary>
public interface IComIID {
	
	/// <summary>The IID guid for the underlying COM interface.</summary>
	/// <remarks>
	/// The <see cref="Guid"/> reference that is returned comes from a permanent memory address,
	/// and is therefore safe to convert to a pointer and pass around or hold long-term.
	/// </remarks>
	public static abstract ref readonly Guid Guid { get ; }
} ;