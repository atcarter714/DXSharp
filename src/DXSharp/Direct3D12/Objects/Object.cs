﻿using Windows.Win32.Graphics.Direct3D12 ;
using DXSharp.Objects ;
using DXSharp.Windows.COM ;
namespace DXSharp.Direct3D12 ;


public abstract class Object: DXComObject, IObject {
	public override ComPtr? ComPtrBase => ComPointer ;
	public virtual ComPtr< ID3D12Object >? ComPointer { get ; protected set ; }
	public nint PointerToIUnknown => ComPointer?.BaseAddress ?? 0x0000 ;
	public ID3D12Object? COMObject => ComPointer?.Interface ;
}