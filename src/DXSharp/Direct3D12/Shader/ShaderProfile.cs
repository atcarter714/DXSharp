#region Using Directives
using System.Runtime.InteropServices ;
using Windows.Win32.Graphics.Direct3D12 ;
#endregion
namespace DXSharp.Direct3D12.Shader ;


[StructLayout(LayoutKind.Sequential)]
public struct ShaderProfile {
	ShaderVersionType _version ;
	uint _major, _minor, _profileMajor, _profileMinor ;
	
	public uint Major => _major ;
	public uint Minor => _minor ;
	public uint ProfileMajor => _profileMajor ;
	public uint ProfileMinor => _profileMinor ;
	public ShaderVersionType Version => _version ;
	
	
	public ShaderProfile( ShaderVersionType version,
						  uint major, uint minor = 0,
						  uint profileMajor = 0, uint profileMinor = 0 ) {
		_version = version ;
		_major = major ;
		_minor = minor ;
		_profileMajor = profileMajor ;
		_profileMinor = profileMinor ;
	}
	
	
	public string GetTypePrefix( ) => _version switch {
		ShaderVersionType.PixelShader    => "ps",
		ShaderVersionType.VertexShader   => "vs",
		ShaderVersionType.GeometryShader => "gs",
		ShaderVersionType.HullShader     => "hs",
		ShaderVersionType.DomainShader   => "ds",
		ShaderVersionType.ComputeShader  => "cs",
		_                                => throw new NotSupportedException( )
	} ;

	public override string ToString( ) {
		return _profileMajor is 0 ?
			$"{GetTypePrefix( )}_{_major}_{_minor}_profile{_profileMajor}_{_profileMinor}" 
			: $"{GetTypePrefix( )}_{_major}_{_minor}" ;
	}

} ;

