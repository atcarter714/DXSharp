using System.Runtime.InteropServices;
using DXSharp.Windows.COM;
using Windows.Win32;
using Windows.Win32.Graphics.Dxgi;

namespace BasicTests.COM;

[TestFixture, Category("BuildBaseline")]
public class Test_IUnknown_Proxies {
    [Test]
    public void Test_IUnknown_QueryInterface() {
        PInvoke.CreateDXGIFactory2(0, typeof(IDXGIFactory7).GUID, out object? factory).ThrowOnFailure();
        try {
            var unknown = (IUnknown)factory!;
            unknown.QueryInterface(typeof(IDXGIFactory4).GUID, out nint pointer).ThrowOnFailure();
            try {
                Assert.That(pointer, Is.Not.EqualTo(nint.Zero));
                Assert.That(Marshal.GetObjectForIUnknown(pointer), Is.InstanceOf<IDXGIFactory4>());
            }
            finally {
                if (pointer != 0) Marshal.Release(pointer);
            }

            var result = unknown.QueryInterface(Guid.NewGuid(), out pointer);
            Assert.That(result.Value, Is.EqualTo(unchecked((int)0x80004002)));
            Assert.That(pointer, Is.EqualTo(nint.Zero));
        }
        finally {
            Marshal.ReleaseComObject(factory!);
        }
    }
}
