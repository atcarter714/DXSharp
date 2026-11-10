using System.Runtime.InteropServices;
using System.Windows.Forms;
using DXSharp.Windows;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Dxgi.Common;
using static Windows.Win32.PInvoke;

namespace BasicTests;

[TestFixture, NonParallelizable, Category("BuildBaseline")]
public class D3D12GraphicsInterop {
    private const D3D12_COMMAND_LIST_TYPE Direct = D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT;
    private readonly Stack<object> _owned = new();

    private T Own<T>(object? value) {
        if (value is not null) _owned.Push(value);
        Assert.That(value, Is.InstanceOf<T>(), typeof(T).Name);
        return (T)value!;
    }

    [TearDown]
    public void ReleaseInterfaces() {
        while (_owned.TryPop(out var value)) Marshal.ReleaseComObject(value);
    }

    private IDXGIFactory7 CreateFactory() {
        CreateDXGIFactory2(0, typeof(IDXGIFactory7).GUID, out object? factory).ThrowOnFailure();
        return Own<IDXGIFactory7>(factory);
    }

    private unsafe IDXGIAdapter CreateWarpAdapter(IDXGIFactory7 factory) {
        var iid = typeof(IDXGIAdapter).GUID;
        factory.EnumWarpAdapter(&iid, out var adapter);
        return Own<IDXGIAdapter>(adapter);
    }

    private ID3D12Device9 CreateDevice(IDXGIFactory7 factory) {
        var warp = CreateWarpAdapter(factory);
        D3D12CreateDevice(warp, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
            typeof(ID3D12Device9).GUID, out object? device).ThrowOnFailure();
        return Own<ID3D12Device9>(device);
    }

    private unsafe ID3D12CommandQueue CreateQueue(ID3D12Device9 device) {
        var desc = new D3D12_COMMAND_QUEUE_DESC { Type = Direct };
        var iid = typeof(ID3D12CommandQueue).GUID;
        var creator = Guid.Empty;
        device.CreateCommandQueue1(&desc, &creator, &iid, out object? queue);
        return Own<ID3D12CommandQueue>(queue);
    }

    [Test]
    public void Create_DXGI_Factory() => CreateFactory();

    [Test]
    public void Get_DXGI_Adapters() {
        var factory = CreateFactory();
        factory.EnumAdapters1(0, out var adapter);
        Own<IDXGIAdapter1>(adapter);
        CreateWarpAdapter(factory);
    }

    [Test]
    public void Create_Direct3D_Debug_Layer() {
        var hr = D3D12GetDebugInterface(typeof(ID3D12Debug5).GUID, out object? debug);
        if (hr.Value == unchecked((int)0x887A002D)) Assert.Ignore("Windows Graphics Tools is unavailable.");
        hr.ThrowOnFailure();
        Own<ID3D12Debug5>(debug);
    }

    [Test]
    public void Create_D3D12_Device() => CreateDevice(CreateFactory());

    [Test]
    public void Create_D3D12_CommandQueue() => CreateQueue(CreateDevice(CreateFactory()));

    [Test]
    public unsafe void Create_D3D12_CommandList() {
        var device = CreateDevice(CreateFactory());
        var iid = typeof(ID3D12GraphicsCommandList5).GUID;
        device.CreateCommandList1(0, Direct, D3D12_COMMAND_LIST_FLAGS.D3D12_COMMAND_LIST_FLAG_NONE, &iid, out object list);
        Own<ID3D12GraphicsCommandList5>(list);
    }

    [Test]
    public unsafe void Create_D3D12_CommandAlloc() {
        var device = CreateDevice(CreateFactory());
        var iid = typeof(ID3D12CommandAllocator).GUID;
        device.CreateCommandAllocator(Direct, &iid, out object? allocator);
        Own<ID3D12CommandAllocator>(allocator);
    }

    [Test, Apartment(ApartmentState.STA)]
    public void Create_RenderForm() {
        using var form = new RenderForm("DXSharp test");
        Assert.That(form.Handle, Is.Not.EqualTo(nint.Zero));
        Assert.That(form.IsHandleCreated, Is.True);
    }

    [Test, Apartment(ApartmentState.STA)]
    public unsafe void Create_DXGI_SwapChain() {
        using var form = new Form { ClientSize = new(320, 240) };
        var factory = CreateFactory();
        var queue = CreateQueue(CreateDevice(factory));
        var desc = new DXGI_SWAP_CHAIN_DESC1 {
            Width = 320,
            Height = 240,
            Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
            SampleDesc = new() { Count = 1 },
            BufferUsage = (DXGI_USAGE)0x20u, // DXGI_USAGE_RENDER_TARGET_OUTPUT
            BufferCount = 2,
            SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD
        };
        factory.CreateSwapChainForHwnd(queue, (HWND)form.Handle, &desc, null, null, out var swapChain);
        try {
            Assert.That(swapChain, Is.InstanceOf<IDXGISwapChain4>());
        }
        finally {
            if (swapChain is not null) Marshal.ReleaseComObject(swapChain);
        }
    }
}
