// COPYRIGHT NOTICES:
// --------------------------------------------------------------------------------
// NOTE: This code was adapted from the implementation by the SharpDX project.
// It has been ported to this DXSharp library implementation and it has been
// cleaned up and polished to modern C# 10.0/11.0 style and code standards ...
// Special thanks to Alexandre Mutel and all contributors who worked on both the
// SharpDX and SlimDX projects in the good ol' days!
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

#pragma warning disable CS1591

#region Using Directives
using System
/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using System.Windows;
After:
using System.ComponentModel;
*/
.
/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
Before:
using System.ComponentModel;
using System.Drawing;
using Windows.Win32;
After:
using System.Drawing;
using System.Windows;

using Windows.Win32;
*/
ComponentModel;

using Windows.Win32;
using Windows.Win32.Foundation;
using DXSharp.Applications ;

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
public class RenderForm: Form, IAppWindow {
	#region Constant Values
	const int WM_SIZE = 0x0005;
	const int SIZE_RESTORED = 0;
	const int SIZE_MINIMIZED = 1;
	const int SIZE_MAXIMIZED = 2;
	const int SIZE_MAXSHOW = 3;
	const int SIZE_MAXHIDE = 4;
	const int WM_ACTIVATEAPP = 0x001C;
	const int WM_POWERBROADCAST = 0x0218;
	const int WM_MENUCHAR = 0x0120;
	const int WM_SYSCOMMAND = 0x0112;
	const uint PBT_APMRESUMESUSPEND = 7;
	const uint PBT_APMQUERYSUSPEND = 0;
	const int SC_MONITORPOWER = 0xF170;
	const int SC_SCREENSAVE = 0xF140;
	const int WM_DISPLAYCHANGE = 0x007E;
	const int MNC_CLOSE = 1;

	const string DEFAULT_TITLE = AppSettings.DEFAULT_APP_NAME ;
	#endregion

	// Private Fields ------------------------------------------------------------------------

	Size cachedSize;
	FormWindowState previousWindowState;
	//DisplayMonitor monitor;

	bool isUserResizing;
	bool allowUserResizing;
	bool isBackgroundFirstDraw;
	bool isSizeChangedWithoutResizeBegin;


	// Constructors --------------------------------------------------------------------------

	/// <summary>
	/// Initializes a new instance of the <see cref="RenderForm"/> class.
	/// </summary>
	public RenderForm( ) : this( DEFAULT_TITLE ) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="RenderForm"/> class.
	/// </summary>
	/// <param name="text">The text.</param>
	public RenderForm( string text ) {
		//this.Text = text ;
		ResizeRedraw = true ;
		AllowUserResizing = true ;
		Icon = LibResources.DXSharp_ICON_512 ;
		previousWindowState = FormWindowState.Normal ;
		ClientSize = AppSettings.DEFAULT_WINDOW_SIZE ;
		
		SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true ) ;
		this.SetTitle( text ) ;
	}

	// Event Handlers ------------------------------------------------------------------------

	#region Event Handlers

	/// <summary>
	/// Occurs when [app activated].
	/// </summary>
	public event EventHandler<EventArgs>? AppActivated;

	/// <summary>
	/// Occurs when [app deactivated].
	/// </summary>
	public event EventHandler<EventArgs>? AppDeactivated;

	/// <summary>
	/// Occurs when [monitor changed].
	/// </summary>
	public event EventHandler<EventArgs>? MonitorChanged;

	/// <summary>
	/// Occurs when [pause rendering].
	/// </summary>
	public event EventHandler<EventArgs>? PauseRendering;

	/// <summary>
	/// Occurs when [resume rendering].
	/// </summary>
	public event EventHandler<EventArgs>? ResumeRendering;

	/// <summary>
	/// Occurs when [screensaver].
	/// </summary>
	public event EventHandler<CancelEventArgs>? Screensaver;

	/// <summary>
	/// Occurs when [system resume].
	/// </summary>
	public event EventHandler<EventArgs>? SystemResume;

	/// <summary>
	/// Occurs when [system suspend].
	/// </summary>
	public event EventHandler<EventArgs>? SystemSuspend;

	/// <summary>
	/// Occurs when [user resized].
	/// </summary>
	public event EventHandler<EventArgs>? UserResized;

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
		get => allowUserResizing;

		set {
			if( allowUserResizing != value ) {
				allowUserResizing = value;
				MaximizeBox = allowUserResizing;
				FormBorderStyle = IsFullscreen
					? FormBorderStyle.None
					: allowUserResizing ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
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
	public bool IsFullscreen { get; set; }

	// Protected Methods ---------------------------------------------------------------------

	/// <summary>
	/// Raises the <see cref="E:System.Windows.Forms.Form.ResizeBegin"/> event.
	/// </summary>
	/// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.</param>
	protected override void OnResizeBegin( EventArgs e ) {
		isUserResizing = true;

		base.OnResizeBegin( e );
		cachedSize = Size;
		OnPauseRendering( e );
	}

	/// <summary>
	/// Raises the <see cref="E:System.Windows.Forms.Form.ResizeEnd"/> event.
	/// </summary>
	/// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.</param>
	protected override void OnResizeEnd( EventArgs e ) {
		base.OnResizeEnd( e );

		if( isUserResizing && cachedSize != Size ) {
			OnUserResized( e );
			// UpdateScreen();
		}

		isUserResizing = false;
		OnResumeRendering( e );
	}

	/// <summary>
	/// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
	/// </summary>
	/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
	protected override void OnLoad( EventArgs e ) => base.OnLoad( e );// UpdateScreen();

	/// <summary>
	/// Paints the background of the control.
	/// </summary>
	/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
	protected override void OnPaintBackground( PaintEventArgs e ) {
		if( !isBackgroundFirstDraw ) {
			base.OnPaintBackground( e );
			isBackgroundFirstDraw = true;
		}
	}

	protected override void OnClientSizeChanged( EventArgs e ) {
		base.OnClientSizeChanged( e );
		if( !isUserResizing && (isSizeChangedWithoutResizeBegin || cachedSize != Size) ) {
			isSizeChangedWithoutResizeBegin = false;
			cachedSize = Size;
			OnUserResized( EventArgs.Empty );
			//UpdateScreen();
		}
	}

	/// <summary>
	/// Override windows message loop handling.
	/// </summary>
	/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
	protected override void WndProc( ref Message m ) {
		long wparam = m.WParam.ToInt64();

		switch( m.Msg ) {
			// !TODO: Figure out what's up with this:
			// (Why is this necessary?)
			case 0x0001:
				CreateHandle( ) ;
				OnHandleCreated( EventArgs.Empty ) ;
				break;
			
			
			case WM_SIZE:
				if( wparam == SIZE_MINIMIZED ) {
					previousWindowState = FormWindowState.Minimized;
					OnPauseRendering( EventArgs.Empty );
				}
				else {
					RECT rect;
					_ = PInvoke.GetClientRect( (HWND)m.HWnd, out rect );
					if( rect.bottom - rect.top == 0 ) {
						// Rapidly clicking the task bar to minimize and restore a window
						// can cause a WM_SIZE message with SIZE_RESTORED when 
						// the window has actually become minimized due to rapid change
						// so just ignore this message
					}
					else if( wparam == SIZE_MAXIMIZED ) {
						if( previousWindowState == FormWindowState.Minimized )
							OnResumeRendering( EventArgs.Empty );

						previousWindowState = FormWindowState.Maximized;

						OnUserResized( EventArgs.Empty );
						//UpdateScreen();
						cachedSize = Size;
					}
					else if( wparam == SIZE_RESTORED ) {
						if( previousWindowState == FormWindowState.Minimized )
							OnResumeRendering( EventArgs.Empty );

						if( !isUserResizing && (Size != cachedSize || previousWindowState == FormWindowState.Maximized) ) {
							previousWindowState = FormWindowState.Normal;

							// Only update when cachedSize is != 0

							/* Unmerged change from project 'DXSharp (net7.0-windows10.0.22621.0)'
							Before:
														if (cachedSize != Size.Empty)
							After:
														if( cachedSize != Size.Empty)
							*/
							if( cachedSize != Size.Empty ) {
								isSizeChangedWithoutResizeBegin = true;
							}
						}
						else
							previousWindowState = FormWindowState.Normal;
					}
				}
				break;
			case WM_ACTIVATEAPP:
				if( wparam != 0 )
					OnAppActivated( EventArgs.Empty );
				else
					OnAppDeactivated( EventArgs.Empty );
				break;
			case WM_POWERBROADCAST:
				if( wparam == PBT_APMQUERYSUSPEND ) {
					OnSystemSuspend( EventArgs.Empty );
					m.Result = new IntPtr( 1 );
					return;
				}
				else if( wparam == PBT_APMRESUMESUSPEND ) {
					OnSystemResume( EventArgs.Empty );
					m.Result = new IntPtr( 1 );
					return;
				}
				break;
			case WM_MENUCHAR:
				m.Result = new IntPtr( MNC_CLOSE << 16 ); // IntPtr(MAKELRESULT(0, MNC_CLOSE));
				return;
			case WM_SYSCOMMAND:
				wparam &= 0xFFF0;
				if( wparam is SC_MONITORPOWER or SC_SCREENSAVE ) {
					var e = new CancelEventArgs();
					OnScreensaver( e );
					if( e.Cancel ) {
						m.Result = IntPtr.Zero;
						return;
					}
				}
				break;
			case WM_DISPLAYCHANGE:
				OnMonitorChanged( EventArgs.Empty );
				break;
		}

		base.WndProc( ref m );
	}

	protected override bool ProcessDialogKey( Keys keyData ) {
		if( keyData is (Keys.Menu | Keys.Alt) or Keys.F10 )
			return true;

		return base.ProcessDialogKey( keyData );
	}

	
	// Private Methods -----------------------------------------------------------------------

	/// <summary>Raises the Pause Rendering event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnPauseRendering( EventArgs e ) => PauseRendering?.Invoke( this, e );

	/// <summary>Raises the Resume Rendering event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnResumeRendering( EventArgs e ) => ResumeRendering?.Invoke( this, e );

	/// <summary>Raises the User resized event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnUserResized( EventArgs e ) => UserResized?.Invoke( this, e );

	/// <summary>Raises the MonitorChanged event.</summary>
	/// <param name="e">Event arguments</param>
	void OnMonitorChanged( EventArgs e ) => MonitorChanged?.Invoke( this, e );

	/// <summary>Raises the On App Activated event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnAppActivated( EventArgs e ) => AppActivated?.Invoke( this, e );

	/// <summary>Raises the App Deactivated event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnAppDeactivated( EventArgs e ) => AppDeactivated?.Invoke( this, e );

	/// <summary>Raises the System Suspend event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnSystemSuspend( EventArgs e ) => SystemSuspend?.Invoke( this, e );

	/// <summary>Raises the System Resume event.</summary>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void OnSystemResume( EventArgs e ) => SystemResume?.Invoke( this, e );

	/// <summary>Raises the <see cref="E:Screensaver"/> event.</summary>
	/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
	void OnScreensaver( CancelEventArgs e ) => Screensaver?.Invoke( this, e ) ;
	
	void InitializeComponent( ) {
		this.SuspendLayout( ) ;
		
		//! --- RenderForm Settings ---
		this.ClientSize = AppSettings.DEFAULT_WINDOW_SIZE ;
		this.Font         = new( AppSettings.Style.DEFAULT_FONT_NAME,
								 AppSettings.Style.DEFAULT_FONT_SIZE,
								 FontStyle.Regular, GraphicsUnit.Point ) ;
		this.Text = AppSettings.DEFAULT_APP_NAME ;
		this.Name = nameof( RenderForm ) ;
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
	public void SetTitle( in string newTitle ) => Text = newTitle ;
	public void SetSize( in Size newSize ) => ClientSize = newSize ;
	public void SetPosition( in Point newLocation ) => Location = newLocation ;
	// ---------------------------------------------------------------------
};