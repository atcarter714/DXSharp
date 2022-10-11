#region Using Directives
using DXSharp.Windows.COM;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dxgi;
#endregion

namespace BasicTests.COM;



[TestFixture]
internal class ComPtrTests
{
	HRESULT hr;
	IntPtr address;
	IDXGIFactory7? factory7;
	
	ComPtr<IDXGIFactory7>? factory7Ptr;

	[OneTimeSetUp]
	public void SetUp() {
		hr = PInvoke.CreateDXGIFactory2(0x00u, typeof(IDXGIFactory7).GUID, out var ppFactory);
		factory7 = (IDXGIFactory7)ppFactory;
	}

	[OneTimeTearDown]
	public void Cleanup()
	{
		hr = default;
		address = default;
		factory7 = null;
		factory7Ptr = null;
	}


	[Test]
	public void Test_Create_ComPtr()
	{
		// Ensure PInvoke call succeeded:
		Assert.True(hr.Succeeded);

		Create_ComPtr_Factory7();
	}

	[Test]
	public void Test_Dispose_ComPtr()
	{
		Release_ComPtr_Factory7();
	}




	void Create_ComPtr_Factory7()
	{
		// Create a ComPtr<IDXGIFactory7>:
		factory7Ptr = new ComPtr<IDXGIFactory7>(factory7);
		this.address = factory7Ptr.Pointer;

		// Assert that the ComPtr has a valid internal pointer:
		Assert.That(factory7Ptr.Pointer, Is.Not.EqualTo(IntPtr.Zero));

		// Assert that the ComPtr interface reference is valid:
		Assert.NotNull(factory7Ptr.Interface);
	}

	void Release_ComPtr_Factory7()
	{
		Assert.NotNull(factory7Ptr);
		factory7Ptr?.Dispose();
		
		Assert.IsTrue(factory7Ptr.Disposed);
		Assert.That(IntPtr.Zero, Is.EqualTo(factory7Ptr.Pointer));
		Assert.IsNull(factory7Ptr.Interface);
	}


}