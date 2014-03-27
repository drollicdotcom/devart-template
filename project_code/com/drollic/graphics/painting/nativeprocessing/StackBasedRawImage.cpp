#include "stdafx.h"
#include "StackBasedRawImage.h"
#include "FastRawImage.h"

using namespace com::raymatthieu::graphics::painting::native::processing;

#include <math.h>

StackBasedRawImage::StackBasedRawImage(int width, int height)
: Width(width), Height(height), xOffset(0), yOffset(0), originalWidth(width), singledata(height * width)
{	
}


StackBasedRawImage::~StackBasedRawImage()
{
}

StackBasedRawImage* StackBasedRawImage::Scale(StackBasedRawImage* input, int width, int height)
{
	StackBasedRawImage* scaledWidth = ScaleWidth(input, width);
	StackBasedRawImage* scaled = ScaleHeight(scaledWidth, height);
	delete scaledWidth;
	return scaled;
}

StackBasedRawImage* StackBasedRawImage::ScaleWidth(StackBasedRawImage* input, int ow)
{
	StackBasedRawImage* output = new StackBasedRawImage(ow, input->Height);

	int u, x, y;
	int res_r, res_g, res_b, acc_r, acc_g, acc_b;
	int p, q;
	int iw = input->Width;
	int area = (ow * iw);

	PixelData cuy;
	PixelData cuPlusOney;

	for (y = 0; y < input->Height; y++) 
	{
		q = iw;
		p = ow;
		acc_r = 0;
		acc_g = 0;
		acc_b = 0;
		x = u = 0;

		while (x < ow) 
		{
			if (u+1 < iw) 
			{
				cuy = input->GetColor(u, y);
				cuPlusOney = input->GetColor(u+1, y);
				res_r = p * cuy.R + (ow - p) * cuPlusOney.R;
				res_g = p * cuy.G + (ow - p) * cuPlusOney.G;
				res_b = p * cuy.B + (ow - p) * cuPlusOney.B;
			}
			else
			{
				cuy = input->GetColor(u, y);
				res_r = ow * cuy.R;
				res_g = ow * cuy.G;
				res_b = ow * cuy.B;						
			}

			if (p < q) 
			{
				acc_r += res_r * p;
				acc_g += res_g * p;
				acc_b += res_b * p;
				q -= p;
				p = ow;
				u++;
			} 
			else 
			{
				acc_r += res_r * q;
				acc_g += res_g * q;
				acc_b += res_b * q;

				output->SetColor(x, y, PixelData((acc_r / area), (acc_g / area), (acc_b / area)));
				acc_r = 0;
				acc_g = 0;
				acc_b = 0;
				p -= q;
				q = iw;
				x++;
			}
		}
	}

	output->Width = ow;
	output->Height = input->Height;

	return output;
}

StackBasedRawImage* StackBasedRawImage::ScaleHeight(StackBasedRawImage* input, int oh)
{
	StackBasedRawImage* output = new StackBasedRawImage(input->Width, oh);

	int x, y, v;
	int res_r, acc_r;
	int res_g, acc_g;
	int res_b, acc_b;
	int p, q;
	int ih = input->Height;
	int area = (ih * oh);

	PixelData cxv;

	for (x = 0; x < input->Width; x++) 
	{
		q = ih;
		p = oh;
		acc_r = 0;
		acc_g = 0;
		acc_b = 0;
		y = v = 0;

		while (y < oh) 
		{
			if (v+1 < ih) 
			{
				cxv = input->GetColor(x, v);
				res_r = p * cxv.R + (oh - p) * cxv.R;
				res_g = p * cxv.G + (oh - p) * cxv.G;
				res_b = p * cxv.B + (oh - p) * cxv.B;

			} 
			else 
			{
				cxv = input->GetColor(x, v);
				res_r = oh * cxv.R;
				res_g = oh * cxv.G;
				res_b = oh * cxv.B;
			}
			if (p < q) 
			{
				acc_r += res_r * p;
				acc_g += res_g * p;
				acc_b += res_b * p;
				q -= p;
				p = oh;
				v++;
			} 
			else 
			{
				acc_r += res_r * q;
				acc_g += res_g * q;
				acc_b += res_b * q;

				output->SetColor(x, y, PixelData((acc_r / area), (acc_g / area), (acc_b / area)));
				acc_r = 0;
				acc_g = 0;
				acc_b = 0;
				p -= q;
				q = ih;
				y++;
			}
		}
	}

	output->Width = input->Width;
	output->Height = oh;

	return output;
}

void StackBasedRawImage::Rotate(FastRawImage* input, double thetaInRadians, StackBasedRawImage &output)
{
	double cosX = cos(thetaInRadians);
	double sinX = sin(thetaInRadians);

	int origXCenter = input->Width / 2;
	int origYCenter = input->Height / 2;
	int xOffset = (int)(output.Width / 2);
	int yOffset = (int)(output.Height / 2);

	int xCenter = (int)((origXCenter * cosX) - (origYCenter * sinX));
	int yCenter = (int)((origXCenter * sinX) + (origYCenter * cosX));
	int xc = 0, yc = 0;

	for (int y=0; y < input->Height; y++)
	{
		for (int x=0; x < input->Width; x++)
		{
			xc = x - origXCenter;
			yc = y - origYCenter;

			int xPrime = (int)((xc * cosX) - (yc * sinX)) + xOffset;
			int yPrime = (int)((xc * sinX) + (yc * cosX)) + yOffset;

			// If we crash here... it's because I changed the length check
			if ((xPrime > 0) && (xPrime < output.Width) && (yPrime > 0) && (yPrime < output.Height))
			{
				output.SetColor(xPrime, yPrime, input->GetColor(x, y));
			}
		}
	}
}


void StackBasedRawImage::Blend(FastRawImage* inp, StackBasedRawImage &stroke, PixelData rgb, int xc, int yc)
{
	int xi, yi; 			/* lower left corner in input image */
	int xa, ya;				/* corresponding corner in alpha image */
	int wa, ha;				/* area in alpha image to be blended */

	wa = stroke.Width;
	xa = 0;
	xi = xc - wa/2;
	if (xi < 0) 
	{
		wa += xi;
		xa -= xi;
		xi = 0;
	}

	// TODO: Probably want to error out here
	if (xi > inp->Width) 
		return;

	// TODO: Probably want to error out here
	if (wa <= 0)
		return;

	if (xi + wa >= inp->Width)
		wa = inp->Width - xi;

	ha = stroke.Height;
	ya = 0;
	yi = yc - ha/2;
	if (yi < 0) 
	{
		ha += yi;
		ya -= yi;
		yi = 0;
	}

	// TODO: Probably want to error out here
	if (yi > inp->Height) 
		return;

	// TODO: Probably want to error out here
	if (ha <= 0)
		return;

	if (yi + ha >= inp->Height)
		ha = inp->Height - yi;

	int upperR;
	double alpha, oneMinusAlpha;
	PixelData lowerC;

	for (int iny = yi, strokey = ya; strokey < ha; iny++, strokey++) 
	{
		for (int inx = xi, strokex = xa; strokex < wa; inx++, strokex++) 
		{
			upperR = stroke.GetColor(strokex, strokey).R;

			alpha = upperR / 255.0;
			oneMinusAlpha = (1.0 - alpha);

			lowerC = inp->GetColor(inx, iny);

			int r = (int)((alpha * rgb.R) + (lowerC.R * oneMinusAlpha));
			int g = (int)((alpha * rgb.G) + (lowerC.G * oneMinusAlpha));
			int b = (int)((alpha * rgb.B) + (lowerC.B * oneMinusAlpha));

			inp->SetColor(inx, iny, PixelData(r, g, b));
		}
	}
}
