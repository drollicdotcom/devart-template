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

#include <map>
#include <list>

#include "NativeStroke.h"
#include "StrokeContainer.h"
#include "IStrokeContainer.h"

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
						#define MAX(x,y) ((x)>(y)?(x):(y))

						class FastRawImage;

						class Segment
						{						
						public:
							typedef std::list<NativeStroke> StrokeCollectionType;

							bool filled;							
							int x;
							int y;
							int Width;
							int Height;
							StrokeCollectionType strokes;

							Segment()
								: filled(false), x(0), y(0), Width(0), Height(0)
							{
							}

							Segment(int x, int y, int width, int height)
								: filled(false), x(x), y(y), Width(width), Height(height)
							{
							}

							~Segment()
							{
							}
						};


						typedef std::pair<int, int> SegmentOffset;

						typedef std::pair<SegmentOffset, Segment> OffsetSegmentPair;


						/// <summary>
						/// Summary description for SegmentedRegion.
						/// </summary>
						class SegmentedRegion : public IStrokeContainer
						{
						public:
							typedef std::map<SegmentOffset, Segment> OffsetToSegmentsHashType;
							typedef std::map<SegmentOffset, SegmentedRegion> OffsetToRegionsHashType;
							int segWidth;
							int segHeight;
							int Width;
							int Height;

							int MaxXOffset;
							int MaxYOffset;

							SegmentedRegion();

							~SegmentedRegion()
							{
							}

							Segment SegmentAt(const SegmentOffset& offset);							

							void SetDimensions(int segwidth, int segheight, int width, int height);
							void AddStroke(const NativeStroke &stroke);
							StrokeContainer* CollectAllStrokes(); 
							StrokeContainer* UnifyStrokes(); 
							bool AttemptIncorporation(SegmentedRegion& toBeIncorporated);
							SegmentedRegion AboveThresholdSegments(double bottomPercentileToDrop);
							void AddSegment(Segment &segment, SegmentOffset &offset);
							void Incorporate(SegmentedRegion& toBeIncorporated, SegmentOffset& absoluteOffset);							

						private:
							OffsetToSegmentsHashType segments;							
							OffsetToRegionsHashType incorporatedRegions;														

							bool AttemptIncorporationAtOffset(SegmentedRegion& toBeIncorporated, SegmentOffset &offset, int acceptableCollisions);
							bool AttemptIncorporationAroundCenter(SegmentedRegion& toBeIncorporated, int acceptableCollisions);
							bool AttemptIncorporationFromBottomRight(SegmentedRegion& toBeIncorporated, int acceptableCollisions);
							bool AttemptIncorporationFromTopLeft(SegmentedRegion& toBeIncorporated, int acceptableCollisions);

							bool FitsInto(SegmentedRegion& destRegion, SegmentOffset& destOffset, int acceptableCollisions);		
						};
					}
				}
			}
		}
	}
}
