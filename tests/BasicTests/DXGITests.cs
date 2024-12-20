﻿#region Using Directives

/* Unmerged change from project 'BasicTests (net6.0-windows10.0.22621.0)'
Before:
using System;
using System.Collections.Generic;
using System.Linq;
After:
using DXSharp;
using DXSharp.DXGI;
using DXSharp.DXGI.XTensions;
*/
using
/* Unmerged change from project 'BasicTests (net6.0-windows10.0.22621.0)'
Before:
using DXSharp;
using DXSharp.DXGI;
using DXSharp.DXGI.XTensions;

using NUnit.Framework.Internal;
After:
using NUnit.Framework.Internal;

using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
*/
DXSharp.DXGI;
#endregion

namespace BasicTests;


[TestFixture( Author = "Aaron T. Carter", Category = "DXGI Interop",
	Description = "Testing DXSharp.DXGI interop code", TestName = "DXSharp.DXGI" )]
internal class DXGITests
{
	#region DXGI Structures

	[Test( TestOf = typeof( ModeDescription ),
		Description = "Test that DXGI.ModeDescription constructors, operators, etc work correctly" )]
	public void Test_DXGI_ModeDescriptions() {
		ModeDescription dmode;
		var dmode_B = new ModeDescription(
			1024, 768, 60, Format.R8G8B8A8_UNORM_SRGB,
			ScanlineOrder.Unspecified, ScalingMode.Centered );

		var dmode1 = new ModeDescription1(
			1024, 768, 60, Format.R8G8B8A8_UNORM_SRGB,
			ScanlineOrder.Unspecified, ScalingMode.Centered, false );

		// Cast from ModeDescription1 to ModeDescription:
		// Assert that the cast data is equal to dmode_b ...
		// We must verify this since we're writing into its
		// memory with a pointer for fast, streamlined code:
		dmode = (ModeDescription)dmode1;
		Assert.That( dmode, Is.EqualTo( dmode_B ) );
	}

	[Test( TestOf = typeof( ModeDescription1 ),
		Description = "Test that DXGI.ModeDescription1 constructors, operators, etc work correctly" )]
	public void Test_DXGI_ModeDescription1s() {
		// Create an empty ModeDescription1 and a target
		// to test against for internal data equality ...
		ModeDescription1 dmode1;
		var dmode1_B = new ModeDescription1(
			1024, 768, 60, Format.R8G8B8A8_UNORM_SRGB,
			ScanlineOrder.Unspecified, ScalingMode.Centered, false );

		// Create a regular ModeDescription:
		var dmode = new ModeDescription(
			1024, 768, 60, Format.R8G8B8A8_UNORM_SRGB,
			ScanlineOrder.Unspecified, ScalingMode.Centered );

		// Convert from ModeDescription to ModeDescription1:
		// Assert that the cast data is equal to dmode1_b ...
		// We must verify this since we're writing into its
		// memory with a pointer for fast, streamlined code:
		dmode1 = dmode;
		Assert.That( dmode1, Is.EqualTo( dmode1_B ) );
	}

	[Test( TestOf = typeof( Rational ),
		Description = "Test that DXGI.Rational constructors, operators, etc work correctly" )]
	public void Test_DXGI_Rationals() {
		// Assignment by constructor, from uint and tuple
		Rational r1 = new( 60 ), r2 = new( 60, 1 ), r3 = 60, r4 = (60, 1);

		// Check equality operator
		bool all_the_same = r1 == r2 && r3 == r4 && r1 == r3;
		Assert.That( all_the_same,
			"Rationals intended to be 60/1 are not equal", new[] { r1, r2, r3, r4 } );

		// Check basic rational reduction:
		r1 = (120, 2);
		r1.Reduce();
		Assert.That( r1, Is.EqualTo( r2 ) );
	}

	#endregion

	//[Test(TestOf = typeof(DXGIFunctions) )]
	//public void DXGI_Factory_Static_Tests()
	//{

	//}
};