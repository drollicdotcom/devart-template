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

using com.drollic.graphics;

namespace com.drollic.graphics.painting
{
    public sealed class BrushCollection
    {
        private UnsafeBitmap[] brushes;

        private static BrushCollection instance = new BrushCollection();

        private BrushCollection()
        {
            brushes = new UnsafeBitmap[3];

            //brushes[0] = new UnsafeBitmap(new Bitmap(Properties.Resources.brush1));
            //brushes[1] = new UnsafeBitmap(new Bitmap(Properties.Resources.brush2));
            //brushes[2] = new UnsafeBitmap(new Bitmap(Properties.Resources.brush3));

            brushes[0] = new UnsafeBitmap(new Bitmap(Properties.Resources.fatbrush));
            brushes[1] = new UnsafeBitmap(new Bitmap(Properties.Resources.fatbrush));
            brushes[2] = new UnsafeBitmap(new Bitmap(Properties.Resources.fatbrush));

            //brushes[3] = new UnsafeBitmap(new Bitmap(Properties.Resources.b1));
            //brushes[4] = new UnsafeBitmap(new Bitmap(Properties.Resources.b2));
            //brushes[5] = new UnsafeBitmap(new Bitmap(Properties.Resources.b4));
        }

        public static BrushCollection Instance
        {
            get
            {
                return instance;
            }
        }

        public Bitmap SingleBrush
        {
            get
            {
                return Properties.Resources.brush1;
            }
        }

        public UnsafeBitmap[] AvailableBrushes
        {
            get
            {
                return this.brushes;
            }
        }
    }
}
