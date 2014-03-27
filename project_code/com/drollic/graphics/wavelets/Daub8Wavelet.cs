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
    /// Summary description for Daub8Wavelet.
    /// </summary>
    public sealed class Daub8Wavelet : GenericWavelet
    {
        static double[] cHDaubechies8 = 
					 {
						 0.230377813309,
						 0.714846570553,
						 0.6308807667930,
						 -0.027983769417,
						 -0.187034811719,
						 0.030841381836,
						 0.032883011667,
						 -0.010597401785
					 };

        public Daub8Wavelet()
        {
            cH = cHDaubechies8;
            nH = cH.Length;
            cHtilde = cHDaubechies8;
            nHtilde = cHtilde.Length;
            offH = 1;
            offG = 5;
            offHtilde = 1;
            offGtilde = 5;
        }

        public override String ToString()
        {
            return "Daubechies 8";
        }
    }
}
