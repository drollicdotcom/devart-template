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
    /// Summary description for Transforms.
    /// </summary>
    public sealed class Transforms
    {
        /// <summary>
        /// This version of the transform method is the same as transform2 except that
        /// it uses the GenericWavelet object tree.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static TChannel[] ForwardTransform(UnsafeBitmap bitmap, GenericWavelet transform)
        {
            double[] rawHorizData = ImageTools.ExtractGreyValues(bitmap);
            Point size = ImageTools.PixelSize(bitmap);

            System.Diagnostics.Debug.Assert(((rawHorizData.Length % 2) == 0), "Image size power of 2!");
            System.Diagnostics.Debug.Assert(((size.X % 2) == 0), "X axis not power of 2!");
            System.Diagnostics.Debug.Assert(((size.Y % 2) == 0), "Y axis not power of 2!");

            // Step #1: Grab raw pixel data from image and store it serially
            int half = rawHorizData.Length / 2;

            // Step #2: Perform transform on data sequenced horizontally
            double[] lowHorizPass = new double[half];
            double[] highHorizPass = new double[half];
            transform.ForwardTransform(rawHorizData, ref lowHorizPass, ref highHorizPass);

            //Bitmap i = GenImage(lowHorizPass, bitmap.Width/2, bitmap.Height/2);
            //i.Save("result.jpg", ImageFormat.Jpeg);

            // Step #3: Once transformed, rotate transformed data sets 90 degrees for
            // transform on vertically sequenced data.
            double[] rawLowVertData = new double[half];
            double[] rawHighVertData = new double[half];
            rawLowVertData = ImageTools.rotate90Clockwise(lowHorizPass, size.X / 2);
            rawHighVertData = ImageTools.rotate90Clockwise(highHorizPass, size.X / 2);

            // Useful for testing
            //TChannel c = new TChannel(rawVertData, size.Y, size.X/2);
            //c.generateQuantizedImage(5).Save("test.jpg", ImageFormat.Jpeg);

            // Step #4: Perform transform on data sequenced vertically
            double[] rawLLPass = new double[half / 2];
            double[] rawLHPass = new double[half / 2];
            double[] rawHLPass = new double[half / 2];
            double[] rawHHPass = new double[half / 2];
            transform.ForwardTransform(rawLowVertData, ref rawLLPass, ref rawLHPass);
            transform.ForwardTransform(rawHighVertData, ref rawHLPass, ref rawHHPass);

            // Sanity check
            System.Diagnostics.Debug.Assert(rawLLPass.Length == (size.Y / 2 * size.X / 2), "raw LL pass data size does not match!");

            // Step #5: Rotate final data sets back to original orientation.
            TChannel llChannel = new TChannel(ImageTools.rotate90CounterClockwise(rawLLPass, size.Y / 2), size.X / 2, size.Y / 2);
            TChannel lhChannel = new TChannel(ImageTools.rotate90CounterClockwise(rawLHPass, size.Y / 2), size.X / 2, size.Y / 2);
            TChannel hlChannel = new TChannel(ImageTools.rotate90CounterClockwise(rawHLPass, size.Y / 2), size.X / 2, size.Y / 2);
            TChannel hhChannel = new TChannel(ImageTools.rotate90CounterClockwise(rawHHPass, size.Y / 2), size.X / 2, size.Y / 2);

            // Useful for testing
            //Bitmap image2 = llChannel.generateQuantizedImage(5);
            //Bitmap image3 = lhChannel.generateQuantizedImage(5);
            //image2.Save("LLImage.jpg", ImageFormat.Jpeg);
            //image3.Save("LHImage.jpg", ImageFormat.Jpeg);

            // Step #6: Store transformed channel data into container and return.
            TChannel[] results = new TChannel[4];
            results[TChannel.LL] = llChannel;
            results[TChannel.LH] = lhChannel;
            results[TChannel.HL] = hlChannel;
            results[TChannel.HH] = hhChannel;

            return results;
        }
    }
}
