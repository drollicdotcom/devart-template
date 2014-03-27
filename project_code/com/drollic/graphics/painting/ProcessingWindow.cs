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
using System.Security.Permissions;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// Summary description for ProcessingWindow.
	/// </summary>
    public sealed class ProcessingWindow : System.IComparable<ProcessingWindow>
	{
		public bool filled = false;
		public ColorImage sourceImage;
		public int x;
		public int y;
		public int Width;
		public int Height;
        public float score;        

		public ProcessingWindow(int x, int y, int width, int height, ColorImage img)
		{
			this.x = x;
			this.y = y;
			this.Width = width;
			this.Height = height;
			this.sourceImage = img;
		}

        public int CompareTo(ProcessingWindow value)
        {
            // NULL is considered to be smaller than
            // any other value.

            if (value == null)
            {
                return 1;
            }

            else if (this.score < value.score)
            {
                return -1;
            }

            else if (this.score == value.score)
            {
                return 0;
            }

            else
            {
                return 1;
            }
        }
	}
}
