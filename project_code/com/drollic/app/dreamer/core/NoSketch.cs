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
    public sealed class NoSketch : Sketch
    {
        public NoSketch(int width, int height, List<Subject> subjects)
        {
            this.width = width;
            this.height = height;
            this.subjectsToSketch = subjects;
        }


        public override void Generate()
        {
            foreach (Subject s in this.subjectsToSketch)
            {
                s.SketchedImage = ImageTools.ScaleByPercent(s.SourceImage, ScalingPercentForSubjectCount());
                if (s.SketchedImage != null)
                {
                    this.subjectsSketched.Add(s);
                }
            }
        }
    }
}
