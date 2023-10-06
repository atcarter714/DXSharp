using System;
using System.Runtime.InteropServices;

public class InstHandle: SafeHandle
{
	public InstHandle( IntPtr preexistingHandle, bool ownsHandle ): base(IntPtr.Zero, ownsHandle) {
		SetHandle(preexistingHandle);
	}
	public override bool IsInvalid => handle == IntPtr.Zero;
	protected override bool ReleaseHandle() {
		return true;
	}
}