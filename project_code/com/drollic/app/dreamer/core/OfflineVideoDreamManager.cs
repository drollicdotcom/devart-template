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
using System.Diagnostics;
using System.IO;

using com.drollic.net;
using com.drollic.util;

namespace com.drollic.app.dreamer.core
{
    public sealed class OfflineVideoDreamManager
    {
        private static OfflineVideoDreamManager instance = new OfflineVideoDreamManager();

        private com.drollic.app.dreamer.webservices.offlinevideo.DreamerVideoService service = new com.drollic.app.dreamer.webservices.offlinevideo.DreamerVideoService();

        private Random randomEngine = new Random();

        private static readonly String loggingComponent = "DreamerVideoService";

        private static long videoDreamCounter = 0;

        private static readonly int desiredDreamDurationInSeconds = 30;
        

        private String workingDirectory = "";

        private OfflineVideoDreamManager()
        {
            // This call is required because it essentially registers this user.
            StatsManager.Instance.FetchStats();
        }

        public static OfflineVideoDreamManager Instance
        {
            get
            {
                return instance;
            }
        }

        public String WorkingDirectory
        {
            set
            {
                this.workingDirectory = value;
            }
        }


        //public bool GenerateVideoDream(String finalVideoFilename, int compositionWidth, int compositionHeight, int subjects, String phrase, String creator, List<int> instruments)
        //{
        //    bool videoGenerated = false;

        //    LayeredComposition videoGenerator = null;
        //    try
        //    {
        //        videoGenerator = new LayeredComposition(compositionWidth, compositionHeight, subjects, phrase, Composition.UrlProvider.Flickr);

        //        // Set the creator
        //        videoGenerator.Creator = creator;

        //        // Compute approximate dream duration and pass that to the soundtrack generator
        //        int approxDreamDuration = 3 + 3 + (subjects * 3) + 6 + 3;

        //        // Instantiate the music generator
        //        SequenceBasedMusicGenerator musicGenerator = new SequenceBasedMusicGenerator(this.workingDirectory, "music", instruments, approxDreamDuration);

        //        // Instantiate the video composer                
        //        VideoComposition vc = new VideoComposition(this.workingDirectory, videoGenerator, musicGenerator, finalVideoFilename);

        //        if (vc.Generate())
        //        {
        //            videoGenerated = true;
        //        }
        //        else
        //        {
        //            if (EventLog.SourceExists(loggingComponent))
        //            {
        //                String message = "VideoComposition.Generate method returned false, failed.";
        //                EventLog.WriteEntry(loggingComponent, message, EventLogEntryType.Error);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (EventLog.SourceExists(loggingComponent))
        //        {
        //            String message = "Unknown error: " + ex.ToString();
        //            EventLog.WriteEntry(loggingComponent, message, EventLogEntryType.Error);
        //        }
        //    }
        //    finally
        //    {
        //        if (videoGenerator != null)
        //        {
        //            videoGenerator.Dispose();
        //        }
        //    }

        //    return videoGenerated;
        //}


        private void deleteDreamRequest(int rowid)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    service.DeleteOfflineVideoRequest(rowid.ToString());
                    return;
                }
                catch (System.Net.WebException ex)
                {
                    service.Abort();

                    service.Dispose();

                    service = new com.drollic.app.dreamer.webservices.offlinevideo.DreamerVideoService();
                }

                if (attempts++ > 5)
                {
                    if (EventLog.SourceExists(loggingComponent))
                    {
                        EventLog.WriteEntry(loggingComponent, "Error deleting, reseting video service", EventLogEntryType.Warning);
                    }

                    attempts = 0;
                }
            }
        }



        //public bool FetchAndProcessRequest(int compositionWidth, int compositionHeight)
        //{
        //    bool dreamGenerated = false;

        //    try
        //    {
        //        String result = "";

        //        int attempts = 0;
        //        while ((result == null) || (result.Length == 0))
        //        {                    
        //            try
        //            {
        //                result = service.GetOfflineVideoRequest("null");
        //            }
        //            catch (System.Net.WebException ex)
        //            {
        //                service.Abort();

        //                service.Dispose();

        //                service = new com.drollic.app.dreamer.webservices.offlinevideo.DreamerVideoService();
        //            }

        //            if (attempts++ > 5)
        //            {
        //                if (EventLog.SourceExists(loggingComponent))
        //                {
        //                    EventLog.WriteEntry(loggingComponent, "Error fetching, reseting video service", EventLogEntryType.Warning);
        //                }

        //                attempts = 0;
        //            }
        //        }

        //        if ((result.Length > 0) && (! result.Equals("error")))
        //        {
        //            // First, split up the result string
        //            String[] parameters = result.Split(new String[] { ":@&%^" }, StringSplitOptions.None);

        //            if (parameters.Length != 5)
        //            {
        //                if (EventLog.SourceExists(loggingComponent))
        //                {
        //                    String message = "Incorrect number of parameters " + parameters.Length;
        //                    EventLog.WriteEntry(loggingComponent, message, EventLogEntryType.Error);
        //                }
        //                return dreamGenerated;
        //            }

        //            String phrase = parameters[0];
        //            String creator = sanitizer.Sanitize(parameters[1]);

        //            // Next, grab the total number of subjects
        //            int subjects = 3;
        //            try
        //            {
        //                subjects = Int32.Parse(parameters[2]);
        //            }
        //            catch (Exception ex)
        //            {
        //                // We have a real problem
        //                if (EventLog.SourceExists(loggingComponent))
        //                {
        //                    String message = "Error parsing rowID from dream request " + ex;
        //                    EventLog.WriteEntry(loggingComponent, message, EventLogEntryType.Error);
        //                }

        //                return false;
        //            }

        //            // Next, grab the total row ID for the request
        //            int rowid = 0;
        //            try
        //            {
        //                rowid = Int32.Parse(parameters[4]);
        //            }
        //            catch (Exception)
        //            {
        //            }

        //            // Set the image provider                  
        //            Composition.UrlProvider source = Composition.UrlProvider.Flickr;

        //            // Parse the instruments
        //            String[] instrumentStrings = parameters[3].Split(new String[] { "," }, StringSplitOptions.None);
        //            List<int> instruments = new List<int>();
        //            foreach (String instrumentString in instrumentStrings)
        //            {
        //                try
        //                {
        //                    instruments.Add(int.Parse(instrumentString));
        //                }
        //                catch (Exception)
        //                {
        //                    // Do nothing
        //                }
        //            }

        //            if (instruments.Count == 0)
        //            {
        //                if (EventLog.SourceExists(loggingComponent))
        //                {
        //                    EventLog.WriteEntry(loggingComponent, "No instruments, aborting", EventLogEntryType.Error);
        //                }

        //                return false;
        //            }

        //            // Instantiate the correct composition class (video generator)
        //            Composition videoGenerator = null;
        //            String basefilename = "movie" + videoDreamCounter + ".wmv";
        //            String finalVideoFilename = this.workingDirectory + "\\" + basefilename;
        //            try
        //            {
        //                // We have a real problem
        //                if (EventLog.SourceExists(loggingComponent))
        //                {
        //                    String message = "Processing request, title: " + phrase;
        //                    EventLog.WriteEntry(loggingComponent, message, EventLogEntryType.Information);
        //                }

        //                if (GenerateVideoDream(finalVideoFilename, compositionWidth, compositionHeight, subjects, phrase, creator, instruments))
        //                {
        //                    videoDreamCounter++;

        //                    // Finally, upload the video (perhaps on a different thread?)
        //                    YouTubeVideo video = new YouTubeVideo();
        //                    video.filepath = finalVideoFilename;
        //                    video.title = phrase;
        //                    video.description = "Create a unique video, visit http://www.drollic.com/projects/dreamer/video See more examples at http://www.drollic.com";
        //                    video.tags = "art,dreamer,drollic";
        //                    video.category = "Film & Animation";
        //                    video.ratings = "Yes";
        //                    video.comments = "Yes";
        //                    video.embedding = "Yes";
        //                    video.vidresp = "Yes";

        //                    YouTubeManager.UploadResult uploadResult = YouTubeManager.Instance.UploadVideo(video);

        //                    // Check for upload success
        //                    if (uploadResult.operationSuccessful)
        //                    {
        //                        dreamGenerated = true;

        //                        // Submit the blog entry
        //                        SubmissionManager.Instance.PerformVideoSubmission(creator, phrase, uploadResult.videoBlockHtml);
        //                    }
        //                    else
        //                    {
        //                        if (EventLog.SourceExists(loggingComponent))
        //                        {
        //                            String message = "YouTube upload failed.";
        //                            EventLog.WriteEntry(loggingComponent, message, EventLogEntryType.Error);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (EventLog.SourceExists(loggingComponent))
        //                    {
        //                        String message = "GenerateVideoDream returned false, not generated";
        //                        EventLog.WriteEntry(loggingComponent, message, EventLogEntryType.Error);
        //                    }
        //                }

        //                deleteDreamRequest(rowid);
        //            }
        //            catch (Exception ex)
        //            {
        //                if (EventLog.SourceExists(loggingComponent))
        //                {
        //                    String message = "Unknown error: " + ex.ToString();
        //                    EventLog.WriteEntry(loggingComponent, message, EventLogEntryType.Error);
        //                }
        //            }
        //            finally
        //            {
        //                if (videoGenerator != null)
        //                {
        //                    videoGenerator.Dispose();
        //                }

        //                // Clean up raw video file
        //                if (finalVideoFilename != null)
        //                {
        //                    if (File.Exists(finalVideoFilename))
        //                    {
        //                        File.Delete(finalVideoFilename);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (EventLog.SourceExists(loggingComponent))
        //        {
        //            String message = "Unknown error: " + ex.ToString();
        //            EventLog.WriteEntry(loggingComponent, message, EventLogEntryType.Error);
        //        }
        //    }

        //    return dreamGenerated;
        //}
    }
}
