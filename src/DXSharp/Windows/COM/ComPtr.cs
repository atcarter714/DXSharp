// COPYRIGHT NOTICES:
// --------------------------------------------------------------------------------
// Copyright © 2022 DXSharp - ATC - Aaron T. Carter
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------


#region Using Directives
using System.Runtime.InteropServices;

/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.Direct3D12;
After:
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;
*/
#endregion

namespace DXSharp.Windows.COM ;


public class ComPtr: IDisposable
{
	/// <summary>The GUID of the IUnknown COM interface</summary>
	public static readonly Guid GUID_IUNKNOWN =
		Guid.Parse( "00000000-0000-0000-C000-000000000046" );

	internal IntPtr Address { get; private set; }
	public bool Disposed => Address == IntPtr.Zero;


	public ComPtr( IntPtr address ) {
		Address = address;
	}

	~ComPtr() {
		Dispose();
	}

	public void Dispose() {
		if( Address != IntPtr.Zero ) {
			Marshal.Release( Address );
			Address = IntPtr.Zero;
		}
		GC.SuppressFinalize( this );
	}
}



public sealed class ComPtr< T >: ComPtr where T : class
{
	internal T? Interface { get; private set; }

	public ComPtr( T? comInterface ): 
				base( Marshal.GetIUnknownForObject( comInterface! ) ) 
											=> Interface = comInterface ;
}
