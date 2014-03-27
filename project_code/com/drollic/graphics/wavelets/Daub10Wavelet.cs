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
    /// Summary description for Daub10Wavelet.
    /// </summary>
    public sealed class Daub10Wavelet : GenericWavelet
    {
        static double[] cHDaubechies10 = 
				 {
					 0.1601023979741929,
					 0.6038292697971895,
					 0.7243085284377726,
					 0.1384281459013203,
					 -0.2422948870663823,
					 -0.0322448695846381,
					 0.0775714938400459,
					 -0.0062414902127983,
					 -0.0125807519990820,
					 0.0033357252854738
				 };

        public Daub10Wavelet()
        {
            cH = cHDaubechies10;
            nH = cH.Length;
            cHtilde = cHDaubechies10;
            nHtilde = cHtilde.Length;
            offH = 1;
            offG = 7;
            offHtilde = 1;
            offGtilde = 7;
        }

        public override String ToString()
        {
            return "Daubechies 10";
        }
    }
}