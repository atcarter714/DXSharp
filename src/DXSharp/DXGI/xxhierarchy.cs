#region Using Directives
using DXSharp.Windows.COM;

using System.Configuration;

using Windows.Win32.Graphics.Dxgi;
#endregion

namespace xxx;



// ========================================================
// COM Pointers:
// ========================================================

class ComPtr
{
	internal IntPtr Address { get; private set; }
}
sealed class ComPtr<T> : ComPtr //where T: class
{
	internal T? Interface { get; private set; }
}

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


interface IUnknown { internal ComPtr ComPtr { get; } }
interface IDXObject<T> : IUnknown { internal new ComPtr<T> ComPtr { get; } }

// DXGI Objects

internal abstract class Factory<T> where T : IDXGIFactory
{

};
internal abstract class Factory1<T>: Factory<T> where T : IDXGIFactory1
{

};
internal abstract class Factory2<T> : Factory1<T> where T : IDXGIFactory2
{

};
internal abstract class Factory3<T> : Factory2<T> where T : IDXGIFactory3
{

};
internal abstract class Factory4<T> : Factory3<T> where T : IDXGIFactory4
{

};
internal abstract class Factory5<T> : Factory4<T> where T : IDXGIFactory5
{

};
internal abstract class Factory6<T> : Factory5<T> where T : IDXGIFactory6
{

};
internal abstract class Factory7<T>: Factory6<T> where T: IDXGIFactory7
{

};



internal class Factory : Factory<IDXGIFactory7>
{

}
internal class Factory1 : Factory1<IDXGIFactory7>
{

}
internal class Factory2 : Factory2<IDXGIFactory7>
{

}
internal class Factory3 : Factory3<IDXGIFactory7>
{

}
internal class Factory4 : Factory4<IDXGIFactory7>
{

}
internal class Factory5 : Factory5<IDXGIFactory7>
{

}
internal class Factory6 : Factory6<IDXGIFactory7>
{

}
internal class Factory7: Factory7<IDXGIFactory7>
{

}