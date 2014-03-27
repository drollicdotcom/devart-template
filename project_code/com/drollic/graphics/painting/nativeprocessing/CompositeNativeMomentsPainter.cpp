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

#include "stdafx.h"

#include "CompositeNativeMomentsPainter.h"
#include "TwoDVector.h"
#include "NativeStroke.h"
#include "ComputationProgress.h"
#include "SegmentedRegion.h"

#include <math.h>
#include <time.h>

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

						CompositeNativeMomentsPainter::CompositeNativeMomentsPainter(int width, int height, std::vector<FastRawImage*> &theOriginals, std::vector<std::pair<int, int> > &theLocations, std::vector<FastRawImage*> &strokes, int S)
							: INativeMomentsPainter(width, height, S, strokes), originals(theOriginals), locations(theLocations)
						{
						}

						CompositeNativeMomentsPainter::~CompositeNativeMomentsPainter()
						{
						}

						FastRawImage* CompositeNativeMomentsPainter::CreatePainting()
						{
							return CreateCompositePainting();
						}
						
						/*
						DWORD MakeDwordColor(int r, int g, int b)
						{
							DWORD dwStuff = ((r << 16) | (g << 8) | (b));

							return dwStuff;
						}
							*/

						FastRawImage* CompositeNativeMomentsPainter::CreateCompositePainting()
						{
							StrokeContainer strokeContainer;
							StrokeContainer::AreaToDimensionsMapType::iterator iter;

							unsigned int totalOriginals = originals.size();
							float totalOriginalsf = static_cast<float>(totalOriginals);
							
							float percentWorkForAllImageAnalysis = 45.0; 
							float percentWorkPerImage = percentWorkForAllImageAnalysis / totalOriginalsf;

							for (unsigned int i=0; i < totalOriginals; i++)							
							{
								GeneratePaintingStrokes(originals[i], S, strokeContainer, locations[i], percentWorkPerImage);
							}
							ComputationProgress::Instance()->percentComplete = 50;							
						
							return Render(strokeContainer);
						}

					}
				}
			}
		}
	}
}
