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
using System.Collections;
using System.Drawing;
using System.Text;
using System.Security.Permissions;

namespace com.drollic.util
{
	/// <summary>
	/// Summary description for StatusReporter.
	/// </summary>
    /// 
	public sealed class StatusManager
	{
        private String highLevelTask = "";
        private String taskCompletionSummary = "";
        private Hashtable taskCompletions = Hashtable.Synchronized(new Hashtable());       

		public static StatusManager instance = new StatusManager();

		public delegate void WorkingImageHandler(Bitmap image);
		public event WorkingImageHandler WorkingImageEvent;

		public delegate void CurrentTaskHandler(String task);
		public event CurrentTaskHandler CurrentTaskEvent;

        public delegate void StatusMessageHandler(String mesg);
        public event StatusMessageHandler StatusMessageEvent;

        public delegate void StatusSummaryUpdateHandler(String mesg);
        public event StatusSummaryUpdateHandler StatusSummaryUpdateEvent;

		private StatusManager()
		{
		}

		public static StatusManager Instance
		{
			get
			{
				return instance;
			}
		}

        public void StatusMessage(String mesg)
        {
            if (this.StatusMessageEvent != null)
                this.StatusMessageEvent(mesg);
        }

		public void SubmitWorkingImage(Bitmap image)
		{
			if (this.WorkingImageEvent != null)
				this.WorkingImageEvent(image);
		}

		public void SetCurrentTask(String task)
		{
            this.highLevelTask = task;

            lock (this.taskCompletions)
            {
                this.taskCompletions.Clear();
            }

            CompileTaskStatusSummary();

			if (this.CurrentTaskEvent != null)
				this.CurrentTaskEvent(task);

            if (this.StatusSummaryUpdateEvent != null)
                this.StatusSummaryUpdateEvent(this.taskCompletionSummary);
		}        

        private void CompileTaskStatusSummary()
        {
            int counter = 1;
            StringBuilder sb = new StringBuilder();

            try
            {
                lock (this.taskCompletions)
                {
                    foreach (int taskId in this.taskCompletions.Keys)
                    {
                        if (counter++ > 1)
                            sb.Append(", ");
                        sb.Append(this.taskCompletions[taskId]);
                        sb.Append("%");
                    }
                }
                this.taskCompletionSummary = sb.ToString();
            }
            catch (InvalidOperationException)
            {
                // This exception will be thrown if the taskCompletions collection is 
                // modified during this iteration
            }
        }

        public void RemoveTask(int taskId)
        {
            lock (this.taskCompletions)
            {
                this.taskCompletions.Remove(taskId);
            }

            CompileTaskStatusSummary();

            if (this.StatusSummaryUpdateEvent != null)
                this.StatusSummaryUpdateEvent(this.taskCompletionSummary);
        }

        public void SetTaskPercentCompletion(int taskId, int percentComplete)
        {
            // Sanity check the data
            if (percentComplete < 0)
                percentComplete = 0;
            if (percentComplete > 100)
                percentComplete = 100;

            lock (this.taskCompletions)
            {
                this.taskCompletions[taskId] = percentComplete;
            }

            CompileTaskStatusSummary();

            if (this.StatusSummaryUpdateEvent != null)
                this.StatusSummaryUpdateEvent(this.taskCompletionSummary);
        }
	}
}