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
    /// Summary description for BurtAdelsonWavelet.
    /// </summary>
    public sealed class BurtAdelsonWavelet : GenericWavelet
    {
        static double[] cHBurtAdelson = 
						 {
							 M_SQRT2 * -1.0 / 20.0,
							 M_SQRT2 *  5.0 / 20.0,
							 M_SQRT2 * 12.0 / 20.0,
							 M_SQRT2 *  5.0 / 20.0,
							 M_SQRT2 * -1.0 / 20.0,
							 0.0
						 };

        static double[] cHtildeBurtAdelson = 
			{
				0.0,
				M_SQRT2 *  -3.0 / 280.0,
				M_SQRT2 * -15.0 / 280.0,
				M_SQRT2 *  73.0 / 280.0,
				M_SQRT2 * 170.0 / 280.0,
				M_SQRT2 *  73.0 / 280.0,
				M_SQRT2 * -15.0 / 280.0,
				M_SQRT2 *  -3.0 / 280.0
			};

        public BurtAdelsonWavelet()
        {
            cH = cHBurtAdelson;
            nH = cH.Length;
            cHtilde = cHtildeBurtAdelson;
            nHtilde = cHtilde.Length;
            offH = 2;
            offG = 2;
            offHtilde = 4;
            offGtilde = 2;
        }

        public override String ToString()
        {
            return "Burt-Adelson";
        }
    }
}