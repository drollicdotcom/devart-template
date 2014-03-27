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

#include "WaveletNativeMomentsPainter.h"
#include "TwoDVector.h"
#include "NativeStroke.h"
#include "ComputationProgress.h"
#include "SegmentedRegion.h"

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
						WaveletNativeMomentsPainter::WaveletNativeMomentsPainter(int width, int height, 
							std::vector<FastRawImage*> &theOriginals, 
							std::vector<FastRawImage*> &theTransformed,
							std::vector<FastRawImage*> &strokes, 
							int S)
							: INativeMomentsPainter(width, height, S, strokes), originals(theOriginals), transformed(theTransformed)
						{
						}

						WaveletNativeMomentsPainter::~WaveletNativeMomentsPainter()
						{
						}

						/*
						*   This is v2 of the wavelet painter algorithm.  This version is much the same as v1, 
						*   but it makes no attempt to "fit" the pieces together.  They can simply overlap.
						*/
						FastRawImage* WaveletNativeMomentsPainter::CreatePaintingV2()
						{							
							// Seed random number generator
							srand(static_cast<unsigned int>(time(static_cast<time_t*>(NULL))));

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

							// First, determine the segment size based upon 
							int widthSum = 0;
							int heightSum = 0;
							for (unsigned int i=0; i < totalOriginals; i++)
							{
								widthSum += originals[i]->Width;
								heightSum += originals[i]->Height;
							}

							// Create a randomized segment size
							int segmentMultiplier = (rand() % 4) + 1;
							int segmentWidth = (widthSum / totalOriginals) / (5 * segmentMultiplier);
							int segmentHeight = (heightSum / totalOriginals) / (5 * segmentMultiplier);

							list<SegmentedRegion> regions;
							list<SegmentedRegion>::iterator regionsIter;

							list< list<Segment> > *allInterestingSegments = new list< list<Segment> >();

							for (unsigned int i=0; i < totalOriginals; i++)
							{											
								list<Segment> interestingSegmentsOnTransform;

								std::pair<int, int> zeroOffset(0, 0);

								// Create a region for the image to be processed
								SegmentedRegion originalRegion;
								originalRegion.SetDimensions(segmentWidth, segmentHeight, originals[i]->Width, originals[i]->Height);								

								// Create a region for the "interesting" segments of the original region
								SegmentedRegion interestingRegion;
								interestingRegion.SetDimensions(segmentWidth, segmentHeight, originals[i]->Width, originals[i]->Height);

								// Generate strokes for the full original painting
								GeneratePaintingStrokes(originals[i], S, originalRegion, zeroOffset, percentWorkPerImage);

								// Next, loop over all pixels (or maybe every other pixel) in the transformed
								// image.  For every "active" pixel, turn on the associated segment.  If a segment
								// is turned on, perhaps we just jump to the pixels for the next segment.
								Segment segment;
								SegmentOffset offset;
								bool interestingSegment = false;
								int interestingSegmentCounter = 0;

								for (int yOffset = 0; yOffset <= originalRegion.MaxYOffset; yOffset++)
								{
									for (int xOffset = 0; xOffset <= originalRegion.MaxXOffset; xOffset++)
									{					
										offset.first = xOffset;
										offset.second = yOffset;

										// Grab the segment at this location
										segment = originalRegion.SegmentAt(offset);

										// Default it to "non interesting"
										interestingSegment = false;

										int maxY = segment.y + segment.Height;
										int maxX = segment.x + segment.Width;
										int counter = 0;

										//double minPercentagePixelsForInteresting = static_cast<double>((rand() % 5) + 1) / 100.0;
										double minPercentagePixelsForInteresting = 0.25;
										int segmentThreshold = static_cast<int>((segment.Width * segment.Height) * minPercentagePixelsForInteresting);

										// Iterate over the pixels contained within this segment
										for (int y = segment.y; y < maxY; y++)
										{
											for (int x = segment.x; x < maxX; x++)
											{
												//std::cout << x << ", " << y << ": R: " << data.R << ", G: " << data.G << ", B: " << data.B << std::endl;

												if (transformed[i]->GetColor(x, y).R > 0)
												{
													//std::cout << x << ", " << y << ": R: " << data.R << ", G: " << data.G << ", B: " << data.B;

													if (++counter > segmentThreshold)
													{
														interestingSegment = true;
														x = maxX;
														y = maxY;

														interestingSegmentsOnTransform.push_back(Segment(x, y, maxX, maxY));

														break;
													}													
												}												
											}  // X pixel loop
										}  // Y pixel loop

										if (interestingSegment)
										{											
											interestingRegion.AddSegment(segment, offset);
										}

									}  // X Offset loop
								} // Y Offset loop


								allInterestingSegments->push_back(interestingSegmentsOnTransform);

								// We have now constructed an "interesting" collection of
								// segments from the region representing the original image.
								// This interesting region is now added to the final list of
								// regions that will be incorporated into a single composition.
								regions.push_back(interestingRegion);
							}

							ComputationProgress::Instance()->percentComplete = 50;

							SegmentedRegion finalRegion;
							finalRegion.SetDimensions(segmentWidth, segmentHeight, width, height);

							//std::cout << "Incorporating segments..." << std::endl;
							int regionCount = 0;
							for (regionsIter = regions.begin(); regionsIter != regions.end(); regionsIter++)
							{	
								SegmentOffset offset;
								if ((regionCount % 4) == 0)
								{
									offset.first = 0;
									offset.second = 0;
								}
								else if ((regionCount % 4) == 1)
								{
									offset.first = 0;
									offset.second = 1;
								}
								else if ((regionCount % 4) == 2)
								{
									offset.first = 1;
									offset.second = 0;
								}
								else
								{
									offset.first = 1;
									offset.second = 1;
								}
								

								// Simply incorporate the region, no fitting.
								finalRegion.Incorporate(*regionsIter, offset);

								regionCount++;
							}

							// Unify the strokes into a single container for rendering
							StrokeContainer *strokeContainer = finalRegion.UnifyStrokes();

							// Render the image
							FastRawImage* painting = Render(*strokeContainer);

							// Final cleanup
							delete strokeContainer;

							WaveletNativeMomentsPainterResult *result = new WaveletNativeMomentsPainterResult();
							result->painting = painting;
							result->interestingSegments = allInterestingSegments;

							return painting;
						}



						FastRawImage* WaveletNativeMomentsPainter::CreatePainting()
						{							
							// Seed random number generator
							srand(static_cast<unsigned int>(time(static_cast<time_t*>(NULL))));

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

							// First, determine the segment size based upon 
							int widthSum = 0;
							int heightSum = 0;
							for (unsigned int i=0; i < totalOriginals; i++)
							{
								widthSum += originals[i]->Width;
								heightSum += originals[i]->Height;
							}

							// Create a randomized segment size
							int segmentMultiplier = (rand() % 4) + 1;
							int segmentWidth = (widthSum / totalOriginals) / (8 * segmentMultiplier);
							int segmentHeight = (heightSum / totalOriginals) / (6 * segmentMultiplier);

							list<SegmentedRegion> regions;
							list<SegmentedRegion>::iterator regionsIter;

							list< list<Segment> > *allInterestingSegments = new list< list<Segment> >();

							for (unsigned int i=0; i < totalOriginals; i++)
							{											
								list<Segment> interestingSegmentsOnTransform;

								std::pair<int, int> zeroOffset(0, 0);

								// Create a region for the image to be processed
								SegmentedRegion originalRegion;
								originalRegion.SetDimensions(segmentWidth, segmentHeight, originals[i]->Width, originals[i]->Height);								

								// Create a region for the "interesting" segments of the original region
								SegmentedRegion interestingRegion;
								interestingRegion.SetDimensions(segmentWidth, segmentHeight, originals[i]->Width, originals[i]->Height);

								// Generate strokes for the full original painting
								GeneratePaintingStrokes(originals[i], S, originalRegion, zeroOffset, percentWorkPerImage);

								// Next, loop over all pixels (or maybe every other pixel) in the transformed
								// image.  For every "active" pixel, turn on the associated segment.  If a segment
								// is turned on, perhaps we just jump to the pixels for the next segment.
								Segment segment;
								SegmentOffset offset;
								bool interestingSegment = false;
								int interestingSegmentCounter = 0;

								for (int yOffset = 0; yOffset <= originalRegion.MaxYOffset; yOffset++)
								{
									for (int xOffset = 0; xOffset <= originalRegion.MaxXOffset; xOffset++)
									{					
										offset.first = xOffset;
										offset.second = yOffset;

										// Grab the segment at this location
										segment = originalRegion.SegmentAt(offset);

										// Default it to "non interesting"
										interestingSegment = false;

										int maxY = segment.y + segment.Height;
										int maxX = segment.x + segment.Width;
										int counter = 0;

										double minPercentagePixelsForInteresting = static_cast<double>((rand() % 5) + 1) / 100.0;
										int segmentThreshold = static_cast<int>((segment.Width * segment.Height) * minPercentagePixelsForInteresting);

										// Iterate over the pixels contained within this segment
										for (int y = segment.y; y < maxY; y++)
										{
											for (int x = segment.x; x < maxX; x++)
											{
												//std::cout << x << ", " << y << ": R: " << data.R << ", G: " << data.G << ", B: " << data.B << std::endl;

												if (transformed[i]->GetColor(x, y).R > 0)
												{
													//std::cout << x << ", " << y << ": R: " << data.R << ", G: " << data.G << ", B: " << data.B;

													if (++counter > segmentThreshold)
													{
														interestingSegment = true;
														x = maxX;
														y = maxY;

														interestingSegmentsOnTransform.push_back(Segment(x, y, maxX, maxY));

														break;
													}													
												}												
											}  // X pixel loop
										}  // Y pixel loop

										if (interestingSegment)
										{											
											interestingRegion.AddSegment(segment, offset);
										}

									}  // X Offset loop
								} // Y Offset loop


								allInterestingSegments->push_back(interestingSegmentsOnTransform);

								// We have now constructed an "interesting" collection of
								// segments from the region representing the original image.
								// This interesting region is now added to the final list of
								// regions that will be incorporated into a single composition.
								regions.push_back(interestingRegion);
							}

							ComputationProgress::Instance()->percentComplete = 50;

							SegmentedRegion finalRegion;
							finalRegion.SetDimensions(segmentWidth, segmentHeight, width, height);

							//std::cout << "Incorporating segments..." << std::endl;
							for (regionsIter = regions.begin(); regionsIter != regions.end(); regionsIter++)
							{								
								finalRegion.AttemptIncorporation(*regionsIter);
							}

							// Unify the strokes into a single container for rendering
							StrokeContainer *strokeContainer = finalRegion.UnifyStrokes();

							// Render the image
							FastRawImage* painting = Render(*strokeContainer);

							// Final cleanup
							delete strokeContainer;

							WaveletNativeMomentsPainterResult *result = new WaveletNativeMomentsPainterResult();
							result->painting = painting;
							result->interestingSegments = allInterestingSegments;

							return painting;
						}


						WaveletNativeMomentsPainterResult* WaveletNativeMomentsPainter::CreatePainting2()
						{							
							// Seed random number generator
							srand(static_cast<unsigned int>(time(static_cast<time_t*>(NULL))));

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

							// First, determine the segment size based upon 
							int widthSum = 0;
							int heightSum = 0;
							for (unsigned int i=0; i < totalOriginals; i++)
							{
								widthSum += originals[i]->Width;
								heightSum += originals[i]->Height;
							}

							// Create a randomized segment size
							int segmentMultiplier = (rand() % 4) + 1;
							int segmentWidth = (widthSum / totalOriginals) / (8 * segmentMultiplier);
							int segmentHeight = (heightSum / totalOriginals) / (6 * segmentMultiplier);

							list<SegmentedRegion> regions;
							list<SegmentedRegion>::iterator regionsIter;

							list< list<Segment> > *allInterestingSegments = new list< list<Segment> >();

							for (unsigned int i=0; i < totalOriginals; i++)
							{											
								list<Segment> interestingSegmentsOnTransform;

								std::pair<int, int> zeroOffset(0, 0);

								// Create a region for the image to be processed
								SegmentedRegion originalRegion;
								originalRegion.SetDimensions(segmentWidth, segmentHeight, originals[i]->Width, originals[i]->Height);								

								// Create a region for the "interesting" segments of the original region
								SegmentedRegion interestingRegion;
								interestingRegion.SetDimensions(segmentWidth, segmentHeight, originals[i]->Width, originals[i]->Height);

								// Generate strokes for the full original painting
								GeneratePaintingStrokes(originals[i], S, originalRegion, zeroOffset, percentWorkPerImage);

								// Next, loop over all pixels (or maybe every other pixel) in the transformed
								// image.  For every "active" pixel, turn on the associated segment.  If a segment
								// is turned on, perhaps we just jump to the pixels for the next segment.
								Segment segment;
								SegmentOffset offset;
								bool interestingSegment = false;
								int interestingSegmentCounter = 0;

								for (int yOffset = 0; yOffset <= originalRegion.MaxYOffset; yOffset++)
								{
									for (int xOffset = 0; xOffset <= originalRegion.MaxXOffset; xOffset++)
									{					
										offset.first = xOffset;
										offset.second = yOffset;

										// Grab the segment at this location
										segment = originalRegion.SegmentAt(offset);

										// Default it to "non interesting"
										interestingSegment = false;

										int maxY = segment.y + segment.Height;
										int maxX = segment.x + segment.Width;
										int counter = 0;

										double minPercentagePixelsForInteresting = static_cast<double>((rand() % 5) + 1) / 100.0;
										int segmentThreshold = static_cast<int>((segment.Width * segment.Height) * minPercentagePixelsForInteresting);

										// Iterate over the pixels contained within this segment
										for (int y = segment.y; y < maxY; y++)
										{
											for (int x = segment.x; x < maxX; x++)
											{
												//std::cout << x << ", " << y << ": R: " << data.R << ", G: " << data.G << ", B: " << data.B << std::endl;

												if (transformed[i]->GetColor(x, y).R > 0)
												{
													//std::cout << x << ", " << y << ": R: " << data.R << ", G: " << data.G << ", B: " << data.B;

													if (++counter > segmentThreshold)
													{
														interestingSegment = true;
														x = maxX;
														y = maxY;

														interestingSegmentsOnTransform.push_back(Segment(x, y, maxX, maxY));

														break;
													}													
												}												
											}  // X pixel loop
										}  // Y pixel loop

										if (interestingSegment)
										{											
											interestingRegion.AddSegment(segment, offset);
										}

									}  // X Offset loop
								} // Y Offset loop


								allInterestingSegments->push_back(interestingSegmentsOnTransform);

								// We have now constructed an "interesting" collection of
								// segments from the region representing the original image.
								// This interesting region is now added to the final list of
								// regions that will be incorporated into a single composition.
								regions.push_back(interestingRegion);
							}

							ComputationProgress::Instance()->percentComplete = 50;

							SegmentedRegion finalRegion;
							finalRegion.SetDimensions(segmentWidth, segmentHeight, width, height);

							//std::cout << "Incorporating segments..." << std::endl;
							for (regionsIter = regions.begin(); regionsIter != regions.end(); regionsIter++)
							{								
								finalRegion.AttemptIncorporation(*regionsIter);
							}

							// Unify the strokes into a single container for rendering
							StrokeContainer *strokeContainer = finalRegion.UnifyStrokes();

							// Render the image
							FastRawImage* painting = Render(*strokeContainer);

							// Final cleanup
							delete strokeContainer;

							WaveletNativeMomentsPainterResult *result = new WaveletNativeMomentsPainterResult();
							result->painting = painting;
							result->interestingSegments = allInterestingSegments;

							return result;
						}
					}
				}
			}
		}
	}
}
