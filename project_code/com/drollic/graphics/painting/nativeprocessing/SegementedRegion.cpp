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
#include "SegmentedRegion.h"
#include "FastRawImage.h"

using namespace com::drollic::graphics::painting::native::processing;

SegmentedRegion::SegmentedRegion()
: Width(0), Height(0), segWidth(0), segHeight(0), MaxXOffset(0), MaxYOffset(0)
{
}

Segment SegmentedRegion::SegmentAt(const SegmentOffset& offset)
{
	return this->segments[offset];   // This will create an empty segment if it doesn't exist.
}

void SegmentedRegion::SetDimensions(int segwidth, int segheight, int width, int height)
{
	this->segWidth = segwidth;
	this->segHeight = segheight;
	this->Width = width;
	this->Height = height;

	int xCount = 0, yCount = 0;
	for (int y=0; y < (Height - segHeight) ; y+= segHeight)
	{
		for (int x=0; x < (Width - segWidth); x+= segWidth)
		{
			AddSegment(Segment(x, y, segWidth, segHeight), SegmentOffset(xCount++, yCount));
		}

		xCount = 0;
		yCount++;
	}

}

StrokeContainer* SegmentedRegion::UnifyStrokes()
{
	StrokeContainer* container = new StrokeContainer();	

	for (OffsetToRegionsHashType::iterator i = this->incorporatedRegions.begin(); i != this->incorporatedRegions.end(); i++)
	{
		SegmentOffset absoluteOffset(i->first);
		SegmentedRegion &region = this->incorporatedRegions[absoluteOffset];

		for (OffsetToSegmentsHashType::iterator i = region.segments.begin(); i != region.segments.end(); i++)
		{
			SegmentOffset segmentOffset(i->first);
			Segment &segment = region.segments[segmentOffset];

			for (std::list<NativeStroke>::iterator strokeIter = segment.strokes.begin(); strokeIter != segment.strokes.end(); strokeIter++)
			{
				(*strokeIter).xc += absoluteOffset.first * region.segWidth;
				(*strokeIter).yc += absoluteOffset.second * region.segHeight;

				container->AddStroke((*strokeIter));
			}
		}
	}

	return container;
}


StrokeContainer* SegmentedRegion::CollectAllStrokes()
{
	StrokeContainer* container = new StrokeContainer();	

	for (OffsetToSegmentsHashType::iterator i = this->segments.begin(); i != this->segments.end(); i++)
	{
		SegmentOffset segmentOffset(i->first);
		Segment &segment = this->segments[segmentOffset];		
				
		for (std::list<NativeStroke>::iterator strokeIter = segment.strokes.begin(); strokeIter != segment.strokes.end(); strokeIter++)
		{
			container->AddStroke(*strokeIter);
		}
	}

	return container;
}

void SegmentedRegion::AddStroke(const NativeStroke &stroke)
{
	for (OffsetToSegmentsHashType::iterator i = this->segments.begin(); i != this->segments.end(); i++)
	{
		SegmentOffset segmentOffset(i->first);
		Segment &segment = this->segments[segmentOffset];

		int xSegBoundary = segment.x + segment.Width - 1;
		int ySegBoundary = segment.y + segment.Height - 1;

		if (((stroke.xc >= segment.x) && (stroke.xc <= xSegBoundary)) &&
			((stroke.yc >= segment.y) && (stroke.yc <= ySegBoundary)))
		{
			//std::cout << "AddStroke: Stroke added!" << std::endl;

			segment.strokes.push_back(stroke);

			break;
		}
	}
}

void SegmentedRegion::AddSegment(Segment &segment, SegmentOffset &offset)
{
	//std::cout << "SegmentedRegion::AddSegment" << std::endl;

	this->segments[offset] = segment;
	this->MaxXOffset = MAX(this->MaxXOffset, offset.first);
	this->MaxYOffset = MAX(this->MaxYOffset, offset.second);
}

void SegmentedRegion::Incorporate(SegmentedRegion& toBeIncorporated, SegmentOffset& absoluteOffset)
{
	//std::cout << "SegmentedRegion::Incorporate!" << std::endl;

	for (OffsetToSegmentsHashType::iterator i = toBeIncorporated.segments.begin(); i != toBeIncorporated.segments.end(); i++)
	{
		SegmentOffset relativeOffset(i->first);
		SegmentOffset windowOffset(absoluteOffset.first + relativeOffset.first, absoluteOffset.second + relativeOffset.second);
		Segment &segment = toBeIncorporated.segments[relativeOffset];
		segment.filled = true;

		//std::cout << "Incorporating segment at " << windowOffset.first << ", " << windowOffset.second << std::endl;

		AddSegment(segment, windowOffset);
	}

	this->incorporatedRegions[absoluteOffset] = toBeIncorporated;
}

bool SegmentedRegion::AttemptIncorporationAtOffset(SegmentedRegion& toBeIncorporated, SegmentOffset &offset, int acceptableCollisions)
{
	if (toBeIncorporated.FitsInto(*this, offset, acceptableCollisions))
	{				
		this->Incorporate(toBeIncorporated, offset);

		//std::cout << "Region fits at location: " << offset.first << ", " << offset.second << std::endl;

		return true;
	}

	return false;
} 

bool SegmentedRegion::AttemptIncorporation(SegmentedRegion& toBeIncorporated)
{
	double percentageCollisionsToAllow = 0.20 + (static_cast<double>((rand() % 10)) / 100.0);
	int acceptableCollisions = static_cast<int>(toBeIncorporated.segments.size() * percentageCollisionsToAllow); // random segment segment collision is OK

	if (this->incorporatedRegions.size() == 0)
		if (AttemptIncorporationAroundCenter(toBeIncorporated, acceptableCollisions))
			return true;

	if ((rand() % 1) == 0)
		return AttemptIncorporationFromTopLeft(toBeIncorporated, acceptableCollisions);

	return AttemptIncorporationFromBottomRight(toBeIncorporated, acceptableCollisions);
}

bool SegmentedRegion::AttemptIncorporationFromBottomRight(SegmentedRegion& toBeIncorporated, int acceptableCollisions)
{
	//std::cout << "SegmentedRegion::AttemptIncorporationFromBottomRight: Acceptable collisions: " << acceptableCollisions << ", Total segements: " << this->segments.size() << std::endl;

	int randomStartingX = static_cast<int>(this->MaxXOffset - (this->MaxXOffset * (static_cast<float>((rand() % 20)) / 100.0)));
	int randomStartingY = static_cast<int>(this->MaxYOffset - (this->MaxYOffset * (static_cast<float>((rand() % 20)) / 100.0)));

	for (int y = randomStartingY; y >= 0; y--)
	{
		for (int x = randomStartingX; x >= 0; x--)
		{
			if (AttemptIncorporationAtOffset(toBeIncorporated, SegmentOffset(x, y), acceptableCollisions))
				return true;
		}
	}	

	return false;
}

bool SegmentedRegion::AttemptIncorporationFromTopLeft(SegmentedRegion& toBeIncorporated, int acceptableCollisions)
{
	//std::cout << "SegmentedRegion::AttemptIncorporationFromTopLeft: Acceptable collisions: " << acceptableCollisions << ", Total segements: " << this->segments.size() << std::endl;

	int randomStartingX = this->MaxXOffset * static_cast<int>((static_cast<float>((rand() % 20)) / 100.0));
	int randomStartingY = this->MaxYOffset * static_cast<int>((static_cast<float>((rand() % 20)) / 100.0));

	for (int y = randomStartingY; y < this->MaxYOffset; y++)
	{
		for (int x = randomStartingX; x < this->MaxXOffset; x++)
		{
			if (AttemptIncorporationAtOffset(toBeIncorporated, SegmentOffset(x, y), acceptableCollisions))
				return true;
		}
	}	

	return false;
}

bool SegmentedRegion::AttemptIncorporationAroundCenter(SegmentedRegion& toBeIncorporated, int acceptableCollisions)
{
	//std::cout << "SegmentedRegion::AttemptIncorporationAroundCenter: Acceptable collisions: " << acceptableCollisions << ", Total segements: " << this->segments.size() << std::endl;

	float divisor = static_cast<float>(2.0 + (rand() % 1) + (rand() % 1) + (static_cast<float>((rand() % 11)) / 100.0));
	int startingY = static_cast<int>(this->MaxYOffset / divisor);
	int startingX = static_cast<int>(this->MaxXOffset / divisor);

	//std::cout << "AttemptIncorporationAroundCenter: maxX, maxY, divisor: " << this->MaxXOffset << ", " << this->MaxYOffset << ", " << divisor << std::endl;
	//std::cout << "AttemptIncorporationAroundCenter: Trying fit at X/Y: " << startingX << "/" << startingY << std::endl;

	for (int y = startingY; y < this->MaxYOffset; y++)
	{
		for (int x = startingX; x < this->MaxXOffset; x++)
		{
			if (AttemptIncorporationAtOffset(toBeIncorporated, SegmentOffset(x, y), acceptableCollisions))
				return true;
		}
	}	

	return false;
}

/*
bool SegmentedRegion::AttemptIncorporation(SegmentedRegion& toBeIncorporated)
{
	std::cout << "SegmentedRegion::AttemptIncorporation" << std::endl;

	double percentageCollisionsToAllow = 0.03 + (static_cast<double>((rand() % 10)) / 100.0);
	int acceptableCollisions = this->segments.size() * percentageCollisionsToAllow; // random segment segment collision is OK

	std::cout << "FitsInto: Acceptable collisions: " << acceptableCollisions << ", Total segements: " << this->segments.size() << std::endl;

	for (int y=0; y < this->MaxYOffset; y++)
	{
		for (int x=0; x < this->MaxXOffset; x++)
		{
			SegmentOffset offset(x, y);

			if (toBeIncorporated.FitsInto(*this, offset, acceptableCollisions))
			{				
				this->Incorporate(toBeIncorporated, offset);

				std::cout << "Region fits at location: " << offset.first << ", " << offset.second << std::endl;

				return true;
			}
		}
	}	

	return false;
}
*/

bool SegmentedRegion::FitsInto(SegmentedRegion& destRegion, SegmentOffset& destOffset, int acceptableCollisions)
{
	int collisions = 0;	

	// For every segment within this "source" region, check to see if 
	// each can be placed in its relative location in the destRegion.
	for (OffsetToSegmentsHashType::iterator i = this->segments.begin(); i != this->segments.end(); i++)
	{
		//std::cout << "FitsInto: Testing segment..." << std::endl;

		SegmentOffset offset(i->first);
		SegmentOffset adjusted(destOffset.first + offset.first, destOffset.second + offset.second);

		if (destRegion.segments.find(adjusted) != destRegion.segments.end())
		{
			//std::cout << "FitsInto: Found segment of adjusted size." << std::endl;

			Segment& segment = destRegion.segments[adjusted];
			if (segment.filled) {				
				if (++collisions >= acceptableCollisions)
				{
					return false;
				}
			}
		}
		else
		{
			return false;
		}
	}

	return true;
}


SegmentedRegion SegmentedRegion::AboveThresholdSegments(double bottomPercentileToDrop)
{	
	std::map<double, std::list<OffsetSegmentPair> * > scores;
	double totalStrokes = 0.0;
	int totalSegments = 0;

	for (OffsetToSegmentsHashType::iterator i = this->segments.begin(); i != this->segments.end(); i++)
	{
		SegmentOffset offset(i->first);
		Segment& segment = this->segments[offset];

		int maxY = segment.y + segment.Height;
		int maxX = segment.x + segment.Width;
		int segmentScore = segment.strokes.size();
		totalStrokes += segmentScore;

		//std::cout << "AboveThresholdSegments: segment score: " << segmentScore << std::endl;

		std::list<OffsetSegmentPair> *list = NULL;
		if (scores.find(segmentScore) != scores.end())
			list = scores[segmentScore];
		else
		{
			list = new std::list<OffsetSegmentPair>();
			scores[segmentScore] = list;
		}
		
		OffsetSegmentPair pair(offset, segment);
		list->push_back(pair);					
		totalSegments++;
	}

	//std::cout << "AboveThresholdSegments: total strokes: " << totalStrokes << std::endl;

	int totalSegmentsAdded = 0;
	int totalSegmentsDropped = 0;

	double percentile = bottomPercentileToDrop / 100.0;
	int totalSegmentsToSkip = static_cast<int>(percentile * static_cast<double>(totalSegments));

	SegmentedRegion result;
	result.SetDimensions(this->segWidth, this->segHeight, this->Width, this->Height);
	for (std::map<double, std::list<OffsetSegmentPair> *>::iterator i = scores.begin(); i != scores.end(); i++)
	{
		double score = i->first;
		std::list<OffsetSegmentPair> *list = scores[score];

		while ((totalSegmentsToSkip > 0) && (list->size() > 0))
		{
			list->pop_front();
				
			totalSegmentsToSkip--;
			totalSegmentsDropped++;
		}

		if (list->size() == 0)
			continue;

		//std::cout << "Threshold: Score: " << score << ", Total: " << totalStrokes << ", %: " << percentile << std::endl;
				
		//std::cout << "Threshold: Score: " << score << ", Total: " << totalStrokes << ", %: " << percentile << std::endl;
		
		for (std::list<OffsetSegmentPair>::iterator i = list->begin(); i != list->end(); i++)
		{
			OffsetSegmentPair pair = *i;

			//std::cout << "AboveThresholdSegments: Adding interesting segment." << std::endl;

			result.AddSegment(pair.second, pair.first);	

			totalSegmentsAdded++;
		}
	}

	//std::cout << "AboveThresholdSegments: Added/Dropped: " << totalSegmentsAdded << "/" << totalSegmentsDropped << std::endl;

	return result;
}
