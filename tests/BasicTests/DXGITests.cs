#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;

using DXSharp;
using DXSharp.DXGI;
using DXSharp.DXGI.XTensions;

using NUnit.Framework.Internal;
#endregion

namespace BasicTests.DXGI;

internal class DXGITests
{
	[Test]
	public void TestDXGIStructs()
	{
		ModeDescription dmode;
		ModeDescription dmode_B = new ModeDescription(
			1024, 768, 60, Format.R8G8B8A8_UNORM_SRGB,
			ScanlineOrder.Unspecified, ScalingMode.Centered );

		ModeDescription1 dmode1 = new ModeDescription1( 
			1024, 768, 60, Format.R8G8B8A8_UNORM_SRGB, 
			ScanlineOrder.Unspecified, ScalingMode.Centered, false );

		// Cast from ModeDescription1 to ModeDescription:
		// Assert that the cast data is equal to dmode_b ...
		// We must verify this since we're writing into its
		// memory with a pointer for fast, streamlined code:
		dmode = (ModeDescription) dmode1;
		Assert.That( dmode, Is.EqualTo( dmode_B) );
	}

	[Test]
	public void TestRationals()
	{
		// Assignment by constructor, from uint and tuple
		Rational r1 = new Rational( 60 ), r2 = new Rational( 60, 1 ), r3 = 60, r4 = (60, 1);

		// Check equality operator
		bool all_the_same = (r1 == r2) && (r3 == r4) && (r1 == r3);
		Assert.IsTrue( all_the_same, 
			"Rationals intended to be 60/1 are not equal", new[] { r1, r2, r3, r4 } );

		// Check basic rational reduction:
		r1 = (120, 2);
		r1.Reduce();
		Assert.IsTrue( r1 == r2 );
	}
}