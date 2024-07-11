using Windows.Win32.Foundation ;
using Windows.Win32.Security ;
namespace DXSharp.Windows.Win32 ;


public struct SecurityAttributes {
	/// <summary>The size, in bytes, of this structure. Set this value to the size of the **SECURITY\_ATTRIBUTES** structure.</summary>
	public uint nLength;

	/// <summary>
	/// <para>A pointer to a [**SECURITY\_DESCRIPTOR**](../winnt/ns-winnt-security_descriptor.md) structure that controls access to the object. If the value of this member is **NULL**, the object is assigned the default security descriptor associated with the [*access token*](/windows/win32/secauthz/access-tokens) of the calling process. This is not the same as granting access to everyone by assigning a **NULL** [*discretionary access control list*](/windows/win32/secauthz/dacls-and-aces) (DACL). By default, the default DACL in the access token of a process allows access only to the user represented by the access token. For information about creating a security descriptor, see [Creating a Security Descriptor](/windows/win32/secauthz/creating-a-security-descriptor-for-a-new-object-in-c--).</para>
	/// <para><see href="https://docs.microsoft.com/windows/win32/api/wtypesbase/ns-wtypesbase-security_attributes#members">Read more on docs.microsoft.com</see>.</para>
	/// </summary>
	public unsafe void* lpSecurityDescriptor ;

	/// <summary>A Boolean value that specifies whether the returned handle is inherited when a new process is created. If this member is **TRUE**, the new process inherits the handle.</summary>
	public BOOL bInheritHandle ;
	
	public unsafe SecurityAttributes ( uint nLength, void* lpSecurityDescriptor, BOOL bInheritHandle ) {
		this.nLength = nLength ;
		this.lpSecurityDescriptor = lpSecurityDescriptor ;
		this.bInheritHandle = bInheritHandle ;
	}
	
	public unsafe SecurityAttributes( in SECURITY_ATTRIBUTES securityAttributes ) : 
		this( securityAttributes.nLength, 
			  securityAttributes.lpSecurityDescriptor, 
			  securityAttributes.bInheritHandle ) { }
} ;