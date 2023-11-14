#region Using Directives
using System.Numerics ;
using DXSharp ;
using Vector4 = DXSharp.Vector4 ;
using Vector3 = DXSharp.Vector3 ;
#endregion
namespace AdvancedDXS.Framework.Graphics ;


public class Camera {
	Vector4 mEye ;
	Vector4 mAt ;
	Vector4 mUp ;

	public Vector3 Eye {
		get => (Vector3)mEye ; 
		set => mEye = new Vector4( value, 1.0f ) ;
	}

	public Vector3 At {
		get => (Vector3)mAt  ; 
		set => mAt  = new Vector4( value, 1.0f ) ;
	}

	public Vector3 Up {
		get => (Vector3)mUp  ; 
		set => mUp  = new Vector4( value, 1.0f ) ;
	}
	
	public float ClipNear { get ; set ; } = 0.01f ;
	public float ClipFar { get ; set ; } = 125.0f ;

	
	public Camera( ) {
		mEye = new Vector4( 0, 0, 0, 1 ) ;
		mAt = new Vector4( 0, 0, 0, 1 ) ;
		mUp = new Vector4( 0, 0, 0, 1 ) ;
	}
	
	public void Get3DViewProjMatrices( out Matrix4x4 view,
									   out Matrix4x4 proj,
									   float fovInDegrees,
									   float screenWidth,
									   float screenHeight ) {
		/*
			float aspectRatio = (float)screenWidth / (float)screenHeight;
		    float fovAngleY = fovInDegrees * XM_PI / 180.0f;

		    if (aspectRatio < 1.0f)
		    {
		        fovAngleY /= aspectRatio;
		    }

		    XMStoreFloat4x4(view, XMMatrixTranspose(XMMatrixLookAtRH(mEye, mAt, mUp)));
		    XMStoreFloat4x4(proj, XMMatrixTranspose(XMMatrixPerspectiveFovRH(fovAngleY, aspectRatio, 0.01f, 125.0f)));

		 */
		
		float aspectRatio = screenWidth  /  screenHeight ;
		float fovAngleY   = fovInDegrees * Mathf.Pi / 180.0f ;
		if ( aspectRatio < 1.0f ) 
			fovAngleY /= aspectRatio;

		var lookAt = Matrix4x4.CreateLookAt( (Vector3)mEye, 
											 (Vector3)mAt, 
											 (Vector3)mUp ) ;
		view = Matrix4x4.Transpose( lookAt )  ;
		
		
		var perspective = Matrix4x4.CreatePerspectiveFieldOfView( fovAngleY, aspectRatio, 
																  0.01f, 125.0f ) ;
		proj = Matrix4x4.Transpose( perspective ) ;
	}
	
	public void GetOrthoProjectionMatrices( out Matrix4x4 view,
											out Matrix4x4 proj,
											float width, float height ) {
		var lookAt = Matrix4x4.CreateLookAt( (Vector3)mEye, 
											 (Vector3)mAt, 
											 (Vector3)mUp ) ;
		view = Matrix4x4.Transpose( lookAt )  ;
		
		proj = Matrix4x4.CreateOrthographic( width, height,
											 0.01f, 125.0f ) ;
	}
	
	public void RotatePitch( float deg ) {
		var right = Vector3.Normalize( Vector3.Cross( (Vector3)mEye, (Vector3)mUp ) ) ;
		var rotation = Matrix4x4.CreateFromAxisAngle( right, deg ) ;
		mEye = Vector4.Transform( mEye, rotation ) ;
	}

	public void RotateYaw( float deg ) {
		var rotation = Matrix4x4.CreateFromAxisAngle( (Vector3)mUp, deg ) ;
		mEye = Vector4.Transform( mEye, rotation ) ;
	}
	
	public void Reset( ) {
		/* Native Sample Code:
		mEye = XMVectorSet(0.0f, 15.0f, -30.0f, 0.0f);
		mAt  = XMVectorSet(0.0f, 8.0f, 0.0f, 0.0f);
		mUp  = XMVectorSet(0.0f, 1.0f, 0.0f, 0.0f);*/
		mEye = new Vector4( 0.0f, 15.0f, -30.0f, 0.0f ) ;
		mAt  = new Vector4( 0.0f, 8.0f, 0.0f, 0.0f ) ;
		mUp  = new Vector4( 0.0f, 1.0f, 0.0f, 0.0f ) ;
	}
	
	public void Set( in Vector4 eye, in Vector4 at, in Vector4 up ) {
		mEye = eye ;
		mAt  = at ;
		mUp  = up ;
	}
} ;