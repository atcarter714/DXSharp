#region Using Directives
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;
using static Windows.Win32.PInvoke;

using DXSharp.DXGI;
using System.Runtime.InteropServices;
#endregion

namespace BasicTests;


[TestFixture()]
public class PInvoke_Interop_Tests
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public unsafe void DXGI_WIN32_Interop_Internal()
	{
		// Prove that essential DXGI internal interop code works
		// this should be broken down and organized into smaller
		// tests but for now this works ...

		var hr = CreateDXGIFactory2( 0u, typeof(IDXGIFactory7).GUID, out var factoryObj );
		Assert.True(hr.Succeeded);

		IDXGIFactory7 factory7 = factoryObj as IDXGIFactory7;
		Assert.NotNull(factory7);

		var adapters = new List<IDXGIAdapter4>();
		Assert.DoesNotThrow(() =>
		{
			for (uint index = 0; index <= 0x10; ++index)
			{
				try
				{
					factory7.EnumAdapters1(index, out var ppAdapter);

					if (ppAdapter is null)
						throw new NullReferenceException("DXGI_WIN32_Interop_Internal(): " +
							"Adapter interface from EnumAdapters1 is null!");

					IDXGIAdapter4 adptr4 = (IDXGIAdapter4)ppAdapter;
					Assert.IsNotNull(adptr4);

					adapters.Add(adptr4);
				}
				catch (COMException comEx)
				{
					if (comEx.HResult == HRESULT.DXGI_ERROR_NOT_FOUND)
						break;

					throw comEx;
				}
			}
		});

		foreach (var a in adapters)
			Marshal.ReleaseComObject(a);

		int factoryRefCount = Marshal.ReleaseComObject(factory7);
	}
}