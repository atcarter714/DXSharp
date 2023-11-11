#region Using Directives

using System.Runtime.Versioning ;
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Direct3D ;
using DXSharp.Applications ;
using DXSharp.DXGI ;
using DXSharp.Windows.Win32.XTensions ;

using static Windows.Win32.PInvoke ;
#endregion
namespace DXSharp.Windows.Win32.Helpers ;

//! Why the hell do I have two abstract "DXApp" classes?!

/// <summary>Helper class for creating, running and managing a DirectX12 application.</summary>
[SupportedOSPlatform("windows7.0")]
public abstract class DXApp: IDisposable {
    static D3D_FEATURE_LEVEL _d3dFeatures =
        D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_0 ;
    
    string _title ;
    float _aspectRatio ;
    bool _useWarpDevice ;
    readonly string _assetsPath ;
    readonly uint _width, _height ;

    public string Title => _title ;
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public string AssetsPath => _assetsPath ;
    public Size Size => new( (int)_width, (int)_height ) ;
    
    public float AspectRatio => ( _aspectRatio =
                                      ( (float)_width / (float)_height ) ) ;
    

    public DXApp( uint width, uint height, string name ) {
        _title = name ;
        _width = width ;
        _height = height ;
        _aspectRatio = width / (float)height ;
        _assetsPath = Path.Combine( Directory.GetCurrentDirectory(), "Assets" ) ;
    }
    ~DXApp( ) => this.Dispose( false ) ;
    
    
    // --------------------------------------
    // Instance Methods ::
    // --------------------------------------
    
    protected virtual void ParseCommandLineArgs( in string[ ] args ) {
        foreach ( var arg in args ) {
            if ( arg.Equals( "-warp", StringComparison.OrdinalIgnoreCase )
                 || arg.Equals( "/warp", StringComparison.OrdinalIgnoreCase ) ) {
                _useWarpDevice =  true ;
                _title         += " (WARP)" ;
            }
        }
    }
    
    public void SetCustomWindowText( in string text ) {
        var windowText = $"{_title}: {text}";
        SetWindowText( Win32Application._HWnd, windowText ) ;
    }
    
    public string GetAssetFullPath( string assetName ) => 
                    Path.Combine( _assetsPath, assetName ) ;

    
    // --------------------------------------
    // Static Methods ::
    [SupportedOSPlatform("windows8.0")]
    public static void GetHardwareAdapter( IDXGIFactory2? dxgiFactory,
                                           out IDXGIAdapter1? dxgiAdapter,
                                           bool requestHighPerformanceAdapter ) {
#if DEBUG || DEBUG_COM || DEV_BUILD
        ArgumentNullException.ThrowIfNull( dxgiFactory, nameof(dxgiFactory) ) ;
#endif
        
        dxgiAdapter = null ;
        var adapterIndex = 0U ;
        while ( dxgiFactory.EnumAdapters1( adapterIndex, out var adapter ).Succeeded ) {
            if ( IsHardwareAdapterSupported(adapter) ) {
                dxgiAdapter = adapter ;
                break ;
            }
            ++adapterIndex ;
        }
    }
    
    static bool IsHardwareAdapterSupported( IDXGIAdapter1 adapter ) {
        adapter.GetDesc1( out var desc ) ;
        
        if ( (desc.Flags & 
              (uint)DXGI_ADAPTER_FLAG3.DXGI_ADAPTER_FLAG3_SOFTWARE) is not 0 ) {
            // Don't select the Basic Render Driver adapter.
            // If you want a software adapter, pass in "/warp" on the command line.
            return false ;
        }

        //! Check if adapter supports D3D12b (but don't create it yet) ...
        return D3D12CreateDevice( adapter, _d3dFeatures, 
                                  Guid.Empty, out _ )
                                        .Succeeded ;
    }
    // --------------------------------------
    
    
    protected void Dispose( bool disposing ) {
        //! release native resources ::
        
        if ( disposing ) {
            //! managed state cleanup ::
        }
        GC.SuppressFinalize( this ) ;
    }
    public void Dispose( ) => this.Dispose( true ) ;

    // --------------------------------------
    [SupportedOSPlatform("windows5.0")]
    public virtual void OnKeyDown( byte wParam ) {
        //(byte)Keys.Escape:
        switch ( (Keys)wParam ) {
            case Keys.Escape: 
                CloseWindow( Win32Application._HWnd ) ;
                Application.Exit( ) ;
                break ;
        }
    }
    public virtual void OnKeyUp( byte wParam ) { }

    public abstract void OnUpdate( ) ;
    public abstract void OnRender( ) ;
    // ======================================================================================
} ;
