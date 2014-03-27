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

#include "INativeMomentsPainter.h"
#include "FastRawImage.h"
#include "NativeStroke.h"
#include "StrokeContainer.h"

#include <vector>

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
						class SegmentedNativeMomentsPainter : public INativeMomentsPainter
						{
						public:
							
							SegmentedNativeMomentsPainter(int width, int height, std::vector<FastRawImage*> &originals, std::vector<FastRawImage*> &strokes, int S);
							~SegmentedNativeMomentsPainter();

							virtual FastRawImage* CreatePainting();

							//static FastRawImage* CreatePainting(FastRawImage* input, FastRawImage* stroke, int S);							
							//static FastRawImage* CreateSegmentedCompositePainting(int width, int height, std::vector<FastRawImage*> &originals, std::vector<FastRawImage*> &strokes, int S);
							//static FastRawImage* CreateCompositePainting(int width, int height, std::vector<FastRawImage*> &originals, std::vector<std::pair<int, int> > &locations, std::vector<FastRawImage*> &strokes, int S);							

						protected:
							std::vector<FastRawImage*> &originals;						

							FastRawImage* CreateSegmentedPainting();
						};
					}
				}
			}
		}
	}
}
