#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DXSharp.DXGI;
#endregion

namespace Windows.Win32.Graphics.Dxgi.Common
{
	internal partial struct DXGI_RATIONAL
	{
		internal DXGI_RATIONAL( uint numerator, uint denominator )
		{
			this.Numerator = numerator;
			this.Denominator = denominator;
		}

		public static implicit operator DXGI_RATIONAL( Rational r ) =>
			new DXGI_RATIONAL( r.Numerator, r.Denominator );

		public static implicit operator Rational( DXGI_RATIONAL r ) =>
			new Rational( r.Numerator, r.Denominator );
	};
}
