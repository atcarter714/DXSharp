#region Using Directives
using Windows.Win32 ;
using Windows.Win32.Graphics.Dxgi ;
using Windows.Win32.Graphics.Direct3D ;
using DXSharp.Windows.Win32.XTensions ;

using static Windows.Win32.PInvoke ;
#endregion
namespace DXSharp.Windows.Win32.Helpers ;


/// <summary>Helper class for creating, running and managing a DirectX12 application.</summary>
public abstract class DXApp: IDisposable {
    static D3D_FEATURE_LEVEL _d3dFeatures =
        D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_0 ;
    
    string _title ;
    bool _useWarpDevice ;
    readonly string _assetsPath ;
    readonly float _aspectRatio ;
    readonly uint _width, _height ;

    public string Title => _title ;
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public string AssetsPath => _assetsPath ;
    public Size Size => new( (int)_width, (int)_height ) ;
    

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
    
    void ParseCommandLineArgs( in string[] args ) {
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
    public static void GetHardwareAdapter( IDXGIFactory2? dxgiFactory,
                                                out IDXGIAdapter1? dxgiAdapter,
                                                bool requestHighPerformanceAdapter ) {
        ArgumentNullException.ThrowIfNull( dxgiFactory, nameof(dxgiFactory) ) ;
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

    public virtual void OnKeyDown( byte wParam ) {
        switch ( wParam ) {
            case (byte)Keys.Escape:
                CloseWindow( Win32Application._HWnd ) ;
                Application.Exit( ) ;
                break ;
        }
    }
    public virtual void OnKeyUp( byte wParam ) { }

    public abstract void OnUpdate() ;
    public abstract void OnRender() ;
} ;
