﻿using System.Diagnostics ;
using Windows.Win32.Foundation ;
using Windows.Win32.Graphics.Dxgi ;
using DXSharp ;
using DXSharp.DXGI ;
using DXSharp.Windows.COM ;

namespace BasicTests ;


[TestFixture, FixtureLifeCycle( LifeCycle.SingleInstance )]
public class DXGI_Interface_Tests {
	IFactory7? factory7 ;
	
	[SetUp] public void SetUp( ) { }
	[TearDown] public void Cleanup( ) => factory7?.Dispose( ) ;


	[Test, Order( 0 )]
	public void Test_Create_Factory7( ) {
		FactoryCreateFlags flags = FactoryCreateFlags.Debug ;
		var hr = DXGIFunctions.CreateFactory2( IFactory7.IID, out var _f2, flags ) ;
		Assert.True( hr.Succeeded ) ;
		Assert.IsNotNull( _f2 ) ;
		factory7 = _f2 as IFactory7 ;
		Assert.IsNotNull( factory7 ) ;

		var rc = factory7 as IComObjectRef< IDXGIFactory7 > ;
		Assert.IsNotNull( rc ) ;
		var pFactory = rc!.ComObject ;
		Assert.IsNotNull( pFactory ) ;

		uint c1 = factory7!.AddRef( ) ;
		uint c2 = factory7!.Release( ) ;
		Assert.That( c2 == ( c1 - 1 ) ) ;
		
		// Assert the debug flag is set:
		var creationFlags = (FactoryCreateFlags)factory7!.GetCreationFlags( ) ;
		Assert.True( creationFlags.HasFlag(FactoryCreateFlags.Debug) ) ;
	}
	
	[Test, Order( 1 )]
	public void Test_SetData( ) {
		const string name = "DXSharp Test Factory" ;
		uint size = 0 ;
		
		unsafe {
			// Create a native string pointer:
			PCWSTR data = name ;
			Assert.IsTrue( data.Value is not null ) ;
			nint   ptr  = (nint)data.Value ;
			Assert.IsTrue( ptr is not 0x00000000 ) ;
			
			// Get the magic GUID stolen from C++ header files:
			var guid = COMUtility.WKPDID_D3DDebugObjectName ;
			
			var hr = factory7!.SetPrivateData( guid, size, ptr ) ;
			Assert.True( hr.Succeeded ) ;
			
			// Get the data back:
			var dst = stackalloc char[ name.Length ] ;
			
			hr = factory7!.GetPrivateData( guid, ref size, (nint)dst ) ;
			Assert.True( hr.Succeeded ) ;
			
			// Create a string from the data:
			string? result = new( dst, 0, (int)size ) ;
			
			// Log the result:
			Debug.WriteLine( $"IDXGIFactory7::GetPrivateData() - \t{result}" ) ;
		}
	}
	
	[Test, Order( 2 )]
	public void Test_EnumAdapters( ) {
		const uint MAX_OUTPUTS = 10 ;
		ObjectDisposedException.ThrowIf( factory7 is null, typeof(IFactory7) ) ;
		unsafe {
			IAdapter4? bestAdapter = _getBestGPU( factory7 ) ;
			Assert.IsNotNull( bestAdapter ) ;
			ObjectDisposedException.ThrowIf( bestAdapter is null, typeof(IAdapter4) ) ;
			
			bestAdapter!.GetDesc3( out var desc) ;
			Debug.WriteLine( $"Best Adapter: {desc.Description}" ) ;
			
			// Get the adapter's outputs:
			for ( uint i = 0 ; i < MAX_OUTPUTS ; ++i ) {
				uint index = 0 ;
				var hr = bestAdapter.EnumOutputs( index, out IOutput? output ) ;
				if( hr == HResult.DXGI_ERROR_NOT_FOUND || output is null ) break ;
				Assert.IsTrue( hr.Succeeded ) ;
				hr.ThrowOnFailure( ) ;
				
				// Downcast the output to IOutput6 with COMUtility:
				hr = output.QueryInterface< IOutput6 >( out var ppOutput ) ;
				Assert.IsTrue( hr.Succeeded ) ;
				output.Dispose( ) ;
				
				Assert.IsNotNull( ppOutput ) ;
				
				// Get the output's description:
				ppOutput!.GetDescription( out var outputDescription ) ;
				Debug.WriteLine( $"{nameof(Test_EnumAdapters)}: " +
								 $"\tGraphics Output: {outputDescription.DeviceName}" ) ;
				
				// Get the output's display modes:
				uint numModes = 0 ;
				const EnumModesFlags flags = EnumModesFlags.None;
				ppOutput.GetDisplayModeList1( Format.R8G8B8A8_UNORM,
											 flags, out numModes,
											 out var modes ) ;

				Assert.That(numModes is not 0);
			}

		}
	}
	
	
	
	static IAdapter4? _getBestGPU( IFactory7 factory ) {
		const uint MAX_ADAPTERS = 10 ;
		IAdapter4? bestAdapter = null ;
		ulong      maxVRAM     = 0 ;
			
		for( uint i = 0; i < MAX_ADAPTERS ; ++i ) {
			IAdapter1? _adapter = default ;
			var        hr       = factory.EnumAdapters1( i, out _adapter ) ;
				
			//! Is this the last adapter?
			if( hr == HResult.DXGI_ERROR_NOT_FOUND || _adapter is null ) break ;
			else hr.ThrowOnFailure( ) ;
				
			var adapter = COMUtility.Cast< IAdapter1, IAdapter4 >( _adapter ) ;
			Assert.That(adapter is not null);
			adapter!.GetDesc3( out var _desc ) ;
				
			// Skip software and remote adapters:
			if( _desc.Flags.HasFlag( AdapterFlag3.Software ) 
				|| _desc.Flags.HasFlag( AdapterFlag3.Remote ) ) continue ;
				
			ulong vram = (ulong)_desc.DedicatedVideoMemory ;
			if( vram > maxVRAM ) {
				maxVRAM     = vram ;
				bestAdapter = (IAdapter4)adapter ;
			}
		}
		return bestAdapter ;
	}

}