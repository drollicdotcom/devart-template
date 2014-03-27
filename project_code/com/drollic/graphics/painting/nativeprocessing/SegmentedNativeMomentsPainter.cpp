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

#include "SegmentedNativeMomentsPainter.h"
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

						SegmentedNativeMomentsPainter::SegmentedNativeMomentsPainter(int width, int height, std::vector<FastRawImage*> &theOriginals, std::vector<FastRawImage*> &strokes, int S)
							: INativeMomentsPainter(width, height, S, strokes), originals(theOriginals)
						{
						}

						SegmentedNativeMomentsPainter::~SegmentedNativeMomentsPainter()
						{
						}

						FastRawImage* SegmentedNativeMomentsPainter::CreatePainting()
						{
							return CreateSegmentedPainting();
						}

						FastRawImage* SegmentedNativeMomentsPainter::CreateSegmentedPainting()
						{														
							unsigned int totalOriginals = originals.size();
							float totalOriginalsf = static_cast<float>(totalOriginals);
							
							// Return if we somehow got no originals
							if (totalOriginals <= 0)
							{
								ComputationProgress::Instance()->percentComplete = 100;
								return NULL;
							}

							float percentWorkForAllImageAnalysis = 45.0; 
							float percentWorkPerImage = percentWorkForAllImageAnalysis / totalOriginalsf;

							std::pair<int, int> zeroOffset(0, 0);

							// First, determine the segment size based upon 
							int widthSum = 0;
							int heightSum = 0;
							for (unsigned int i=0; i < totalOriginals; i++)
							{
								widthSum += originals[i]->Width;
								heightSum += originals[i]->Height;
							}
							int segmentWidth = (widthSum / totalOriginals) / 4;
							int segmentHeight = (heightSum / totalOriginals) / 3;

							list<SegmentedRegion> regions;
							list<SegmentedRegion>::iterator regionsIter;

							// Seed random number generator
							srand(static_cast<unsigned int>(time(static_cast<time_t*>(NULL))));

							for (unsigned int i=0; i < totalOriginals; i++)
							{
								SegmentedRegion region;

								region.SetDimensions(segmentWidth, segmentHeight, originals[i]->Width, originals[i]->Height);								

								GeneratePaintingStrokes(originals[i], S, region, zeroOffset, percentWorkPerImage);

								// Generate the interesting segments which are basically above
								// a given percentile in terms of number of strokes
								double interestingSegmentBaseThreshold = 10.0;
								int interestingSegmentPossibleRange = 25;
								double interestingSegmentsAreAboveThisPercentile = interestingSegmentBaseThreshold + (rand() % interestingSegmentPossibleRange);
								regions.push_back(region.AboveThresholdSegments(interestingSegmentsAreAboveThisPercentile));
								
								//std::cout << "Completed. Threshold was " << interestingSegmentsAreAboveThisPercentile << std::endl;
							}

							ComputationProgress::Instance()->percentComplete = 50;

							SegmentedRegion finalRegion;
							finalRegion.SetDimensions(segmentWidth, segmentHeight, width, height);

							for (regionsIter = regions.begin(); regionsIter != regions.end(); regionsIter++)
							{								
								finalRegion.AttemptIncorporation(*regionsIter);
							}
							
							StrokeContainer *strokeContainer = finalRegion.UnifyStrokes();

							// Render the image
							FastRawImage* painting = Render(*strokeContainer);

							// Final cleanup
							delete strokeContainer;

							return painting;
						}



					}
				}
			}
		}
	}
}
