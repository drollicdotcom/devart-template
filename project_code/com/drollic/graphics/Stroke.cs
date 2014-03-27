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
using System.Security.Permissions;

namespace com.drollic.graphics
{
	public struct Stroke : System.IComparable
	{
		public int xc, yc;
		public float w, l;
		public int red;
		public int green;
		public int blue;
		public float theta;
		public int level;

		public int CompareTo(System.Object obj)
		{
			// Less than zero if this instance is less than obj.
			// Zero if this instance is equal to obj.
			// Greater than zero if this instance is greater than obj.
			Stroke right = (Stroke)(obj);

			float leftArea = this.l * this.w;
			float rightArea = right.l * right.w;

			// We wish to reverse sort the strokes so the big strokes are painted first.
			if (leftArea < rightArea)
				return 1;
			if (leftArea == rightArea)
				return 0;
			return -1;
		}
	}
}