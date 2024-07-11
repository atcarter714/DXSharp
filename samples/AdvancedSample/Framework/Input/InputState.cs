namespace AdvancedDXS.Framework.Input ;


public struct InputState: IEquatable< InputState > {
	public bool rightArrowPressed ;
	public bool leftArrowPressed ;
	public bool upArrowPressed ;
	public bool downArrowPressed ;
	public bool animate ;
	
	
	public InputState( bool rightArrowPressed = false, 
					   bool leftArrowPressed = false, 
					   bool upArrowPressed = false, 
					   bool downArrowPressed = false, 
					   bool animate = false ) {
		this.rightArrowPressed = rightArrowPressed ;
		this.leftArrowPressed = leftArrowPressed ;
		this.upArrowPressed = upArrowPressed ;
		this.downArrowPressed = downArrowPressed ;
		this.animate = animate ;
	}

	
	public bool IsAnyKeyPressed => rightArrowPressed || leftArrowPressed || 
								   upArrowPressed || downArrowPressed ;
	public bool AreNoKeysPressed => !rightArrowPressed && !leftArrowPressed && 
									!upArrowPressed && !downArrowPressed ;
	
	public override int GetHashCode( ) => 
		HashCode.Combine( rightArrowPressed, leftArrowPressed, 
						  upArrowPressed, downArrowPressed, animate ) ;
	public bool Equals( InputState other ) => this == other ;
	public override bool Equals( object? obj ) => obj is InputState other && Equals( other ) ;
	public override string ToString( ) =>    $"{{ {nameof(rightArrowPressed)}: {rightArrowPressed}, " +
												$"{nameof(leftArrowPressed)}: {leftArrowPressed}, " +
												$"{nameof(upArrowPressed)}: {upArrowPressed}, " +
												$"{nameof(downArrowPressed)}: {downArrowPressed}, " +
												$"{nameof(animate)}: {animate} }}" ;
	
	public static bool operator ==( in InputState left, in InputState right ) => 
		left.rightArrowPressed == right.rightArrowPressed && 
		left.leftArrowPressed == right.leftArrowPressed && 
		left.upArrowPressed == right.upArrowPressed && 
		left.downArrowPressed == right.downArrowPressed && 
		left.animate == right.animate ;
	public static bool operator !=( in InputState left, in InputState right ) => !(left == right ) ;
} ;