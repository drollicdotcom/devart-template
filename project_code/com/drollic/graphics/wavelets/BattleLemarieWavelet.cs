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
    /// Summary description for BattleLemarieWavelet.
    /// </summary>
    public sealed class BattleLemarieWavelet : GenericWavelet
    {
        static double[] cHBattleLemarie = 
			{
				M_SQRT2 * -0.002,
				M_SQRT2 * -0.003,
				M_SQRT2 *  0.006,
				M_SQRT2 *  0.006,
				M_SQRT2 * -0.013,
				M_SQRT2 * -0.012,
				M_SQRT2 *  0.030,  /*  5 and 6 sign change from Mallat's paper */
				M_SQRT2 *  0.023,
				M_SQRT2 * -0.078,
				M_SQRT2 * -0.035,
				M_SQRT2 *  0.307,
				M_SQRT2 *  0.542,
				M_SQRT2 *  0.307,
				M_SQRT2 * -0.035,
				M_SQRT2 * -0.078,
				M_SQRT2 *  0.023,
				M_SQRT2 *  0.030,
				M_SQRT2 * -0.012,
				M_SQRT2 * -0.013,
				M_SQRT2 *  0.006,
				M_SQRT2 *  0.006,
				M_SQRT2 * -0.003,
				M_SQRT2 * -0.002,
				0.0
			};

        public BattleLemarieWavelet()
        {
            cH = cHBattleLemarie;
            nH = cH.Length;
            cHtilde = cHBattleLemarie;
            nHtilde = cH.Length;
            offH = 11;
            offG = 11;
            offHtilde = 11;
            offGtilde = 11;
        }

        public override String ToString()
        {
            return "Battle-Lemarie";
        }
    }
}