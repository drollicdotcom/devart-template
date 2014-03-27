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
#include "CompositeNativeMomentPainterWrapper.h"
#include "ImageConversion.h"
#include "NativeStroke.h"
#include "ComputationProgress.h"
#include "CompositeNativeMomentsPainter.h"

#include <list>
#include <vector>
#include <queue>

#pragma managed

using namespace System;
using namespace System::Collections;
using namespace System::Runtime::InteropServices;

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
						int CompositeNativeMomentPainterWrapper::GetPercentComplete()
						{
							return ComputationProgress::Instance()->percentComplete;
						}

						UnsafeBitmap^ CompositeNativeMomentPainterWrapper::CreatePainting(int width, int height, array<UnsafeBitmap^>^ input, array<Point>^ locations, array<UnsafeBitmap^>^ strokes, int S)
						{
							array<UnsafeBitmap^>^ garbage;
							System::String^ filename;
							return CreatePainting(width, height, input, locations, strokes, S, false, garbage, filename);
						}

						UnsafeBitmap^ CompositeNativeMomentPainterWrapper::CreatePainting(int width, int height, array<UnsafeBitmap^>^ input, array<Point>^ locations, array<UnsafeBitmap^>^ strokes, int S, bool generateMovie, array<UnsafeBitmap^>^ introFrames, System::String^ filename)
						{
							ComputationProgress::Instance()->percentComplete = 0;

							std::vector<FastRawImage*> inputOriginals;							
							std::vector<std::pair<int, int> > originalLocations;							
							static std::vector<FastRawImage*> faststrokes;

							float percentWorkForAllConversion = 5.0;  // Five percent
							float percentWorkDonePerConversion = percentWorkForAllConversion / input->Length;

							for (int i=0; i < input->Length; i++)
							{
								ComputationProgress::Instance()->percentComplete += 1;

								inputOriginals.push_back(ImageConversion::Unsafe2FastRaw(input[i]));

								ComputationProgress::Instance()->percentComplete += 1;

								originalLocations.push_back(std::pair<int, int>(locations[i].X, locations[i].Y));

								// Update percentage work done
								ComputationProgress::Instance()->percentComplete += static_cast<int>(percentWorkDonePerConversion);
							}

							if (faststrokes.size() == 0)
							{
								for (int i=0; i < strokes->Length; i++)
								{
									faststrokes.push_back(ImageConversion::Unsafe2FastRaw(strokes[i]));									
								}
							}							

							char *unmanagedFilename = NULL;

							CompositeNativeMomentsPainter painter(width, height, inputOriginals, originalLocations, faststrokes, S);

							// Convert intro movie frames, if movie generation has been enabled
							std::vector<FastRawImage*> inputIntroMovieFrames;
							if (generateMovie)
							{
								// Convert intro movie frames
								for (int i=0; i < introFrames->Length; i++)
								{
									inputIntroMovieFrames.push_back(ImageConversion::Unsafe2FastRaw(introFrames[i]));
								}

								unmanagedFilename = (char *)Marshal::StringToHGlobalAnsi(filename).ToPointer();							

								// Pass movie frames, and enable movie generation
								painter.EnableMovieGeneration(inputIntroMovieFrames, unmanagedFilename);
							}

							//FastRawImage* painting = NativeMomentPainter::CreateCompositePainting(width, height, inputOriginals, originalLocations, faststrokes, S);
							FastRawImage* painting = painter.CreatePainting();
							UnsafeBitmap^ result = ImageConversion::FastRaw2Unsafe(painting);

							for (unsigned int i=0; i < inputOriginals.size(); i++)
							{
								delete inputOriginals[i];
								inputOriginals[i] = NULL;
							}
							

							// Delete title movie frame, if it was created
							for (unsigned int i=0; i < inputIntroMovieFrames.size(); i++)
							{
								delete inputIntroMovieFrames[i];
							}

							// Clean up
							if (unmanagedFilename != NULL)
							{
								Marshal::FreeHGlobal(IntPtr(unmanagedFilename));
							}

							delete painting;
							return result;
						}
					
					}
				}
			}
		}
	}
}