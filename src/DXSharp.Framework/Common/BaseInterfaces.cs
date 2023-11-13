namespace DXSharp.Framework.Common ;


public interface IInitialize {
	bool IsInitialized { get ; }
	void Initialize( ) ;
} ;
public interface IInitialize< T >: IInitialize { void Initialize( T arg ) ; } ;
public interface IInitializeWithArgs: IInitialize { void Initialize( __arglist ) ; } ;
public interface IInitializeWithParams: IInitialize { void Initialize< T >( params T[ ] @params ) ; } ;

public interface IRun {
	bool IsRunning { get ; }
	void Run( ) ;
} ;
public interface IRun< T > : IRun { void Run( T arg ) ; } ;
public interface IRunWithArgs: IRun { void Run( __arglist ) ; } ;
public interface IRunWithParams: IRun { void Run< T >( params T[ ] @params ) ; } ;

public interface ILoad {
	bool IsLoaded { get ; }
	void Load( ) ;
} ;
public interface IUnload: ILoad { void Unload( ) ; } ;

public interface IDraw { void Draw( ) ; } ;
public interface IUpdate { void Update( ) ; } ;

public interface ITick { void Tick( float delta ) ; } ;

public interface IStep { void Step( ) ; } ;
public interface IStep< in TIn, out TOut > { TOut Step( TIn delta ) ; } ;

public interface IStart { void Start( ) ; } ;
public interface IStop { void Stop( ) ; } ;

public interface IPause {
	bool IsPaused { get ; }
	void Pause( ) ;
} ;
public interface IResume: IPause { void Resume( ) ; } ;

public interface IShutdown { void Shutdown( ) ; } ;

public interface IAbort {
	bool IsAborted { get ; }
	bool IsAborting { get ; }
	void Abort( ) ;
} ;