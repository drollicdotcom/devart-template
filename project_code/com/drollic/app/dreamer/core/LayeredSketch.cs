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
using System.Security.Permissions;


using com.drollic.util;
using com.drollic.graphics;

namespace com.drollic.app.dreamer.core
{
    public sealed class LayeredSketch : Sketch
    {
        public LayeredSketch(int width, int height, List<Subject> subjects)
        {
            this.width = width;
            this.height = height;
            this.subjectsToSketch = subjects;
        }


        public override void Generate()
        {
            Random rand = new Random();
            //int subjectCount = 0;
            int totalSubjectsToSketch = this.subjectsToSketch.Count;

            //System.Console.WriteLine("Sketching " + totalSubjectsToSketch + " subjects.");
            
            foreach (Subject s in this.subjectsToSketch)
            {
                int sketchQuadrant = this.subjectsSketched.Count % 4;
                s.SketchedImage = new Bitmap(this.width, this.height);
                Graphics g = Graphics.FromImage(s.SketchedImage);
                Rectangle sketchAt;

                /*
                if (sketchQuadrant == 0)
                {
                    sketchAt = new Rectangle(0, 0, this.width - 50, this.height - 50);
                }
                else if (sketchQuadrant == 1)
                {
                    sketchAt = new Rectangle(50, 0, this.width - 50, this.height - 50);
                }
                else if (sketchQuadrant == 2)
                {
                    sketchAt = new Rectangle(0, 50, this.width - 50, this.height - 50);
                }
                else
                {
                    sketchAt = new Rectangle(50, 50, this.width - 50, this.height - 50);
                }
                */

                sketchAt = new Rectangle(0, 0, this.width, this.height);

                g.DrawImage(s.SourceImage, sketchAt);
                g.Flush();
                
                if (s.SketchedImage != null)
                {
                    //System.Console.WriteLine("Sketched image at " + imageBounds.X + ", " + imageBounds.Y);
                    this.subjectsSketched.Add(s);
                }
            }
        }
    }
}
