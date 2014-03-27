#pragma once

//#include "RawImage.h"
#include "FastRawImage.h"
#include "NativeStroke.h"

#include <vector>
#include <map>
#include <set>

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
					namespace processing
					{
						enum StrokeGenerationResult
						{
							ValidStroke = 0,
							InvalidStroke = 1
						};

						struct StrokeContainer
						{
						public:
							typedef std::map<int, std::set< std::pair<int, int> >, std::greater<int> > AreaToDimensionsMapType;
							typedef std::multimap< std::pair<int, int>, NativeStroke> DimensionToStrokesMapType;

							AreaToDimensionsMapType areaToStrokeDimensionsMap;
							DimensionToStrokesMapType strokeDimensionsToStrokesMap;
						};


						class NativeMomentComputation
						{
						public:
							//typedef std::map< int, std::map< std::pair<int, int>, std::vector<NativeStroke> >, greater<int> > StrokeContainer;

							static FastRawImage* CreatePainting(FastRawImage* input, FastRawImage* stroke, int S);
							static FastRawImage* CreatePaintingReducedAlloc(FastRawImage* input, FastRawImage* strokeImage, int S);
							static FastRawImage* CreateCompositePainting(int width, int height, std::vector<FastRawImage*> &originals, std::vector<std::pair<int, int> > &locations, std::vector<FastRawImage*> &strokes, int S);
							static void GeneratePaintingStrokes(FastRawImage* input, int s, StrokeContainer &strokemap, std::pair<int, int> strokeOffset = std::pair<int,int>(0,0));
							static StrokeGenerationResult GenerateStroke(FastRawImage *input, int x, int y, int s, int factor, int level, NativeStroke &stroke);
							static inline int ComputeMoments(FastRawImage &input, PixelData &bgr, int S, double &m00, double &m01, double &m10, double &m11, double &m02, double &m20);
							static FastRawImage* Dither(FastRawImage* inImage, double s, double p);
							static void GetStrokes(FastRawImage* input, int s, FastRawImage* area, int factor, int level, StrokeContainer &strokemap, std::pair<int, int> strokeOffset = std::pair<int,int>(0,0));
							//static RawImage* ComputeAreaImage(RawImage* inMem, int s);
							static FastRawImage* ComputeAreaImageFast2(FastRawImage* inMem, int s);
							//static double ComputeStrokeAreaPixel(RawImage* inMem, int i, int j, int s);
							static inline double ColorDistance(PixelData &bgr1, PixelData &bgr2);												
							static inline double Moment00Fast2(FastRawImage &piece, PixelData &bgr);
							static inline double Moment00(PixelData *singledata, int width, int height, PixelData &bgr);
							static inline double MomentI(PixelData &bgrxy, PixelData &bgr);
						};
					}
				}
			}
		}
	}
}
