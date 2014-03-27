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

using namespace System;
using namespace System::Drawing;
using namespace System::Collections;
using namespace System::Security::Permissions;


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
						public ref class WaveletNativeMomentPainterWrapperResult sealed
						{
						public: 
							UnsafeBitmap^ image;
							array< array<System::Drawing::Rectangle^>^ >^ interestingSegments;
						};

						public ref class WaveletNativeMomentPainterWrapper sealed
						{
						public:		
							static UnsafeBitmap^ CreatePaintingV2(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S);
							static UnsafeBitmap^ CreatePaintingV2(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S, bool generateMovie, array<UnsafeBitmap^>^ introFrames);

							static UnsafeBitmap^ CreatePainting(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S);
							static UnsafeBitmap^ CreatePainting(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S, bool generateMovie, array<UnsafeBitmap^>^ introFrames);
							static WaveletNativeMomentPainterWrapperResult^ CreatePainting2(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S);
							static WaveletNativeMomentPainterWrapperResult^ CreatePainting2(int width, int height, array<UnsafeBitmap^>^ originals, array<UnsafeBitmap^>^ transformed, array<UnsafeBitmap^>^ strokes, int S, bool generateMovie, array<UnsafeBitmap^>^ introFrames);
							static int GetPercentComplete();
						};
					}
				}
			}
		}
	}
}