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

using System;
using System.Drawing;

namespace com.drollic.graphics.wavelets
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    public sealed class TChannel
    {
        public static readonly int LL = 0;
        public static readonly int LH = 1;
        public static readonly int HL = 2;
        public static readonly int HH = 2;

        public double[] data;
        //public int[] qdata;	// Integer based representation of data, quantized.
        public int width;
        public int height;

        public TChannel(double[] theData, int w, int h)
        {
            width = w;
            height = h;
            data = theData;

            // Perform some processing on the data
            int[] idata = DataUtils.ToIntegerValues(theData);
            //qdata = DataUtils.QuantizeModulo(idata, 10, 2);
        }


        public static TChannel[] GetTestObjects()
        {
            int size = 16;
            double[] data = new double[size];
            bool flip = false;
            int val;

            // Populate test object with very simple data set.
            for (int i = 0; i < size; i++)
            {
                val = flip ? 1 : 2;
                flip = !flip;
                data[i] = val;
            }

            TChannel[] objs = new TChannel[2];
            objs[0] = new TChannel(data, 4, 4);

            double[] data2 = new double[size];
            data2[0] = 2.0;
            data2[1] = 1.0;
            data2[2] = 2.0;
            data2[3] = 1.0;

            data2[4] = 1.0;
            data2[5] = 2.0;
            data2[6] = 1.0;
            data2[7] = 2.0;

            data2[8] = 2.0;
            data2[9] = 1.0;
            data2[10] = 2.0;
            data2[11] = 1.0;

            data2[12] = 1.0;
            data2[13] = 2.0;
            data2[14] = 1.0;
            data2[15] = 2.0;

            objs[1] = new TChannel(data2, 4, 4);

            // Return test objects
            return objs;
        }


        /*
        public int VirtualQuantizedPixel(int x, int y)
        {
            int val = qdata[(y * width) + x];
            //System.Console.WriteLine("Pixel (" + x + "," + y + ") value: " + val);
            return val;
        }
        */

        public double VirtualPixel(int x, int y)
        {
            double val = data[(y * width) + x];
            //System.Console.WriteLine("Pixel (" + x + "," + y + ") value: " + val);
            return val;
        }

        /*
        public Bitmap generateQuantizedImage()
        {
            int count = 0;
            int value = 0;
            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    value = qdata[count++] - 1;
                    value *= 70;

                    // Set the pixel from quantized data
                    bitmap.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }

            return bitmap;
        }
         */


        public Bitmap generateLimitedRangeImageAbs(int min, int max)
        {
            System.Diagnostics.Debug.Assert(data.Length == (width * height), "Data does not match image dimensions!");
            int count = 0;
            double tValue = 0;
            int value = 0;
            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Very simple modification to keep data within specified range.
                    tValue = data[count++];
                    if (tValue < min) tValue = Math.Abs(tValue);
                    if (tValue > max) value = max;
                    else value = (int)tValue;  // Throw away precision
                    bitmap.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }

            return bitmap;
        }


        public Bitmap generateLimitedRangeImage(int min, int max)
        {
            System.Diagnostics.Debug.Assert(data.Length == (width * height), "Data does not match image dimensions!");
            int count = 0;
            double tValue = 0;
            int value = 0;
            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Very simple modification to keep data within specified range.
                    tValue = data[count++];
                    if (tValue < min) value = min;
                    else if (tValue > max) value = max;
                    else
                    {
                        value = (int)(tValue);
                        if (tValue < 25) value = (int)(tValue * 10);  // Throw away precision
                    }
                    bitmap.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }

            return bitmap;
        }

        /*
        public Bitmap generateQuantizedImage(Quantizer q)
        {
            System.Diagnostics.Debug.Assert(data.Length == (width * height), "Data does not match image dimensions!");	
            int count = 0;
            double tValue = 0;
            int value = 0;
            Bitmap bitmap = new Bitmap(width, height);

            QuantizationBoundary [] bounds = q.generateBoundaries(data);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tValue = data[count++];
				
                    for (int i=0; i < bounds.Length; i++)
                    {
                        if (tValue < bounds[i].Boundary)
                        {
                            value = bounds[i].Value;
                            bitmap.SetPixel(x, y, Color.FromArgb(value, value, value));
                            break;
                        }
                        else if (i == (bounds.Length-1))
                        {
                            value = 255;
                            bitmap.SetPixel(x, y, Color.FromArgb(value, value, value));
                        }
                    }
                }
            }

            return bitmap;
        }
        */
    }
}