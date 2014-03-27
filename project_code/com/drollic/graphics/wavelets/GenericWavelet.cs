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

using com.drollic.graphics;

namespace com.drollic.graphics.wavelets
{
    public abstract class GenericWavelet
    {
        public static double M_SQRT2 = 1.41421356237309504880168872420969808;
        public static double SQRT15 = 3.87298334620741688517927;
        public static double SQRT3 = 1.73205080756887729352745;

        protected double[] cH = null;		// forward transform smoothing coeff's (h-values)
        protected int nH = 0;				// total number of smoothing coeff's
        protected double[] cHtilde = null;	// inverse transform smoothing coeff's (i-h-values)
        protected int nHtilde = 0;			// total number of inverse smoothing coeff's
        protected int offH = 0;				// ?
        protected int offG = 0;				// ?
        protected int offHtilde = 0;		// ?
        protected int offGtilde = 0;		// ?
        protected bool swapped;				// ?


        private int MOD(int i, int j)
        {
            return ((i) >= 0 ? (i) % (j) : ((j) - ((-(i)) % (j))) % (j));
        }


        public Bitmap ForwardTransformWorks(UnsafeBitmap img, double[] datax, double[] low, double[] high)
        {
            double[] data = ExtractGreyValues(img);
            double[] tdata = new double[data.Length];

            wfltr_convolve(true, data, 0, 1, data.Length, tdata, 0);

            int half = data.Length / 2;
            low = new double[half];
            high = new double[half];
            for (int i = 0; i < data.Length / 2; i++)
            {
                low[i] = tdata[i];
                high[i] = tdata[half + i];
            }

            Bitmap bitmap = null; //GenerateImage(low, img.Width / 2, img.Height / 2);

            /*
            Bitmap bitmap = new Bitmap(width, height);
            double tValue;
            int count = 0;
            int value;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Very simple modification to keep data within specified range.
                    tValue = low[count++];
                    //if (tValue < 0) tValue = Math.Abs(tValue);
                    if (tValue > 255) value = 255;
                    else value = (int)tValue;  // Throw away precision
                    bitmap.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }	
            */
            return bitmap;
        }


        public void ForwardTransform(double[] data, ref double[] low, ref double[] high)
        {
            double[] tdata = new double[data.Length];

            wfltr_convolve(true, data, 0, 1, data.Length, tdata, 0);

            int half = data.Length / 2;
            low = new double[half];
            high = new double[half];
            for (int i = 0; i < data.Length / 2; i++)
            {
                low[i] = tdata[i];
                high[i] = tdata[half + i];
            }
        }


        public static double[] ExtractGreyValues(UnsafeBitmap bitmap)
        {
            double[] data = new double[bitmap.Height * bitmap.Width];

            int count = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    double dvalue = (0.299 * c.R) + (0.587 * c.G) + (0.114 * c.B);
                    data[count++] = dvalue;
                }
            }

            return data;
        }

        /*
        public Bitmap Test(UnsafeBitmap img)
        {
            // This call was just removed, should not be needed.
            //ImageUtils.MakeGrey1(img);
            double[] data = ExtractGreyValues(img);
            double[] tdata = new double[data.Length];
            wfltr_convolve(true, data, 0, 1, data.Length, tdata, 0);

            double[] low = new double[data.Length / 2];
            for (int i = 0; i < data.Length / 2; i++)
            {
                low[i] = tdata[i];
            }

            int height = img.Height / 2;
            int width = img.Width / 2;
            UnsafeBitmap bitmap = new UnsafeBitmap(new Bitmap(width, height));
            double tValue;
            int count = 0;
            int value;
            PixelData pdata;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Very simple modification to keep data within specified range.
                    tValue = low[count++];
                    //if (tValue < 0) tValue = Math.Abs(tValue);
                    if (tValue > 255) value = 255;
                    else value = (int)tValue;  // Throw away precision
                    
                    bitmap.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }

            return bitmap;
        }
        */


        private void wfltr_convolve(
            bool isFwd,			/* in: true <=> forward transform */
            double[] aIn,	/* in: input data */
            int aInBase,
            int incA,			/* in: spacing of elements in aIn[] and aXf[] */
            int n,				/* in: size of aIn and aXf */
            double[] aXf,	/* out: output data (OK if == aIn) */
            int aXfBase)
        {
            int nDiv2 = n / 2;
            int iA, iHtilde, iGtilde, j, jH, jG, i;
            double sum;
            bool flip;

            double[] aTmp1D = new double[aIn.Length];

            /*
             *	According to Daubechies:
             *
             *	H is the analyzing smoothing filter.
             *	G is the analyzing detail filter.
             *	Htilde is the reconstruction smoothing filter.
             *	Gtilde is the reconstruction detail filter.
             */

            /* the reconstruction detail filter is the mirror of the analysis smoothing filter */
            int nGtilde = nH;

            /* the analysis detail filter is the mirror of the reconstruction smoothing filter */
            int nG = nHtilde;

            if (isFwd)
            {
                /*
                 *	A single step of the analysis is summarized by:
                 *		aTmp1D[0..n/2-1] = H * a (smooth component)
                 *		aTmp1D[n/2..n-1] = G * a (detail component)
                 */
                for (i = 0; i < nDiv2; i++)
                {
                    /*
                     *	aTmp1D[0..nDiv2-1] contains the smooth components.
                     */
                    sum = 0.0;
                    for (jH = 0; jH < nH; jH++)
                    {
                        /*
                         *	Each row of H is offset by 2 from the previous one.
                         *
                         *	We assume our data is periodic, so we wrap the aIn[] values
                         *	if necessary.  If we have more samples than we have H
                         *	coefficients, we also wrap the H coefficients.
                         */
                        iA = MOD(2 * i + jH - offH, n);
                        //System.Console.WriteLine("Low: element #" + (aInBase + incA * iA).ToString());
                        sum += cH[jH] * aIn[aInBase + incA * iA];
                    }
                    aTmp1D[i] = sum;

                    /*
                     *	aTmp1D[nDiv2..n-1] contains the detail components.
                     */
                    sum = 0.0;
                    flip = true;
                    for (jG = 0; jG < nG; jG++)
                    {
                        /*
                         *	We construct the G coefficients on-the-fly from the
                         *	Htilde coefficients.
                         *
                         *	Like H, each row of G is offset by 2 from the previous
                         *	one.  As with H, we also allow the coefficents of G to
                         *	wrap.
                         *
                         *	Again as before, the aIn[] values may wrap.
                         */
                        iA = MOD(2 * i + jG - offG, n);
                        if (flip)
                            sum -= cHtilde[nG - 1 - jG] * aIn[aInBase + incA * iA];
                        else
                            sum += cHtilde[nG - 1 - jG] * aIn[aInBase + incA * iA];
                        flip = !flip;
                    }
                    aTmp1D[nDiv2 + i] = sum;
                }
            }
            else
            {
                /*
                 *	The inverse transform is a little trickier to do efficiently.
                 *	A single step of the reconstruction is summarized by:
                 *
                 *		aTmp1D = Htilde^t * aIn[incA * (0..n/2-1)]
                 *				+ Gtilde^t * aIn[incA * (n/2..n-1)]
                 *
                 *	where x^t is the transpose of x.
                 */
                for (i = 0; i < n; i++)
                    aTmp1D[i] = 0.0;	/* necessary */
                for (j = 0; j < nDiv2; j++)
                {
                    for (iHtilde = 0; iHtilde < nHtilde; iHtilde++)
                    {
                        /*
                         *	Each row of Htilde is offset by 2 from the previous one.
                         */
                        iA = MOD(2 * j + iHtilde - offHtilde, n);
                        aTmp1D[iA] += cHtilde[iHtilde] * aIn[aInBase + incA * j];
                    }
                    flip = true;
                    for (iGtilde = 0; iGtilde < nGtilde; iGtilde++)
                    {
                        /*
                         *	As with Htilde, we also allow the coefficents of Gtilde,
                         *	which is the mirror of H, to wrap.
                         *
                         *	We assume our data is periodic, so we wrap the aIn[] values
                         *	if necessary.  If we have more samples than we have Gtilde
                         *	coefficients, we also wrap the Gtilde coefficients.
                         */
                        iA = MOD(2 * j + iGtilde - offGtilde, n);
                        if (flip)
                        {
                            aTmp1D[iA] -= cH[nGtilde - 1 - iGtilde]
                                * aIn[aInBase + incA * (j + nDiv2)];
                        }
                        else
                        {
                            aTmp1D[iA] += cH[nGtilde - 1 - iGtilde]
                                * aIn[aInBase + incA * (j + nDiv2)];
                        }
                        flip = !flip;
                    }
                }
            }
            for (i = 0; i < n; i++)
                aXf[aXfBase + incA * i] = aTmp1D[i];
        }

    }

}