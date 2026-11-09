using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DXSharp.Windows.COM;

namespace BasicTests;

[TestFixture, Category("BuildBaseline")]
internal unsafe class ComPtrNativeBalanceTests {
    [ComImport, Guid("B6E3C41D-57DD-482A-A8B4-AED20D32B2FE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IProbe : IUnknown { }

    private struct Entry {
        internal nint* Vtable;
        internal State* Owner;
    }

    private struct State {
        internal Entry Identity;
        internal Entry Typed;
        internal int Count;
    }

    [Test]
    public void ReleasesExactlyTheReferencesItAcquiresAcrossDistinctInterfaces() {
        nint* vtable = stackalloc nint[3] {
            (nint)(delegate* unmanaged[Stdcall]<Entry*, Guid*, nint*, int>)&Query,
            (nint)(delegate* unmanaged[Stdcall]<Entry*, uint>)&AddRef,
            (nint)(delegate* unmanaged[Stdcall]<Entry*, uint>)&Release
        };
        State state = default;
        state.Identity = new Entry { Vtable = vtable, Owner = &state };
        state.Typed = new Entry { Vtable = vtable, Owner = &state };
        state.Count = 1;
        nint identity = (nint)(&state.Identity);
        var rcw = Marshal.GetObjectForIUnknown(identity);
        try {
            var typed = (IProbe)rcw;
            int baseline = state.Count;
            using var owner = new ComPtr<IProbe>(typed);
            Assert.That(owner.BaseAddress, Is.EqualTo(identity));
            Assert.That(owner.InterfaceVPtr, Is.EqualTo((nint)(&state.Typed)));
            Assert.That(state.Count, Is.EqualTo(baseline + 2));

            owner.IncrementReferences();
            Assert.That(state.Count, Is.EqualTo(baseline + 3));
            owner.ReleaseReference();
            Assert.That(state.Count, Is.EqualTo(baseline + 2));
            nint detached = owner.Detach();
            try { Assert.That(state.Count, Is.EqualTo(baseline + 1)); }
            finally { Marshal.Release(detached); }
            Assert.That(state.Count, Is.EqualTo(baseline));

            owner.Attach(typed);
            owner.IncrementReferences();
            owner.Dispose();
            owner.Dispose();
            Assert.That(state.Count, Is.EqualTo(baseline));
        }
        finally {
            Marshal.ReleaseComObject(rcw);
            Marshal.Release(identity);
        }
        Assert.That(state.Count, Is.Zero);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int Query(Entry* entry, Guid* iid, nint* result) {
        State* owner = entry->Owner;
        if (*iid == ComPtr.GUID_IUNKNOWN) *result = (nint)(&owner->Identity);
        else if (*iid == typeof(IProbe).GUID) *result = (nint)(&owner->Typed);
        else {
            *result = 0;
            return unchecked((int)0x80004002);
        }
        Interlocked.Increment(ref owner->Count);
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static uint AddRef(Entry* entry) => (uint)Interlocked.Increment(ref entry->Owner->Count);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static uint Release(Entry* entry) => (uint)Interlocked.Decrement(ref entry->Owner->Count);
}
