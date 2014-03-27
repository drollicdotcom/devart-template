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
#include "WaveletNativeMomentPainterWrapper.h"
#include "ImageConversion.h"
#include "NativeStroke.h"
#include "ComputationProgress.h"
#include "WaveletNativeMomentsPainter.h"

#include <list>
#include <vector>
#include <queue>

#pragma managed

using namespace System;
using namespace System::Collections;

using namespace com::drollic::util;
using namespace com::drollic::graphics::painting::native::processing;


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
					namespace wrapper
					{
						int WaveletNativeMomentPainterWrapper::GetPercentComplete()
						{
							return ComputationProgress::Instance()->percentComplete;
						}


						UnsafeBitmap^ WaveletNativeMomentPainterWrapper::CreatePaintingV2(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S)
						{
							array<UnsafeBitmap^>^ garbage;
							return CreatePaintingV2(width, height, originals, transformed, strokes, S, false, garbage);
						}

						UnsafeBitmap^ WaveletNativeMomentPainterWrapper::CreatePaintingV2(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S, bool generateMovie, array<UnsafeBitmap^>^ introFrames)
						{
							// Sanity check inputs
							if (originals->Length != transformed->Length)
							{
								ComputationProgress::Instance()->percentComplete = 100;
								UnsafeBitmap^ dummy;						
								return dummy;
							}

							ComputationProgress::Instance()->percentComplete = 0;

							std::vector<FastRawImage*> inputOriginals;							
							std::vector<FastRawImage*> inputTransformed;																				
							static std::vector<FastRawImage*> faststrokes;

							float percentWorkForAllConversion = 5.0;  // Five percent
							float percentWorkDonePerConversion = percentWorkForAllConversion / (originals->Length * 2);

							for (int i=0; i < originals->Length; i++)
							{
								ComputationProgress::Instance()->percentComplete += 1;

								inputOriginals.push_back(ImageConversion::Unsafe2FastRaw(originals[i]));

								// Update percentage work done
								ComputationProgress::Instance()->percentComplete += static_cast<int>(percentWorkDonePerConversion);

								inputTransformed.push_back(ImageConversion::Unsafe2FastRaw(transformed[i]));

								// Update percentage work done
								ComputationProgress::Instance()->percentComplete += static_cast<int>(percentWorkDonePerConversion);
							}

							// The faststrokes collection is static, so this
							// work only needs to be done once.
							if (faststrokes.size() == 0)
							{
								for (int i=0; i < strokes->Length; i++)
								{
									faststrokes.push_back(ImageConversion::Unsafe2FastRaw(strokes[i]));									
								}
							}

							WaveletNativeMomentsPainter painter(width, height, inputOriginals, inputTransformed, faststrokes, S);
							
							// Convert intro movie frames, if movie generation has been enabled
							std::vector<FastRawImage*> inputIntroMovieFrames;
							if (generateMovie)
							{
								// Convert intro movie frames
								for (int i=0; i < introFrames->Length; i++)
								{
									inputIntroMovieFrames.push_back(ImageConversion::Unsafe2FastRaw(introFrames[i]));
								}

								// Pass movie frames, and enable movie generation
								painter.EnableMovieGeneration(inputIntroMovieFrames, "junk");
							}

							FastRawImage* painting = painter.CreatePaintingV2();
							UnsafeBitmap^ result = ImageConversion::FastRaw2Unsafe(painting);

							for (unsigned int i=0; i < inputOriginals.size(); i++)
							{
								delete inputOriginals[i];
								delete inputTransformed[i];
								inputOriginals[i] = NULL;
								inputTransformed[i] = NULL;
							}
							
							// The faststrokes colleciton is static, so we DO NOT delete the
							// contents of the collection.

							// Delete title movie frame, if it was created
							for (unsigned int i=0; i < inputIntroMovieFrames.size(); i++)
							{
								delete inputIntroMovieFrames[i];
							}

							delete painting;
							return result;
						}					


						


						UnsafeBitmap^ WaveletNativeMomentPainterWrapper::CreatePainting(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S)
						{
							array<UnsafeBitmap^>^ garbage;
							return CreatePainting(width, height, originals, transformed, strokes, S, false, garbage);
						}

						UnsafeBitmap^ WaveletNativeMomentPainterWrapper::CreatePainting(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S, bool generateMovie, array<UnsafeBitmap^>^ introFrames)
						{
							// Sanity check inputs
							if (originals->Length != transformed->Length)
							{
								ComputationProgress::Instance()->percentComplete = 100;
								UnsafeBitmap^ dummy;						
								return dummy;
							}

							ComputationProgress::Instance()->percentComplete = 0;

							std::vector<FastRawImage*> inputOriginals;							
							std::vector<FastRawImage*> inputTransformed;																				
							static std::vector<FastRawImage*> faststrokes;

							float percentWorkForAllConversion = 5.0;  // Five percent
							float percentWorkDonePerConversion = percentWorkForAllConversion / (originals->Length * 2);

							for (int i=0; i < originals->Length; i++)
							{
								ComputationProgress::Instance()->percentComplete += 1;

								inputOriginals.push_back(ImageConversion::Unsafe2FastRaw(originals[i]));

								// Update percentage work done
								ComputationProgress::Instance()->percentComplete += static_cast<int>(percentWorkDonePerConversion);

								inputTransformed.push_back(ImageConversion::Unsafe2FastRaw(transformed[i]));

								// Update percentage work done
								ComputationProgress::Instance()->percentComplete += static_cast<int>(percentWorkDonePerConversion);
							}

							// The faststrokes collection is static, so this
							// work only needs to be done once.
							if (faststrokes.size() == 0)
							{
								for (int i=0; i < strokes->Length; i++)
								{
									faststrokes.push_back(ImageConversion::Unsafe2FastRaw(strokes[i]));									
								}
							}

							WaveletNativeMomentsPainter painter(width, height, inputOriginals, inputTransformed, faststrokes, S);
							
							// Convert intro movie frames, if movie generation has been enabled
							std::vector<FastRawImage*> inputIntroMovieFrames;
							if (generateMovie)
							{
								// Convert intro movie frames
								for (int i=0; i < introFrames->Length; i++)
								{
									inputIntroMovieFrames.push_back(ImageConversion::Unsafe2FastRaw(introFrames[i]));
								}

								// Pass movie frames, and enable movie generation
								painter.EnableMovieGeneration(inputIntroMovieFrames, "junk");
							}

							FastRawImage* painting = painter.CreatePainting();
							UnsafeBitmap^ result = ImageConversion::FastRaw2Unsafe(painting);

							for (unsigned int i=0; i < inputOriginals.size(); i++)
							{
								delete inputOriginals[i];
								delete inputTransformed[i];
								inputOriginals[i] = NULL;
								inputTransformed[i] = NULL;
							}
							
							// The faststrokes colleciton is static, so we DO NOT delete the
							// contents of the collection.

							// Delete title movie frame, if it was created
							for (unsigned int i=0; i < inputIntroMovieFrames.size(); i++)
							{
								delete inputIntroMovieFrames[i];
							}

							delete painting;
							return result;
						}					


						
						WaveletNativeMomentPainterWrapperResult^ WaveletNativeMomentPainterWrapper::CreatePainting2(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S)
						{
							array<UnsafeBitmap^>^ garbage;
							return CreatePainting2(width, height, originals, transformed, strokes, S, false, garbage);
						}


						WaveletNativeMomentPainterWrapperResult^ WaveletNativeMomentPainterWrapper::CreatePainting2(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S, bool generateMovie, array<UnsafeBitmap^>^ introFrames)
						{
							// Sanity check inputs
							if (originals->Length != transformed->Length)
							{
								ComputationProgress::Instance()->percentComplete = 100;
								WaveletNativeMomentPainterWrapperResult^ dummy;									
								return dummy;
							}

							ComputationProgress::Instance()->percentComplete = 0;

							std::vector<FastRawImage*> inputOriginals;							
							std::vector<FastRawImage*> inputTransformed;																				
							static std::vector<FastRawImage*> faststrokes;

							float percentWorkForAllConversion = 5.0;  // Five percent
							float percentWorkDonePerConversion = percentWorkForAllConversion / (originals->Length * 2);

							for (int i=0; i < originals->Length; i++)
							{
								ComputationProgress::Instance()->percentComplete += 1;

								inputOriginals.push_back(ImageConversion::Unsafe2FastRaw(originals[i]));

								// Update percentage work done
								ComputationProgress::Instance()->percentComplete += static_cast<int>(percentWorkDonePerConversion);

								inputTransformed.push_back(ImageConversion::Unsafe2FastRaw(transformed[i]));

								// Update percentage work done
								ComputationProgress::Instance()->percentComplete += static_cast<int>(percentWorkDonePerConversion);
							}

							// The faststrokes collection is static, so this
							// work only needs to be done once.
							if (faststrokes.size() == 0)
							{
								for (int i=0; i < strokes->Length; i++)
								{
									faststrokes.push_back(ImageConversion::Unsafe2FastRaw(strokes[i]));									
								}
							}

							WaveletNativeMomentsPainter painter(width, height, inputOriginals, inputTransformed, faststrokes, S);

							// Convert intro movie frames, if movie generation has been enabled
							std::vector<FastRawImage*> inputIntroMovieFrames;
							if (generateMovie)
							{
								// Convert intro movie frames
								for (int i=0; i < introFrames->Length; i++)
								{
									inputIntroMovieFrames.push_back(ImageConversion::Unsafe2FastRaw(introFrames[i]));
								}

								// Pass movie frames, and enable movie generation
								painter.EnableMovieGeneration(inputIntroMovieFrames, "junk");
							}


							WaveletNativeMomentsPainterResult* result = painter.CreatePainting2();

							WaveletNativeMomentPainterWrapperResult^ retValue = gcnew WaveletNativeMomentPainterWrapperResult();
							retValue->image = ImageConversion::FastRaw2Unsafe(result->painting);
							
							//List< List< System::Drawing::Rectangle

							for (unsigned int i=0; i < inputOriginals.size(); i++)
							{
								delete inputOriginals[i];
								delete inputTransformed[i];
								inputOriginals[i] = NULL;
								inputTransformed[i] = NULL;
							}
							
							// The faststrokes colleciton is static, so we DO NOT delete the
							// contents of the collection.

							// Delete title movie frame, if it was created
							for (unsigned int i=0; i < inputIntroMovieFrames.size(); i++)
							{
								delete inputIntroMovieFrames[i];
							}

							delete result->painting;
							return retValue;
						}					
					}
				}
			}
		}
	}
}