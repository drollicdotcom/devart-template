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

#include <tchar.h>  // For queryperformancecounter
#include <windows.h> // For queryperformancecounter

#include "INativeMomentsPainter.h"
#include "TwoDVector.h"
#include "NativeStroke.h"
#include "ComputationProgress.h"
#include "SegmentedRegion.h"

//#include "wmvFile.h"

#include <math.h>
#include <time.h>
#include <iostream>

using namespace std;

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

						#define iround(x) (int) (x + 0.5)

						#define MAX(x,y) ((x)>(y)?(x):(y))

						#define swap(a,b) \
							do { \
							float *tmp; \
							tmp = a; \
							a = b; \
							b = tmp; \
							} while (0)


						INativeMomentsPainter::INativeMomentsPainter(int theWidth, int theHeight, int theS, std::vector<FastRawImage*> &theStrokes)
							: width(theWidth), height(theHeight), S(theS), strokes(theStrokes), generateMovie(false)
						{
						}

						INativeMomentsPainter::~INativeMomentsPainter()
						{
						}

						void INativeMomentsPainter::EnableMovieGeneration(std::vector<FastRawImage*> frames, char *filename)
						{
							this->introFrames = frames;
							this->generateMovie = true;
							this->movieFilename = filename;
						}

						inline int INativeMomentsPainter::ComputeMoments(FastRawImage &input, PixelData &bgr, int S, double &m00, double &m01, double &m10, double &m11, double &m02, double &m20)
						{
							double Ival;

							m00 = m01 = m10 = m11 = m02 = m20 = 0;

							PixelData c;
							for (int y = 1; y <= S; y++) 
							{
								for (int x = 1; x <= S; x++) 
								{
									Ival = 0.0; 
									if ((x < input.Width) && (y < input.Height))
									{
										c = input.GetColor(x, y);
										Ival = MomentI(c, bgr) * 255.0;
									}

									m00 += Ival;
									m01 += y * Ival;
									m10 += x * Ival;
									m11 += y * x * Ival;
									m02 += y * y * Ival;
									m20 += x * x * Ival;
								}
							}

							return 1;
						}

						IStrokeGenerationResult INativeMomentsPainter::GenerateStroke(FastRawImage *input, int x, int y, int s, int factor, int level, NativeStroke &stroke) 
						{
							double m00, m01, m10, m11, m02, m20;
							double a, b, c;
							double tempval;
							double dw, dxc, dyc;
							int xc, yc;
							double theta;
							float w, l;

							m00 = m01 = m10 = m11 = m02 = m20 = 0;

							PixelData firstColor = input->GetColor(x, y); 

							FastRawImage piece(s, s, false);
							input->Crop(x, y, s, s, piece);

							ComputeMoments(piece, firstColor, s, m00, m01, m10, m11, m02, m20);

							// Sanity check data
							if (m00 <= 0)
								return InvalidStroke;

							dxc = m10 / m00;
							dyc = m01 / m00;
							a = (m20 / m00) - (double)((dxc)*(dxc));
							b = 2 * (m11 / m00 - (double)((dxc)*(dyc)));
							c = (m02 / m00) - (double)((dyc)*(dyc));
							theta = atan2(b, (a-c)) / 2;
							tempval = sqrt(b*b + (a-c)*(a-c));

							// Sanity check
							if (((a+c - tempval) < 0.0) || ((a+c + tempval) < 0.0))
								return InvalidStroke;

							dw = sqrt(6 * (a+c - tempval));
							w = (float)(sqrt(6 * (a+c - tempval)));
							l = (float)(sqrt(6 * (a+c + tempval)));
							xc = (int)(x + iround(dxc - s/2)); 
							yc = (int)(y + iround(dyc - s/2)); 

							stroke.xc = factor*xc;
							stroke.yc = factor*yc;

							// TODO: If painting looks off, take out this rounding.
							stroke.w = iround(factor*w);
							stroke.l = iround(factor*l);
							stroke.area = stroke.l * stroke.w;
							stroke.theta = (float) theta;
							stroke.rgb.R = firstColor.R;
							stroke.rgb.G = firstColor.G;
							stroke.rgb.B = firstColor.B;
							stroke.level = level;

							return ValidStroke;
						}



						FastRawImage* INativeMomentsPainter::Dither(FastRawImage* inImage, double s, double p)
						{
							int width = inImage->Width;
							int height = inImage->Height;

							TwoDVector<int> input(height, width);

							FastRawImage* img = new FastRawImage(width, height);
							int w, h;
							float error, val;
							float a = (float) ((s - 1) / pow(255.0, p));        

							TwoDVector<int> inputPrime(height, width);

							// Convert into collection of Color objects
							int r;
							for (int y = 0; y < inImage->Height; y++) 
							{
								for (int x = 0; x < inImage->Width; x++) 
								{               
									r = inImage->GetColor(x, y).R;
									input.SetAt(y, x, r);
								}
							}

							// Seed random number system
							srand(static_cast<unsigned int>(time(NULL)));

							w = width;
							h = height;

							// Allocate the buffers	
							float *cur = new float [w];
							float *nxt = new float [w];

							for (int x = 0; x < w; x++) 
							{
								nxt[x] = MAX(1.0f /(a * (float)(pow(static_cast<double>(input[0][x]), p)) + 1), 0.5f);
							}

							int yClone = 0;
							for (int y=0; y < h-1; y++, yClone = y) 
							{
								// The next line becomes current line
								swap(cur, nxt);							

								// Copy the next line to buffer
								nxt[0] = MAX(1.0f /(a *(float)(pow(static_cast<double>(input[y+1][0]), p)) + 1), 0.5f);

								for (int x = 1; x < w; x++) 
								{
									nxt[x] = 1.0f / (a * (float)(pow(static_cast<double>(input[y+1][x]), p)) + 1);
								}

								// Spread out the error value
								int xClone = 0;
								for (int x = 0; x < w-1; x++, xClone = x) 
								{
									val = cur[x] > 1.0f ? 1.0f : 0.0f;
									error = cur[x] - val;
									inputPrime[y][x] = val > 0.0 ? 0 : 255;
									switch (rand() % 4) 
									{
									case 0:
										nxt[x + 1] += error/16.0f;
										if (x > 0)
											nxt[x - 1] += 3*error/16.0f;
										nxt[x]     += 5*error/16.0f;
										cur[x + 1] += 7*error/16.0f;
										break;
									case 1:
										nxt[x + 1] += 7*error/16.0f;
										if (x > 0)
											nxt[x - 1] += error/16.0f;
										nxt[x]     += 3*error/16.0f;
										cur[x + 1] += 5*error/16.0f;
										break;
									case 2:
										nxt[x + 1] += 5*error/16.0f;
										if (x > 0)
											nxt[x - 1] += 7*error/16.0f;
										nxt[x]     += error/16.0f;
										cur[x + 1] += 3*error/16.0f;
										break;
									case 3:
										nxt[x + 1] += 3*error/16.0f;
										if (x > 0)
											nxt[x - 1] += 5*error/16.0f;
										nxt[x]     += 7*error/16.0f;
										cur[x + 1] += error/16.0f;
										break;
									}
								}

								inputPrime.SetAt(y, xClone, cur[xClone] > 1.0 ? 0 : 255);
							}

							// Clip the last line
							for (int x = 0; x < w; x++)
								inputPrime.SetAt(yClone, x, nxt[x] > 1.0 ? 0 : 255);

							// Generate bitmap
							for (int y2=0; y2 < height; y2++)
								for (int x2=0; x2 < width; x2++)
									img->SetColor(x2, y2, PixelData(inputPrime[y2][x2], inputPrime[y2][x2], inputPrime[y2][x2]));         

							// Clean up
							delete [] cur;
							delete [] nxt;

							return img;
						}

						////////////////////////
/*
						FastRawImage* NativeMomentPainter::CreateSegmentedCompositePainting(int width, int height, std::vector<FastRawImage*> &originals, std::vector<FastRawImage*> &strokes, int S)
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

							// Seed random number generator on first red value
							srand(time(NULL));

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
							
							NativeStroke stroke;
							FastRawImage *scaledStroke;
							FastRawImage *painting = new FastRawImage(width, height);
							painting->WhiteOut();

							StrokeContainer *strokeContainer = finalRegion.UnifyStrokes();
							StrokeContainer::AreaToDimensionsMapType::iterator iter;

							StrokeContainer::DimensionToStrokesMapType::iterator strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator start_strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator end_strokeIter;
							std::set< std::pair<int, int> >::iterator dimensionsIter;
							std::pair<int, int> dimensions;
							int totalStrokes = 0;
							int totalScalings = 0;
							int totalAreaValues = 0;
							float maxAreaValues = static_cast<float>(strokeContainer->areaToStrokeDimensionsMap.size());

							for(iter = strokeContainer->areaToStrokeDimensionsMap.begin(); iter != strokeContainer->areaToStrokeDimensionsMap.end(); ++iter)
							{
								totalAreaValues++;
								ComputationProgress::Instance()->percentComplete = ((static_cast<float>(totalAreaValues+1) / maxAreaValues) * 50) + 50;

								for (dimensionsIter = iter->second.begin(); dimensionsIter != iter->second.end(); ++dimensionsIter) 
								{
									dimensions = *dimensionsIter;
									scaledStroke = FastRawImage::Scale(strokes[rand() % strokes.size()], static_cast<int>(dimensions.first), static_cast<int>(dimensions.second));
									totalScalings++;

									start_strokeIter = strokeContainer->strokeDimensionsToStrokesMap.lower_bound(*dimensionsIter);
									end_strokeIter = strokeContainer->strokeDimensionsToStrokesMap.upper_bound(*dimensionsIter);
									int max = (scaledStroke->Width > scaledStroke->Height) ? scaledStroke->Width : scaledStroke->Height;

									// TODO: This object is on the stack but the underlying singledata is still on the heap, and it's not freed.  MEMORY PENALTY
									FastRawImage rotatedStroke(max + max, max + max);									

									for (strokeIter = start_strokeIter; strokeIter != end_strokeIter; ++strokeIter)
									{
										rotatedStroke.BlackOut();

										FastRawImage::Rotate(scaledStroke, strokeIter->second.theta, rotatedStroke);
										FastRawImage::Blend(painting, rotatedStroke, strokeIter->second.rgb, strokeIter->second.xc, strokeIter->second.yc);

										totalStrokes++;
									}

									delete scaledStroke;
								}
							}

							return painting;
						}



						////////////////////////

						FastRawImage* NativeMomentPainter::CreateCompositePainting(int width, int height, std::vector<FastRawImage*> &originals, std::vector<std::pair<int, int> > &locations, std::vector<FastRawImage*> &strokes, int S)
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

							NativeStroke stroke;
							FastRawImage *scaledStroke;
							FastRawImage *painting = new FastRawImage(width, height);
							painting->WhiteOut();

							StrokeContainer::DimensionToStrokesMapType::iterator strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator start_strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator end_strokeIter;
							std::set< std::pair<int, int> >::iterator dimensionsIter;
							std::pair<int, int> dimensions;
							int totalStrokes = 0;
							int totalScalings = 0;
							int totalAreaValues = 0;
							float maxAreaValues = static_cast<float>(strokeContainer.areaToStrokeDimensionsMap.size());

							for(iter = strokeContainer.areaToStrokeDimensionsMap.begin(); iter != strokeContainer.areaToStrokeDimensionsMap.end(); ++iter)
							{
								totalAreaValues++;
								ComputationProgress::Instance()->percentComplete = ((static_cast<float>(totalAreaValues+1) / maxAreaValues) * 50) + 50;

								for (dimensionsIter = iter->second.begin(); dimensionsIter != iter->second.end(); ++dimensionsIter) 
								{
									dimensions = *dimensionsIter;
									scaledStroke = FastRawImage::Scale(strokes[rand() % strokes.size()], static_cast<int>(dimensions.first), static_cast<int>(dimensions.second));
									totalScalings++;

									start_strokeIter = strokeContainer.strokeDimensionsToStrokesMap.lower_bound(*dimensionsIter);
									end_strokeIter = strokeContainer.strokeDimensionsToStrokesMap.upper_bound(*dimensionsIter);
									int max = (scaledStroke->Width > scaledStroke->Height) ? scaledStroke->Width : scaledStroke->Height;

									// TODO: This object is on the stack but the underlying singledata is still on the heap, and it's not freed.  MEMORY PENALTY
									FastRawImage rotatedStroke(max + max, max + max);
									//StackBasedRawImage rotatedStroke(max + max, max + max);

									for (strokeIter = start_strokeIter; strokeIter != end_strokeIter; ++strokeIter)
									{
										rotatedStroke.BlackOut();

										//rotatedStroke = FastRawImage::Rotate(scaledStroke, strokeIter->second.theta);
										FastRawImage::Rotate(scaledStroke, strokeIter->second.theta, rotatedStroke);
										FastRawImage::Blend(painting, rotatedStroke, strokeIter->second.rgb, strokeIter->second.xc, strokeIter->second.yc);										

										//delete rotatedStroke;

										totalStrokes++;
									}

									delete scaledStroke;
								}
							}

							return painting;
						}


						FastRawImage* NativeMomentPainter::CreatePainting(FastRawImage* input, FastRawImage* strokeImage, int S)
						{
							ComputationProgress::Instance()->percentComplete = 0;

							StrokeContainer strokeContainer;
							StrokeContainer::AreaToDimensionsMapType::iterator iter;

							GeneratePaintingStrokes(input, S, strokeContainer);

							NativeStroke stroke;
							FastRawImage *scaledStroke;
							FastRawImage *painting = new FastRawImage(input->Width, input->Height);
							painting->WhiteOut();

							StrokeContainer::DimensionToStrokesMapType::iterator strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator start_strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator end_strokeIter;
							std::set< std::pair<int, int> >::iterator dimensionsIter;
							std::pair<int, int> dimensions;
							int totalStrokes = 0;
							int totalScalings = 0;
							int totalAreaValues = 0;
							for(iter = strokeContainer.areaToStrokeDimensionsMap.begin(); iter != strokeContainer.areaToStrokeDimensionsMap.end(); ++iter)
							{
								for (dimensionsIter = iter->second.begin(); dimensionsIter != iter->second.end(); ++dimensionsIter) 
								{
									dimensions = *dimensionsIter;
									scaledStroke = FastRawImage::Scale(strokeImage, static_cast<int>(dimensions.first), static_cast<int>(dimensions.second));
									totalScalings++;

									start_strokeIter = strokeContainer.strokeDimensionsToStrokesMap.lower_bound(*dimensionsIter);
									end_strokeIter = strokeContainer.strokeDimensionsToStrokesMap.upper_bound(*dimensionsIter);
									int max = (scaledStroke->Width > scaledStroke->Height) ? scaledStroke->Width : scaledStroke->Height;

									// TODO: This object is on the stack but the underlying singledata is still on the heap, and it's not freed.  MEMORY PENALTY
									FastRawImage rotatedStroke(max + max, max + max);
									//StackBasedRawImage rotatedStroke(max + max, max + max);

									for (strokeIter = start_strokeIter; strokeIter != end_strokeIter; ++strokeIter)
									{
										rotatedStroke.BlackOut();

										//rotatedStroke = FastRawImage::Rotate(scaledStroke, strokeIter->second.theta);
										FastRawImage::Rotate(scaledStroke, strokeIter->second.theta, rotatedStroke);
										FastRawImage::Blend(painting, rotatedStroke, strokeIter->second.rgb, strokeIter->second.xc, strokeIter->second.yc);										

										//delete rotatedStroke;

										totalStrokes++;
									}

									delete scaledStroke;
								}
							}

							return painting;
						}


						
						*/

						void INativeMomentsPainter::GeneratePaintingStrokes(FastRawImage* input, int S, IStrokeContainer &strokeContainer, std::pair<int, int> strokeOffset, float percentWorkToBeCompletedInThisCall) 
						{
							float halfPercentage = percentWorkToBeCompletedInThisCall / 2.0f;

							FastRawImage *areaImage = ComputeAreaImageFast2(input, S);
							
							ComputationProgress::Instance()->percentComplete += static_cast<int>(halfPercentage);

							int factor = 1;
							int level = 1;
							int size = MAX(input->Width, input->Height);						
							
							while (size > 4*S) 
							{
								GetStrokes(input, S, areaImage, factor, level, strokeContainer, strokeOffset);
								size /= 2;
								factor *= 2;
								level++;
							}			

							ComputationProgress::Instance()->percentComplete += static_cast<int>(halfPercentage);

							delete areaImage;
						}


						
						void INativeMomentsPainter::GetStrokes(FastRawImage* input, int S, FastRawImage* area, int factor, int level, IStrokeContainer &strokeContainer, std::pair<int, int> strokeOffset) 
						{	
							int x, y;

							FastRawImage* scaledWidth = FastRawImage::ScaleWidth(area, area->Width / factor);
							FastRawImage* scaledAreaImg = FastRawImage::ScaleHeight(scaledWidth, area->Height / factor);

							LARGE_INTEGER start, stop, freq;
							QueryPerformanceCounter(&start);

							FastRawImage* dithered = Dither(scaledAreaImg, 4*S/sqrt(static_cast<double>(level)), 2.0f/level);

							// your function code here

							QueryPerformanceCounter(&stop);
							QueryPerformanceFrequency(&freq);

							printf("Dither took %f ms\n", (double)(stop.QuadPart - start.QuadPart) / (double)freq.QuadPart * 1000.0);


							QueryPerformanceCounter(&start);

							FastRawImage* inputScaledWidth = FastRawImage::ScaleWidth(input, input->Width / factor);
							FastRawImage* inscaled = FastRawImage::ScaleHeight(inputScaledWidth, input->Height / factor);

							QueryPerformanceCounter(&stop);
							QueryPerformanceFrequency(&freq);

							printf("Scaling took %f ms\n", (double)(stop.QuadPart - start.QuadPart) / (double)freq.QuadPart * 1000.0);


							int maxY = dithered->Height; 
							int maxX = dithered->Width;

							NativeStroke s;
							pair<int, int> lwPair;
							
							QueryPerformanceCounter(&start);


							for (y = 0; y < maxY; y++) 
							{
								for (x = 0; x < maxX; x++) 
								{
									if (dithered->GetColor(x, y).R == 0)
									{
										if (GenerateStroke(inscaled, x, y, S, factor, level, s) == ValidStroke) 
										{
											s.xc += strokeOffset.first;
											s.yc += strokeOffset.second;
											strokeContainer.AddStroke(s);
											//strokeContainer.areaToStrokeDimensionsMap[s.area].insert( pair<int, int>(s.l, s.w) );
											//strokeContainer.strokeDimensionsToStrokesMap.insert(pair< std::pair<int, int>, NativeStroke>(pair<int, int>(s.l, s.w), s));		
										}
									}			
								}
							}	

							QueryPerformanceCounter(&stop);
							QueryPerformanceFrequency(&freq);

							printf("All GenerateStroke calls took %f ms\n", (double)(stop.QuadPart - start.QuadPart) / (double)freq.QuadPart * 1000.0);



							// Clean up
							delete scaledWidth;
							delete scaledAreaImg;
							delete dithered;
							delete inputScaledWidth;
							delete inscaled;

							//return list.size;
						}
					

						FastRawImage* INativeMomentsPainter::ComputeAreaImageFast2(FastRawImage* inMem, int s)
						{
							FastRawImage* strokeAreaImage = new FastRawImage(inMem->Width, inMem->Height);
							int pixel = 0;

							FastRawImage piece(s, s, false);

							for (int i = 0; i < inMem->Height; i++) 
							{				
								//StatusReporter::Instance->SetPercentComplete(((float)i / (float)input->Height) * 100.0f);

								for (int j = 0; j < inMem->Width; j++) 
								{		
									inMem->Crop(j, i, s, s, piece);
									PixelData topColor = inMem->GetColor(j, i);
									double m = Moment00Fast2(piece, topColor);	                 						
									pixel = (int)(255.0 * m / (piece.Width * piece.Height));

									strokeAreaImage->SetColor(j, i, PixelData(pixel, pixel, pixel));																
									// For fast access
									//strokeAreaImage->singledata[i * strokeAreaImage->Width + j] = PixelData(pixel, pixel, pixel);
								}
							}

							return strokeAreaImage;
						}


						inline double INativeMomentsPainter::MomentI(PixelData &bgrxy, PixelData &bgr)
						{
							double d2, r2;
							static double d02 = 150.0 * 150.0;

							d2 = ColorDistance(bgrxy, bgr);

							if (d2 >= d02)
								return 0.0;

							r2 = d2 / d02;

							return ((1 - r2) * (1 - r2));
						}

						inline double INativeMomentsPainter::Moment00Fast2(FastRawImage &piece, PixelData &bgr)					
						{
							double m = 0.0;

							for (int i = 0; i < piece.Height; i++) 
							{
								for (int j = 0; j < piece.Width; j++) 
								{
									PixelData c = piece.GetColor(j, i); //[i * width + j];

									m += MomentI(c, bgr);
								}
							}

							return m;
						}

						/*
						DWORD MakeDwordColor(int r, int g, int b)
						{
							DWORD dwStuff = ((r << 16) | (g << 8) | (b));

							return dwStuff;
						}
						*/

						int INativeMomentsPainter::ComputeTotalStrokes(StrokeContainer &strokeContainer)
						{
							StrokeContainer::DimensionToStrokesMapType::iterator strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator start_strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator end_strokeIter;
							std::set< std::pair<int, int> >::iterator dimensionsIter;
							std::pair<int, int> dimensions;

							int strokeCounter = 0;
							for(StrokeContainer::AreaToDimensionsMapType::iterator iter = strokeContainer.areaToStrokeDimensionsMap.begin(); iter != strokeContainer.areaToStrokeDimensionsMap.end(); ++iter)
							{
								for (dimensionsIter = iter->second.begin(); dimensionsIter != iter->second.end(); ++dimensionsIter) 
								{
									dimensions = *dimensionsIter;									

									start_strokeIter = strokeContainer.strokeDimensionsToStrokesMap.lower_bound(*dimensionsIter);
									end_strokeIter = strokeContainer.strokeDimensionsToStrokesMap.upper_bound(*dimensionsIter);

									for (strokeIter = start_strokeIter; strokeIter != end_strokeIter; ++strokeIter)
									{
										strokeCounter++;
									}
								}
							}

							return strokeCounter;
						}


						//int INativeMomentsPainter::GenerateMovieFrames(CwmvFile *wmvFile, FastRawImage *sourceImageData, int frames)
						//{																		
						//	int counter = 0;
						//	int totalVideoArrayElements = (sourceImageData->Height * sourceImageData->Width) - 1;
						//	DWORD* dwArray = (DWORD*) malloc(sourceImageData->Height * sourceImageData->Width * sizeof(DWORD));		

						//	// First, construct data array from source Image
						//	for(int y=(sourceImageData->Height-1);  y >= 0; y--)
						//	{
						//		for(int x=0;  x < sourceImageData->Width; x++)
						//		{
						//			PixelData &colorData = sourceImageData->GetColor(x,y);
						//			dwArray[counter++] = ((colorData.R << 16) | (colorData.G << 8) | (colorData.B)); //MakeDwordColor(colorData.R, colorData.G, colorData.B); 
						//		}
						//	}

						//	// Next, generate the requested number of frames
						//	int generatedFrames = 0;
						//	for (int i=0; i < frames; i++)
						//	{
						//		if(FAILED(wmvFile->AppendNewFrame(sourceImageData->Width, sourceImageData->Height, dwArray, 32)))
						//		{
						//			// TODO: How to handle this error?
						//		}
						//		else
						//		{
						//			generatedFrames++;
						//		}
						//	}

						//	free(dwArray);
						//	dwArray = NULL;								

						//	// Return number of generated frames
						//	return generatedFrames;
						//}


						FastRawImage* INativeMomentsPainter::Render(StrokeContainer &strokeContainer)
						{				
							//std::cout << "Entering INativeMomentsPainter::Render" << std::endl;

							NativeStroke stroke;
							FastRawImage *scaledStroke;
							FastRawImage *painting = new FastRawImage(width, height);
							painting->WhiteOut();

							StrokeContainer::DimensionToStrokesMapType::iterator strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator start_strokeIter;
							StrokeContainer::DimensionToStrokesMapType::iterator end_strokeIter;
							std::set< std::pair<int, int> >::iterator dimensionsIter;
							std::pair<int, int> dimensions;
							int totalStrokes = 0;
							int totalScalings = 0;
							int totalAreaValues = 0;
							float maxAreaValues = static_cast<float>(strokeContainer.areaToStrokeDimensionsMap.size());

							//CwmvFile wmvFile;
							int precomputedTotalStrokes = 0;
							int strokeVideoModulo = 3;
							int framesPerSecond = 100;
							float desiredPaintingTimeInSeconds = 8;
							int desiredFrames = desiredPaintingTimeInSeconds * framesPerSecond;
							int totalFrames = 0;
							if (this->generateMovie)
							{
								precomputedTotalStrokes = ComputeTotalStrokes(strokeContainer);								

								strokeVideoModulo = static_cast<int>(static_cast<float>(precomputedTotalStrokes) / static_cast<float>(desiredFrames));
								if (strokeVideoModulo == 0)
								{
									strokeVideoModulo = 1;
								}

								std::cout << precomputedTotalStrokes << " strokes " << std::flush;
/*
								wmvFile.PrepareToGenerate(WMProfile_V80_384Video, 1, this->movieFilename);

								for (int introFrameIndex=0; introFrameIndex < this->introFrames.size(); introFrameIndex++)
								{
									totalFrames += GenerateMovieFrames(&wmvFile, this->introFrames[introFrameIndex], 300);
								}*/
							}


							for(StrokeContainer::AreaToDimensionsMapType::iterator iter = strokeContainer.areaToStrokeDimensionsMap.begin(); iter != strokeContainer.areaToStrokeDimensionsMap.end(); ++iter)
							{
								totalAreaValues++;
								ComputationProgress::Instance()->percentComplete = static_cast<int>(((static_cast<float>(totalAreaValues+1) / maxAreaValues) * 50) + 50);

								for (dimensionsIter = iter->second.begin(); dimensionsIter != iter->second.end(); ++dimensionsIter) 
								{
									dimensions = *dimensionsIter;									
									scaledStroke = FastRawImage::Scale(strokes[rand() % strokes.size()], static_cast<int>(dimensions.first), static_cast<int>(dimensions.second));
									totalScalings++;

									start_strokeIter = strokeContainer.strokeDimensionsToStrokesMap.lower_bound(*dimensionsIter);
									end_strokeIter = strokeContainer.strokeDimensionsToStrokesMap.upper_bound(*dimensionsIter);
									int max = (scaledStroke->Width > scaledStroke->Height) ? scaledStroke->Width : scaledStroke->Height;

									// TODO: This object is on the stack but the underlying singledata is still on the heap, and it's not freed.  MEMORY PENALTY
									FastRawImage rotatedStroke(max + max, max + max);
									//StackBasedRawImage rotatedStroke(max + max, max + max);

									for (strokeIter = start_strokeIter; strokeIter != end_strokeIter; ++strokeIter)
									{
										rotatedStroke.BlackOut();

										//rotatedStroke = FastRawImage::Rotate(scaledStroke, strokeIter->second.theta);
										FastRawImage::Rotate(scaledStroke, strokeIter->second.theta, rotatedStroke);
										FastRawImage::Blend(painting, rotatedStroke, strokeIter->second.rgb, strokeIter->second.xc, strokeIter->second.yc);										

										//delete rotatedStroke;

										// Generate movie frames, if enabled
										/*
										if (this->generateMovie)
										{
											if ((totalStrokes % strokeVideoModulo) == 0)
											{	
												totalFrames += GenerateMovieFrames(&wmvFile, painting, 1);
											}
										}										
										*/

										totalStrokes++;
									}

									delete scaledStroke;
								}
							}

							// Finally, render the final frame for another few seconds
							if (this->generateMovie)
							{
								int totalFinalFrames = framesPerSecond * 3;  // Hold final for 3 seconds 
								//totalFrames += GenerateMovieFrames(&wmvFile, painting, totalFinalFrames);
							}

							std::cout << totalFrames << " frames..." << std::flush;

							//std::cout << "Exiting INativeMomentsPainter::Render" << std::endl;
							return painting;
						}
					}
				}
			}
		}
	}
}
