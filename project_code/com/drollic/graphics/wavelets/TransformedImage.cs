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
    /// Summary description for TransformedImage.
    /// </summary>
    public class TransformedImage
    {
        public TransformedImage(Bitmap i)
        {
            this.originalImage = i;
        }

        public void Transform()
        {
            this.level1 = Transforms.ForwardTransform(this.originalImage, new Daub4Wavelet());
        }

        private TChannel[] level1;
        public TChannel[] Level1
        {
            get
            {
                return this.level1;
            }
        }

        private TChannel[] level2;
        public TChannel[] Level2
        {
            get
            {
                return this.level2;
            }
        }


        private TChannel[] level3;
        public TChannel[] Level3
        {
            get
            {
                return this.level3;
            }
        }


        private Bitmap originalImage;
        public Bitmap Original
        {
            get
            {
                return this.originalImage;
            }
        }
    }
}