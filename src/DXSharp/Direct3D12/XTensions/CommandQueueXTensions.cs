namespace DXSharp.Direct3D12.XTensions ;

public static class CommandQueueXTensions
{
	static ICommandList[ ] _tempCommandLists = new ICommandList[ 1 ] ;

	public static void ExecuteCommandLists< C >( this ICommandQueue queue, C commandList ) 
																		where C: class, ICommandList {
		_tempCommandLists[ 0 ] = commandList ;
		var _tempSpan = _tempCommandLists.AsSpan( ) ;
		queue.ExecuteCommandLists( 1, _tempSpan ) ;
	}
}