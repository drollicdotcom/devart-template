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
    /// Summary description for Daub4Wavelet.
    /// </summary>
    public sealed class Daub4Wavelet : GenericWavelet
    {
        static double[] cHDaubechies4 = {
												M_SQRT2 * (1 + SQRT3) / 8.0,
												M_SQRT2 * (3 + SQRT3) / 8.0,
												M_SQRT2 * (3 - SQRT3) / 8.0,
												M_SQRT2 * (1 - SQRT3) / 8.0
											};

        public Daub4Wavelet()
        {
            cH = cHDaubechies4;
            nH = cH.Length;
            cHtilde = cHDaubechies4;
            nHtilde = cHtilde.Length;
            offH = 1;
            offG = 1;
            offHtilde = 1;
            offGtilde = 1;
        }

        public override String ToString()
        {
            return "Daubechies 4";
        }
    }
}