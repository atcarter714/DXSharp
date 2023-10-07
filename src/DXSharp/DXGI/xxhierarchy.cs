#region Using Directives
using DXSharp.Windows.COM;

using System.Runtime.InteropServices;

using Windows.Win32.Graphics.Dxgi;
#endregion

namespace DXSharp.DXGI ;



// ========================================================
// COM Pointers:
// ========================================================

//class ComPtr
//{
//	internal IntPtr Address { get; private set; }
//}
//sealed class ComPtr<T> : ComPtr //where T: class
//{
//	internal T? Interface { get; private set; }
//}

// ========================================================
// Interfaces:
// ========================================================


/*
interface IObject: IUnknown { }
interface IObject<T> : IDXObject<T> where T: class, IDXGIObject { }
interface IFactoryX<T>: IObject<T> where T: class, IDXGIFactory { }

interface IFactory: IObject, IFactoryX<IDXGIFactory> { }
interface IFactory1: IFactory, IFactoryX<IDXGIFactory1> { }
interface IFactory2: IFactory1, IFactoryX<IDXGIFactory2> { }
interface IFactory3: IFactory2, IFactoryX<IDXGIFactory3> { }
interface IFactory4: IFactory3, IFactoryX<IDXGIFactory4> { }
interface IFactory5: IFactory4, IFactoryX<IDXGIFactory5> { }
interface IFactory6: IFactory5, IFactoryX<IDXGIFactory6> { }
interface IFactory7: IFactory6, IFactoryX<IDXGIFactory7> { }
	*/

//public interface IObject<T> : IDXObject<T> where T : IDXGIObject { }

//public interface IFactory<T> : IObject<T> where T : IDXGIFactory { }
//public interface IFactory1<T> : IFactory<T> where T : IDXGIFactory1 { }
//public interface IFactory2<T> : IFactory1<T> where T : IDXGIFactory2 { }
//public interface IFactory3<T> : IFactory2<T> where T : IDXGIFactory3 { }
//public interface IFactory4<T> : IFactory3<T> where T : IDXGIFactory4 { }
//public interface IFactory5<T> : IFactory4<T> where T : IDXGIFactory5 { }
//public interface IFactory6<T> : IFactory5<T> where T : IDXGIFactory6 { }
//public interface IFactory7<T> : IFactory6<T> where T : IDXGIFactory7 { }

//interface IFactoryX<T>: IFactory where T: IDXGIFactory { }
//interface IFactory<T>: IObject<T> where T: IDXGIObject { }
//interface IFactory1<T>: IFactory<T> where T: IDXGIFactory { }
//interface IFactory2<T> : IFactory1<T> where T : IDXGIFactory1 { }
//interface IFactory3<T> : IFactory2<T> where T : IDXGIFactory2 { }
//interface IFactory4<T> : IFactory3<T> where T : IDXGIFactory3 { }
//interface IFactory5<T> : IFactory4<T> where T : IDXGIFactory4 { }
//interface IFactory6<T> : IFactory5<T> where T : IDXGIFactory5 { }
//interface IFactory7<T> : IFactory6<T> where T : IDXGIFactory6 { }

//interface IFactory<IDXGIFactory> : IObject<IDXGIObject> { new ComPtr<IDXGIFactory> ComPtr { get; } }
//interface IFactory1<TIDXGIFactory1> : IFactory<IDXGIFactory> { new ComPtr<IDXGIFactory1> ComPtr { get; } }
//interface IFactory2<TIDXGIFactory2> : IFactory1<IDXGIFactory1> { new ComPtr<IDXGIFactory2> ComPtr { get; } }
//interface IFactory3<TIDXGIFactory3> : IFactory2<IDXGIFactory2> { new ComPtr<IDXGIFactory3> ComPtr { get; } }
//interface IFactory4<TIDXGIFactory4> : IFactory3<IDXGIFactory3> { new ComPtr<IDXGIFactory4> ComPtr { get; } }
//interface IFactory5<TIDXGIFactory5> : IFactory4<IDXGIFactory4> { new ComPtr<IDXGIFactory5> ComPtr { get; } }
//interface IFactory6<TIDXGIFactory6> : IFactory5<IDXGIFactory5> { new ComPtr<IDXGIFactory6> ComPtr { get; } }
//interface IFactory7<TIDXGIFactory7> : IFactory6<IDXGIFactory6> { new ComPtr<IDXGIFactory7> ComPtr { get; } }

//interface IAdapter<IDXGIAdapter> : IObject<IDXGIObject> { }
//interface IAdapter1<TIDXGIAdapter1> : IAdapter<IDXGIAdapter> { }
//interface IAdapter2<TIDXGIAdapter2> : IAdapter1<IDXGIAdapter1> { }
//interface IAdapter3<TIDXGIAdapter3> : IAdapter2<IDXGIAdapter2> { }
//interface IAdapter4<TIDXGIAdapter4> : IAdapter3<IDXGIAdapter3> { }

// ========================================================
// Classes:
// ========================================================

//internal class COMBaseObject : IUnknown
//{
//	internal virtual ComPtr ComPtr { get; }
//	ComPtr IUnknown.ComPtr => this.ComPtr;
//}

//internal class COMBaseObject<T> : COMBaseObject where T : IDXObject
//{
//	internal override ComPtr<T> ComPtr { get; }
//}

//internal class Object : COMBaseObject, IObject<IDXGIObject>
//{
//	internal ComPtr<IDXGIObject> ComPtr { get; protected set; }
//}

// --------------------------------------------------------

//internal class Factory : Object<IDXGIFactory>, IFactory<IDXGIFactory>
//{
//	ComPtr<IDXGIFactory> IDXObject<IDXGIFactory>.ComPtr { get; }
//	ComPtr IUnknown.ComPtr { get; }
//}
//internal class Factory1 : Factory, IFactory1<IDXGIFactory1>
//{
//	internal ComPtr<IDXGIFactory1> IObject<IDXGIFactory1>.ComPtr { get; }
//}
//internal class Factory2 : Factory1, IFactory2
//{
//	ComPtr<IDXGIFactory2> IObject<IDXGIFactory2>.ComPtr { get; }
//}
//internal class Factory3 : Factory2, IFactory3
//{
//	ComPtr<IDXGIFactory3> IObject<IDXGIFactory3>.ComPtr { get; }
//}
//internal class Factory4 : Factory3, IFactory4
//{
//	ComPtr<IDXGIFactory4> IObject<IDXGIFactory4>.ComPtr { get; }
//}
//internal class Factory5 : Factory4, IFactory5
//{
//	ComPtr<IDXGIFactory5> IObject<IDXGIFactory5>.ComPtr { get; }
//}
//internal class Factory6 : Factory5, IFactory6
//{
//	ComPtr<IDXGIFactory6> IObject<IDXGIFactory6>.ComPtr { get; }
//}
//internal class Factory7 : Factory6, IFactory7<IDXGIFactory7>
//{
//	ComPtr<IDXGIFactory7> ComPtr { get; }

//	void fn()
//	{

//	}
//}

// --------------------------------------------------------

//class Adapter : IAdapter { }
//class Adapter1 : Adapter, IAdapter1 { }
//class Adapter2 : Adapter1, IAdapter2 { }
//class Adapter3 : Adapter2, IAdapter3 { }

// --------------------------------------------------------


// COM Objects:




public interface IDXObject<T> where T: class, IDXGIObject
{
	//ComPtr? IUnknown.ComPtr => (ComPtr?)this.ComPtr;
	internal ComPtr<T>? ComPtr { get; }



	/// <summary>Sets application-defined data to the object and associates that data with a GUID.</summary>
	/// <param name="Name">
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID that identifies the data. Use this GUID in a call to <a href="https://docs.microsoft.com/windows/desktop/api/dxgi/nf-dxgi-idxgiobject-getprivatedata">GetPrivateData</a> to get the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="DataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a></b> The size of the object's data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>const void*</b> A pointer to the object's data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> values.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedata">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetPrivateData<I>( uint DataSize, nint pData ) where I : T;

	/// <summary>Set an interface in the object's private data.</summary>
	/// <param name="Name">
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID identifying the interface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pUnknown">
	/// <para>Type: <b>const <a href="https://docs.microsoft.com/windows/desktop/api/unknwn/nn-unknwn-iunknown">IUnknown</a>*</b> The interface to set.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the following <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-setprivatedatainterface">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void SetPrivateDataInterface<I>( I pUnknown ) where I : T;

	/// <summary>Get a pointer to the object's data.</summary>
	/// <param name="Name">
	/// <para>Type: <b><a href="https://docs.microsoft.com/openspecs/windows_protocols/ms-oaut/6e7d7108-c213-40bc-8294-ac13fe68fd50">REFGUID</a></b> A GUID identifying the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pDataSize">
	/// <para>Type: <b><a href="https://docs.microsoft.com/windows/desktop/WinProg/windows-data-types">UINT</a>*</b> The size of the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="pData">
	/// <para>Type: <b>void*</b> Pointer to the data.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the following <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a>.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getprivatedata">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetPrivateData<I>( in uint pDataSize, nint pData ) where I : T;

	/// <summary>Gets the parent of the object.</summary>
	/// <param name="riid">
	/// <para>Type: <b>REFIID</b> The ID of the requested interface.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getparent#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <param name="ppParent">
	/// <para>Type: <b>void**</b> The address of a pointer to the parent object.</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getparent#parameters">Read more on docs.microsoft.com</see>.</para>
	/// </param>
	/// <returns>
	/// <para>Type: <b><a href="/windows/win32/com/structure-of-com-error-codes">HRESULT</a></b> Returns one of the <a href="/windows/desktop/direct3ddxgi/dxgi-error">DXGI_ERROR</a> values.</para>
	/// </returns>
	/// <remarks>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api//dxgi/nf-dxgi-idxgiobject-getparent">Learn more about this API from docs.microsoft.com</see>.</para>
	/// </remarks>
	void GetParent<I>( out I? ppParent ) where I : class, T;

};


public interface IUnknown { }

//public interface IObject
//{
//	void SetPrivateData<I>( uint DataSize, nint pData ) where I : class;
//	void SetPrivateDataInterface<I>( I pUnknown ) where I : class;
//	void GetPrivateData<I>( in uint pDataSize, nint pData ) where I : class;
//	void GetParent<I>( out I? ppParent ) where I : class;
//};

public interface IFactory<T>: IDXObject<T> where T : class, IDXGIFactory
{

};




// DXGI Objects

/// <summary>
/// The DXGI.Object&lt;T&gt; class is an abstract wrapper class around the 
/// <a href="https://learn.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiobject">IDXGIObject</a> interface.
/// <para>
/// The IDXGIObject interface is a base interface for all DXGI objects; 
/// IDXGIObject supports associating caller-defined (private data) with 
/// an object and retrieval of an interface to the parent object.
/// </para>
/// </summary>
/// <typeparam name="T">Type of IDXGIObject interface wrapped by this class</typeparam>
internal abstract class Object<T>: IDXObject<T> where T : class, IDXGIObject
{
	ComPtr<T>? IDXObject<T>.ComPtr => ComPtr;
	internal abstract ComPtr<T>? ComPtr { get; private protected set; }

	public void SetPrivateData<I>( uint DataSize, nint pData ) where I : T {
#if DEBUG || !STRIP_CHECKS
		if( ComPtr is null || ComPtr.Interface is null )
			throw new COMException( $"{GetType().Name}<{nameof( T )}>" +

				/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
				Before:
								$"{ nameof( SetPrivateData ) }<{ nameof(I) }>( " +
								$"{ DataSize }, { pData.ToString("X8") } ): " +
				After:
								$"{ nameof( SetPrivateData ) }<{ nameof(I )}>( " +
								$"{ DataSize }, { pData.ToString("X8" )} ): " +
				*/
				$"{nameof( SetPrivateData )}<{nameof( I )}>( " +
				$"{DataSize}, {pData.ToString( "X8" )} ): " +
				(ComPtr is null ? $"Internal ComPtr<{nameof( T )}> is null!" :
					$"Internal ComPtr<{nameof( T )}> has a null pointer!") );
#endif

		unsafe {
			var riid = typeof(I).GUID;
			ComPtr.Interface.SetPrivateData( &riid, DataSize, (void*)pData );
		}
	}

	public void SetPrivateDataInterface<I>( I pUnknown ) where I : T {
#if DEBUG || !STRIP_CHECKS
		if( ComPtr is null || ComPtr.Interface is null )
			throw new COMException( $"{GetType().Name}<{nameof( T )}>" +
				$"{nameof( SetPrivateDataInterface )}<{nameof( I )}>( {pUnknown} ): " +
				(ComPtr is null ? $"Internal ComPtr<{nameof( T )}> is null!" :
					$"Internal ComPtr<{nameof( T )}> has a null pointer!") );
#endif


	}

	public void GetPrivateData<I>( in uint pDataSize, nint pData ) where I : T {
#if DEBUG || !STRIP_CHECKS
		if( ComPtr is null || ComPtr.Interface is null )
			throw new COMException( $"{GetType().Name}<{nameof( T )}>" +
				$"{nameof( GetPrivateData )}<{nameof( I )}>( " +
				$"{pDataSize}, {pData.ToString( "X8" )} ): " +
				(ComPtr is null ? $"Internal ComPtr<{nameof( T )}> is null!" :
					$"Internal ComPtr<{nameof( T )}> has a null pointer!") );
#endif


	}

	public void GetParent<I>( out I? ppParent ) where I : class, T {
#if DEBUG || !STRIP_CHECKS
		if( ComPtr is null || ComPtr.Interface is null )
			throw new COMException( $"{GetType().Name}<{nameof( T )}>" +
				$"{nameof( GetParent )}<{nameof( I )}>( {nameof( ppParent )} ): " +
				(ComPtr is null ? $"Internal ComPtr<{nameof( T )}> is null!" :
					$"Internal ComPtr<{nameof( T )}> has a null pointer!") );
#endif

		unsafe {
			object? parentObj = default;
			var riid = typeof(I).GUID;

			ComPtr.Interface.GetParent( &riid, out parentObj );

			if( parentObj is not null )
				ppParent = parentObj as I;
			else
				ppParent = null;
		}
	}
};

internal abstract class Factory<T>: Object<T> where T : class, IDXGIFactory
{

};
internal abstract class Factory1<T>: Factory<T> where T : class, IDXGIFactory1
{

};
internal abstract class Factory2<T>: Factory1<T> where T : class, IDXGIFactory2
{

};
internal abstract class Factory3<T>: Factory2<T> where T : class, IDXGIFactory3
{

};
internal abstract class Factory4<T>: Factory3<T> where T : class, IDXGIFactory4
{

};
internal abstract class Factory5<T>: Factory4<T> where T : class, IDXGIFactory5
{

};
internal abstract class Factory6<T>: Factory5<T> where T : class, IDXGIFactory6
{

};
internal abstract class Factory7<T>: Factory6<T> where T : class, IDXGIFactory7
{

};



internal class Factory: Factory<IDXGIFactory>
{
	internal override ComPtr<IDXGIFactory>? ComPtr { get; private protected set; }
};
internal class Factory1: Factory1<IDXGIFactory1>
{
	internal override ComPtr<IDXGIFactory1>? ComPtr { get; private protected set; }
};
internal class Factory2: Factory2<IDXGIFactory2>
{
	internal override ComPtr<IDXGIFactory2>? ComPtr { get; private protected set; }
};
internal class Factory3: Factory3<IDXGIFactory3>
{
	internal override ComPtr<IDXGIFactory3>? ComPtr { get; private protected set; }
};
internal class Factory4: Factory4<IDXGIFactory4>
{
	internal override ComPtr<IDXGIFactory4>? ComPtr { get; private protected set; }
};
internal class Factory5: Factory5<IDXGIFactory5>
{
	internal override ComPtr<IDXGIFactory5>? ComPtr { get; private protected set; }
};
internal class Factory6: Factory6<IDXGIFactory6>
{
	internal override ComPtr<IDXGIFactory6>? ComPtr { get; private protected set; }
};
internal class Factory7: Factory7<IDXGIFactory7>
{
	internal override ComPtr<IDXGIFactory7>? ComPtr { get; private protected set; }
};