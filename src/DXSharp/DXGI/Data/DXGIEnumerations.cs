
public enum GraphicsPreemptionGranularity {
	DMA_BUFFER_BOUNDARY  = 0,
	PRIMITIVE_BOUNDARY   = 1,
	TRIANGLE_BOUNDARY    = 2,
	PIXEL_BOUNDARY       = 3,
	INSTRUCTION_BOUNDARY = 4,
} ;
public enum ComputePreemptionGranularity {
	DMA_BUFFER_BOUNDARY   = 0,
	DISPATCH_BOUNDARY     = 1,
	THREAD_GROUP_BOUNDARY = 2,
	THREAD_BOUNDARY       = 3,
	INSTRUCTION_BOUNDARY  = 4,
} ;