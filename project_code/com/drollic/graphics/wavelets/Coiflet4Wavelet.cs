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
    /// Summary description for Coiflet4Wavelet.
    /// </summary>
    public sealed class Coiflet4Wavelet : GenericWavelet
    {
        static double[] cHCoiflet4 = 
	{
		0.0011945726958388,
		-0.01284557955324,
		0.024804330519353,
		0.050023519962135,
		-0.15535722285996,
		-0.071638282295294,
		0.57046500145033,
		0.75033630585287,
		0.28061165190244,
		-0.0074103835186718,
		-0.014611552521451,
		-0.0013587990591632
	};


        public Coiflet4Wavelet()
        {
            cH = cHCoiflet4;
            nH = cH.Length;
            cHtilde = cHCoiflet4;
            nHtilde = cHtilde.Length;
            offH = 6;
            offG = 4;
            offHtilde = 6;
            offGtilde = 4;
        }

        public override String ToString()
        {
            return "Coiflet 4";
        }
    }
}