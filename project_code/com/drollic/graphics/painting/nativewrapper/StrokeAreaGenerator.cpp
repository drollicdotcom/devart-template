#include "stdafx.h"
#include "StrokeAreaGenerator.h"
#include "NativeMomentComputation.h"
#include "RawImage.h"
#include "NativeStroke.h"
#include "ComputationProgress.h"

#include <list>
#include <vector>
#include <queue>

#pragma managed

using namespace System;
using namespace System::Collections;

using namespace com::raymatthieu::util;
using namespace com::raymatthieu::graphics::painting::native::processing;


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
						UnsafeBitmap^ StrokeAreaGenerator::FastRaw2Unsafe(FastRawImage* input)
						{
							Bitmap^ bmp = gcnew Bitmap(input->Width, input->Height);
							UnsafeBitmap^ result = gcnew UnsafeBitmap(bmp);

							com::raymatthieu::graphics::painting::native::processing::PixelData pixel;
							com::raymatthieu::graphics::PixelData pd;
							for (int y=0; y < input->Height; y++)
							{
								for (int x=0; x < input->Width; x++)
								{
									pixel = input->GetColor(x, y);
									pd.red = pixel.R;
									pd.green = pixel.G;
									pd.blue = pixel.B;
									result->SetPixel(x, y, pd);
								}
							}

							return result;
						}

						int StrokeAreaGenerator::GetPaitingPercentComplete()
						{
							return ComputationProgress::Instance()->percentComplete;
						}

						UnsafeBitmap^ StrokeAreaGenerator::GenerateCompositePainting(int width, int height, array<UnsafeBitmap^>^ input, array<Point>^ locations, array<UnsafeBitmap^>^ strokes, int S)
						{
							std::vector<FastRawImage*> inputOriginals;							
							std::vector<std::pair<int, int> > originalLocations;							
							static std::vector<FastRawImage*> faststrokes;

							for (int i=0; i < input->Length; i++)
							{
								inputOriginals.push_back(Unsafe2FastRaw(input[i]));
								originalLocations.push_back(std::pair<int, int>(locations[i].X, locations[i].Y));
							}

							if (faststrokes.size() == 0)
							{
								for (int i=0; i < strokes->Length; i++)
								{
									faststrokes.push_back(Unsafe2FastRaw(strokes[i]));									
								}
							}

							//FastRawImage* fastBrush = Unsafe2FastRaw(stroke);
							FastRawImage* painting = NativeMomentComputation::CreateCompositePainting(width, height, inputOriginals, originalLocations, faststrokes, S);
							UnsafeBitmap^ result = FastRaw2Unsafe(painting);

							for (unsigned int i=0; i < inputOriginals.size(); i++)
							{
								delete inputOriginals[i];
								inputOriginals[i] = NULL;
							}
							
							delete painting;
							return result;
						}


						UnsafeBitmap^ StrokeAreaGenerator::GeneratePainting(UnsafeBitmap^ input, UnsafeBitmap^ stroke, int S)
						{
							FastRawImage* fastInput = Unsafe2FastRaw(input);
							FastRawImage* fastBrush = Unsafe2FastRaw(stroke);
							FastRawImage* painting = NativeMomentComputation::CreatePaintingReducedAlloc(fastInput, fastBrush, S);
							UnsafeBitmap^ result = FastRaw2Unsafe(painting);
							delete painting;
							delete fastInput;
							delete fastBrush;
							return result;
						}

						Hashtable^ StrokeAreaGenerator::GenerateTestImages(UnsafeBitmap ^input, UnsafeBitmap^ stroke, int S)
						{
							Hashtable^ results = gcnew Hashtable();

							FastRawImage* painting = NativeMomentComputation::CreatePainting(Unsafe2FastRaw(input), Unsafe2FastRaw(stroke), S);
							results->Add("Lots of allocations", FastRaw2Unsafe(painting));

							FastRawImage* painting2 = NativeMomentComputation::CreatePaintingReducedAlloc(Unsafe2FastRaw(input), Unsafe2FastRaw(stroke), S);
							results->Add("Reduced allocation", FastRaw2Unsafe(painting2));

							/*
							FastRawImage *area = NativeMomentComputation::ComputeAreaImageFast2(Unsafe2FastRaw(input), S);
							results->Add("Area Image", FastRaw2Unsafe(area));

							int factor = 1;
							int level = 1;
							int size = System::Math::Max(input->Width, input->Height);						

							while (size > 4*S) 
							{
							FastRawImage* scaledWidth = FastRawImage::ScaleWidth(area, area->Width / factor);
							FastRawImage* img = FastRawImage::ScaleHeight(scaledWidth, area->Height / factor);							
							results->Add(String::Concat("Scaled area, factor: ", factor), FastRaw2Unsafe(img));

							FastRawImage* dithered = NativeMomentComputation::Dither(img, 4*S/System::Math::Sqrt(static_cast<double>(level)), 2.0f/level);
							results->Add(String::Concat("Dithered, factor: ", factor), FastRaw2Unsafe(dithered));

							FastRawImage* inputScaledWidth = FastRawImage::ScaleWidth(area, input->Width / factor);
							FastRawImage* inscaled = FastRawImage::ScaleHeight(inputScaledWidth, input->Height / factor);
							results->Add(String::Concat("Input scaled, factor: ", factor), FastRaw2Unsafe(inscaled));

							size /= 2;
							factor *= 2;
							level++;
							}
							*/


							return results;
						}

						ArrayList^ StrokeAreaGenerator::GeneratePaintingStrokes(UnsafeBitmap ^input, int s)
						{
							DateTime start = DateTime::Now;
							FastRawImage *inMem = Unsafe2FastRaw(input);

							std::vector<NativeStroke> strokes;
							// TODO: MUST PUT THIS BACK IN OR FAST RENDERING WON'T WORK FROM MANAGED CODE
							//NativeMomentComputation::GeneratePaintingStrokes(inMem, s, strokes);

							//array<Stroke>^ finalStrokes = gcnew array<Stroke> (strokes.size());
							ArrayList ^finalStrokes = gcnew ArrayList();
							Stroke str;
							std::vector<int> test;
							for (unsigned int i=0; i < strokes.size(); i++)
							{
								str.xc = strokes[i].xc;
								str.yc = strokes[i].yc;
								str.w = strokes[i].w;
								str.l = strokes[i].l;
								str.theta = strokes[i].theta;
								str.red = strokes[i].rgb.R;
								str.green = strokes[i].rgb.G;
								str.blue = strokes[i].rgb.B;
								str.level = strokes[i].level;
								finalStrokes->Add(str);
							}

							TimeSpan duration = DateTime::Now - start;
							System::Console::WriteLine("Native Stroke generation computed in " + duration.TotalMilliseconds + "ms");

							return finalStrokes;
						}


						/*
						array<Color>^ StrokeAreaGenerator::Generate(UnsafeBitmap ^input, int s)
						{			
						DateTime start = DateTime::Now;

						array<Color>^ cImg = gcnew array<Color> (input->Width * input->Height);
						RawImage *inMem = new RawImage(input->Width, input->Height);

						Color c;
						for (int j=0; j < input->Height; j++)
						{		
						for (int i=0; i < input->Width; i++)
						{
						c = input->GetPixel(i, j);
						inMem->singledata[j * input->Width + i] = com::raymatthieu::graphics::painting::native::processing::PixelData(c.R, c.G, c.B);;
						}
						}

						int counter = 0;
						int pixVal = 0;

						for (int i = 0; i < input->Height; i++) 
						{				
						StatusReporter::Instance->SetPercentComplete(static_cast<int>(((float)i / (float)input->Height) * 100.0f));

						for (int j = 0; j < input->Width; j++) 
						{						
						pixVal = static_cast<int>(NativeMomentComputation::ComputeStrokeAreaPixel(inMem, i, j, s));

						cImg[i * input->Width + j] = Color::FromArgb(pixVal, pixVal, pixVal);
						}
						}

						//img.Save("area.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

						//TimeSpan duration = DateTime::Now - start;
						//System::Console::WriteLine("Stroke area computed in " + duration.TotalMilliseconds + "ms");

						delete inMem;

						return cImg;
						}




						array<Color>^ StrokeAreaGenerator::GenerateFast1(UnsafeBitmap ^input, int s)
						{			
						DateTime start = DateTime::Now;

						array<Color>^ cImg = gcnew array<Color> (input->Width * input->Height);
						RawImage *inMem = new RawImage(input->Width, input->Height);

						Color c;
						for (int j=0; j < input->Height; j++)
						{		
						for (int i=0; i < input->Width; i++)
						{
						c = input->GetPixel(i, j);
						inMem->singledata[j * input->Width + i] = com::raymatthieu::graphics::painting::native::processing::PixelData(c.R, c.G, c.B);;
						}
						}

						int counter = 0;
						int pixVal = 0;

						DateTime startAnalysis = DateTime::Now;
						RawImage *strokeAreaImage = NativeMomentComputation::ComputeAreaImage(inMem, s);
						TimeSpan durationAnalysis = DateTime::Now - startAnalysis;
						StatusReporter::Instance->StatusMessage("Native Analysis duration: " + durationAnalysis.TotalMilliseconds + "ms");


						for (int y = 0; y < input->Height; y++) 
						{											
						for (int x = 0; x < input->Width; x++) 
						{							
						pixVal = strokeAreaImage->GetColor(x, y).R;  // All colors are same.								
						cImg[y * input->Width + x] = Color::FromArgb(pixVal, pixVal, pixVal);
						}
						}

						//img.Save("area.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

						delete inMem;

						TimeSpan duration = DateTime::Now - start;
						System::Console::WriteLine("Native Stroke area computed in " + duration.TotalMilliseconds + "ms");

						return cImg;
						}
						*/

						FastRawImage* StrokeAreaGenerator::Unsafe2FastRaw(UnsafeBitmap ^input)
						{
							FastRawImage *inMem = new FastRawImage(input->Width, input->Height);

							Color c;
							for (int j=0; j < input->Height; j++)
							{		
								for (int i=0; i < input->Width; i++)
								{
									c = input->GetPixel(i, j);
									inMem->singledata[j * input->Width + i] = com::raymatthieu::graphics::painting::native::processing::PixelData(c.R, c.G, c.B);
								}
							}

							return inMem;
						}

						array<Color>^ StrokeAreaGenerator::GenerateFast2(UnsafeBitmap ^input, int s)
						{			
							DateTime start = DateTime::Now;

							array<Color>^ cImg = gcnew array<Color> (input->Width * input->Height);
							FastRawImage *inMem = new FastRawImage(input->Width, input->Height);

							Color c;
							for (int j=0; j < input->Height; j++)
							{		
								for (int i=0; i < input->Width; i++)
								{
									c = input->GetPixel(i, j);
									inMem->singledata[j * input->Width + i] = com::raymatthieu::graphics::painting::native::processing::PixelData(c.R, c.G, c.B);;
								}
							}

							int counter = 0;
							int pixVal = 0;

							DateTime startAnalysis = DateTime::Now;
							FastRawImage *strokeAreaImage = NativeMomentComputation::ComputeAreaImageFast2(inMem, s);
							TimeSpan durationAnalysis = DateTime::Now - startAnalysis;
							StatusManager::Instance->StatusMessage("Native Analysis duration: " + durationAnalysis.TotalMilliseconds + "ms");


							for (int y = 0; y < input->Height; y++) 
							{											
								for (int x = 0; x < input->Width; x++) 
								{							
									pixVal = strokeAreaImage->GetColor(x, y).R;  // All colors are same.								
									cImg[y * input->Width + x] = Color::FromArgb(pixVal, pixVal, pixVal);
								}
							}

							//img.Save("area.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

							delete inMem;

							TimeSpan duration = DateTime::Now - start;
							System::Console::WriteLine("Native Stroke area computed in " + duration.TotalMilliseconds + "ms");

							return cImg;
						}

						/*

						#pragma unmanaged

						inline RawImage* StrokeAreaProcessing::ComputeAreaImage(RawImage* inMem, int s)
						{
						RawImage* strokeAreaImage = new RawImage(inMem->Width, inMem->Height);
						int pixel = 0;

						for (int i = 0; i < inMem->Height; i++) 
						{				
						//StatusReporter::Instance->SetPercentComplete(((float)i / (float)input->Height) * 100.0f);

						for (int j = 0; j < inMem->Width; j++) 
						{						
						pixel = StrokeAreaProcessing::ComputeStrokeAreaPixel(inMem, i, j, s);
						strokeAreaImage->SetColor(j, i, PixelData(pixel, pixel, pixel));																
						}
						}

						return strokeAreaImage;
						}

						inline double StrokeAreaProcessing::ComputeStrokeAreaPixel(RawImage* inMem, int i, int j, int s)
						{
						RawImage* piece = inMem->Crop(j, i, s, s);			         						

						PixelData topColor = inMem->GetColor(j, i);

						double m = StrokeAreaProcessing::Moment00(piece->singledata, piece->Width, piece->Height, topColor);	                 						

						int result = (int)(255.0 * m / (piece->Width * piece->Height));

						delete piece;

						return  result;
						}

						inline double StrokeAreaProcessing::ColorDistance(PixelData &bgr1, PixelData &bgr2)
						{
						int dr = bgr1.R - bgr2.R;
						int dg = bgr1.G - bgr2.G;
						int db = bgr1.B - bgr2.B;

						return dr * dr + dg * dg + db * db;
						}


						inline double StrokeAreaProcessing::MomentI(PixelData &bgrxy, PixelData &bgr)
						{
						double d2, r2;
						static double d02 = 150.0 * 150.0;

						d2 = ColorDistance(bgrxy, bgr);

						if (d2 >= d02)
						return 0.0;

						r2 = d2 / d02;

						return ((1 - r2) * (1 - r2));
						}


						inline double StrokeAreaProcessing::Moment00(PixelData *singledata, int width, int height, PixelData &bgr)
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
						*/
					}
				}
			}
		}
	}
}