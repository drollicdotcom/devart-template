using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using com.drollic.util;
using com.drollic.graphics.painting;


namespace com.drollic.app.dreamer.core
{
    public sealed class LayeredComposition : Composition
    {
        public LayeredComposition(int width, int height, int totalSubjects, String searchText, UrlProvider urlProvider)
            : base(width, height, totalSubjects, searchText, urlProvider)
        {
            
        }

        private LayeredComposition(int width, int height)
            : base(width, height, 1, "", UrlProvider.Yahoo)
        {
        }


        public static Bitmap Test(int width, int height, String searchText, UrlProvider urlProvider)
        {
            System.Random rand = new System.Random();
            LayeredComposition c = new LayeredComposition(width, height, rand.Next(1, 9), searchText, urlProvider);
            c.Compose();

            return c.FinalResult;
        }

        public static Bitmap Test(int width, int height, Bitmap testImage)
        {
            LayeredComposition c = new LayeredComposition(width, height);
            c.subjects.Add(new Subject("http://www.google.com", testImage));
            StatusManager.Instance.StatusMessage("Sketching...");
            c.CreateSketch();
            StatusManager.Instance.StatusMessage("Sketching completed.");
            return c.CreateComposition();
        }

        protected override void CreateSketch()
        {
            // Instantiate the sketcher.  We are guaranteed that subjects will be set by this point.
            this.sketch = new LayeredSketch(this.compositionWidth, this.compositionHeight, this.subjects);
            
            // Generate the "sketch"
            sketch.Generate();
        }

        protected override Bitmap CreateComposition()
        {
            List<Bitmap> originals = new List<Bitmap>();
            List<Point> locations = new List<Point>();

            try
            {
                // Set the locations determined by the sketch
                foreach (Subject s in this.sketch.SketchedSubjects)
                {
                    originals.Add(s.SketchedImage);
                    locations.Add(new Point(s.sketchAtX, s.sketchAtY));
                }

                // Instantiate the painter and create the composition
                using (Painter painter = new CompositeNativeMomentsPainter(this.compositionWidth, this.compositionHeight, originals.ToArray(), locations.ToArray()))
                {
                    painter.GenerateMovie = this.generateMovie;
                    painter.IntroMovieFrames = this.movieIntroFrames;
                    painter.MovieFilename = this.movieFilename;
                    DateTime start = DateTime.Now;
                    Bitmap res = painter.Paint();
                    System.Console.WriteLine("Painting took: " + (DateTime.Now - start).TotalSeconds + " seconds");
                    return res;
                }
            }
            finally
            {
                // We're done with this, but the bitmaps will be disposed of when this object
                // is disposed of
                originals.Clear();
                originals = null;
                locations.Clear();
                locations = null;
            }
        }
    }
}
