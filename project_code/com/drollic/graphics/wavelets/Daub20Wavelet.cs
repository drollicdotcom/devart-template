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
    /// Summary description for Daub20Wavelet.
    /// </summary>
    public sealed class Daub20Wavelet : GenericWavelet
    {
        static double[] cHDaubechies20 = 
					 {
						 0.026670057901,
						 0.188176800078,
						 0.527201188932,
						 0.688459039454,
						 0.281172343661,
						 -0.249846424327,
						 -0.195946274377,
						 0.127369340336,
						 0.093057364604,
						 -0.071394147166,
						 -0.029457536822,
						 0.033212674059,
						 0.003606553567,
						 -0.010733175483,
						 0.001395351747,
						 0.001992405295,
						 -0.000685856695,
						 -0.000116466855,
						 0.000093588670,
						 -0.000013264203
					 };

        public Daub20Wavelet()
        {
            cH = cHDaubechies20;
            nH = cH.Length;
            cHtilde = cHDaubechies20;
            nHtilde = cHtilde.Length;
            offH = 2;
            offG = 16;
            offHtilde = 2;
            offGtilde = 16;
        }

        public override String ToString()
        {
            return "Daubechies 20";
        }
    }
}