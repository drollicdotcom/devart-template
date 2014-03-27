using System;
using System.Collections;
using System.Drawing;
using System.Security.Permissions;

using com.drollic.util;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// Summary description for SimpleComposer.
	/// </summary>
	public class SimpleComposer
	{
		private ArrayList resizedImages = new ArrayList();
		private int maxImageWidth = 0;
		private int maxImageHeight = 0;
		//private ColorImage composition = null;
		private Bitmap paintingBitmap = null;

		public SimpleComposer(ArrayList images, Bitmap canvas)
		{	
			this.paintingBitmap = canvas;
			//this.composition = new ColorImage(canvas);

			double divisor = 1;
			if (images.Count == 1) divisor = 1;
			if (images.Count < 5) divisor = 1.5;
			else divisor = 1.75;

			this.maxImageWidth = (int)((double)canvas.Width / divisor);
			this.maxImageHeight = (int)((double)canvas.Height / divisor);
	
			foreach (Bitmap image in images)
			{
				if (image != null)
				{
					resizedImages.Add(ImageTools.ResizeImage(image, this.maxImageWidth, this.maxImageHeight));
				}
			}
		}

		public Bitmap Compose()
		{	
			Bitmap strokeBitmap = new Bitmap(Properties.Resources.brush1);
			
			int counter = 1;
			//int totalTasks = resizedImages.Count * 3 + 1;
			//StatusReporter.Instance.SetTotalTasks(totalTasks);
			int taskCounter = 1;

			DateTime start = DateTime.Now;

			SegmentedStrokes finalRegion = new SegmentedStrokes(10, 10, this.paintingBitmap.Width, this.paintingBitmap.Height);

			ArrayList strokes = null;
			foreach (Bitmap image in resizedImages)
			{
				MomentsBasedPainter painter = new MomentsBasedPainter(15, strokeBitmap);
				strokes = painter.GenerateStrokes(image);

				SegmentedStrokes segStrokes = new SegmentedStrokes(10, 10, image.Width, image.Height, strokes);
				SegmentedStrokes interesting = segStrokes.AboveAverageSegments();

				//Bitmap results = interesting.GenerateBitmap();
				//ImageTools.GenWorkingImage(results);
				//image.Save("downloaded-" + counter + ".jpg");
				//results.Save("average-" + counter + ".jpg");
				counter++;

				finalRegion.AttemptIncorporation(interesting);	
			}

			ArrayList finalStrokes = finalRegion.UnifyStrokes();
			finalStrokes.Sort();
			Console.WriteLine("Final total strokes: " + finalStrokes.Count);

			IPainter p = new MomentsBasedPainter(15, strokeBitmap);
			Bitmap final = p.Render(finalStrokes, this.paintingBitmap);
			
			//ImageTools.GenWorkingImage(final);

			TimeSpan duration = DateTime.Now - start;				

			final.Save("composition.jpg");

			Console.WriteLine("Composition completed in " + duration.TotalMilliseconds + "ms");

			return final;
		}
	}
}