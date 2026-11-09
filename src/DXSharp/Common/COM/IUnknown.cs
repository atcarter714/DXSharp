using System.Runtime.InteropServices;

namespace DXSharp.Windows.COM;

// The CLR provides the three IUnknown slots for an InterfaceIsIUnknown RCW.
[NativeLibrary(null, "IUnknown", "unknwn.h", "COM")]
[ComImport, Guid("00000000-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IUnknown { }

public static class IUnknownExtensions {
    public static uint AddRef(this IUnknown obj) {
        nint unknown = Marshal.GetIUnknownForObject(obj);
        // GetIUnknownForObject already acquired the requested reference.
        return unchecked((uint)COMUtility.GetRefCount(unknown));
    }

    public static uint Release(this IUnknown obj) {
        nint unknown = Marshal.GetIUnknownForObject(obj);
        Marshal.Release(unknown);
        return unchecked((uint)Marshal.Release(unknown));
    }


    /// <summary>Queries a COM object. The caller must release the returned interface on success.</summary>
    public static HResult QueryInterface(this IUnknown obj, in Guid iid, out nint pointer) {
        nint unknown = Marshal.GetIUnknownForObject(obj);
        try { return (HResult)Marshal.QueryInterface(unknown, in iid, out pointer); }
        finally { Marshal.Release(unknown); }
    }
}
