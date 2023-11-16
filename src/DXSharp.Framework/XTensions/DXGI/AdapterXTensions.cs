using System.Diagnostics.CodeAnalysis ;
using DXSharp.DXGI ;
using DXSharp.Windows ;

namespace DXSharp.Framework.XTensions.DXGI ;

public static class AdapterXTensions {
	public const uint MAX_OUTPUTS = 0x10 ;
	
	public static List< O > GetAllOutputs< O >( this IAdapter adapter ) where O: IOutput {
		List< O > outputs = new ( 2 ) ;
		for ( uint ordinal = 0; ordinal < MAX_OUTPUTS; ++ordinal ) {
			var hr = adapter.EnumOutputs( ordinal, out var output ) ;
			
			if ( hr.Failed || output is null ) {
				if ( hr == HResult.DXGI_ERROR_NOT_FOUND ) {
					return outputs ; //! No more outputs
				}
				break ;
			}
			
			// Is this a plain output?
			if( typeof(O) == typeof(IOutput) ) {
				outputs.Add( (O)output ) ;
				continue ;
			}

			// Query for the requested output type:
			var hr2 = output.QueryInterface< O >( out var _outputX ) ;
			if( _outputX is not null ) {
				outputs.Add( _outputX ) ;
				
				// Release the reference to the plain output:
				_outputX.Release( ) ;
				_outputX.Dispose( ) ;
				continue ;
			}
			
			// Release the reference to base output:
			output.Release( ) ;
			output.Dispose( ) ;
			throw new DirectXComError( $"Failed to query output interface: " +
									   $"\"{typeof(O).Name}\" (GUID: {O.Guid})." ) ;
		}
		
		return outputs ;
	}
}