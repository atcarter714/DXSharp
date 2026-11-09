// COPYRIGHT NOTICES:
// --------------------------------------------------------------------------------
// Copyright © 2023 Arkaen Solutions, All Rights Reserved.
// (Original: Copyright © 2022 DXSharp - ATC - Aaron T. Carter)
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

using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;
using static DXSharp.InteropUtils;

namespace DXSharp.Windows.COM;

/// <summary>Owns explicit COM interface references independently of the managed RCW.</summary>
/// <remarks>Raw pointers and RCWs exposed by this owner are borrowed. Synchronize their use with
/// disposal, and use the owner only in an apartment where its COM object may be called.</remarks>
public abstract class ComPtr : DisposableObject {
    public const int NULL_PTR = 0;
    public static readonly Guid GUID_IUNKNOWN = new("00000000-0000-0000-C000-000000000046");

    private static readonly Dictionary<nint, int> Allocations = new();
    private static readonly object SwapGate = new();
    protected readonly object Gate = new();
    private nint _identity;
    private nint _pointer;
    private int _references;
    private object? _interface;
    private bool _finalizationSuppressed;

    public abstract Type ComType { get; }
    internal int RefCount { get { lock (Gate) return _references; } }
    internal nint BaseAddress { get { lock (Gate) return _identity; } }
    protected nint TypedPointer { get { lock (Gate) return _pointer; } }
    public abstract nint InterfaceVPtr { get; }
    internal object? InterfaceObjectRef { get { lock (Gate) return _interface; } }
    protected int LastNativeCount { get; private set; }
    public override bool Disposed => BaseAddress == 0;

    internal ComPtr() { }
    internal ComPtr(nint ptr) => Attach(ptr);
    public ComPtr(int addr) : this((nint)addr) { }
    public ComPtr(uint addr) : this((nint)addr) { }
    public ComPtr(long addr) : this((nint)addr) { }
    public ComPtr(ulong addr) : this((nint)addr) { }
    public unsafe ComPtr(void* ptr) : this((nint)ptr) { }
    public ComPtr(in IUnknown comInterface) => Attach(comInterface);
    public ComPtr(in IUnknownWrapper comInterface) : this(comInterface.BasePointer) { }
    public ComPtr(in IUnknownWrapper<IUnknown> comInterface) : this(comInterface.BasePointer) { }
    public ComPtr(in object? comObj) => AttachObject(comObj, replace: false);

    /// <summary>Acquires references from a borrowed pointer. The caller retains its reference.</summary>
    public void Attach(nint ptr) => Bind(ptr, null, replace: false);

    /// <summary>Acquires references for this owner's interface type from a managed COM object.</summary>
    public void Attach<C>(C obj) where C : IUnknown => AttachObject(obj, replace: false);

    internal void Set(nint newPtr) => Bind(newPtr, null, replace: true);
    internal virtual void Set<TInterface>(in TInterface? newComObject) where TInterface : IUnknown =>
        AttachObject(newComObject, replace: true);

    private void AttachObject(object? obj, bool replace) {
        ArgumentNullException.ThrowIfNull(obj);
        nint unknown = Marshal.GetIUnknownForObject(obj);
        try { Bind(unknown, obj, replace); }
        finally { Marshal.Release(unknown); }
    }

    protected void Bind(nint source, object? obj, bool replace) {
        if (source == 0) throw new ArgumentNullException(nameof(source));
        lock (Gate) {
            if (!replace && _pointer != 0) throw new InvalidOperationException("A COM pointer is already attached.");
            Guid iid = ComType.GUID;
            Marshal.ThrowExceptionForHR(Marshal.QueryInterface(source, in iid, out nint typed));
            nint identity = 0;
            try {
                var unknown = GUID_IUNKNOWN;
                Marshal.ThrowExceptionForHR(Marshal.QueryInterface(typed, in unknown, out identity));
                object rcw = obj ?? Marshal.GetObjectForIUnknown(typed);
                if (!ComType.IsInstanceOfType(rcw))
                    throw new InvalidCastException($"The COM object does not implement {ComType.FullName}.");

                Clear();
                TrackPointer(identity);
                if (identity == typed) Marshal.Release(identity);
                _identity = identity;
                _pointer = typed;
                _interface = rcw;
                _references = 1;
                RegisterFinalizerIfAttached();
            }
            catch {
                if (identity != 0) Marshal.Release(identity);
                Marshal.Release(typed);
                throw;
            }
        }
    }

    /// <summary>Transfers the single owned typed interface reference to the caller.</summary>
    public virtual nint Detach() {
        lock (Gate) {
            if (_references > 1) throw new InvalidOperationException("Release additional references before detaching.");
            nint pointer = _pointer;
            Clear(transfer: true);
            return pointer;
        }
    }

    public virtual void Swap(ComPtr other) {
        ArgumentNullException.ThrowIfNull(other);
        if (ComType != other.ComType) throw new ArgumentException("COM interface types must match.", nameof(other));
        lock (SwapGate)
        lock (Gate)
        lock (other.Gate) {
            (_identity, other._identity) = (other._identity, _identity);
            (_pointer, other._pointer) = (other._pointer, _pointer);
            (_interface, other._interface) = (other._interface, _interface);
            (_references, other._references) = (other._references, _references);
            (LastNativeCount, other.LastNativeCount) = (other.LastNativeCount, LastNativeCount);
            RegisterFinalizerIfAttached();
            other.RegisterFinalizerIfAttached();
        }
    }

    private void RegisterFinalizerIfAttached() {
        if (_pointer == 0 || !_finalizationSuppressed) return;
        GC.ReRegisterForFinalize(this);
        _finalizationSuppressed = false;
    }

    protected nint RequirePointer() {
        ObjectDisposedException.ThrowIf(_pointer == 0, this);
        return _pointer;
    }

    protected int AcquireReference() {
        lock (Gate) {
            var pointer = RequirePointer();
            int next = checked(_references + 1);
            LastNativeCount = Marshal.AddRef(pointer);
            return _references = next;
        }
    }

    protected int DropReference() {
        lock (Gate) {
            var pointer = RequirePointer();
            if (_references == 1) {
                Clear();
                return 0;
            }
            LastNativeCount = Marshal.Release(pointer);
            return --_references;
        }
    }

    private void Clear(bool transfer = false) {
        nint pointer = _pointer;
        nint identity = _identity;
        int references = _references;
        _pointer = _identity = 0;
        _references = 0;
        _interface = null;
        UntrackPointer(identity);
        if (!transfer) {
            for (int i = 0; i < references; i++) LastNativeCount = Marshal.Release(pointer);
        }
        if (identity != 0 && identity != pointer) LastNativeCount = Marshal.Release(identity);
    }

    protected override void Dispose(bool disposing) {
        lock (Gate) {
            Clear();
            if (disposing) {
                GC.SuppressFinalize(this);
                _finalizationSuppressed = true;
            }
        }
    }

    protected override ValueTask DisposeUnmanaged() {
        lock (Gate) Clear();
        return ValueTask.CompletedTask;
    }

    public override ValueTask DisposeAsync() {
        Dispose();
        return ValueTask.CompletedTask;
    }

    internal static nint[] GetAllAllocations() {
        lock (Allocations) return Allocations.Keys.ToArray();
    }

    private static void TrackPointer(nint ptr) {
        lock (Allocations) Allocations[ptr] = Allocations.GetValueOrDefault(ptr) + 1;
    }

    private static void UntrackPointer(nint ptr) {
        lock (Allocations) {
            if (!Allocations.TryGetValue(ptr, out int owners)) return;
            if (owners == 1) Allocations.Remove(ptr);
            else Allocations[ptr] = owners - 1;
        }
    }

    public override string ToString() => $"COM OBJECT[ Address: 0x{BaseAddress:X} ]";
    public override int GetHashCode() => BaseAddress.GetHashCode();
    public override bool Equals(object? obj) =>
        (obj is ComPtr ptr && ptr.BaseAddress == BaseAddress)
        || (obj is nint address && address == BaseAddress);

    internal static ComPtr<TInto> Convert<TFrom, TInto>(ComPtr<TFrom>? other)
        where TFrom : IUnknown where TInto : IUnknown {
        ArgumentNullException.ThrowIfNull(other);
        if (!typeof(TFrom).IsAssignableTo(typeof(TInto))) throw new InvalidCastException();
        return other.QueryOwner<TInto>();
    }
}

[DebuggerDisplay("{ToString()}")]
public sealed class ComPtr<T> : ComPtr where T : IUnknown {
    public override Type ComType => typeof(T);
    public Type InterfaceType => typeof(T);
    public T? Interface => (T?)InterfaceObjectRef;
    public override nint InterfaceVPtr => TypedPointer;

#if DEBUG || DEBUG_COM
    public int MarshalRefCount => LastNativeCount;
#endif

    internal ComPtr() { }
    public ComPtr(in T comInterface) => Attach(comInterface);

    /// <summary>Acquires references from a borrowed interface pointer.</summary>
    public ComPtr(nint address) : base(address) { }

    public ComPtr(nint interfaceVPtr, T @interface) {
        ArgumentNullException.ThrowIfNull(@interface);
        Bind(interfaceVPtr, null, replace: false);
        if (!ReferenceEquals(Interface, @interface)) {
            nint identity = Marshal.GetIUnknownForObject(@interface);
            try {
                if (identity != BaseAddress) throw new ArgumentException("The pointer and interface identify different COM objects.", nameof(@interface));
            }
            catch { Dispose(); throw; }
            finally { Marshal.Release(identity); }
        }
    }

    internal int IncrementReferences() => AcquireReference();
    internal int ReleaseReference(bool final = false) => DropReference();
    internal T? GetReference() {
        GetReference(out var result);
        return result;
    }
    internal int GetReference(out T? pInterface) {
        lock (Gate) {
            int count = AcquireReference();
            pInterface = Interface;
            return count;
        }
    }

    public ComPtr<T> Clone() => QueryOwner<T>();
    public ComPtr<TOut> Cast<TOut>() where TOut : IUnknown => QueryOwner<TOut>();

    internal ComPtr<TOut> QueryOwner<TOut>() where TOut : IUnknown {
        lock (Gate) return new ComPtr<TOut>(RequirePointer());
    }

    /// <summary>Returns a method address for a borrowed interface of this owner.</summary>
    public unsafe nint GetVTableMethod<TUnknown>(int index) where TUnknown : IUnknown {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        lock (Gate) {
            nint pointer = RequirePointer();
            if (typeof(TUnknown) != typeof(T))
                throw new ArgumentException("Acquire an owner for the requested interface before accessing its vtable.");
            return (*(nint**)pointer)[index];
        }
    }

    public static ComPtr<C> AttachedTo<C>(C obj) where C : IUnknown => new(obj);
    public static ComPtr<T> operator ++(ComPtr<T> ptr) { ptr.IncrementReferences(); return ptr; }
    public static ComPtr<T> operator --(ComPtr<T> ptr) { ptr.ReleaseReference(); return ptr; }

	#region Conversion Operators

	public static explicit operator ComPtr< IUnknown >?( ComPtr< T >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		return new( other.BaseAddress ) ;
	}

	public static explicit operator ComPtr< T >?( ComPtr< IUnknown >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		if( other.Interface is T _interface ) return new( other.BaseAddress ) ;
		
		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(T)}!" ) ;
	}

	public static explicit operator ComPtr< IDXGIObject >( ComPtr< T >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		
		if( other.InterfaceType.IsAssignableTo(typeof(IDXGIObject)) 
			&& other.Interface is IDXGIObject dxgiObj ) return new( dxgiObj ) ;
		
		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(ID3D12Object)}!" ) ;
	}

	public static explicit operator ComPtr< T >( ComPtr< IDXGIObject >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		
		if( typeof(T).IsAssignableFrom(typeof(IDXGIObject))
			&& other.Interface is T _interface ) return new( other.BaseAddress ) ;

		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(T)}!" ) ;
	}

	public static explicit operator ComPtr< ID3D12Object >( ComPtr< T >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		
		if( other.InterfaceType.IsAssignableTo(typeof(ID3D12Object)) 
			&& other.Interface is ID3D12Object d3dObj ) return new( d3dObj ) ;
		
		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(ID3D12Object)}!" ) ;
	}

	public static explicit operator ComPtr< T >( ComPtr< ID3D12Object >? other ) {
		ArgumentNullException.ThrowIfNull( other, nameof(other) ) ;
		
		if( typeof(T).IsAssignableFrom(typeof(ID3D12Object))
			&& other.Interface is T _interface ) return new( other.BaseAddress ) ;

		throw new InvalidCastException( $"{nameof(ComPtr<T>)} :: Invalid cast! " +
										$"The COM interface type {other.InterfaceType.Name} cannot be cast to {nameof(T)}!" ) ;
	}
	
	#endregion
	
	// ==========================================================
} ;
