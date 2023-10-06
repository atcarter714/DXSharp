using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Dxgi;
using DXGI;
using static Windows.Win32.PInvoke;

using DXSharp.DXGI;
using static DXSharp.DXGI.DXGIFunctions;

namespace DXSharp.Windows.Win32.Helpers;

public class DXApp: IDisposable
{
    private readonly uint _width;
    private readonly uint _height;
    private string _title;
    private readonly string _assetsPath;
    private readonly float _aspectRatio;
    private bool _useWarpDevice;

    public string AssetsPath => _assetsPath ;
    public string Title => _title ;
    public Size Size => new( (int)_width, (int)_height ) ;
    public int Width { get; set; }
    public int Height { get; set; }

    public DXApp(uint width, uint height, string name)
    {
        _width = width;
        _height = height;
        _title = name;
        _assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
        _aspectRatio = width / (float)height;
    }

    public string GetAssetFullPath(string assetName)
    {
        return Path.Combine(_assetsPath, assetName);
    }

    public static void GetHardwareAdapter<T>( IDXGIFactory2?     dxgiFactory,
                                              out IDXGIAdapter1? dxgiAdapter ,
                                              bool               requestHighPerformanceAdapter ) 
                                                    //where T: class, IDXGIFactory2, new( ) 
    {
        dxgiAdapter = null;
        var adapterIndex = 0U;
        while ( dxgiFactory.EnumAdapters1(adapterIndex, out var adapter).Succeeded )
        {
            if (IsHardwareAdapterSupported(adapter))
            {
                dxgiAdapter = adapter;
                break;
            }
            adapterIndex++;
        }
    }

    private static bool IsHardwareAdapterSupported(IDXGIAdapter1 adapter)
    {
        adapter.GetDesc1(out var desc);

        if ((desc.Flags & (uint)DXGI_ADAPTER_FLAG3.DXGI_ADAPTER_FLAG3_SOFTWARE) != 0)
        {
            // Don't select the Basic Render Driver adapter.
            // If you want a software adapter, pass in "/warp" on the command line.
            return false;
        }

        // Check to see whether the adapter supports Direct3D 12, but don't create the
        // actual device yet.
        return PInvoke.D3D12CreateDevice(adapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, Guid.Empty, out _) == 0;
    }

    public void SetCustomWindowText(string text)
    {
        var windowText = $"{_title}: {text}";
        PInvoke.SetWindowText(Win32Application.HWnd, windowText);
    }

    public void ParseCommandLineArgs( string[] args )
    {
        foreach (var arg in args)
        {
            if (arg.Equals("-warp", StringComparison.OrdinalIgnoreCase) || arg.Equals("/warp", StringComparison.OrdinalIgnoreCase))
            {
                _useWarpDevice = true;
                _title += " (WARP)";
            }
        }
    }

    public void Dispose()
    {
        // Add any necessary cleanup code here.
    }

    public void OnKeyDown( byte wParam ) {

    }
    public void OnKeyUp( byte wParam ) {

    }

    public void OnUpdate()
    {
        throw new NotImplementedException();
    }

    public void OnRender()
    {
        throw new NotImplementedException();
    }
}
