using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using com.drollic.graphics.painting;
using com.drollic.util;


namespace com.drollic.app.dreamer.core
{
    public sealed class UserSpecifiedComposition : Composition
    {
        public UserSpecifiedComposition(int width, int height, int totalSubjects, String searchText, UrlProvider urlProvider)
            : base(width, height, totalSubjects, searchText, urlProvider)
        {
            
        }

        private UserSpecifiedComposition(int width, int height)
            : base(width, height, 1, "", UrlProvider.Yahoo)
        {
        }

        public static Bitmap Test(int width, int height, Bitmap testImage)
        {
            UserSpecifiedComposition c = new UserSpecifiedComposition(width, height);
            c.subjects.Add(new Subject("http://www.google.com", testImage));
            StatusManager.Instance.StatusMessage("Sketching...");
            c.CreateSketch();
            StatusManager.Instance.StatusMessage("Sketching completed.");
            return c.CreateComposition();
        }

        protected override void CreateSketch()
        {
            // For this class, the sketch is set by the interactive form, so there's
            // nothing to instantiate.
            
            // Generate the "sketch"
            if (sketch != null)
            {
                sketch.Generate();
            }
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
                    return painter.Paint();
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
