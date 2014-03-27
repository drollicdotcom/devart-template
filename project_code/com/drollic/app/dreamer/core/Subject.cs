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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Security.Permissions;


namespace com.drollic.app.dreamer.core
{
    public sealed class Subject : IDisposable
    {
        public int sketchAtX;
        public int sketchAtY;
        public String filename;
        public int sizeInBytes;
        private Bitmap sourceImage;
        private Bitmap sketchedImage;
        public String fullURL;
        public String website;

        public Subject(String url, Bitmap theImage)
        {
            fullURL = url;
            sourceImage = theImage;
        }

        public Bitmap SourceImage
        {
            get
            {
                return this.sourceImage;
            }
        }

        public Bitmap SketchedImage
        {
            set
            {
                this.sketchedImage = value;
            }
            get
            {
                return this.sketchedImage;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.sourceImage != null)
            {
                this.sourceImage.Dispose();
                this.sourceImage = null;
            }

            if (this.sketchedImage != null)
            {
                this.sketchedImage.Dispose();
                this.sketchedImage = null;
            }
        }

        #endregion
    }
}
