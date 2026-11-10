using Windows.Win32.Graphics.Dxgi;
using DXSharp;
using DXSharp.DXGI;
using DXSharp.Windows.COM;

namespace BasicTests;

[TestFixture, Category("BuildBaseline")]
public class DXGI_Interface_Tests {
    private IFactory7 _factory = null!;

    [SetUp]
    public void SetUp() {
        DXGIFunctions.CreateFactory2(IFactory7.IID, out var factory, FactoryCreateFlags.None).ThrowOnFailure();
        _factory = (IFactory7)factory!;
    }

    [TearDown]
    public void Cleanup() => _factory?.Dispose();

    [Test]
    public void Test_Create_Factory7() {
        Assert.That(_factory, Is.InstanceOf<IComObjectRef<IDXGIFactory7>>());
        _factory.AddRef();
        _factory.Release();
        Assert.That(_factory.IsCurrent(), Is.True);
        Assert.That(_factory.GetCreationFlags(), Is.EqualTo(FactoryCreateFlags.None));
    }

    [Test]
    public unsafe void Test_SetData() {
        var key = Guid.NewGuid();
        uint data = 0x1234abcd;
        uint actual = 0;
        uint size = sizeof(uint);
        _factory.SetPrivateData(key, size, (nint)(&data)).ThrowOnFailure();
        _factory.GetPrivateData(key, ref size, (nint)(&actual)).ThrowOnFailure();
        Assert.That(size, Is.EqualTo(sizeof(uint)));
        Assert.That(actual, Is.EqualTo(data));
    }

    [Test]
    public void Test_EnumAdapters() {
        _factory.EnumAdapters1(0, out var adapter).ThrowOnFailure();
        using (adapter) {
            Assert.That(adapter, Is.Not.Null);
            adapter!.QueryInterface<IAdapter4>(out var version4).ThrowOnFailure();
            using (version4) {
                Assert.That(version4, Is.Not.Null);
                version4!.GetDesc3(out var desc);
                Assert.That(desc.Description.ToString(), Is.Not.Empty);
            }
        }
    }
}
