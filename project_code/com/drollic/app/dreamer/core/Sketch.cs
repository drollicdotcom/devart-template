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
    /// <summary>
    /// 
    /// </summary>
    public abstract class Sketch : IDisposable
    {
        protected int width;
        protected int height;
        protected List<Subject> subjectsToSketch;
        protected List<Subject> subjectsSketched = new List<Subject>();
        protected Random rand = new Random();


        public Sketch()
        {
        }

        public Sketch(int width, int height, List<Subject> subjects)
        {
            this.width = width;
            this.height = height;
            this.subjectsToSketch = subjects;
        }

        public abstract void Generate();

        public List<Subject> Subjects
        {
            get
            {
                return this.subjectsToSketch;
            }
        }

        public List<Subject> SketchedSubjects
        {
            get
            {
                return this.subjectsSketched;
            }
        }

        protected int RandomScalingPercentFavorLarge()
        {
            if (rand.Next(100) < 75)
            {
                return rand.Next(45, 85);  // random scale
            }
            
            return rand.Next(35, 45);
        }

        protected int RandomScalingPercentFavorSmall()
        {
            if (rand.Next(100) < 75)
            {
                return rand.Next(35, 45);  // random scale
            }

            return rand.Next(45, 85);
        }        

        /// <summary>
        /// This method will return a random scaling percentage which is roughly based on
        /// the number of total images to sketch.  This percentage is meant to be a percentage
        /// of the final composition size, not a percentage of the individual image.
        /// </summary>
        /// <returns></returns>
        protected int ScalingPercentForSubjectCount()
        {
            if (this.subjectsToSketch.Count < 5)
                return RandomScalingPercentFavorLarge();

            return RandomScalingPercentFavorSmall();
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            // Null out the sketched list, disposal is not necessary because the
            // contained objects are the same as the ones in the subjectsToSketch
            // list, merely a subset.
            this.subjectsSketched = null;

            // Dispose of the original subjects.  This is the only subject list
            // that needs to be disposed of
            foreach (Subject subject in this.subjectsToSketch)
            {
                subject.Dispose();
            }

            // Now null out the original subjects list
            this.subjectsToSketch = null;
        }

        #endregion
    }
}
