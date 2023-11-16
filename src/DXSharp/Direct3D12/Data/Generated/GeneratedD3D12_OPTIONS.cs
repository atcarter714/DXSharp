#region Using Directives
using DXSharp ;
using DXSharp.Direct3D12 ;
using DXSharp.DXGI ;
#endregion
namespace Windows.Win32.Graphics.Direct3D12 ;


[CsWin32, EquivalentOf( typeof( D3D12Options ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS {
	public static implicit operator D3D12Options( in D3D12_FEATURE_DATA_D3D12_OPTIONS options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS* pOptions = &options ) {
				return *(D3D12Options*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS( in D3D12Options options ) {
		unsafe {
			fixed ( D3D12Options* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options1 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS1 {
	public static implicit operator D3D12Options1( in D3D12_FEATURE_DATA_D3D12_OPTIONS1 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS1* pOptions = &options ) {
				return *(D3D12Options1*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS1( in D3D12Options1 options ) {
		unsafe {
			fixed ( D3D12Options1* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS1*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options2 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS2 {
	public static implicit operator D3D12Options2( in D3D12_FEATURE_DATA_D3D12_OPTIONS2 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS2* pOptions = &options ) {
				return *(D3D12Options2*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS2( in D3D12Options2 options ) {
		unsafe {
			fixed ( D3D12Options2* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS2*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options3 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS3 {
	public static implicit operator D3D12Options3( in D3D12_FEATURE_DATA_D3D12_OPTIONS3 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS3* pOptions = &options ) {
				return *(D3D12Options3*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS3( in D3D12Options3 options ) {
		unsafe {
			fixed ( D3D12Options3* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS3*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options4 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS4 {
	public static implicit operator D3D12Options4( in D3D12_FEATURE_DATA_D3D12_OPTIONS4 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS4* pOptions = &options ) {
				return *(D3D12Options4*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS4( in D3D12Options4 options ) {
		unsafe {
			fixed ( D3D12Options4* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS4*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options5 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS5 {
	public static implicit operator D3D12Options5( in D3D12_FEATURE_DATA_D3D12_OPTIONS5 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS5* pOptions = &options ) {
				return *(D3D12Options5*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS5( in D3D12Options5 options ) {
		unsafe {
			fixed ( D3D12Options5* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS5*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options6 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS6 {
	public static implicit operator D3D12Options6( in D3D12_FEATURE_DATA_D3D12_OPTIONS6 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS6* pOptions = &options ) {
				return *(D3D12Options6*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS6( in D3D12Options6 options ) {
		unsafe {
			fixed ( D3D12Options6* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS6*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options7 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS7 {
	public static implicit operator D3D12Options7( in D3D12_FEATURE_DATA_D3D12_OPTIONS7 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS7* pOptions = &options ) {
				return *(D3D12Options7*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS7( in D3D12Options7 options ) {
		unsafe {
			fixed ( D3D12Options7* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS7*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options8 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS8 {
	public static implicit operator D3D12Options8( in D3D12_FEATURE_DATA_D3D12_OPTIONS8 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS8* pOptions = &options ) {
				return *(D3D12Options8*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS8( in D3D12Options8 options ) {
		unsafe {
			fixed ( D3D12Options8* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS8*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options9 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS9 {
	public static implicit operator D3D12Options9( in D3D12_FEATURE_DATA_D3D12_OPTIONS9 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS9* pOptions = &options ) {
				return *(D3D12Options9*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS9( in D3D12Options9 options ) {
		unsafe {
			fixed ( D3D12Options9* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS9*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf( typeof( D3D12Options10 ) )]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS10 {
	public static implicit operator D3D12Options10( in D3D12_FEATURE_DATA_D3D12_OPTIONS10 options ) {
		unsafe {
			fixed ( D3D12_FEATURE_DATA_D3D12_OPTIONS10* pOptions = &options ) {
				return *(D3D12Options10*)pOptions ;
			}
		}
	}
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS10( in D3D12Options10 options ) {
		unsafe {
			fixed ( D3D12Options10* pOptions = &options ) {
				return *(D3D12_FEATURE_DATA_D3D12_OPTIONS10*)pOptions ;
			}
		}
	}
} ;

[CsWin32, EquivalentOf(typeof(D3D12Options11))]
public partial struct D3D12_FEATURE_DATA_D3D12_OPTIONS11 {
	public static implicit operator D3D12Options11( D3D12_FEATURE_DATA_D3D12_OPTIONS11 options ) =>
		new D3D12Options11( options.AtomicInt64OnDescriptorHeapResourceSupported ) ;
	
	public static implicit operator D3D12_FEATURE_DATA_D3D12_OPTIONS11( D3D12Options11 options ) =>
		new D3D12_FEATURE_DATA_D3D12_OPTIONS11 {
			AtomicInt64OnDescriptorHeapResourceSupported = options.AtomicInt64OnDescriptorHeapResourceSupported
		} ;
} ;