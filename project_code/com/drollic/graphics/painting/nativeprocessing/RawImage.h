// RawImage.h
#pragma once

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
						/*
						struct PixelData
						{
						public:
							PixelData(int r=0, int g=0, int b=0) : R(r), G(g), B(b) {}
							int R;
							int G;
							int B;
						};

						class RawImage	
						{
						public:
							int Width;
							int Height;
							PixelData *singledata;
							//std::vector<PixelData> singledata;

							inline PixelData GetColor(int x, int y)
							{
								return this->singledata[y * this->Width + x];
							}

							inline void SetColor(int x, int y, PixelData data)
							{
								this->singledata[y * this->Width + x] = data;							
							}

							RawImage* Crop(int xc, int yc, int w, int h);												

							RawImage(int width, int height);
							//RawImage(const RawImage &raw);
							~RawImage();
						};
						*/
					}
				}
			}
		}
	}
}