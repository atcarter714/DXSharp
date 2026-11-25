using System.Runtime.InteropServices;

namespace DXSharp.Interop;

/// <summary>Raw owned-output entry points for deterministic Media Foundation and WIC interface leases.</summary>
/// <remarks>Successful outputs each transfer one native COM reference to the caller. Adopt with NativeInterface in Core.</remarks>
public static partial class MediaNative {
    /// <summary>Creates an attribute store and transfers one native IMFAttributes reference.</summary>
    [LibraryImport("mfplat.dll", EntryPoint = "MFCreateAttributes")]
    public static partial int CreateAttributes(out nint attributes, uint capacity);
    /// <summary>Creates a writable section-backed IWICBitmap and transfers one reference. Keep the section alive through bitmap disposal.</summary>
    [LibraryImport("windowscodecs.dll", EntryPoint = "WICCreateBitmapFromSectionEx")]
    public static partial int CreateBitmap(uint width, uint height, in Guid format, nint section, uint stride, uint offset, uint access, out nint bitmap);
    /// <summary>Activates an in-process COM class and transfers one reference of the requested interface.</summary>
    [LibraryImport("ole32.dll", EntryPoint = "CoCreateInstance")]
    public static partial int CreateInstance(in Guid classId, nint outer, uint context, in Guid interfaceId, out nint instance);
}
