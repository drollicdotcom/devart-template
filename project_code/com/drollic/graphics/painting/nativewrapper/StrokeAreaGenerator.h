// RawImage.h
#pragma once

using namespace System;
using namespace System::Drawing;
using namespace System::Collections;

#include "FastRawImage.h"

using namespace com::raymatthieu::graphics::painting::native::processing;

//using namespace System;
//using namespace System::Drawing;

/*
#pragma unmanaged

namespace com
{
namespace raymatthieu
{
namespace graphics
{
namespace painting
{
namespace nativeprocessing
{
class StrokeAreaProcessing
{
public:
static inline RawImage* ComputeAreaImage(RawImage* inMem, int s);
static inline double ComputeStrokeAreaPixel(RawImage* inMem, int i, int j, int s);
static inline double ColorDistance(PixelData &bgr1, PixelData &bgr2);
static inline double Moment00(PixelData *singledata, int width, int height, PixelData &bgr);
static inline double MomentI(PixelData &bgrxy, PixelData &bgr);
};
}
}
}
}
}

#pragma managed
*/

namespace com
{
	namespace raymatthieu
	{
		namespace graphics
		{
			namespace painting
			{
				namespace native
				{
					namespace wrapper
					{
						public ref class StrokeAreaGenerator	
						{
						public:		
							static UnsafeBitmap^ GenerateCompositePainting(int width, int height, array<UnsafeBitmap^>^ input, array<Point>^ locations, array<UnsafeBitmap^>^ strokes, int S);
							static UnsafeBitmap^ GeneratePainting(UnsafeBitmap^ input, UnsafeBitmap^ stroke, int S);
							static UnsafeBitmap^ FastRaw2Unsafe(FastRawImage* input);

							static int GetPaitingPercentComplete();
							static Hashtable^ GenerateTestImages(UnsafeBitmap ^input, UnsafeBitmap^ stroke, int s);
							static ArrayList^ GeneratePaintingStrokes(UnsafeBitmap ^input, int s);
							//static array<Color>^ Generate(UnsafeBitmap ^input, int s);
							//static array<Color>^ GenerateFast1(UnsafeBitmap ^input, int s);
							static array<Color>^ GenerateFast2(UnsafeBitmap ^input, int s);
						private:
							static FastRawImage* Unsafe2FastRaw(UnsafeBitmap ^input);
						};
					}
				}
			}
		}
	}
}