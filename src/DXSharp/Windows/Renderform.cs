#pragma warning disable CS1591
#pragma warning disable IDE0051
// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

// COPYRIGHT NOTICES:
// --------------------------------------------------------------------------------
// NOTE: This code was adapted from the implementation by the SharpDX project.
// It has been ported to this "DXSharp" library implementation and it has been
// cleaned up and polished to modern C# 10.0/11.0 style and code standards ...
// Special thanks to Alexandre Mutel and all contributors who worked on both the
// SharpDX and SlimDX projects in the good ol' days!
// --------------------------------------------------------------------------------
// P.S. - @xoofx: Thanks for the fond memories, and for the inspiration! I hope my
//				  work here does your original code justice and you see this! :-) 
// --------------------------------------------------------------------------------
// ORIGINAL COPYRIGHT NOTICE:
// --------------------------------------------------------------------------------
// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
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
// Original code from SlimDX project.
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/* SlimDX Copyright Notice (original):
 * Copyright (c) 2007-2011 SlimDX Group
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#region Using Directives
using System.ComponentModel ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices ;
using System.Runtime.Versioning ;

using Windows.Win32 ;
using Windows.Win32.Foundation ;
using Windows.Win32.UI.WindowsAndMessaging ;

using DXSharp.Applications ;
using DXSharp.Windows.Win32 ;
#endregion
namespace DXSharp.Windows ;



/// <summary>
/// Default Rendering Form.
/// </summary>
/// <remarks>
/// <para><h3><b>NOTE:</b></h3></para>
/// This code was adapter from SharpDX project's SharpDX.Windows.RenderForm type.
/// It is provided as a helper class, and for its familiarity and usefulness.
/// </remarks>
[SupportedOSPlatform("windows5.1.2600")]
public class RenderForm: Form, IAppWindow {
	#region Constant Values
	const int   WM_SIZE              = 0x0005 ;
	const int   SIZE_RESTORED        = 0x0000 ;
	const int   SIZE_MINIMIZED       = 0x0001 ;
	const int   SIZE_MAXIMIZED       = 0x0002 ;
	const int   SIZE_MAXSHOW         = 0x0003 ;
	const int   SIZE_MAXHIDE         = 0x0004 ;
	const int   WM_ACTIVATEAPP       = 0x001C ;
	const int   WM_POWERBROADCAST    = 0x0218 ;
	const int   WM_MENUCHAR          = 0x0120 ;
	const int   WM_SYSCOMMAND        = 0x0112 ;
	const nuint PBT_APMRESUMESUSPEND = 0x0007 ;
	const nuint PBT_APMQUERYSUSPEND  = 0x0000 ;
	const int   SC_MONITORPOWER      = 0xF170 ;
	const int   SC_SCREENSAVE        = 0xF140 ;
	const int   WM_DISPLAYCHANGE     = 0x007E ;
	const int   MNC_CLOSE            = 0x0001 ;
	const int   WM_DPICHANGED        = 0x02E0 ;

	
	const int USER_DEFAULT_SCREEN_DPI = 96; //! set to 96
	const string DEFAULT_TITLE = AppSettings.DEFAULT_APP_NAME ;
	const ControlStyles RENDERFORM_STYLES =
		(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint) |
			ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.ResizeRedraw | ControlStyles.EnableNotifyMessage ;
	#endregion
	
	
	// Private Fields ------------------------------------------------------------------------
	Size cachedSize ;
	Size _initialSize ;
	bool isUserResizing ;
	bool allowUserResizing ;
	bool isBackgroundFirstDraw ;
	FormWindowState previousWindowState ;
	bool isSizeChangedWithoutResizeBegin ;
	
	
	// Public Properties ---------------------------------------------------------------------
	public Screen CurrentScreen  => Screen.FromControl( this ) ;
	public Screen? PrimaryScreen => Screen.PrimaryScreen ;
	public Screen[ ] AllScreens  => Screen.AllScreens ;
	Screen? _startingScreen ;
	
	// Constructors --------------------------------------------------------------------------
	/// <summary>Initializes a new instance of the <see cref="RenderForm"/> class.</summary>
	public RenderForm( ): this( DEFAULT_TITLE ) { }
	
	/// <summary>Initializes a new instance of the <see cref="RenderForm"/> class.</summary>
	/// <param name="captionText">The <see cref="IAppWindow"/> title bar ("caption") text.</param>
	public RenderForm( string captionText ):
						this( captionText, AppSettings.DEFAULT_WINDOW_SIZE ) { }
	
	public RenderForm( string captionText, USize desiredSize ) {
		ResizeRedraw = true ;
		AllowUserResizing = true ;
		Icon = LibResources.DXSharp_ICON_512 ;
		previousWindowState = FormWindowState.Normal ;
		SetStyle( RENDERFORM_STYLES, true ) ;
		SetTitle( captionText ) ;
		
		//this.AutoSizeMode = AutoSizeMode.GrowOnly ;
		this._initialSize = desiredSize ;
		this.AutoScaleMode  = AutoScaleMode.Dpi ;
		this.StartPosition  = FormStartPosition.CenterScreen ;
		this.AutoScaleDimensions = new( 96, 96 ) ;
	}
	
	
	
	Size _wndTotalSize, _targetClientSize ;
	protected override void OnActivated( EventArgs e ) {
		base.OnActivated( e ) ;
	}
	
	protected override void OnShown( EventArgs e ) {
		_startingScreen = Screen.FromControl( this ) ;
		ClientSize = _targetClientSize ;
		base.OnShown( e ) ;
	}
	protected override void OnHandleCreated( EventArgs e ) {
		AutoScaleBaseSize = _wndTotalSize = SizeFromClientSize( _initialSize ) ;
		ClientSize   = _targetClientSize  = _initialSize ;
		AutoSizeMode = AutoSizeMode.GrowOnly ;
		AutoSize = true ;
		
		base.OnHandleCreated( e ) ;
	}
	
	// Event Handlers ------------------------------------------------------------------------
	#region Event Handlers

	/// <summary>
	/// Occurs when [app activated].
	/// </summary>
	public event EventHandler< EventArgs >? AppActivated ;

	/// <summary>
	/// Occurs when [app deactivated].
	/// </summary>
	public event EventHandler< EventArgs >? AppDeactivated ;

	/// <summary>
	/// Occurs when [monitor changed].
	/// </summary>
	public event EventHandler< EventArgs >? MonitorResolutionChanged ;

	/// <summary>
	/// Occurs when [pause rendering].
	/// </summary>
	public event EventHandler< EventArgs >? PauseRendering ;

	/// <summary>
	/// Occurs when [resume rendering].
	/// </summary>
	public event EventHandler< EventArgs >? ResumeRendering ;

	/// <summary>
	/// Occurs when [screensaver].
	/// </summary>
	public event EventHandler<CancelEventArgs>? Screensaver ;

	/// <summary>
	/// Occurs when [system resume].
	/// </summary>
	public event EventHandler< EventArgs >? SystemResume ;

	/// <summary>
	/// Occurs when [system suspend].
	/// </summary>
	public event EventHandler< EventArgs >? SystemSuspend ;

	/// <summary>
	/// Occurs when [user resized].
	/// </summary>
	public event UserResizeEventHandler? UserResized ;
	
	// --------------------------------------------------------
	// New Events:
	// --------------------------------------------------------
	readonly object _lock = new( ) ;
	event ContentsResizedEventHandler? _contentsResized ;
	event WndProcDelegate? _windowsMessageReceived ;
	event DPIChangedEventHandler? _dpiChanged ;
	
	public event ContentsResizedEventHandler? ContentsResized {
		add {
			lock ( _lock ) {
				_contentsResized += value ;
			}
		}
		remove {
			lock ( _lock ) {
				_contentsResized -= value ;
			}
		}
	}
	
	public event DPIChangedEventHandler? DPIChanged {
		add {
			lock ( _lock ) {
				_dpiChanged += value ;
			}
		}
		remove {
			lock ( _lock ) {
				_dpiChanged -= value ;
			}
		}
	}
	
	public event WndProcDelegate WindowsMessageReceived {
		add {
			lock ( _lock ) {
				_windowsMessageReceived += value ;
			}
		}
		remove {
			lock ( _lock ) {
				_windowsMessageReceived -= value ;
			}
		}
	}

	#endregion

	
	// Public Methods ------------------------------------------------------------------------
	/// <summary>
	/// Gets or sets a value indicating whether this form can be resized by the user. See remarks.
	/// </summary>
	/// <remarks>
	/// This property alters <see cref="Form.FormBorderStyle"/>, 
	/// for <c>true</c> value it is <see cref="FormBorderStyle.Sizable"/>, 
	/// for <c>false</c> - <see cref="FormBorderStyle.FixedSingle"/>.
	/// </remarks>
	/// <value><c>true</c> if this form can be resized by the user (by default); otherwise, <c>false</c>.</value>
	public bool AllowUserResizing {
		get => allowUserResizing ;
		set {
			if( allowUserResizing != value ) {
				allowUserResizing = value ;
				MaximizeBox = allowUserResizing ;
				FormBorderStyle = IsFullscreen ?
									  FormBorderStyle.None : allowUserResizing ?
										  FormBorderStyle.Sizable : FormBorderStyle.FixedSingle ;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicationg whether the current render form is in fullscreen mode. See remarks.
	/// </summary>
	/// <remarks>
	/// If Toolkit is used, this property is set automatically,
	/// otherwise user should maintain it himself as it affects the behavior of <see cref="AllowUserResizing"/> property.
	/// </remarks>
	public bool IsFullscreen { get ; set ; }
	
	
	// Protected Methods ---------------------------------------------------------------------
	/// <summary>
	/// Raises the <see cref="E:System.Windows.Forms.Form.ResizeBegin"/> event.
	/// </summary>
	/// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.</param>
	protected override void OnResizeBegin( EventArgs e ) {
		isUserResizing = true ;
		base.OnResizeBegin( e ) ;
		cachedSize = Size ;
		
		OnPauseRendering( e ) ;
	}

	/// <summary>
	/// Raises the <see cref="E:System.Windows.Forms.Form.ResizeEnd"/> event.
	/// </summary>
	/// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.</param>
	protected override void OnResizeEnd( EventArgs e ) {
		base.OnResizeEnd( e ) ;

		if( isUserResizing && cachedSize != Size ) {
			OnUserResized( new(Size) ) ; // UpdateScreen();
		}
		
		isUserResizing = false ;
		OnResumeRendering( e ) ;
		
		Rectangle newRect = new( this.ClientRectangle.X, this.ClientRectangle.Y,
											cachedSize.Width, cachedSize.Height ) ;
		
		_contentsResized?.Invoke( this, new ContentsResizedEventArgs(newRect) ) ;
	}

	/// <summary>
	/// Paints the background of the control.
	/// </summary>
	/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
	protected override void OnPaintBackground( PaintEventArgs e ) {
		if( !isBackgroundFirstDraw ) {
			base.OnPaintBackground( e ) ;
			isBackgroundFirstDraw = true ;
		}
	}

	/// <summary>
	/// Raised when the client rectangle size of the form changes.
	/// </summary>
	/// <param name="e">Empty event args.</param>
	protected override void OnClientSizeChanged( EventArgs e ) {
		base.OnClientSizeChanged( e ) ;
		if( !isUserResizing && (isSizeChangedWithoutResizeBegin || cachedSize != Size) ) {
			isSizeChangedWithoutResizeBegin = false ;
			cachedSize = Size ;
			
			OnUserResized( new(cachedSize) ) ; //UpdateScreen();
		}
	}
	
	/// <summary>Override windows message loop handling.</summary>
	/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
	protected override void WndProc( ref Message m ) {
		WParam wParam = (WParam)m.WParam ;
		LParam lParam = m.LParam ;
		
		switch( m.Msg ) {
			// !TODO: Figure out what's up with this: Why is this necessary?)
			case 0x0001:
				CreateHandle( ) ;
				OnHandleCreated( EventArgs.Empty ) ;
				break ;
			
			case WM_SIZE:
				if( wParam == SIZE_MINIMIZED ) {
					previousWindowState = FormWindowState.Minimized ;
					OnPauseRendering( EventArgs.Empty ) ;
				}
				else {
                    _ = PInvoke.GetClientRect( m.HWnd, out RECT rect ) ;
					// Rapidly clicking the task bar to minimize and restore a window
					// can cause a WM_SIZE message with SIZE_RESTORED when 
					// the window has actually become minimized due to rapid change
					// so just ignore this message
                    if ( rect.bottom - rect.top is 0 ) break ;
					
					if( wParam == SIZE_MAXIMIZED ) {
						if( previousWindowState is FormWindowState.Minimized )
							OnResumeRendering( EventArgs.Empty ) ;
						
						previousWindowState = FormWindowState.Maximized ;
						OnUserResized( new(Size) ) ;   //UpdateScreen();
						cachedSize = Size ;
					}
					
					else if( wParam == SIZE_RESTORED ) {
						if( previousWindowState is FormWindowState.Minimized )
							OnResumeRendering( EventArgs.Empty ) ;
						
						if( !isUserResizing && (Size != cachedSize 
												|| previousWindowState is FormWindowState.Maximized) ) {
							previousWindowState = FormWindowState.Normal ;
							
							//! Only update when cachedSize is != 0:
							if( cachedSize != Size.Empty )
								isSizeChangedWithoutResizeBegin = true ;
						}
						else previousWindowState = FormWindowState.Normal ;
					}
				}
				break ;
			
			case WM_ACTIVATEAPP:
				if( wParam != 0 )
					OnAppActivated( EventArgs.Empty ) ;
				else
					OnAppDeactivated( EventArgs.Empty ) ;
				break ;
			
			case WM_POWERBROADCAST:
				if( wParam == PBT_APMQUERYSUSPEND ) {
					OnSystemSuspend( EventArgs.Empty ) ;
					m.Result = new nint( 1 ) ;
					return ;
				}
				if( wParam == PBT_APMRESUMESUSPEND ) {
					OnSystemResume( EventArgs.Empty ) ;
					m.Result = new nint( 1 ) ;
					return ;
				}
				break ;
			
			case WM_MENUCHAR:
				m.Result = new( MNC_CLOSE << 16 ) ;
				return ;
			
			case WM_SYSCOMMAND:
				wParam &= 0xFFF0 ;
				if( wParam == SC_MONITORPOWER || wParam == SC_SCREENSAVE ) {
					CancelEventArgs e = new( ) ;
					OnScreensaver( e ) ;
					if( e.Cancel ) {
						m.Result = nint.Zero ;
						return ;
					}
				}
				break ;
			
			case WM_DISPLAYCHANGE:
				OnMonitorResolutionChanged( EventArgs.Empty ) ;
				break ;
			
			// --------------------------------------------------------
			//! DPI Change Handling (new feature):
			case WM_DPICHANGED:
				ushort yAxis    = HIWORD( wParam ), 
					   xAxis    = LOWORD( wParam ) ;
				int g_dpi       = yAxis ;
				float newDPI    = ( (float)xAxis / USER_DEFAULT_SCREEN_DPI ) ;
				var newRect     = Marshal.PtrToStructure< Rect >( lParam ) ;
				var newPosition = new Point( newRect.Left, newRect.Top ) ;
				
				/*// Call Win32 SetWindowPos function to update the window:
				PInvoke.SetWindowPos( m.HWnd, HWND.Null,
									  newRect.Left, newRect.Top,
									  newRect.Right - newRect.Left,
									  newRect.Bottom - newRect.Top,
									  (SET_WINDOW_POS_FLAGS)(SetWindowPosFlags.SWP_NOZORDER 
															 | SetWindowPosFlags.SWP_NOACTIVATE) ) ;
															 */
				
				OnDPIChanged( new( newDPI, newRect, newPosition ) ) ;
				break ;
			/* --------------------------------
			 DPI value		Scaling percentage
				96			100%
				120			125%
				144			150%
				192			200%
			 * -------------------------------- */
			// --------------------------------------------------------
		}
		base.WndProc( ref m ) ;
		return ;


		//! Local Functions: ------------------------------------------------- *
		//! These are direct rip-offs from the Win32 headers:
		// ------------------------------------------------------------------- *
		// #define HIWORD(l)   ((WORD)((((DWORD_PTR)(l)) >> 16) & 0xffff))
		// #define LOWORD(l)   ((WORD)(((DWORD_PTR)(l)) & 0xffff))
		// ------------------------------------------------------------------- *
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static ushort HIWORD( nuint value ) => (ushort)( ( ((ulong)value) >> 16 ) & 0xFFFF ) ;
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static ushort LOWORD( nuint value ) => (ushort)( ((ulong)value) & 0xFFFF ) ;
	}
	
	protected override bool ProcessDialogKey( Keys keyData ) {
		if ( keyData is ( Keys.Menu | Keys.Alt ) or Keys.F10 )
			return true ;

		return base.ProcessDialogKey( keyData ) ;
	}
	
	protected override void OnMouseDown( MouseEventArgs e ) {
		base.OnMouseDown( e ) ;
	}

	protected override void OnNotifyMessage( Message m ) {
		base.OnNotifyMessage( m ) ;
		_windowsMessageReceived?.Invoke( this, m, (WParam)m.WParam, m.LParam ) ;
	}


	/// <summary>
	/// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
	/// </summary>
	/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
	protected override void OnLoad( EventArgs e ) {
		base.OnLoad( e ) ; // UpdateScreen();
	}


	// Private Methods -----------------------------------------------------------------------
	/// <summary>Raises the Pause Rendering event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnPauseRendering( EventArgs e ) => PauseRendering?.Invoke( this, e ) ;

	/// <summary>Raises the Resume Rendering event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnResumeRendering( EventArgs e ) => ResumeRendering?.Invoke( this, e ) ;

	/// <summary>Raises the User resized event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnUserResized( UserResizeEventArgs e ) => UserResized?.Invoke( this, e ) ;
	
	/// <summary>Raises the MonitorChanged event.</summary>
	/// <param name="e">Event arguments</param>
	void OnMonitorResolutionChanged( EventArgs e ) => 
								MonitorResolutionChanged?.Invoke( this, e ) ;

	/// <summary>Raises the On App Activated event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnAppActivated( EventArgs e ) => AppActivated?.Invoke( this, e ) ;

	/// <summary>Raises the App Deactivated event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnAppDeactivated( EventArgs e ) => AppDeactivated?.Invoke( this, e ) ;

	/// <summary>Raises the System Suspend event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnSystemSuspend( EventArgs e ) => SystemSuspend?.Invoke( this, e ) ;

	/// <summary>Raises the System Resume event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnSystemResume( EventArgs e ) => SystemResume?.Invoke( this, e ) ;

	/// <summary>Raises the <see cref="E:Screensaver"/> event.</summary>
	/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
	void OnScreensaver( CancelEventArgs e ) => Screensaver?.Invoke( this, e ) ;

	/// <summary>
	/// Raises an event indicating that the DPI of the screen has changed.
	/// </summary>
	/// <param name="e">
	/// A <see cref="DPIChangedEventArgs"/> that contains the event data, such as the
	/// new DPI value, size of the screen, and the new position of the window.
	/// </param>
	void OnDPIChanged( DPIChangedEventArgs e ) => _dpiChanged?.Invoke( this, e ) ;
	
	protected override void OnSizeChanged( EventArgs e ) { base.OnSizeChanged( e ) ; }
	
	
	void InitializeComponent( ) {
		this.SuspendLayout( ) ;
		this.ResumeLayout( false ) ;
		// ----------------------------
	}
	
	
	// IAppWindow Implementation --------------------------------------------
	public string Title => this.Text ;
	public bool IsVisible => base.Visible ;
	public bool IsChild => Parent is not null ;
	public bool IsMinimized => WindowState is FormWindowState.Minimized ;
	public bool IsMaximized => WindowState is FormWindowState.Maximized ;
	
	public void Minimize( ) => WindowState = FormWindowState.Minimized ;
	public void Maximize( ) => WindowState = FormWindowState.Maximized ;
	public void SetTitle( in string newTitle ) => this.Text = newTitle ;
	public void SetSize( in Size newSize ) => this.ClientSize = newSize ;
	public void SetPosition( in Point newLocation ) => Location = newLocation ;
	// ---------------------------------------------------------------------
	
	
	public static int ScaleValueByDPI( int value, int g_dpi = 96 ) {
		return PInvoke.MulDiv( value, g_dpi,
							   USER_DEFAULT_SCREEN_DPI ) ;
	}
	
	public static float GetDPIScaleFactor( int value, int g_dpi = 96 ) {
		float fscale = ((float)g_dpi / USER_DEFAULT_SCREEN_DPI) ;
		return (value * fscale) ;
	}
	
	
	// ========================================================================================
} ;