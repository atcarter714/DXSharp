#region Using Directives
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Windows.COM ;
#endregion
namespace DXSharp.Direct3D12 ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList))]
public class GraphicsCommandList: CommandList,
								  IGraphicsCommandList {
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList ) ;
	public new static Guid InterfaceGUID => typeof(ID3D12GraphicsCommandList).GUID ;
	public new ComPtr< ID3D12GraphicsCommandList >? ComPointer { get ; protected set ; }

	public new ID3D12GraphicsCommandList? COMObject => ComPointer?.Interface ;
	
	//! This makes the COMObject ref not null when interpreted as an ICommandList:
	ID3D12CommandList? ICommandList.COMObject => ComPointer?.Interface ;

	internal GraphicsCommandList( ) { }
	internal GraphicsCommandList( nint ptr ) => ComPointer = new(ptr) ;
	internal GraphicsCommandList( ComPtr< ID3D12GraphicsCommandList > comPointer ) => ComPointer = comPointer ;
	internal GraphicsCommandList( ID3D12GraphicsCommandList obj ) => ComPointer = new(obj) ;

	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = InterfaceGUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}


} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList1 ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList1))]
public class GraphicsCommandList1: GraphicsCommandList,
								   IGraphicsCommandList1 {
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList1 ) ;
	public new static Guid InterfaceGUID => typeof(ID3D12GraphicsCommandList1).GUID ;
	public new ComPtr< ID3D12GraphicsCommandList1 >? ComPointer { get ; protected set ; }

	public new ID3D12GraphicsCommandList1? COMObject => ComPointer?.Interface ;
	
	//! This makes the COMObject ref not null when interpreted as an ICommandList:
	ID3D12CommandList? ICommandList.COMObject => ComPointer?.Interface ;

	internal GraphicsCommandList1( ) { }
	internal GraphicsCommandList1( nint ptr ) => ComPointer = new(ptr) ;
	internal GraphicsCommandList1( ComPtr< ID3D12GraphicsCommandList1 > comPointer ) => ComPointer = comPointer ;
	internal GraphicsCommandList1( ID3D12GraphicsCommandList1 obj ) => ComPointer = new(obj) ;

	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = InterfaceGUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}


} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList2 ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList2))]
public class GraphicsCommandList2: GraphicsCommandList1,
								   IGraphicsCommandList2 {
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList2 ) ;
	public new static Guid InterfaceGUID => typeof(ID3D12GraphicsCommandList2).GUID ;
	public new ComPtr< ID3D12GraphicsCommandList2 >? ComPointer { get ; protected set ; }

	public new ID3D12GraphicsCommandList2? COMObject => ComPointer?.Interface ;
	
	//! This makes the COMObject ref not null when interpreted as an ICommandList:
	ID3D12CommandList? ICommandList.COMObject => ComPointer?.Interface ;

	internal GraphicsCommandList2( ) { }
	internal GraphicsCommandList2( nint ptr ) => ComPointer = new(ptr) ;
	internal GraphicsCommandList2( ComPtr< ID3D12GraphicsCommandList2 > comPointer ) => ComPointer = comPointer ;
	internal GraphicsCommandList2( ID3D12GraphicsCommandList2 obj ) => ComPointer = new(obj) ;

	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = InterfaceGUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

} ;


// ------------------------------------------------------------------------------------------
// ID3D12GraphicsCommandList3 ::
// ------------------------------------------------------------------------------------------

[Wrapper(typeof(ID3D12GraphicsCommandList3))]
public class GraphicsCommandList3: GraphicsCommandList2,
								   IGraphicsCommandList3 {
	
	public new static Type ComType => typeof(ID3D12GraphicsCommandList3 ) ;
	public new static Guid InterfaceGUID => typeof(ID3D12GraphicsCommandList3).GUID ;
	public new ComPtr< ID3D12GraphicsCommandList3 >? ComPointer { get ; protected set ; }

	public new ID3D12GraphicsCommandList3? COMObject => ComPointer?.Interface ;

	//! This makes the COMObject ref not null when interpreted as an ICommandList:
	ID3D12CommandList? ICommandList.COMObject => ComPointer?.Interface ;

	internal GraphicsCommandList3( ) { }
	internal GraphicsCommandList3( nint ptr ) => ComPointer = new(ptr) ;
	internal GraphicsCommandList3( ComPtr< ID3D12GraphicsCommandList3 > comPointer ) => ComPointer = comPointer ;
	internal GraphicsCommandList3( ID3D12GraphicsCommandList3 obj ) => ComPointer = new(obj) ;

	
	public new static ref readonly Guid Guid {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get {
			ReadOnlySpan< byte > data = InterfaceGUID.ToByteArray( ) ;
			return ref Unsafe.As< byte, Guid >( ref MemoryMarshal
													.GetReference(data) ) ;
		}
	}

} ;