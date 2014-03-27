// FastRawImage.h
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

#include "PixelData.h"

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
						class FastRawImage	
						{
						public:
							int Width;
							int Height;
							int xOffset;
							int yOffset;
							int originalWidth;
							PixelData *singledata;
							bool memoryOwner;

							static void FastRawImage::Rotate(FastRawImage* input, double thetaInRadians, FastRawImage &output);
							static FastRawImage* Rotate(FastRawImage* input, double thetaInRadians);
							static FastRawImage* Scale(FastRawImage* input, int width, int height);
							static FastRawImage* ScaleHeight(FastRawImage* input, int oh);
							static FastRawImage* ScaleWidth(FastRawImage* input, int ow);
							static void Blend(FastRawImage* inp, FastRawImage* stroke, PixelData rgb, int xc, int yc);
							static void FastRawImage::Blend(FastRawImage* inp, FastRawImage &stroke, PixelData rgb, int xc, int yc);

							inline void WhiteOut()
							{
								static PixelData white(255, 255, 255);
								int total = this->Height * this->Width;
								for (int i=0; i < total; ++i) this->singledata[i] = white;
							}

							inline void BlackOut()
							{
								static PixelData white(0, 0, 0);
								int total = this->Height * this->Width;
								for (int i=0; i < total; ++i) this->singledata[i] = white;
							}


							inline PixelData& GetColor(int x, int y)
							{								
								return this->singledata[((yOffset + y) * this->originalWidth) + (x + xOffset)];
							}

							inline void SetColor(int x, int y, PixelData &data)
							{
								this->singledata[(yOffset + y) * this->Width + (x + xOffset)] = data;							
							}

							void Crop(int xc, int yc, int w, int h, FastRawImage &result);

							FastRawImage() : singledata(0), memoryOwner(true) {}
							FastRawImage(int width, int height, bool allocateMemory);
							FastRawImage(int width, int height);
							//FastRawImage(int width, int height, PixelData *source);
							~FastRawImage();
						};
					}
				}
			}
		}
	}
}