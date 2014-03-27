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
using System.Security.Permissions;

using com.drollic.net;

namespace com.drollic.app.dreamer.core
{
    public sealed class SubjectAcquisitionStatus
    {
        public Subject subject;
        public int bytesDownloaded;
        public TimeSpan timeElapsed;
        public int throughput;
    };

    public sealed class SubjectManager
    {
        //event AcquisitionProgress( ArrayList<SubjectAcquisitionStatus> );
        //timer progressTimer;

        private SubjectManager()
        {
        }

        private static SubjectManager instance = new SubjectManager();
        public static SubjectManager Instance
        {
            get
            {
                return instance;
            }
        }


        public List<Subject> Acquire(int totalDesired, List<String> urls)
        {
            ImageLocalizer localizer = new ImageLocalizer(new Queue<string>(urls), totalDesired);
            List<DownloadedImage> results = localizer.Download();
            List<Subject> subjects = new List<Subject>();
            foreach (DownloadedImage dl in results)
            {
                subjects.Add(new Subject(dl.Url, dl.Image));
            }

            return subjects;
        }
    }
}
