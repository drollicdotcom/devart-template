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
    /// Summary description for Daub12Wavelet.
    /// </summary>
    public sealed class Daub12Wavelet : GenericWavelet
    {
        static double[] cHDaubechies12 = 
						 {
							 0.1115407433501095,
							 0.4946238903984533,
							 0.7511339080210959,
							 0.3152503517091982,
							 -0.2262646939654400,
							 -0.1297668675672625,
							 0.0975016055873225,
							 0.0275228655303053,
							 -0.0315820393184862,
							 0.0005538422011614,
							 0.0047772575119455,
							 -0.0010773010853085
						 };

        public Daub12Wavelet()
        {
            cH = cHDaubechies12;
            nH = cH.Length;
            cHtilde = cHDaubechies12;
            nHtilde = cHtilde.Length;
            offH = 1;
            offG = 9;
            offHtilde = 1;
            offGtilde = 9;
        }

        public override String ToString()
        {
            return "Daubechies 12";
        }
    }
}