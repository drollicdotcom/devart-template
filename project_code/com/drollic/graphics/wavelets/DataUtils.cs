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
    /// Summary description for DataUtils.
    /// </summary>
    public sealed class DataUtils
    {
        public static int[] ToIntegerValues(double[] values)
        {
            int[] ivals = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                ivals[i] = (int)values[i];
            }

            return ivals;
        }

        /// <summary>
        /// RSM: THIS IS A NEW METHOD!!! Added on new computer!!!
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int[] ToIntegerValues(double[] values, int low, int high)
        {
            int[] ivals = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > high)
                {
                    ivals[i] = high;
                }
                else if (values[i] < low)
                {
                    ivals[i] = low;
                }
                else
                {
                    ivals[i] = (int)values[i];
                }

            }

            return ivals;
        }        

        public static int[] ToRange(int[] values, int low, int high)
        {
            int[] newvals = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > high)
                {
                    newvals[i] = high;
                }
                else if (values[i] < low)
                {
                    newvals[i] = low;
                }
                else
                {
                    newvals[i] = values[i];
                }
            }

            return newvals;
        }
    }
}
