#region Using Directives
using System.Diagnostics ;
using System.Runtime.CompilerServices ;
#endregion
namespace DXSharp.Applications ;


// ------------------------------------------------------------
// Time-Keeping Interfaces:
// ------------------------------------------------------------

public interface ITimeSnapshot {
	TimeSpan DeltaTime { get ; }
	TimeSpan Elapsed { get ; }
	TimeSpan LastFrame { get ; }
	TimeSpan TotalTime { get ; }
	TimeSpan AverageFrameTime { get ; }
} ;


/// <summary>Contract for a time service provider in a looping application.</summary>
public interface ITimeProvider: ITimeSnapshot {
	int UpdateDelay { get ; set ; }
	float ActualDeltaTime { get ; }
	Action< float >? OnTickAction { get ; set ; }
	
	void Start( ) ;
	void Stop( ) ;
	void Reset( ) ;
	Task RunAsync( ) ;
	
	void Update( ) ;
	Timing GetTimingInfo( ) ;
} ;


// ------------------------------------------------------------
// Game Timing Mechanism:
// ------------------------------------------------------------

public class Time: ITimeProvider {
	object _lock = new( ) ;
	Stopwatch _stopwatch = new( ) ;
	CancellationTokenSource _tokenSource ;
	
	ulong _frameCount         = 0x0000L ;
	long  _frameTimestamp     = 0x00000000L,
		  _lastFrameTimestamp = Stopwatch.GetTimestamp( ) ;
	public long LastTimestamp => _lastFrameTimestamp ;
	public ulong FrameCount => _frameCount ;
	
	int _currentFrame = 0 ;
	const int _AVG_FRAMES = 60 ;
	DateTime _startTime = DateTime.MinValue ;
	float[ ] _frameTimes = new float[ _AVG_FRAMES ] ;
	TimeSpan _deltaTime, _lastFrame, _averageFrameTimeCache ;
	
	
	public TimeSpan DeltaTime {
		get { lock(_lock) return _deltaTime ; }
		protected set { lock(_lock) _deltaTime = value ; }
	}
	public TimeSpan Elapsed => _stopwatch.Elapsed ;
	public TimeSpan LastFrame {
		get { lock(_lock) return _lastFrame ; }
		protected set { lock(_lock) _lastFrame = value ; }
	}
	public TimeSpan TotalTime => DateTime.Now - _startTime ;
	public TimeSpan AverageFrameTime => _getAverageFrameTime( ) ;
	TimeSpan _getAverageFrameTime( ) {
		//! TODO: Implement this with SIMD intrinsics
		//  This "unsafe" version is better than Linq or regular array indexing
		//  but it's still only using single-data-point operations ...
		lock( _lock ) {
			float avg = 0f ;
			Span< float > frameTimes = _frameTimes.AsSpan( ) ;
			unsafe { fixed ( float* p = frameTimes ) {
					float* ptr = p ;
					for ( int i = 0; i < _AVG_FRAMES; ++i, ++ptr ) {
						avg += *ptr ;
					}
				}
			}
			return _averageFrameTimeCache =
					   TimeSpan.FromSeconds( avg / _AVG_FRAMES ) ;
		}
	}

	
	public int UpdateDelay { get ; set ; } = 0 ;
	public Action< float >? OnTickAction { get ; set ; } = ( float delta ) => { } ;
	
	/// <summary>Computes actual delta time <b>now</b> (in seconds) since the last frame.</summary>
	/// <remarks>
	/// <b>NOTE:</b> Not the same as <see cref="DeltaTime"/> ...<para/>
	/// Ordinary time snapshots are taken at the end of each frame, so checking
	/// <see cref="DeltaTime"/> will return the delta time for the previous update.
	/// This may or may not be accurate depending on the timing of the update.
	/// <b>ActualDeltaTime</b> will always return the delta time for the current
	/// moment in time since the last update, but has to be computed. So it's best
	/// to only use this when you actually need it.
	/// </remarks>
	public float ActualDeltaTime => (float)Stopwatch.GetElapsedTime( _lastFrameTimestamp, 
															  ( _frameTimestamp = Stopwatch.GetTimestamp() ) )
																			.TotalSeconds;
	
	public CancellationTokenSource TaskCancellationSource => _tokenSource ;


	public Time( ) => _tokenSource = new( ) ;
	public Time( CancellationTokenSource cancelSrc ) => this._tokenSource = cancelSrc ;
	
	protected virtual void onAwake( ) {
		_tokenSource ??= new( ) ;
		_stopwatch ??= new( ) ;
	}
	
	

	public Task RunAsync( ) {
		var _runTask = Task.Run(( ) => {
			Start( ) ;
			while ( !_tokenSource.IsCancellationRequested ) {
				 Update( ) ;
				 
				 // Optional: Slight delay to avoid tight-looping
				 if( UpdateDelay > 0 ) Task.Delay( UpdateDelay )
												.Wait( ) ;
			}
			Stop( ) ;
		}, _tokenSource.Token ) ;
		return _runTask ;
	}
	
	public void Start( ) {
		onAwake( ) ;
		
		if ( _stopwatch.IsRunning ) return ;
		_startTime = DateTime.Now ;
		_stopwatch.Start( ) ;
	}
	
	public void Stop( ) {
		_tokenSource?.Cancel( ) ;
		_stopwatch?.Stop( ) ;
	}
	
	public void Reset( ) {
		_stopwatch?.Reset( ) ;
		_tokenSource?.Cancel( ) ;
		
		_currentFrame = 0 ;
		_startTime = DateTime.MinValue ;
		_deltaTime = _lastFrame = _averageFrameTimeCache = TimeSpan.Zero ;
		_frameCount = (ulong)(_frameTimestamp = _lastFrameTimestamp = 0x0000L) ;
		
		unsafe {
			fixed ( float* ptr = _frameTimes )
				Unsafe.InitBlock( ptr, 0x00,
								  (_AVG_FRAMES * sizeof(float)) ) ;
		}
	}
	
	public void Update( ) {
		lock ( _lock ) { unchecked {
			++_frameCount ;
			_lastFrameTimestamp = _frameTimestamp ;
			_frameTimestamp = Stopwatch.GetTimestamp( ) ;
			
			_frameTimes[ _currentFrame ] = (float)_deltaTime.TotalSeconds ;
			_currentFrame = ( ++_currentFrame % _AVG_FRAMES ) ;
			
			_lastFrame = _deltaTime ;
			_deltaTime = _stopwatch.Elapsed ;
		}}
		OnTickAction?.Invoke( (float)_deltaTime.TotalSeconds ) ;
	}
	
	public Timing GetTimingInfo( ) => new Timing {
		// D - E - L - T - A !!! :-)
		DeltaTime        = this.DeltaTime,
		Elapsed          = this.Elapsed,
		LastFrame        = this.LastFrame,
		TotalTime        = this.TotalTime,
		AverageFrameTime = _getAverageFrameTime( ),
	} ;
} ;

// ------------------------------------------------------------
// Timing Data Structures:
// ------------------------------------------------------------

public readonly struct Timing: ITimeSnapshot {
	public TimeSpan DeltaTime { get ; init ; }
	public TimeSpan Elapsed { get ; init ; }
	public TimeSpan LastFrame { get ; init ; }
	public TimeSpan TotalTime { get ; init ; }
	public TimeSpan AverageFrameTime { get ; init ; }
	
	public Timing( in TimeSpan delta        = default, 
				   in TimeSpan elapsed      = default, 
				   in TimeSpan lastFrame    = default, 
				   in TimeSpan totalTime    = default, 
				   in TimeSpan averageFrame = default ) {
		DeltaTime        = delta ;
		Elapsed      = elapsed ;
		LastFrame    = lastFrame ;
		TotalTime    = totalTime ;
		AverageFrameTime = averageFrame ;
	}
} ;

public ref struct TimingRef {
	public TimeSpan Delta ;
	public TimeSpan Elapsed ;
	public TimeSpan LastFrame ;
	public TimeSpan TotalTime ;
	public TimeSpan AverageFrame ;
} ;

// ============================================================


//! Highly Experimental Stuff: --------------------------------
//! TODO: Finish implementing this frame counter with SIMD intrinsics 
unsafe struct FrameCounter {
	const ulong AVERAGING_FRAMES = 60 ;
	int _current ;
	ulong _frameCount, _averagingFrames ;
	fixed ulong _frameTimes[ (int)AVERAGING_FRAMES ] ;
	
	public void Reset( ) {
		_current = 0 ;
		_frameCount = 0 ;
		_averagingFrames = 0 ;
		fixed( ulong* p = _frameTimes) {
			Unsafe.InitBlock( (void*)p, 0x0000,
							  (uint)AVERAGING_FRAMES ) ;
		}
	}
	
	public void Tick( float delta ) {
		++_frameCount ;
		++_averagingFrames ;
		_frameTimes[ ((++_current) % (int)AVERAGING_FRAMES) ] = 
								(ulong)Stopwatch.GetTimestamp( ) ;
		
		if ( _averagingFrames >= AVERAGING_FRAMES ) {
			Reset( ) ;
		}
	}
	
} ;