#include "stdafx.h"

#include "NativeMomentComputation.h"
#include "ComputationProgress.h"

#include <math.h>

#include "TwoDVector.h"
#include "NativeStroke.h"

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

#define iround(x) (int) (x + 0.5)

#define MAX(x,y) ((x)>(y)?(x):(y))

#define swap(a,b) \
	do { \
	float *tmp; \
	tmp = a; \
	a = b; \
	b = tmp; \
	} while (0)

bool compare_NativeStrokes(const NativeStroke &a, const NativeStroke &b) 
{
    return (a.l*a.w) > (b.l*b.w);
}


						inline int NativeMomentComputation::ComputeMoments(FastRawImage &input, PixelData &bgr, int S, double &m00, double &m01, double &m10, double &m11, double &m02, double &m20)
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

						StrokeGenerationResult NativeMomentComputation::GenerateStroke(FastRawImage *input, int x, int y, int s, int factor, int level, NativeStroke &stroke) 
						{
							//ArrayList list = new ArrayList();
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



						FastRawImage* NativeMomentComputation::Dither(FastRawImage* inImage, double s, double p)
						{
							int width = inImage->Width;
							int height = inImage->Height;

							TwoDVector<int> input(height, width);

							FastRawImage* img = new FastRawImage(width, height);
							int w, h;
							float error, val;
							float a = (float) ((s - 1) / pow(255.0, p));        

							TwoDVector<int> inputPrime(height, width);
							/*
							int **inputPrime = NULL;//new int[height][width];
							inputPrime = new int*[height];
							for (int i = 0; i < height; i++)
							inputPrime[i] = new int[width];
							*/

							// Convert into collection of Color objects
							for (int y = 0; y < inImage->Height; y++) 
							{
								for (int x = 0; x < inImage->Width; x++) 
								{               
									int r = inImage->GetColor(x, y).R;
									input.SetAt(y, x, r);
								}
							}

							//System.Random rand = new System->Random();	
							srand(100);

							w = width;
							h = height;

							/* allocates working space */	
							float *cur = new float [w];
							float *nxt = new float [w];

							/* inicializa o buffer de proxima linha */
							for (int x = 0; x < w; x++) 
							{
								nxt[x] = MAX(1.0f /(a * (float)(pow(static_cast<double>(input[0][x]), p)) + 1), 0.5f);
							}

							int yClone = 0;
							for (int y=0; y < h-1; y++, yClone = y) 
							{
								/* next line becomes current line */
								swap(cur, nxt);							

								/* copies next line to local buffer */
								nxt[0] = MAX(1.0f /(a *(float)(pow(static_cast<double>(input[y+1][0]), p)) + 1), 0.5f);

								for (int x = 1; x < w; x++) 
								{
									nxt[x] = 1.0f / (a * (float)(pow(static_cast<double>(input[y+1][x]), p)) + 1);
								}

								/* spread error */
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

							/* clip whole last line */
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


						FastRawImage* NativeMomentComputation::CreatePainting(FastRawImage* input, FastRawImage* strokeImage, int S)
						{
							StrokeContainer strokeContainer;
							StrokeContainer::AreaToDimensionsMapType::iterator iter;

							GeneratePaintingStrokes(input, S, strokeContainer);
							
							NativeStroke stroke;
							FastRawImage *scaledStroke, *rotatedStroke;
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

									for (strokeIter = start_strokeIter; strokeIter != end_strokeIter; ++strokeIter)
									{
										rotatedStroke = FastRawImage::Rotate(scaledStroke, strokeIter->second.theta);
										FastRawImage::Blend(painting, rotatedStroke, strokeIter->second.rgb, strokeIter->second.xc, strokeIter->second.yc);										

										delete rotatedStroke;

										totalStrokes++;
									}

									delete scaledStroke;
								}
							}

							return painting;
						}

						FastRawImage* NativeMomentComputation::CreateCompositePainting(int width, int height, std::vector<FastRawImage*> &originals, std::vector<std::pair<int, int> > &locations, std::vector<FastRawImage*> &strokes, int S)
						{
							StrokeContainer strokeContainer;
							StrokeContainer::AreaToDimensionsMapType::iterator iter;

							unsigned int totalOriginals = originals.size();
							float totalOriginalsf = static_cast<float>(totalOriginals);
							
							ComputationProgress::Instance()->percentComplete = 0;
							for (unsigned int i=0; i < totalOriginals; i++)
							{
								GeneratePaintingStrokes(originals[i], S, strokeContainer, locations[i]);
								float percentComplete = (static_cast<float>(i+1) / totalOriginalsf);
								ComputationProgress::Instance()->percentComplete = percentComplete * 50;								
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


						FastRawImage* NativeMomentComputation::CreatePaintingReducedAlloc(FastRawImage* input, FastRawImage* strokeImage, int S)
						{
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


						void NativeMomentComputation::GeneratePaintingStrokes(FastRawImage* input, int S, StrokeContainer &strokeContainer, std::pair<int, int> strokeOffset) 
						{
							FastRawImage *areaImage = ComputeAreaImageFast2(input, S);

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

							delete areaImage;
						}



						void NativeMomentComputation::GetStrokes(FastRawImage* input, int S, FastRawImage* area, int factor, int level, StrokeContainer &strokeContainer, std::pair<int, int> strokeOffset) 
						{	
							int x, y;

							FastRawImage* scaledWidth = FastRawImage::ScaleWidth(area, area->Width / factor);
							FastRawImage* scaledAreaImg = FastRawImage::ScaleHeight(scaledWidth, area->Height / factor);

							FastRawImage* dithered = Dither(scaledAreaImg, 4*S/sqrt(static_cast<double>(level)), 2.0f/level);

							FastRawImage* inputScaledWidth = FastRawImage::ScaleWidth(input, input->Width / factor);
							FastRawImage* inscaled = FastRawImage::ScaleHeight(inputScaledWidth, input->Height / factor);

							int maxY = dithered->Height; 
							int maxX = dithered->Width;

							NativeStroke s;
							pair<int, int> lwPair;
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
											strokeContainer.areaToStrokeDimensionsMap[s.area].insert( pair<int, int>(s.l, s.w) );
											strokeContainer.strokeDimensionsToStrokesMap.insert(pair< std::pair<int, int>, NativeStroke>(pair<int, int>(s.l, s.w), s));		
										}
									}			
								}
							}	

							// Clean up
							delete scaledWidth;
							delete scaledAreaImg;
							delete dithered;
							delete inputScaledWidth;
							delete inscaled;

							//return list.size;
						}


						/*
						RawImage* NativeMomentComputation::ComputeAreaImage(RawImage* inMem, int s)
						{
							RawImage* strokeAreaImage = new RawImage(inMem->Width, inMem->Height);
							int pixel = 0;

							for (int i = 0; i < inMem->Height; i++) 
							{				
								//StatusReporter::Instance->SetPercentComplete(((float)i / (float)input->Height) * 100.0f);

								for (int j = 0; j < inMem->Width; j++) 
								{						
									pixel = static_cast<int>(NativeMomentComputation::ComputeStrokeAreaPixel(inMem, i, j, s));
									strokeAreaImage->SetColor(j, i, PixelData(pixel, pixel, pixel));																
								}
							}

							return strokeAreaImage;
						}
						*/


						FastRawImage* NativeMomentComputation::ComputeAreaImageFast2(FastRawImage* inMem, int s)
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
									double m = NativeMomentComputation::Moment00Fast2(piece, topColor);	                 						
									pixel = (int)(255.0 * m / (piece.Width * piece.Height));

									strokeAreaImage->SetColor(j, i, PixelData(pixel, pixel, pixel));																
									// For fast access
									//strokeAreaImage->singledata[i * strokeAreaImage->Width + j] = PixelData(pixel, pixel, pixel);
								}
							}

							return strokeAreaImage;
						}



/*
						double NativeMomentComputation::ComputeStrokeAreaPixel(RawImage* inMem, int i, int j, int s)
						{
							RawImage *piece = inMem->Crop(j, i, s, s);

							PixelData topColor = inMem->GetColor(j, i);

							double m = NativeMomentComputation::Moment00(piece->singledata, piece->Width, piece->Height, topColor);	                 						

							int result = (int)(255.0 * m / (piece->Width * piece->Height));

							delete piece;

							return  result;
						}
						*/


						inline double NativeMomentComputation::ColorDistance(PixelData &bgr1, PixelData &bgr2)
						{
							int dr = bgr1.R - bgr2.R;
							int dg = bgr1.G - bgr2.G;
							int db = bgr1.B - bgr2.B;

							return dr * dr + dg * dg + db * db;
						}


						inline double NativeMomentComputation::MomentI(PixelData &bgrxy, PixelData &bgr)
						{
							double d2, r2;
							static double d02 = 150.0 * 150.0;

							d2 = ColorDistance(bgrxy, bgr);

							if (d2 >= d02)
								return 0.0;

							r2 = d2 / d02;

							return ((1 - r2) * (1 - r2));
						}

						inline double NativeMomentComputation::Moment00Fast2(FastRawImage &piece, PixelData &bgr)					
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

						inline double NativeMomentComputation::Moment00(PixelData *singledata, int width, int height, PixelData &bgr)					
						{
							double m = 0.0;

							for (int i = 0; i < height; i++) 
							{
								for (int j = 0; j < width; j++) 
								{
									PixelData c = singledata[i * width + j];

									m += MomentI(c, bgr);
								}
							}

							return m;
						}

					}
				}
			}
		}
	}
}
