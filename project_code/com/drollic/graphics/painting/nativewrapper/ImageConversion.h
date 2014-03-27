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


#include "FastRawImage.h"


using namespace com::drollic::graphics::painting::native::processing;


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
						public ref class ImageConversion sealed
						{
						public:	
							static UnsafeBitmap^ FastRaw2Unsafe(FastRawImage* input)
							{
								Bitmap^ bmp = gcnew Bitmap(input->Width, input->Height);
								UnsafeBitmap^ result = gcnew UnsafeBitmap(bmp);

								com::drollic::graphics::painting::native::processing::PixelData pixel;
								com::drollic::graphics::PixelData pd;
								for (int y=0; y < input->Height; y++)
								{
									for (int x=0; x < input->Width; x++)
									{
										pixel = input->GetColor(x, y);
										pd.red = pixel.R;
										pd.green = pixel.G;
										pd.blue = pixel.B;
										result->SetPixel(x, y, pd);
									}
								}

								return result;
							}

							static FastRawImage* Unsafe2FastRaw(UnsafeBitmap ^input)
							{
								FastRawImage *inMem = new FastRawImage(input->Width, input->Height);

								int skip = 0;
								com::drollic::graphics::PixelData data;

								for (int j=0; j < input->Height; j++)
								{		
									skip = j * input->Width;
									for (int i=0; i < input->Width; i++)
									{
										data = input->GetPixelData(i, j);
										inMem->singledata[skip + i] = com::drollic::graphics::painting::native::processing::PixelData(data.red, data.green, data.blue);

										// Older, slower version
										//c = input->GetPixel(i, j);
										//inMem->singledata[skip + i] = com::drollic::graphics::painting::native::processing::PixelData(c.R, c.G, c.B);
									}
								}

								return inMem;
							}					

						};
					}
				}
			}
		}
	}
}