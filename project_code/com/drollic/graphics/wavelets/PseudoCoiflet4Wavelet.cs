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
    /// Summary description for PseudoCoiflet4Wavelet.
    /// </summary>
    public sealed class PseudoCoiflet4Wavelet : GenericWavelet
    {
        static double[] cHPseudocoiflet4 = 
					 {
						 M_SQRT2 *  -1.0 / 512.0,
						 0.0,
						 M_SQRT2 *  18.0 / 512.0,
						 M_SQRT2 * -16.0 / 512.0,
						 M_SQRT2 * -63.0 / 512.0,
						 M_SQRT2 * 144.0 / 512.0,
						 M_SQRT2 * 348.0 / 512.0,
						 M_SQRT2 * 144.0 / 512.0,
						 M_SQRT2 * -63.0 / 512.0,
						 M_SQRT2 * -16.0 / 512.0,
						 M_SQRT2 *  18.0 / 512.0,
						 0.0,
						 M_SQRT2 *  -1.0 / 512.0,
						 0.0
					 };

        static double[] cHtildePseudocoiflet4 = 
			{
				0.0,
				M_SQRT2 *  -1.0 / 32.0,
				0.0,
				M_SQRT2 *   9.0 / 32.0,
				M_SQRT2 *  16.0 / 32.0,
				M_SQRT2 *   9.0 / 32.0,
				0.0,
				M_SQRT2 *  -1.0 / 32.0
			};

        public PseudoCoiflet4Wavelet()
        {
            cH = cHPseudocoiflet4;
            nH = cH.Length;
            cHtilde = cHtildePseudocoiflet4;
            nHtilde = cHtilde.Length;
            offH = 6;
            offG = 2;
            offHtilde = 4;
            offGtilde = 6;
        }

        public override String ToString()
        {
            return "Pseudo-Coiflet 4";
        }
    }
}