// StackBasedRawImage.h
#pragma once

#include "RawImage.h"

#include <iostream>
#include <vector>

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
					namespace processing
					{
						class FastRawImage;

						class StackBasedRawImage	
						{
						public:
							int Width;
							int Height;
							int xOffset;
							int yOffset;
							int originalWidth;
							std::vector<PixelData> singledata;

							static void Rotate(FastRawImage* input, double thetaInRadians, StackBasedRawImage &output);
							static StackBasedRawImage* Scale(StackBasedRawImage* input, int width, int height);
							static StackBasedRawImage* ScaleHeight(StackBasedRawImage* input, int oh);
							static StackBasedRawImage* ScaleWidth(StackBasedRawImage* input, int ow);							
							static void Blend(FastRawImage* inp, StackBasedRawImage &stroke, PixelData rgb, int xc, int yc);

							inline void WhiteOut()
							{
								static PixelData white(255, 255, 255);
								int total = this->Height * this->Width;
								for (int i=0; i < total; ++i) this->singledata[i] = white;
							}

							inline void BlackOut()
							{
								static PixelData black(0, 0, 0);
								int total = this->Height * this->Width;
								for (int i=0; i < total; ++i) this->singledata[i] = black;
							}


							inline PixelData GetColor(int x, int y)
							{
								//std::cout << "piece (" << x << "," << y << ") = source [" << ((yOffset + y) * this->originalWidth) + (x + xOffset) << "]" << std::endl;
								return this->singledata[((yOffset + y) * this->originalWidth) + (x + xOffset)];
							}

							inline void SetColor(int x, int y, PixelData &data)
							{
								this->singledata[(yOffset + y) * this->Width + (x + xOffset)] = data;							
							}

							StackBasedRawImage(int width, int height);							
							~StackBasedRawImage();
						};
					}
				}
			}
		}
	}
}