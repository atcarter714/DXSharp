using System.Runtime.InteropServices;
using DXSharp.Windows.COM;
using Windows.Win32;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;

namespace BasicTests;

[TestFixture, Category("BuildBaseline")]
internal class ComPtrReferenceCountingTests {
    private IDXGIFactory7 _factory = null!;

    [SetUp]
    public void CreateFactory() {
        PInvoke.CreateDXGIFactory2(0, typeof(IDXGIFactory7).GUID, out object? factory).ThrowOnFailure();
        _factory = (IDXGIFactory7)factory!;
    }

    [TearDown]
    public void ReleaseFactory() {
        if (_factory is not null) Marshal.ReleaseComObject(_factory);
    }

    [Test]
    public void ManagedOwnerClearsAllStateOnDispose() {
        using var ptr = new ComPtr<IDXGIFactory7>(_factory);
        Assert.That(ptr.RefCount, Is.EqualTo(1));
        Assert.That((bool)ptr.Interface!.IsCurrent(), Is.True);

        ptr.Dispose();
        ptr.Dispose();
        Assert.That(ptr.Disposed, Is.True);
        Assert.That(ptr.BaseAddress, Is.Zero);
        Assert.That(ptr.InterfaceVPtr, Is.Zero);
        Assert.That(ptr.Interface, Is.Null);
        Assert.That(ptr.RefCount, Is.Zero);
    }

    [Test]
    public void BorrowedInputRemainsOwnedByCaller() {
        nint input = Marshal.GetIUnknownForObject(_factory);
        try {
            using (var ptr = new ComPtr<IDXGIFactory7>(input)) {
                Assert.That(ptr.BaseAddress, Is.EqualTo(input));
                Assert.That((bool)ptr.Interface!.IsCurrent(), Is.True);
            }
            COMUtility.QueryInterface<IDXGIFactory7>(input, out nint result).ThrowOnFailure();
            Marshal.Release(result);
        }
        finally { Marshal.Release(input); }
    }

    [Test]
    public void CloneSurvivesOriginalDisposal() {
        using var original = new ComPtr<IDXGIFactory7>(_factory);
        using var clone = original.Clone();
        Assert.That(original.RefCount, Is.EqualTo(1));
        Assert.That(clone.BaseAddress, Is.EqualTo(original.BaseAddress));
        original.Dispose();
        Assert.That((bool)clone.Interface!.IsCurrent(), Is.True);
        Assert.That(ComPtr.GetAllAllocations(), Does.Contain(clone.BaseAddress));
    }

    [Test]
    public void BalancedManualReferencesAndLastRelease() {
        using var ptr = new ComPtr<IDXGIFactory7>(_factory);
        Assert.That(ptr.IncrementReferences(), Is.EqualTo(2));
        Assert.That(ptr.ReleaseReference(), Is.EqualTo(1));
        Assert.That((bool)ptr.Interface!.IsCurrent(), Is.True);
        Assert.That(ptr.ReleaseReference(), Is.Zero);
        Assert.That(ptr.Disposed, Is.True);
        Assert.Throws<ObjectDisposedException>(() => ptr.ReleaseReference());
    }

    [Test]
    public void AttachInitializesTheTypedPointer() {
        using var ptr = new ComPtr<IDXGIFactory7>();
        ptr.Attach(_factory);
        Assert.That((bool)ptr.Interface!.IsCurrent(), Is.True);
        Assert.Throws<InvalidOperationException>(() => ptr.Attach(_factory));
        Assert.That(ptr.RefCount, Is.EqualTo(1));
    }

    [Test]
    public void AttachAndDetachTransferATypedReference() {
        nint input = Marshal.GetIUnknownForObject(_factory);
        try {
            using var ptr = new ComPtr<IDXGIFactory7>();
            ptr.Attach(input);
            nint typed = ptr.InterfaceVPtr;
            nint detached = ptr.Detach();
            try {
                Assert.That(detached, Is.EqualTo(typed));
                Assert.That(ptr.Disposed, Is.True);
                Assert.That(ptr.Interface, Is.Null);
                COMUtility.QueryInterface<IDXGIFactory4>(detached, out nint result).ThrowOnFailure();
                Marshal.Release(result);
            }
            finally { Marshal.Release(detached); }
        }
        finally { Marshal.Release(input); }
    }

    [Test]
    public void CastOwnsASeparateInterfaceReference() {
        using var original = new ComPtr<IDXGIFactory7>(_factory);
        using var cast = original.Cast<IDXGIFactory4>();
        Assert.That(cast.BaseAddress, Is.EqualTo(original.BaseAddress));
        original.Dispose();
        Assert.That((bool)cast.Interface!.IsCurrent(), Is.True);
    }

    [Test]
    public void FailedCastLeavesTheSourceUsable() {
        using var original = new ComPtr<IDXGIFactory7>(_factory);
        Assert.That(Assert.Catch(() => original.Cast<ID3D12Device>())!.HResult, Is.EqualTo(unchecked((int)0x80004002)));
        Assert.That(original.RefCount, Is.EqualTo(1));
        Assert.That((bool)original.Interface!.IsCurrent(), Is.True);
    }

    [Test]
    public void FailedReplacementKeepsThePreviousOwner() {
        using var ptr = new ComPtr<IDXGIFactory7>(_factory);
        using var unsupported = new ComPtr<IDXGIFactory7>();
        Assert.Throws<ArgumentNullException>(() => ptr.Set(unsupported.InterfaceVPtr));
        Assert.That(ptr.RefCount, Is.EqualTo(1));
        Assert.That((bool)ptr.Interface!.IsCurrent(), Is.True);
    }

    [Test]
    public void SwapMovesAllOwnershipState() {
        using var source = new ComPtr<IDXGIFactory7>(_factory);
        using var destination = new ComPtr<IDXGIFactory7>();
        source.IncrementReferences();
        source.Swap(destination);
        Assert.That(source.Disposed, Is.True);
        Assert.That(destination.RefCount, Is.EqualTo(2));
        Assert.That((bool)destination.Interface!.IsCurrent(), Is.True);
    }

    [Test]
    public void MultipleOwnersRemainTrackedUntilLastDispose() {
        using var a = new ComPtr<IDXGIFactory7>(_factory);
        using var b = new ComPtr<IDXGIFactory7>(_factory);
        nint identity = a.BaseAddress;
        a.Dispose();
        Assert.That(ComPtr.GetAllAllocations(), Does.Contain(identity));
        b.Dispose();
        Assert.That(ComPtr.GetAllAllocations(), Does.Not.Contain(identity));
    }
}
