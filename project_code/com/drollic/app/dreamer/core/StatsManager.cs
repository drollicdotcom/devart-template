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
using System.Security;
using System.Collections.Generic;
using System.Text;

using com.drollic.net;

namespace com.drollic.app.dreamer.core
{
    public sealed class StatsManager
    {
        private static StatsManager instance = new StatsManager();

        private String totalDreams = "?";        
        private String totalSubmissions = "?";
        private String totalUsers = "?";
        private String mySubmissions = "?";
        private String myRank = "?";        
        private bool haveSubmissionAddress = false;
        private String submissionAddress = "";

        private com.drollic.app.dreamer.webservices.DreamerWebService service = new com.drollic.app.dreamer.webservices.DreamerWebService();


        private StatsManager()
        {         
        }

        public static StatsManager Instance
        {
            get
            {
                return instance;
            }
        }

        public String TotalDreams
        {
            get
            {
                return this.totalDreams;
            }
        }

        public String TotalSubmissions
        {
            get
            {
                return this.totalSubmissions;
            }
        }

        public String TotalUsers
        {
            get
            {
                return this.totalUsers;
            }
        }

        public String MySubmissions
        {
            get
            {
                return this.mySubmissions;
            }
        }

        public String MyRank
        {
            get
            {
                return this.myRank;
            }
        }

        public bool HaveSubmissionAddress
        {
            get
            {
                return this.haveSubmissionAddress;
            }
        }

        public String SubmissionAddress
        {
            get
            {
                return this.submissionAddress;
            }
        }


        public void RecordSubmission()
        {
            String result = service.RecordSubmission(NetworkUtils.GetPhysicalAddress());
        }

        public void RecordDream()
        {
            String result = service.RecordDream(NetworkUtils.GetPhysicalAddress());
        }

        public void FetchSubmissionAddress()
        {
            String result = service.GetDreamerSubmissionEmail(NetworkUtils.GetPhysicalAddress());
            if (result.Contains("@"))
            {                
                this.submissionAddress = result;
                this.haveSubmissionAddress = true;
            }
        }


        public bool Start()
        {
            bool startSuccessful = false;

            try
            {
                FetchStats();
                startSuccessful = true;
            }
            catch (Exception)
            {
            }

            return startSuccessful;
        }


        public void FetchStats()
        {
            String result = service.GetDreamerStats(NetworkUtils.GetPhysicalAddress());
            String[] totals = result.Split(new char[] { ':' });

            if (totals.Length > 0)
                this.totalDreams = totals[0];
            if (totals.Length > 1)
                this.totalSubmissions = totals[1];
            if (totals.Length > 2)
                this.mySubmissions = totals[2];
            if (totals.Length > 3)
                this.myRank = totals[3];
        }
    }
}
