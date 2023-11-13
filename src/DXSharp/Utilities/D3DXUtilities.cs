

public class D3DXUtilities {
	public const int ComponentMappingMask  = 0x07,
					 ComponentMappingShift = 0x03,
					 ComponentMappingAlwaysSetBitAvoidingZeromemMistakes
						 = ( 0x01 << ( ComponentMappingShift * 0x04 ) ) ;
	
	public static int ComponentMapping( int src0, int src1, int src2, int src3 ) {
		return ( (   ((src0) & ComponentMappingMask) |
				   ( ((src1) & ComponentMappingMask) << (ComponentMappingShift) )        |
				   ( ((src2) & ComponentMappingMask) << (ComponentMappingShift * 0x02) ) |
				   ( ((src3) & ComponentMappingMask) << (ComponentMappingShift * 0x03) ) |
								ComponentMappingAlwaysSetBitAvoidingZeromemMistakes ) ) ;
	}
	
	public static int DefaultComponentMapping( ) => ComponentMapping(0, 1, 2, 3) ;
	
	public static int ComponentMapping( int ComponentToExtract, int Mapping ) => 
		((Mapping >> (ComponentMappingShift * ComponentToExtract) & ComponentMappingMask)) ;
} ;