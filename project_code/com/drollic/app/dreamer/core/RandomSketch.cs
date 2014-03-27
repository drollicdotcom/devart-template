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
using System.Drawing.Drawing2D;

using com.drollic.util;
using com.drollic.graphics;
using System.Security.Permissions;

namespace com.drollic.app.dreamer.core
{
    public sealed class RandomSketch : Sketch
    {
        public RandomSketch(int width, int height, List<Subject> subjects)
        {
            this.width = width;
            this.height = height;
            this.subjectsToSketch = subjects;
        }


        public override void Generate()
        {
            Random rand = new Random();

            foreach (Subject s in this.subjectsToSketch)
            {
                s.SketchedImage = ImageTools.ScaleByPercent(s.SourceImage, ScalingPercentForSubjectCount());

                if (s.SketchedImage != null)
                {
                    GraphicsUnit unit = GraphicsUnit.Pixel;
                    RectangleF imageBounds = s.SketchedImage.GetBounds(ref unit);

                    int upperX = this.width / 2;
                    if (this.width > s.SketchedImage.Width)
                    {
                        upperX = this.width - s.SketchedImage.Width;
                    }

                    int upperY = this.height / 2;
                    if (this.height > s.SketchedImage.Height)
                    {
                        upperY = this.height - s.SketchedImage.Height;
                    }

                    s.sketchAtX = rand.Next(0, upperX);
                    s.sketchAtY = rand.Next(0, upperY);

                    //StatusManager.Instance.StatusMessage("Sketched image at " + s.sketchAtX + ", " + s.sketchAtY);

                    this.subjectsSketched.Add(s);
                }
            }
        }
    }
}
