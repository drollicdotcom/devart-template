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
#include "NativeMomentPainterWrapper.h"
#include "ImageConversion.h"
#include "NativeStroke.h"
#include "ComputationProgress.h"
#include "NativeMomentPainter.h"

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
						int NativeMomentPainterWrapper::GetPaitingPercentComplete()
						{
							return ComputationProgress::Instance()->percentComplete;
						}

						UnsafeBitmap^ NativeMomentPainterWrapper::GenerateSegmentedCompositePainting(int width, int height, array<UnsafeBitmap^>^ input, array<UnsafeBitmap^>^ strokes, int S)
						{
							ComputationProgress::Instance()->percentComplete = 0;

							std::vector<FastRawImage*> inputOriginals;							
							static std::vector<FastRawImage*> faststrokes;

							float percentWorkForAllConversion = 5.0;
							float percentWorkDonePerConversion = percentWorkForAllConversion / input->Length;

							for (int i=0; i < input->Length; i++)
							{
								inputOriginals.push_back(ImageConversion::Unsafe2FastRaw(input[i]));

								// Update percentage work done
								ComputationProgress::Instance()->percentComplete += percentWorkDonePerConversion;
							}

							if (faststrokes.size() == 0)
							{
								for (int i=0; i < strokes->Length; i++)
								{
									faststrokes.push_back(ImageConversion::Unsafe2FastRaw(strokes[i]));									
								}
							}

							FastRawImage* painting = NativeMomentPainter::CreateSegmentedCompositePainting(width, height, inputOriginals, faststrokes, S);
							UnsafeBitmap^ result = ImageConversion::FastRaw2Unsafe(painting);

							for (unsigned int i=0; i < inputOriginals.size(); i++)
							{
								delete inputOriginals[i];
								inputOriginals[i] = NULL;
							}
							
							delete painting;
							return result;
						}

						UnsafeBitmap^ NativeMomentPainterWrapper::GenerateCompositePainting(int width, int height, array<UnsafeBitmap^>^ input, array<Point>^ locations, array<UnsafeBitmap^>^ strokes, int S)
						{
							ComputationProgress::Instance()->percentComplete = 0;

							std::vector<FastRawImage*> inputOriginals;							
							std::vector<std::pair<int, int> > originalLocations;							
							static std::vector<FastRawImage*> faststrokes;

							float percentWorkForAllConversion = 5.0;  // Five percent
							float percentWorkDonePerConversion = percentWorkForAllConversion / input->Length;

							for (int i=0; i < input->Length; i++)
							{
								inputOriginals.push_back(ImageConversion::Unsafe2FastRaw(input[i]));
								originalLocations.push_back(std::pair<int, int>(locations[i].X, locations[i].Y));

								// Update percentage work done
								ComputationProgress::Instance()->percentComplete += percentWorkDonePerConversion;
							}

							if (faststrokes.size() == 0)
							{
								for (int i=0; i < strokes->Length; i++)
								{
									faststrokes.push_back(ImageConversion::Unsafe2FastRaw(strokes[i]));									
								}
							}

							FastRawImage* painting = NativeMomentPainter::CreateCompositePainting(width, height, inputOriginals, originalLocations, faststrokes, S);
							UnsafeBitmap^ result = ImageConversion::FastRaw2Unsafe(painting);

							for (unsigned int i=0; i < inputOriginals.size(); i++)
							{
								delete inputOriginals[i];
								inputOriginals[i] = NULL;
							}
							
							delete painting;
							return result;
						}


						UnsafeBitmap^ NativeMomentPainterWrapper::GeneratePainting(UnsafeBitmap^ input, UnsafeBitmap^ stroke, int S)
						{
							FastRawImage* fastInput = ImageConversion::Unsafe2FastRaw(input);
							FastRawImage* fastBrush = ImageConversion::Unsafe2FastRaw(stroke);
							FastRawImage* painting = NativeMomentPainter::CreatePainting(fastInput, fastBrush, S);
							UnsafeBitmap^ result = ImageConversion::FastRaw2Unsafe(painting);
							delete painting;
							delete fastInput;
							delete fastBrush;
							return result;
						}						
					}
				}
			}
		}
	}
}