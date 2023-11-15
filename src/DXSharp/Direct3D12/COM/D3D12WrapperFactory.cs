#region Using Directives
using System.Collections ;
using System.Collections.ObjectModel ;
using DXSharp ;
using DXSharp.Direct3D12 ;
using DXSharp.Direct3D12.Debugging ;
using DXSharp.Windows.COM ;

#endregion
namespace Windows.Win32.Graphics.Direct3D12 ;


internal static class D3D12WrapperFactory {
	
	// Video-related interfaces are not yet implemented and must remain commented out for the moment ...
	//{ typeof( ID3D12VideoDecoderHeap ), IVideoDecoderHeap._videoDecoderHeapCreationFunctions },
	//{ typeof( ID3D12VideoProcessor ), IVideoProcessor._videoProcessorCreationFunctions },
	//{ typeof( ID3D12VideoDecodeCommandList ), IVideoDecodeCommandList._videoDecodeCommandListCreationFunctions },
	//{ typeof( ID3D12VideoProcessCommandList ), IVideoProcessCommandList._videoProcessCommandListCreationFunctions },

	//! Lookup wrapper creation functions by native COM interface type:
	// The base interface type that is instantiated is the one with the creation functions for all derrived types
	// No need to put entire inheritance tree here, just the base interface type (e.g., just `IDevice` instead of `IDevice1`, `IDevice2`, etc.)
	internal static ReadOnlyDictionary< Type, IEnumerable > _creationFunctionEnumerablesMap =
		new( new Dictionary< Type, IEnumerable > {
			{ typeof( ID3D12Debug ), IDebug._layerCreationFunctions },
			{ typeof( ID3D12Debug1 ), IDebug._layerCreationFunctions },
			{ typeof( ID3D12Debug2 ), IDebug._layerCreationFunctions },
			//{ typeof(ID3D12DebugCommandList), IDebugCommandList._layerCreationFunctions },

			{ typeof( ID3D12Device ), IDevice._deviceCreationFunctions },
			{ typeof( ID3D12CommandList ), ICommandList._listCreationFunctions },
			{ typeof( ID3D12CommandAllocator ), ICommandAllocator._allocatorCreationFunctions },
			{ typeof( ID3D12CommandQueue ), ICommandQueue._queueCreationFunctions },
			{ typeof( ID3D12Fence ), IFence._fenceCreationFunctions },
			{ typeof( ID3D12DescriptorHeap ), IDescriptorHeap._descHeapCreationFunctions },
			{ typeof( ID3D12Heap ), IHeap._heapCreationFunctions },
			{ typeof( ID3D12Resource ), IResource._resourceCreationFunctions },
			{ typeof( ID3D12PipelineState ), IPipelineState._pipelineStateCreationFunctions },
			{ typeof( ID3D12RootSignature ), IRootSignature._rootSignatureCreationFunctions },
			{ typeof( ID3D12QueryHeap ), IQueryHeap._queryHeapCreationFunctions },
			{ typeof( ID3D12CommandSignature ), ICommandSignature._commandSignatureCreationFunctions },
			{ typeof( ID3D12PipelineLibrary ), IPipelineLibrary._pipelineLibraryCreationFunctions },


			//! These interfaces still need to be fully implemented:
			/*{ typeof( ID3D12LifetimeOwner ), ILifetimeOwner._lifetimeOwnerCreationFunctions },
			{ typeof( ID3D12MetaCommand ), IMetaCommand._metaCommandCreationFunctions },
			{ typeof( ID3D12ProtectedSession ), IProtectedSession._protectedSessionCreationFunctions },
			{ typeof( ID3D12ShaderCacheSession ), IShaderCacheSession._shaderCacheSessionCreationFunctions },
			{ typeof( ID3D12StateObject ), IStateObject._stateObjectCreationFunctions },
			{ typeof( ID3D12StateObjectProperties ), IStateObjectProperties._stateObjectPropertiesCreationFunctions },
			{ typeof( ID3D12VersionedRootSignatureDeserializer ), IVersionedRootSignatureDeserializer._versionedRootSignatureDeserializerCreationFunctions },
			{ typeof( ID3D12DeviceRemovedExtendedDataSettings ), IDeviceRemovedExtendedDataSettings._deviceRemovedExtendedDataSettingsCreationFunctions },
			{ typeof( ID3D12DeviceRemovedExtendedData ), IDeviceRemovedExtendedData._deviceRemovedExtendedDataCreationFunctions },
			{ typeof(ID3D12SDKConfiguration), ISDKConfiguration._sdkConfigurationCreationFunctions },
			{ typeof(ID3D12Tools), ITools._toolsCreationFunctions },*/
		} ) ;

	
	//! Lookup native interop RCW Interface by COM IID:
	internal static ReadOnlyDictionary< Guid, Type > _rcwTypesByCOMIID =
		new( new Dictionary< Guid, Type > {
			{ typeof(ID3D12Debug).GUID, typeof(ID3D12Debug) },
			{ typeof(ID3D12Debug1).GUID, typeof(ID3D12Debug1) },
			{ typeof(ID3D12Debug2).GUID, typeof(ID3D12Debug2) },
			//{ typeof(ID3D12DebugCommandList).GUID, typeof(ID3D12DebugCommandList) },

			{ typeof(ID3D12Device).GUID, typeof(ID3D12Device) },
			{ typeof(ID3D12CommandList).GUID, typeof(ID3D12CommandList) },
			{ typeof(ID3D12CommandAllocator).GUID, typeof(ID3D12CommandAllocator) },
			{ typeof(ID3D12CommandQueue).GUID, typeof(ID3D12CommandQueue) },
			{ typeof(ID3D12Fence).GUID, typeof(ID3D12Fence) },
			{ typeof(ID3D12DescriptorHeap).GUID, typeof(ID3D12DescriptorHeap) },
			{ typeof(ID3D12Heap).GUID, typeof(ID3D12Heap) },
			{ typeof(ID3D12Resource).GUID, typeof(ID3D12Resource) },
			{ typeof(ID3D12PipelineState).GUID, typeof(ID3D12PipelineState) },
			{ typeof(ID3D12RootSignature).GUID, typeof(ID3D12RootSignature) },
			{ typeof(ID3D12QueryHeap).GUID, typeof(ID3D12QueryHeap) },
			{ typeof(ID3D12CommandSignature).GUID, typeof(ID3D12CommandSignature) },
			{ typeof(ID3D12PipelineLibrary).GUID, typeof(ID3D12PipelineLibrary) },
			 
		} ) ;
	
	//! Lookup Proxy Interface by COM IID:
	internal static ReadOnlyDictionary< Guid, Type > _proxyTypesByCOMIID =
		new( new Dictionary< Guid, Type > {
			{ typeof(ID3D12Debug).GUID, typeof(IDebug) },
			{ typeof(ID3D12Debug1).GUID, typeof(IDebug) },
			{ typeof(ID3D12Debug2).GUID, typeof(IDebug) },
			//{ typeof(ID3D12DebugCommandList).GUID, typeof(IDebugCommandList) },

			{ typeof(ID3D12Device).GUID, typeof(IDevice) },
			{ typeof(ID3D12CommandList).GUID, typeof(ICommandList) },
			{ typeof(ID3D12CommandAllocator).GUID, typeof(ICommandAllocator) },
			{ typeof(ID3D12CommandQueue).GUID, typeof(ICommandQueue) },
			{ typeof(ID3D12Fence).GUID, typeof(IFence) },
			{ typeof(ID3D12DescriptorHeap).GUID, typeof(IDescriptorHeap) },
			{ typeof(ID3D12Heap).GUID, typeof(IHeap) },
			{ typeof(ID3D12Resource).GUID, typeof(IResource) },
			{ typeof(ID3D12PipelineState).GUID, typeof(IPipelineState) },
			{ typeof(ID3D12RootSignature).GUID, typeof(IRootSignature) },
			{ typeof(ID3D12QueryHeap).GUID, typeof(IQueryHeap) },
			{ typeof(ID3D12CommandSignature).GUID, typeof(ICommandSignature) },
			{ typeof(ID3D12PipelineLibrary).GUID, typeof(IPipelineLibrary) },
		} ) ;

	//! Lookup Proxy Interface by RCW Interface Type:
	internal static ReadOnlyDictionary< Type, Type > _proxyTypesByRCWTypes =
		new( new Dictionary< Type, Type > {
			{ typeof(IDebug), typeof(ID3D12Debug) },
			{ typeof(ICommandList), typeof(ID3D12CommandList) },
			{ typeof(ICommandAllocator), typeof(ID3D12CommandAllocator) },
			{ typeof(ICommandQueue), typeof(ID3D12CommandQueue) },
			{ typeof(IFence), typeof(ID3D12Fence) },
			{ typeof(IDescriptorHeap), typeof(ID3D12DescriptorHeap) },
			{ typeof(IHeap), typeof(ID3D12Heap) },
			{ typeof(IResource), typeof(ID3D12Resource) },
			{ typeof(IPipelineState), typeof(ID3D12PipelineState) },
			{ typeof(IRootSignature), typeof(ID3D12RootSignature) },
			{ typeof(IQueryHeap), typeof(ID3D12QueryHeap) },
			{ typeof(ICommandSignature), typeof(ID3D12CommandSignature) },
			{ typeof(IPipelineLibrary), typeof(ID3D12PipelineLibrary) },
		} ) ;
	
	
	//internal static bool _HasEntryForIID( Guid iid ) => _proxyTypesByCOMIID.ContainsKey(iid) ;
	
	internal static bool _HasEntryFor( Type comType ) =>
		_creationFunctionEnumerablesMap.Any( kvp => kvp.Key.IsAssignableFrom( comType ) ) ;

	internal static Type _GetWrapperTypeForIID( Guid iid ) => _proxyTypesByCOMIID[ iid ] ;
	
	internal static IEnumerable _getCreationFuncEnumerable( Type type ) => _creationFunctionEnumerablesMap[ type ] ;
	internal static IEnumerable _getCreationFuncEnumerable( Guid iid ) => _creationFunctionEnumerablesMap[ _proxyTypesByCOMIID[iid] ] ;
	internal static ReadOnlyDictionary< Guid, Func< TIn, IInstantiable > > GetFunctions< TIn >( IEnumerable dictionary )
		where TIn: IUnknown {
		var functionSet = dictionary as ReadOnlyDictionary< Guid, Func< TIn, IInstantiable > > 
						  ?? throw new ArgumentException( ) ;
		return functionSet ;
	}

	
	internal static Delegate GetFunction< TProxy >( Guid iid ) 
		where TProxy: IInstantiable, IComIID => GetFunction( TProxy.Guid ) ;
	
	internal static Delegate GetFunction( Guid iid ) {
		var functionSet = _getCreationFuncEnumerable( iid ) ;
		foreach ( var entry in functionSet ) {
			KeyValuePair<Guid, Delegate> kvp = (KeyValuePair<Guid, Delegate>)entry ;
			if ( kvp.Key == iid ) return kvp.Value ;
		}

		throw new ArgumentException( nameof( iid ) ) ;
	}
} ;




	/*
	public static bool _HasEntryForIID( Guid guid ) {
		foreach( var kvp in _creationFunctionEnumerablesMap ) {
			if( kvp.Key.GUID == guid ) {
				return true ;
			}
		}
		return false ;
	}
	*/
	