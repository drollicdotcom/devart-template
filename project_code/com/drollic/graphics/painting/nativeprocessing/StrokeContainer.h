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

#include "FastRawImage.h"
#include "NativeStroke.h"

#include <map>
#include <set>
#include <functional>

#include "NativeStroke.h"
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
						class StrokeContainer : public IStrokeContainer
						{
						public:
							typedef std::map<int, std::set< std::pair<int, int> >, std::greater<int> > AreaToDimensionsMapType;
							typedef std::multimap< std::pair<int, int>, NativeStroke> DimensionToStrokesMapType;

							AreaToDimensionsMapType areaToStrokeDimensionsMap;
							DimensionToStrokesMapType strokeDimensionsToStrokesMap;

							void AddStroke(const NativeStroke &s)
							{
								this->areaToStrokeDimensionsMap[s.area].insert( std::pair<int, int>(s.l, s.w) );
								this->strokeDimensionsToStrokesMap.insert( std::pair< std::pair<int, int>, NativeStroke>(std::pair<int, int>(s.l, s.w), s));		
							}
						};
					}
				}
			}
		}
	}
}