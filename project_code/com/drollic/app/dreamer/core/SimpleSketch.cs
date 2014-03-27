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
    public sealed class SimpleSketch : Sketch
    {
        public SimpleSketch(int width, int height, List<Subject> subjects)
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
                s.SketchedImage = ImageTools.ScaleByPercent(s.SourceImage, ScalingPercentForSubjectCount());

                if (s.SketchedImage != null)
                {
                    GraphicsUnit unit = GraphicsUnit.Pixel;
                    RectangleF imageBounds = s.SketchedImage.GetBounds(ref unit);
                    imageBounds.Width *= 0.85F;  // Acceptable overlap in X
                    imageBounds.Height *= 0.85F; // Acceptable overlap in Y                

                    // Assign a random starting value to the image X/Y offset so
                    // each subject is tested against slightly different coordinates
                    // and not 0, 5, 10, 15...                
                    imageBounds.Y = rand.Next(1, this.height / 5);

                    int maxHeightOffScreen = (int)(this.height * 1.1);
                    int maxWidthOffScreen = (int)(this.width * 1.1);
                    bool fitIntoSketch = false;

                    while (((imageBounds.Y + imageBounds.Height) < maxHeightOffScreen) && (!fitIntoSketch))
                    {
                        imageBounds.X = rand.Next(1, this.width / 5);

                        while (((imageBounds.X + imageBounds.Width) < maxWidthOffScreen) && (!fitIntoSketch))
                        {
                            s.sketchAtX = (int)imageBounds.X;
                            s.sketchAtY = (int)imageBounds.Y;

                            if (this.subjectsSketched.Count == 0)
                            {
                                fitIntoSketch = true;
                                //StatusManager.Instance.StatusMessage("Sketched image at " + imageBounds.X + ", " + imageBounds.Y);
                                this.subjectsSketched.Add(s);
                            }
                            else
                            {
                                fitIntoSketch = true;

                                foreach (Subject subjectAlreadySketched in this.subjectsSketched)
                                {
                                    RectangleF sketchBounds = subjectAlreadySketched.SketchedImage.GetBounds(ref unit);
                                    sketchBounds.X = subjectAlreadySketched.sketchAtX;
                                    sketchBounds.Y = subjectAlreadySketched.sketchAtY;

                                    if (imageBounds.IntersectsWith(sketchBounds))
                                    {
                                        fitIntoSketch = false;
                                        break;
                                    }
                                }

                                if (fitIntoSketch)
                                {
                                    //System.Console.WriteLine("Sketched image at " + imageBounds.X + ", " + imageBounds.Y);
                                    this.subjectsSketched.Add(s);
                                }
                            }

                            imageBounds.X += rand.Next(1, this.width / 20);
                        }

                        imageBounds.Y += rand.Next(1, this.height / 20);
                    }
                }
            }
        }
    }
}
