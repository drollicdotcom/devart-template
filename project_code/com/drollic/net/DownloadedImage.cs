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

namespace com.drollic.net
{
	/// <summary>
	/// Summary description for DownloadedImage.
	/// </summary>
	public sealed class DownloadedImage
	{
		private String url = null;
		private Bitmap image = null;

		public DownloadedImage(Bitmap image, String url)
		{
			this.image = image;
			this.url = url;
		}

		public String Url
		{
			get
			{
				return this.url;
			}
		}

        public Bitmap Image
        {
            get
            {
                return this.image;
            }
        }
    }
}
