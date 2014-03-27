using System;
using System.Drawing;

namespace com.raymatthieu.graphics.painting
{
	/// <summary>
	/// Summary description for TestCenter.
	/// </summary>
	public class TestCenter
	{
		public TestCenter()
		{
		}

		public void ConductStrokeAreaTest()
		{
			MomentsBasedPainter painter = new MomentsBasedPainter(15, "brush.jpg");
			DateTime start = DateTime.Now;			
			painter.GenerateStrokeArea(new Bitmap("canoe.jpg"), 15);
			TimeSpan duration = DateTime.Now - start;
			Console.WriteLine("Managed generation completed in " + duration.TotalMilliseconds + "ms");

			start = DateTime.Now;			
			//ProcessingLib.StrokeAreaGenerator.Generate(new Bitmap("canoe.jpg"), 15);
			duration = DateTime.Now - start;
			Console.WriteLine("Raw generation completed in " + duration.TotalMilliseconds + "ms");

			Console.WriteLine("Test completed");
		}

        public void ConductPaintingTest()
        {
            IImageProvider provider = new YahooSearchService();
            provider.TotalDesiredImages = this.totalSubjects;
            ArrayList images = provider.Find(this.textBox1.Text);

            SimpleComposer composer = new SimpleComposer(images, this.compositionCanvas);
            Bitmap composition = composer.Compose();
        }

		public void ConductScalingTest()
		{
			Bitmap area = new Bitmap("test.jpg");
			int factor = 2;

			// Scale with GDI+
			DateTime start = DateTime.Now;
			Bitmap areascaled = new Bitmap(area.Width / factor, area.Height / factor);
			Graphics g = Graphics.FromImage(areascaled);   
			g.ScaleTransform(1.0f / (float)factor, 1.0f / (float)factor);   
			g.DrawImage(area, 0, 0, area.Width, area.Height);  
			TimeSpan duration = DateTime.Now - start;
			Console.WriteLine("GDI Scaling completed in " + duration.TotalMilliseconds + "ms");
			areascaled.Save("gdi-scale-test" + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);				

			start = DateTime.Now;
			ColorImage areaCI = new ColorImage(new UnsafeBitmap(area));
			ColorImage widthScaled = ColorImage.ScaleWidth(areaCI, area.Width / factor);
			ColorImage fullyScaled = ColorImage.ScaleHeight(widthScaled, area.Height / factor);
			duration = DateTime.Now - start;
			Console.WriteLine("ColorImage Scaling completed in " + duration.TotalMilliseconds + "ms");
			fullyScaled.GenerateBitmap().Save("colorimage-scale-test.jpg" + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);				
		}
	}
}
