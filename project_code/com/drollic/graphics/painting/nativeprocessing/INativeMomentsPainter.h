#pragma once

/*
Copyright © 2006, Drollic
All rights reserved.
http://www.drollic.com

Redistribution of this software in source or binary forms, with or 
without modification, is expressly prohibited. You may not reverse-assemble, 
reverse-compile, or otherwise reverse-engineer this software in any way.

THIS SOFTWARE ("SOFTWARE") IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#include "FastRawImage.h"
#include "NativeStroke.h"
#include "StrokeContainer.h"

#include <vector>
#include <map>
#include <set>


namespace com
{
	namespace drollic
	{
		namespace graphics
		{
			namespace painting
			{
				namespace native					
				{
					namespace processing
					{
						class CwmvFile;

						enum IStrokeGenerationResult
						{
							ValidStroke = 0,
							InvalidStroke = 1
						};


						class INativeMomentsPainter
						{
						public:
							INativeMomentsPainter(int theWidth, int theHeight, int theS, std::vector<FastRawImage*> &theStrokes);
							virtual ~INativeMomentsPainter();

							virtual FastRawImage* CreatePainting() = 0;						

							void EnableMovieGeneration(std::vector<FastRawImage*> introFrames, char *filename);

							//static FastRawImage* CreatePainting(FastRawImage* input, FastRawImage* stroke, int S);							
							//static FastRawImage* CreateSegmentedCompositePainting(int width, int height, std::vector<FastRawImage*> &originals, std::vector<FastRawImage*> &strokes, int S);
							//static FastRawImage* CreateCompositePainting(int width, int height, std::vector<FastRawImage*> &originals, std::vector<std::pair<int, int> > &locations, std::vector<FastRawImage*> &strokes, int S);
							
						protected:		
							int S;
							int width;
							int height;
							std::vector<FastRawImage*> &strokes;
							bool generateMovie;
							std::vector<FastRawImage*> introFrames;
							char *movieFilename;

							static void GeneratePaintingStrokes(FastRawImage* input, int s, IStrokeContainer &strokemap, std::pair<int, int> strokeOffset = std::pair<int,int>(0,0), float percentWorkToBeCompletedInThisCall = 0.0);
							static IStrokeGenerationResult GenerateStroke(FastRawImage *input, int x, int y, int s, int factor, int level, NativeStroke &stroke);
							static inline int ComputeMoments(FastRawImage &input, PixelData &bgr, int S, double &m00, double &m01, double &m10, double &m11, double &m02, double &m20);
							
							static void GetStrokes(FastRawImage* input, int s, FastRawImage* area, int factor, int level, IStrokeContainer &strokemap, std::pair<int, int> strokeOffset = std::pair<int,int>(0,0));
							static FastRawImage* ComputeAreaImageFast2(FastRawImage* inMem, int s);

							static inline double ColorDistance(PixelData &bgr1, PixelData &bgr2)
							{
								int dr = bgr1.R - bgr2.R;
								int dg = bgr1.G - bgr2.G;
								int db = bgr1.B - bgr2.B;

								return dr * dr + dg * dg + db * db;
							}

							int GenerateMovieFrames(CwmvFile *wmvFile, FastRawImage *sourceImageData, int frames);
							static inline int ComputeTotalStrokes(StrokeContainer &strokeContainer);
							static inline double Moment00Fast2(FastRawImage &piece, PixelData &bgr);
							static inline double Moment00(PixelData *singledata, int width, int height, PixelData &bgr);
							static inline double MomentI(PixelData &bgrxy, PixelData &bgr);						
							static FastRawImage* Dither(FastRawImage* inImage, double s, double p);

							FastRawImage* Render(StrokeContainer &strokeContainer);
						};
					}
				}
			}
		}
	}
}
