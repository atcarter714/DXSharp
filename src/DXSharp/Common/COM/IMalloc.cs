#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981

#region Using Directives
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;

using DXSharp ;
#endregion
namespace Windows.Win32.System.Com ;


/// <summary>
/// An enumerated type used to indicate which <see cref="IMalloc"/> allocator
/// first allocated a block of memory.
/// </summary>
public enum AllocCreatorFlag: int {
	/// <summary>The block of memory was allocated by this allocator.</summary>
	ThisAllocator = 1,
	/// <summary>The block of memory was not allocated by this allocator.</summary>
	OtherAllocator = 2,
	/// <summary>This method cannot determine whether this allocator allocated the block of memory.</summary>
	Indeterminate = -1,
} ;



[SupportedOSPlatform("windows5.0")]
[ComImport, Guid("00000002-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ]
[NativeLibrary("ole32.dll", "IMalloc", "objidlbase.h")]
public interface IMalloc: DXSharp.Windows.COM.IUnknown {
	// -----------------------------------------------------------------
	
	/// <summary>Allocates a block of memory. (IMalloc.Alloc)</summary>
	/// <param name="cb">The size of the memory block to be allocated, in bytes.</param>
	/// <returns>
	/// <para>If the method succeeds, the return value is a pointer to the allocated block of memory.
	/// Otherwise, it is <b>NULL</b>. Applications should always check the return value from this method,
	/// even when requesting small amounts of memory, because there is no guarantee the memory will be
	/// allocated.</para>
	/// </returns>
	/// <remarks>
	/// <para>The initial contents of the returned memory block are undefined and there is no guarantee
	/// that the block has been initialized, so you should initialize it in your code. The allocated
	/// block may be larger than <i>cb</i> bytes because of the space required for alignment and for
	/// maintenance information. If <i>cb</i> is zero, <b>Alloc</b> allocates a zero-length item and
	/// returns a valid pointer to that item. If there is insufficient memory available, <b>Alloc</b>
	/// returns <b>NULL</b>.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/objidl/nf-objidl-imalloc-alloc#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] unsafe void* Alloc( nuint cb ) ;

	/// <summary>The IMalloc::Realloc (objidlbase.h) method changes the size of a previously allocated block of memory.</summary>
	/// <param name="pv">
	/// A pointer to the block of memory to be reallocated. This parameter can be
	/// <b>NULL</b>, as discussed in the Remarks section below.
	/// </param>
	/// <param name="cb">
	/// The size of the memory block to be reallocated, in bytes. This parameter can be 0,
	/// as discussed in the Remarks section below.
	/// </param>
	/// <returns>
	/// If the method succeeds, the return value is a pointer to the reallocated block of memory.
	/// Otherwise, it is <b>NULL</b>.
	/// </returns>
	/// <remarks>
	/// <para>This method reallocates a block of memory, but does not guarantee that its contents are
	/// initialized. Therefore, the caller is responsible for subsequently initializing the memory.
	/// The allocated block may be larger than <i>cb</i> bytes because of the space required for
	/// alignment and for maintenance information. The <i>pv</i> argument points to the beginning of
	/// the block. If <i>pv</i> is <b>NULL</b>, <b>Realloc</b> allocates a new memory block in the
	/// same way that
	/// <a href="https://docs.microsoft.com/windows/desktop/api/objidl/nf-objidl-imalloc-alloc">IMalloc::Alloc</a> does.
	/// If <i>pv</i> is not <b>NULL</b>, it should be a pointer returned by a prior call to <b>Alloc</b>.
	/// The <i>cb</i> argument specifies the size of the new block, in bytes. The contents of the block
	/// are unchanged up to the shorter of the new and old sizes, although the new block can be in a
	/// different location. Because the new block can be in a different memory location, the pointer
	/// returned by <b>Realloc</b> is not guaranteed to be the pointer passed through the <i>pv</i>
	/// argument. If <i>pv</i> is not <b>NULL</b> and <i>cb</i> is zero, the memory pointed to by
	/// <i>pv</i> is freed. <b>Realloc</b> returns a void pointer to the reallocated (and possibly moved)
	/// block of memory. The return value is <b>NULL</b> if the size is zero and the buffer argument is
	/// not <b>NULL</b>, or if there is not enough memory available to expand the block to the specified
	/// size. In the first case, the original block is freed; in the second, the original block is
	/// unchanged. The storage space pointed to by the return value is guaranteed to be suitably
	/// aligned for storage of any type of object. To get a pointer to a type other than <b>void</b>,
	/// use a type cast on the return value.</para>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/objidlbase/nf-objidlbase-imalloc-realloc#">Read more on docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] unsafe void* Realloc( [Optional] void* pv, nuint cb ) ;

	/// <summary>
	/// The IMalloc::Free (objidlbase.h) method frees a previously allocated block of memory.
	/// </summary>
	/// <param name="pv">
	/// A pointer to the memory block to be freed.
	/// If this parameter is <b>NULL</b>, this method has no effect.
	/// </param>
	/// <remarks>
	/// This method frees a block of memory previously allocated through a call to
	/// <a href="https://docs.microsoft.com/windows/desktop/api/objidl/nf-objidl-imalloc-alloc">IMalloc::Alloc</a>
	/// or <a href="https://docs.microsoft.com/windows/desktop/api/objidl/nf-objidl-imalloc-realloc">IMalloc::Realloc</a>.
	/// The number of bytes freed equals the number of bytes that were allocated. After the call, the block of memory pointed
	/// to by <i>pv</i> is invalid and can no longer be used.
	/// </remarks>
	[PreserveSig] unsafe void Free( [Optional] void* pv ) ;

	/// <summary>
	/// The IMalloc::GetSize (objidlbase.h) method retrieves the size of a previously allocated block of memory.
	/// </summary>
	/// <param name="pv">A pointer to the block of memory.</param>
	/// <returns>The size of the allocated memory block in bytes or, if <i>pv</i> is a <b>NULL</b> pointer, -1.</returns>
	/// <remarks>
	/// To get the size in bytes of a memory block, the block must have been previously allocated with
	/// <a href="https://docs.microsoft.com/windows/desktop/api/objidl/nf-objidl-imalloc-alloc">IMalloc::Alloc</a>
	/// or <a href="https://docs.microsoft.com/windows/desktop/api/objidl/nf-objidl-imalloc-realloc">IMalloc::Realloc</a>.
	/// The size returned is the actual size of the allocation, which may be greater than the size requested when the
	/// allocation was made.
	/// </remarks>
	[PreserveSig] unsafe nuint GetSize( [Optional] void* pv ) ;

	/// <summary>
	/// The IMalloc::DidAlloc (objidlbase.h) method determines whether this allocator was used to allocate the specified block of memory.
	/// </summary>
	/// <param name="pv">A pointer to the block of memory. If this parameter is a <b>NULL</b> pointer, -1 is returned.</param>
	/// <returns>
	/// Return values can be:<para/>
	/// <c>1</c>: The specified memory block was allocated by this allocator.<para/>
	/// <c>0</c>: The specified memory block was not allocated by this allocator.<para/>
	/// <c>-1</c>: The allocator does not know whether it allocated the memory block. This value is returned when <i>pv</i> is <b>NULL</b>.
	/// </returns>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/objidlbase/nf-objidlbase-imalloc-didalloc">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] unsafe int DidAlloc( [Optional] void* pv ) ;

	/// <summary>
	/// The IMalloc::HeapMinimize (objidlbase.h) method minimizes the heap by releasing unused memory to the operating system.
	/// </summary>
	/// <remarks>
	/// <para><a href="https://docs.microsoft.com/windows/win32/api/objidlbase/nf-objidlbase-imalloc-heapminimize">Learn more about this API from docs.microsoft.com</a>.</para>
	/// </remarks>
	[PreserveSig] void HeapMinimize( ) ;
	
	// =================================================================
} ;
