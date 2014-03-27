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
    public sealed class UserSpecifiedSketch : Sketch
    {
        private List<Rectangle> originalSubjectSizes = new List<Rectangle>();
        private List<Rectangle> specifiedSubjectSizes = new List<Rectangle>();
        private List<bool> currentlyShownList = new List<bool>();
        private List<int> currentPercentOfOriginal = new List<int>();

        public UserSpecifiedSketch(int width, int height, List<Subject> subjects)
        {
            this.width = width;
            this.height = height;
            this.subjectsToSketch = subjects;

            foreach (Subject s in this.subjectsToSketch)
            {    
                int subjectWidth = s.SourceImage.Width;
                int subjectHeight = s.SourceImage.Height;
                int currentPercentage = 100;

                if ((s.SourceImage.Width >= width) || (s.SourceImage.Height >= height))
                {
                    subjectWidth = (int)(subjectWidth * 0.5);
                    subjectHeight = (int)(subjectHeight * 0.5);
                    currentPercentage = 50;
                }

                // Start the rectangle off in a slightly offset initial location
                Rectangle rect = new Rectangle((int)(width * 0.025), (int)(height * 0.025), (int)(subjectWidth), (int)(subjectHeight));
                this.originalSubjectSizes.Add(rect);
                this.specifiedSubjectSizes.Add(rect);
                this.currentPercentOfOriginal.Add(currentPercentage);
                this.currentlyShownList.Add(false);
            }
        }

        public void SetSubjectVisibile(int index, bool isVisible)
        {
            this.currentlyShownList[index] = isVisible;
        }

        public void SetSubjectLocation(int index, int x, int y)
        {
            Rectangle rect = this.specifiedSubjectSizes[index];
            this.specifiedSubjectSizes[index] = new Rectangle(x, y, rect.Width, rect.Height);
        }

        public void ResizeSubject(int index, int percentWidth, int percentHeight)
        {
            this.currentPercentOfOriginal[index] = percentWidth;

            double widthMultiplier = percentWidth / 100.0;
            double heightMultiplier = percentHeight / 100.0;
            Rectangle currentLocation = this.specifiedSubjectSizes[index];
            Rectangle originalSize = this.originalSubjectSizes[index];
            this.specifiedSubjectSizes[index] = new Rectangle(currentLocation.X, currentLocation.Y, (int)(originalSize.Width * widthMultiplier), (int)(originalSize.Height * heightMultiplier));
        }

        public List<bool> SubjectVisibility
        {
            get
            {
                return this.currentlyShownList;
            }
        }

        public List<int> PercentageOfOriginal
        {
            get
            {
                return this.currentPercentOfOriginal;
            }
        }


        public List<Rectangle> SubjectSizes
        {
            get
            {
                return this.specifiedSubjectSizes;
            }
        }


        public override void Generate()
        {
            int subjectIndex = 0;

            foreach (Subject s in this.subjectsToSketch)
            {
                if (subjectIndex >= this.subjectsToSketch.Count)
                    break;

                Rectangle rect = this.specifiedSubjectSizes[subjectIndex++];

                int finalWidth = rect.Width;
                int finalHeight = rect.Height;
                s.SketchedImage = ImageTools.ScaleBySize(s.SourceImage, finalWidth, finalHeight);
                s.sketchAtX = rect.X;
                s.sketchAtY = rect.Y;

                //Console.WriteLine("Final Image: " + rect.X + ", " + rect.Y + ", " + (rect.X + finalWidth) + ", " + (rect.Y + finalHeight));

                this.subjectsSketched.Add(s);
            }
        }
    }
}
