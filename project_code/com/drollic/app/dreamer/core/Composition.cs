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
using System.Net.Mail;

using com.drollic.util;
using com.drollic.graphics;
using com.drollic.graphics.painting;
using com.drollic.net.search.image;

namespace com.drollic.app.dreamer.core
{
    public abstract class Composition : IDisposable
    {
        public enum StructureType
        {
            VeryStructured,
            Random,
            Segmented,
            Interactive,
            Selective,
            SurpriseMe
        };

        public enum UrlProvider
        {
            Flickr,
            Yahoo,
            Mixture,
            Google,
            SurpriseMe
        };

        protected int totalSubjects;
        protected String searchText;
        protected int compositionWidth;
        protected int compositionHeight;
        protected static Random rand = new Random();
        protected UrlProvider urlProvider;
        protected Bitmap finalResult;
        protected List<Subject> subjects = new List<Subject>();
        protected Sketch sketch = null;
        protected bool submitted = false;
        protected bool submitting = false;
        protected bool generateMovie = false;
        protected List<Bitmap> movieIntroFrames = new List<Bitmap>();
        protected String movieFilename = "";
        protected String creator = "";

        public Composition(int width, int height, int totalSubjects, String searchText, UrlProvider urlProvider)
        {
            this.compositionWidth = width;
            this.compositionHeight = height;
            this.totalSubjects = totalSubjects;
            this.searchText = searchText;
            this.urlProvider = urlProvider;
        }

        public int DesiredSubjects
        {
            get
            {
                return this.totalSubjects;
            }
        }

        public String Creator
        {
            set
            {
                this.creator = value;
            }
        }

        public String MovieFilename
        {
            set
            {
                this.movieFilename = value;
            }
        }

        public bool GenerateMovie
        {
            get
            {
                return this.generateMovie;
            }

            set
            {
                this.generateMovie = value;

                if (this.generateMovie)
                {
                    GenerateIntroMovieFrames();
                }
            }
        }

        public bool Submitted
        {
            get
            {
                return this.submitted;
            }
            set
            {
                this.submitted = value;
            }
        }

        public bool Submitting
        {
            get
            {
                return this.submitting;
            }
            set
            {
                this.submitting = value;
            }
        }

        public Sketch Sketch
        {
            get
            {
                return this.sketch;
            }
            set
            {
                this.sketch = value;
            }
        }

        public Bitmap FinalResult
        {
            get
            {
                return this.finalResult;
            }
        }

        public String Title
        {
            get
            {
                return this.searchText;
            }
        }

        public List<Subject> Subjects
        {
            get
            {
                return this.subjects;
            }
        }

        public int Width
        {
            get
            {
                return this.compositionWidth;
            }
        }

        public int Height
        {
            get
            {
                return this.compositionHeight;
            }
        }

        private void GenerateIntroMovieFrames()
        {
            Bitmap firstFrame = Properties.Resources.movieTitleFrame; // (Bitmap)Bitmap.FromFile("movieTitleFrame.jpg");
            Bitmap movieReadySourceImage = new Bitmap(this.compositionWidth, this.compositionHeight);
            Graphics g = Graphics.FromImage(movieReadySourceImage);
            g.DrawImage(firstFrame, new Rectangle(0, 0, this.compositionWidth, this.compositionHeight));
            g.Flush();

            // First movie frame is the title frame, add it.
            movieIntroFrames.Add(movieReadySourceImage);

            Bitmap secondFrame = Properties.Resources.movieTitleFrame2; //(Bitmap)Bitmap.FromFile("movieTitleFrame2.jpg");
            Bitmap titleFrame = new Bitmap(this.compositionWidth, this.compositionHeight);
            g = Graphics.FromImage(titleFrame);
            g.DrawImage(secondFrame, new Rectangle(0, 0, this.compositionWidth, this.compositionHeight));

            // This should be a constant
            int maxTitleLineLength = 20;
            int maxTitleLines = 3;

            String remainingTitle = "\"" + this.Title + "\"";
            String[] words = remainingTitle.Split(' ');
            int wordCount = 0;
            String lineString = "";
            List<String> titleLines = new List<String>();            
            while (wordCount < words.Length)
            {
                if (titleLines.Count >= maxTitleLines)
                    break;

                String word = words[wordCount];

                if (word.Length > maxTitleLineLength)
                {
                    String tempWord = word.Substring(0, maxTitleLineLength - 1);
                    word = tempWord;
                }

                if ((lineString.Length + word.Length) < maxTitleLineLength)
                {
                    if (lineString.Length > 0)
                        lineString += " ";
                    lineString += word;
                    wordCount++;
                }
                else
                {
                    String newTitleLine = lineString;
                    titleLines.Add(newTitleLine);
                    lineString = "";
                }
            }

            if (lineString.Length > 0)
                titleLines.Add(lineString);

            if (this.creator.Length > 0)
            {
                String tempCreator = this.creator;
                if (tempCreator.Length > maxTitleLineLength)
                {
                    tempCreator = tempCreator.Substring(0, maxTitleLineLength - 1);
                }
                titleLines.Add("by");
                titleLines.Add(tempCreator);
            }

            int titleLineCounter = 1;
            foreach (String titleLine in titleLines)
            {
                if (titleLine.Equals("by"))
                    titleLineCounter++;

                g.DrawString(titleLine, new Font("Century", 20, FontStyle.Bold), Brushes.White, new PointF(10, 25 + (25 * titleLineCounter)));
                titleLineCounter++;
            }

            g.Flush();

            movieIntroFrames.Add(titleFrame);
        }

        private ImageUrlProvider CreateProvider()
        {
            ImageUrlProvider provider = null;

            if (urlProvider == UrlProvider.Flickr)
                provider = new Flickr();
            else if (urlProvider == UrlProvider.Google)
                provider = new GoogleCustomSearchImageFinder();
            else if (urlProvider == UrlProvider.Mixture)
                provider = new CrossServiceImageFinder();
            else
            {
                provider = new CrossServiceImageFinder();
            }

            provider.TotalDesiredImages = this.totalSubjects;

            return provider; 

        }

        public void ObtainSubjects()
        { 
            ImageUrlProvider provider = CreateProvider();

            StatusManager.Instance.SetCurrentTask("Searching...");

            List<String> imageurls = provider.Search(this.searchText);

            if ((imageurls == null) || (imageurls.Count == 0))
            {
                StatusManager.Instance.SetCurrentTask("No subjects found.");                
            }
            else
            {
                StatusManager.Instance.SetCurrentTask("Acquiring subjects...");
                this.subjects = SubjectManager.Instance.Acquire(provider.TotalDesiredImages, imageurls);
            }
        }

        public void Submit(String author, String url)
        {
            try
            {
                if ((this.FinalResult != null) && (!this.Submitted))
                {
                    // Sanitize the author
                    String safeAuthor = author;                    

                    // Sanitize the location
                    String safeUrl = url;

                    StringBuilder originalUrlList = new StringBuilder();
                    foreach (Subject s in this.subjects)
                    {
                        String filename = s.fullURL.Substring(s.fullURL.LastIndexOf("/") + 1);
                        if (filename.Length > 80)
                        {
                            filename = filename.Substring(0, 70);
                        }
                        originalUrlList.Append("<a href=\"");
                        originalUrlList.Append(s.fullURL);
                        originalUrlList.Append("\" target=\"_blank\">");
                        originalUrlList.Append(filename);
                        originalUrlList.Append("</a><br>");
                    }

                    // Submit the composition
                    //SubmissionManager.SubmissionResult result = SubmissionManager.Instance.PerformSubmission(safeAuthor, safeUrl, this.Title, this.FinalResult, originalUrlList.ToString());
                    //if (result.operationSuccessful)
                    //{
                        // Mark the composition as submitted
                    //    this.Submitted = true;
                    //}
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// This method performs the work of submitting the composition.   It will
        /// only allow the composition to be submitted once.
        /// </summary>
        /*
        public void SubmitViaEmail(String author, String location)
        {
            if ((this.FinalResult != null) && (! this.Submitted))
            {
                // This method shouldn't be called if the address isn't
                // known, but do this sanity check anyhow.
                if (!StatsManager.Instance.HaveSubmissionAddress)
                {
                    return;
                }

                String safeAuthor = author;
                if (this.sanitizer.ContainsBadWord(author))
                    safeAuthor = "Jerk";

                String safeLocation = location;
                if (this.sanitizer.ContainsBadWord(location))
                    safeLocation = "Jerksville";


                MailMessage message = new MailMessage("dreamer@drollic.com", StatsManager.Instance.SubmissionAddress);
                message.Body = "<b>Creator: </b>" + safeAuthor + "<br>" +
                               "<b>Location: </b>" + safeLocation + "<br>";
                message.Subject = this.Title;

                // Attach the FinalResult as an attachment to the submission email
                System.IO.MemoryStream imgStream = new System.IO.MemoryStream();
                this.FinalResult.Save(imgStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] b = imgStream.ToArray();
                // Is this write even needed?
                imgStream.Write(b, 0, b.GetLength(0));
                imgStream.Seek(0, System.IO.SeekOrigin.Begin);

                Attachment attachment = new Attachment(imgStream, "image.jpg", "image/jpeg");
                message.Attachments.Add(attachment);

                // Ask the Mailer to deliver this submission via email
                Mailer.Instance.QueueForDelivery(message);

                // Mark the composition as submitted
                this.Submitted = true;

                // Record the submission via the StatsManager
                StatsManager.Instance.RecordSubmission();
            }
        }
        */


        protected abstract void CreateSketch();

        protected abstract Bitmap CreateComposition();

        public bool Compose()
        {
            if ((subjects == null) || (subjects.Count < 1))
                ObtainSubjects();

            if ((subjects == null) || (subjects.Count < 1))
                return false;

            // Prune the available subjects down to how many we actually need
            if (subjects.Count > this.totalSubjects)
                subjects.RemoveRange(this.totalSubjects, subjects.Count - this.totalSubjects);

            // Create the sketch to be rendered
            // TODO: Make dimensions a property of something!
            StatusManager.Instance.SetCurrentTask("Sketching subjects...");

            // Have the implementing class generate the appropriate sketch
            CreateSketch();

            //StatusManager.Instance.StatusMessage("Sketched " + sketch.SketchedSubjects.Count + " subjects");         

            if (this.sketch.SketchedSubjects.Count > 0)
            {
                // Now create the painting
                StatusManager.Instance.SetCurrentTask("Composing dream...");

                // Set the into frames, if we're generating a movie
                if (this.generateMovie)
                {
                    foreach (Subject s in this.subjects)
                    {                        
                        Bitmap tempImage = ImageTools.ScaleBySize(s.SourceImage, this.compositionWidth, this.compositionHeight);                        
                        Bitmap movieReadySourceImage = new Bitmap(this.compositionWidth, this.compositionHeight);
                        Graphics g = Graphics.FromImage(movieReadySourceImage);
                        g.DrawImage(tempImage,new Rectangle(0,0,this.compositionWidth, this.compositionHeight));
                        g.Flush();
                        this.movieIntroFrames.Add(movieReadySourceImage);
                    }
                }

                DateTime start = DateTime.Now;
                // Generate the dream
                this.finalResult = CreateComposition();

                TimeSpan duration = DateTime.Now - start;
                System.Console.Write(duration.TotalMilliseconds + " ms ");

                StatusManager.Instance.SetCurrentTask("Dreaming complete.");

                return true;
            }
            else
            {
                return false;
            }
        }


        public bool DownloadAndSketch()
        {
            if ((subjects == null) || (subjects.Count < 1))
                ObtainSubjects();

            if ((subjects == null) || (subjects.Count < 1))
                return false;

            // Prune the available subjects down to how many we actually need
            if (subjects.Count > this.totalSubjects)
                subjects.RemoveRange(this.totalSubjects, subjects.Count - this.totalSubjects);

            // Create the sketch to be rendered
            // TODO: Make dimensions a property of something!
            StatusManager.Instance.SetCurrentTask("Sketching subjects...");

            // Have the implementing class generate the appropriate sketch
            CreateSketch();

            //StatusManager.Instance.StatusMessage("Sketched " + sketch.SketchedSubjects.Count + " subjects");         

            return true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Dispose of the final painting
            if (this.finalResult != null)
                this.finalResult.Dispose();



            // Dispose of the sketch -- which will take care of disposing the subjects, 
            // but only if it has been created
            if (this.sketch != null)
            {
                this.sketch.Dispose();
            }
            else
            {
                if (this.subjects != null)
                {
                    foreach (Subject subject in this.subjects)
                    {
                        subject.Dispose();
                    }
                }
            }

            // We're done using the subjects, and the sketch will dispose of these
            this.subjects = null;

            this.sketch = null;
        }

        #endregion
    }
}
