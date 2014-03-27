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

using com.drollic.net;

namespace com.drollic.app.dreamer.core
{
    public sealed class OfflineDreamManager
    {
        private static OfflineDreamManager instance = new OfflineDreamManager();

        private com.drollic.app.dreamer.webservices.offline.DreamerOfflineService service = new com.drollic.app.dreamer.webservices.offline.DreamerOfflineService();

        private Random randomEngine = new Random();

        private int compositionWidth = 400;

        private int compositionHeight = 300;


        private OfflineDreamManager()
        {
            // This call is required because it essentially registers this user.
            StatsManager.Instance.FetchStats();
        }

        public static OfflineDreamManager Instance
        {
            get
            {
                return instance;
            }
        }


        public bool FetchAndProcessRequest()
        {
            bool dreamGenerated = false;
            try
            {
                String result = service.GetOfflineDreamRequest("null");

                if ((result.Length > 0) && (! result.Equals("error")))
                {
                    // First, split up the result string
                    String[] parameters = result.Split(new String[] { ":@&%^" }, StringSplitOptions.None);

                    if (parameters.Length != 4)
                    {
                        if (EventLog.SourceExists("DreamerService"))
                        {
                            String message = "Incorrect number of parameters " + parameters.Length;
                            EventLog.WriteEntry("DreamerService", message, EventLogEntryType.Error);
                        }
                        return dreamGenerated;
                    }

                    String phrase = parameters[0];

                    // Next, grab the total number of subjects
                    int subjects = 3;
                    try
                    {
                        subjects = Int32.Parse(parameters[2]);
                    }
                    catch (Exception)
                    {
                    }

                    // Next, grab the image provider
                    Composition.UrlProvider source = Composition.UrlProvider.Yahoo;
                    if (parameters[3].ToLower().Equals("yahoo"))
                    {
                        source = Composition.UrlProvider.Yahoo;
                    }
                    else if (parameters[3].ToLower().Equals("flickr"))
                    {
                        source = Composition.UrlProvider.Flickr;
                    }
                    else if (parameters[3].ToLower().Equals("mixture"))
                    {
                        source = Composition.UrlProvider.Mixture;
                    }
                    else if (parameters[3].ToLower().Equals("surprise"))
                    {
                        source = Composition.UrlProvider.SurpriseMe;
                    }

                    // Finally, instantiate the correct composition class
                    Composition c = null;
                    try
                    {
                        /*
                        if (parameters[1].ToLower().Equals("structured"))
                        {
                            c = new SimpleComposition(compositionWidth, compositionHeight, subjects, phrase, source);
                        }
                        else if (parameters[1].ToLower().Equals("chaos"))
                        {
                            c = new RandomComposition(compositionWidth, compositionHeight, subjects, phrase, source);
                        }
                        else if (parameters[1].ToLower().Equals("segmented"))
                        {
                            c = new WaveletTransformComposition(compositionWidth, compositionHeight, subjects, phrase, source);
                        }
                        else if (parameters[1].ToLower().Equals("surprise"))
                        {
                            int value = randomEngine.Next(1, 4);
                            if (value == 1)
                            {
                                c = new SimpleComposition(compositionWidth, compositionHeight, subjects, phrase, source);
                            }
                            else if (value == 2)
                            {
                                c = new WaveletTransformComposition(compositionWidth, compositionHeight, subjects, phrase, source);
                            }
                            else
                            {
                                c = new RandomComposition(compositionWidth, compositionHeight, subjects, phrase, source);
                            }
                        }
                        */

                        c = new LayeredComposition(compositionWidth, compositionHeight, subjects, phrase, source);

                        // Generate the dream
                        if (c != null)
                        {
                            // Finally, handle the request.
                            ProcessRequest(c);

                            dreamGenerated = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        if (EventLog.SourceExists("DreamerService"))
                        {
                            String message = "Unknown error: " + ex.ToString();
                            EventLog.WriteEntry("DreamerService", message, EventLogEntryType.Error);
                        }
                    }
                    finally
                    {
                        if (c != null)
                        {
                            c.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventLog.SourceExists("DreamerService"))
                {
                    String message = "Unknown error: " + ex.ToString();
                    EventLog.WriteEntry("DreamerService", message, EventLogEntryType.Error);
                }
            }

            return dreamGenerated;
        }


        private void ProcessRequest(Composition c)
        {
            c.Compose();
            c.Submit("", "");
        }
    }
}
