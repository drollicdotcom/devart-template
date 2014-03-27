#include "stdafx.h"
#include "RawImage.h"

using namespace com::raymatthieu::graphics::painting::native::processing;

#include <iostream>

RawImage::RawImage(int width, int height)
: Width(width), Height(height) //, singledata(width * height, PixelData(0,0,0))
{	
	singledata = new PixelData[this->Height * this->Width];
}

/*
RawImage::RawImage(const RawImage &raw)
: Width(raw.Width), Height(raw.Height), singledata(raw.singledata)
{
}
*/

RawImage::~RawImage()
{
	delete [] singledata;
}

RawImage* RawImage::Crop(int xc, int yc, int w, int h)
{
	int x0, y0;

	/* adjust region to fit in source image */
	x0 = xc - w/2;
	y0 = yc - h/2;
	if (x0 < 0) 
	{
		w += x0;
		x0 = 0;
	}

	if (x0 > this->Width) x0 = this->Width;
	if (y0 < 0) 
	{
		h += y0;
		y0 = 0;
	}
	if (y0 > this->Height) y0 = this->Height;
	if (x0 + w > this->Width) w = this->Width - x0; 
	if (y0 + h > this->Height) h = this->Height - y0; 

	// create cropped result
	RawImage *output = new RawImage(w, h);	
	
	int xCounter = 0, yCounter = 0;
	for (int outY=y0; outY < y0 + h; outY++)
	{
		xCounter = 0;
		for (int outX=x0; outX < x0 + w; outX++)
		{
			//std::cout << "cropped [" << yCounter * output->Width + xCounter << "] = source [" << outY * this->Width + outX << "]" << std::endl;
			output->singledata[yCounter * output->Width + xCounter++] = this->singledata[outY * this->Width + outX];
		}
		yCounter++;
	}

	return output;
}
