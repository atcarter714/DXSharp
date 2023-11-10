using System.Runtime.InteropServices ;
namespace DXSharp.Utilities ;

//! TODO: Investigate the package that purportedly assembles x64 on the fly ...
public static class ASM {
	public static byte[ ] Assemble( string assemblyCode ) {
		throw new NotImplementedException( ) ;
	}
} ;


public static class AssemblyExecutor {
	
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	delegate void AssemblyCodeDelegate( ) ;
	
	public static void Execute( Span< byte > assemblyCode ) {
#if DEBUG || DEV_BUILD
		if ( assemblyCode is not { Length: > 0 } )
			throw new ArgumentException( $"{nameof(AssemblyExecutor)} :: " +
										 $"Input buffer is empty!", nameof(assemblyCode) ) ;
#endif
		
		nint codePointer = default ;
		try {
			// Allocate memory for the assembly code
			codePointer = VirtualAlloc( default,
										(nuint)assemblyCode.Length,
										( AllocationType.Commit | AllocationType.Reserve ),
										MemoryProtection.ExecuteReadWrite
									  ) ;
			
			// Copy the assembly code to the allocated memory
			unsafe {
				Span< byte > dst = new( (void *)codePointer, assemblyCode.Length ) ;
				assemblyCode.CopyTo( dst ) ;
			}
			
			// Get a delegate to the assembly code
			AssemblyCodeDelegate asmDelegate = 
				Marshal.GetDelegateForFunctionPointer< AssemblyCodeDelegate >( codePointer ) ;

			// Invoke the assembly code
			asmDelegate( ) ;
		}
		finally {
			// Free the allocated memory
			if ( codePointer != nint.Zero ) {
				VirtualFree( codePointer, 0, FreeType.Release ) ;
			}
		}
	}

	
	// ----------------------------------------------------------------------------------
	// P/Invoke Signatures ::
	// ----------------------------------------------------------------------------------
	[DllImport( "kernel32.dll", SetLastError = true )]
	static extern nint VirtualAlloc( nint lpAddress, nuint dwSize, 
											 AllocationType flAllocationType,
													MemoryProtection flProtect ) ;

	[DllImport( "kernel32" )]
	static extern bool VirtualFree( nint lpAddress, uint dwSize,
									[MarshalAs(UnmanagedType.U4)] FreeType dwFreeType ) ;
	// ----------------------------------------------------------------------------------
	[Flags] enum AllocationType: uint {
		Commit  = 0x1000,
		Reserve = 0x2000,
	}
	[Flags] enum MemoryProtection: uint {
		ExecuteReadWrite = 0x40,
	}
	[Flags] enum FreeType: uint {
		/// <summary>
		/// Decommits the specified region of committed pages. After the operation, the pages are in the reserved state.
		/// The function does not fail if you attempt to decommit an uncommitted page. This means that you can decommit
		/// a range of pages without first determining the current commitment state.
		/// </summary>
		/// <remarks>
		/// The <b><c>MEM_DECOMMIT</c></b> value is not supported when the <i><c>lpAddress</c></i> parameter provides
		/// the base address for an enclave. This is true for enclaves that do not support dynamic memory management
		/// (i.e. SGX1). SGX2 enclaves permit <b><c>MEM_DECOMMIT</c></b> anywhere in the enclave.
		/// </remarks>
		Decommit = 0x4000,
		/// <summary>
		/// <para>Releases the specified region of pages, or placeholder (for a placeholder, the address space is released and
		/// available for other allocations). After this operation, the pages are in the free state.</para>
		/// <para>If you specify this value, dwSize must be <c>0</c> (zero), and <i><c>lpAddress</c></i> must point to the base address
		/// returned by the <see cref="AssemblyExecutor.VirtualAlloc"/> function when the region is reserved.
		/// The function fails if either of these conditions is not met.</para>
		/// </summary>
		/// <remarks>
		/// <para>If any pages in the region are committed currently, the function first decommits, and then releases them.</para>
		/// <para>The function does not fail if you attempt to release pages that are in different states, some reserved
		/// and some committed. This means that you can release a range of pages without first determining the current
		/// commitment state.</para>
		/// </remarks>
		Release  = 0x8000,
		/// <summary>
		/// To coalesce two adjacent placeholders, specify <b><c>MEM_RELEASE | MEM_COALESCE_PLACEHOLDERS</c></b>.
		/// When you coalesce placeholders, <i><c>lpAddress</c></i> and <i><c>dwSize</c></i> must exactly match the overall range of the
		/// placeholders to be merged. 
		/// </summary>
		CoalescePlaceholders = 0x00000001,
		/// <summary>
		/// Frees an allocation back to a placeholder (after you've replaced a placeholder with a private allocation
		/// using <b><c>VirtualAlloc2</c></b> or <b><c>Virtual2AllocFromApp</c></b>). 
		/// </summary>
		/// <remarks>
		/// To split a placeholder into two placeholders, specify <b><c>MEM_RELEASE | MEM_PRESERVE_PLACEHOLDER</c></b>.
		/// </remarks>
		PreservePlaceholder  = 0x00000002,
	}
	// ==================================================================================
} ;
