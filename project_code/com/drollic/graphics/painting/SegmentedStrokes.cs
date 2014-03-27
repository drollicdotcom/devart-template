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
using System.Drawing;
using System.Collections;
using System.Security.Permissions;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// Summary description for SegmentedRegion.
	/// </summary>
	public sealed class SegmentedStrokes
	{
		private System.Random rand = new System.Random();

		public Hashtable segments = new Hashtable();
		public int segWidth;
		public int segHeight;
		public int Width;
		public int Height;

		public int MaxXOffset = 0;
		public int MaxYOffset = 0;


		public SegmentedStrokes(int segWidth, int segHeight, int width, int height)
		{
			this.Width = width;
			this.Height = height;
			this.segWidth = segWidth;
			this.segHeight = segHeight;

			int xCount = 0, yCount = 0;
			for (int y=0; y < (height - segHeight) ; y+= segHeight)
			{
				for (int x=0; x < (width - segWidth); x+= segWidth)
				{
					StrokeCollection col = new StrokeCollection();
					col.strokes = new ArrayList();

					AddSegment(col, new SegmentOffset(xCount++, yCount));
				}

				xCount = 0;
				yCount++;
			}
		}

		public SegmentedStrokes(int segWidth, int segHeight, int width, int height, ArrayList strokes)
		{			
			this.Width = width;
			this.Height = height;
			this.segWidth = segWidth;
			this.segHeight = segHeight;

			int xCount = 0, yCount = 0;
			int endOfSegmentY = 0, endOfSegmentX = 0;
			int strokeCount = strokes.Count;

			for (int y=0; y < (height - segHeight) ; y+= segHeight)
			{
				endOfSegmentY = y + (segHeight - 1);

				for (int x=0; x < (width - segWidth); x+= segWidth)
				{
					endOfSegmentX = x + (segWidth - 1);

					StrokeCollection col = new StrokeCollection();
					col.strokes = new ArrayList();

					for (int i=0; i < strokes.Count; i++)
					{
						Stroke stroke = (Stroke)strokes[i];
						
						if ((stroke.xc >= x) && (stroke.xc <= endOfSegmentX) &&
							(stroke.yc >= y) && (stroke.yc <= endOfSegmentY))
						{
							//Console.WriteLine("Segmenting stroke, remaining: " + --strokeCount);
							col.strokes.Add(stroke);
						}
					}


					AddSegment(col, new SegmentOffset(xCount++, yCount));
				}

				xCount = 0;
				yCount++;
			}
		}


		public void AddSegment(StrokeCollection strokes, SegmentOffset offset)
		{
			this.segments[offset] = strokes;
			this.MaxXOffset = System.Math.Max(this.MaxXOffset, offset.xOffset);
			this.MaxYOffset = System.Math.Max(this.MaxYOffset, offset.yOffset);
		}

		public ArrayList UnifyStrokes()
		{
			ArrayList unifiedStrokes = new ArrayList();

			foreach (SegmentOffset offset in this.segments.Keys)
			{
				StrokeCollection col = (StrokeCollection)this.segments[offset];
				
				foreach (Stroke stroke in col.strokes)
				{
					//stroke.xc += col.strokeOffset.X;
					//stroke.yc += col.strokeOffset.Y;
					unifiedStrokes.Add(stroke);
				}
			}

			return unifiedStrokes;
		}

		public void Incorporate(SegmentedStrokes toBeIncorporated, SegmentOffset absoluteOffset)
		{			
			int randX = this.rand.Next(1, this.Width / 6);
			int randY = this.rand.Next(1, this.Height / 6);

			Point strokeOffset = new Point(randX + (absoluteOffset.xOffset * this.segWidth), randY + (absoluteOffset.yOffset * this.segHeight));

			foreach (SegmentOffset relativeOffset in toBeIncorporated.segments.Keys)
			{				
				SegmentOffset windowOffset = new SegmentOffset(absoluteOffset.xOffset + relativeOffset.xOffset, absoluteOffset.yOffset + relativeOffset.yOffset);
				StrokeCollection segment = (StrokeCollection)toBeIncorporated.segments[relativeOffset];
				segment.filled = true;
				segment.strokeOffset = strokeOffset;

				AddSegment(segment, windowOffset);
			}
		}

		public Bitmap GenerateBitmap()
		{
			Bitmap bmp = new Bitmap(this.Width, this.Height);

			foreach (SegmentOffset offset in this.segments.Keys)
			{
				ProcessingWindow segment = (ProcessingWindow)this.segments[offset];
				int maxY = segment.y + segment.Height;
				int maxX = segment.x + segment.Width;
				int destX = offset.xOffset * this.segWidth;
				int destY = offset.yOffset * this.segHeight;
				for (int y = segment.y; y < maxY; y++, destY++)
				{
					destX = offset.xOffset * this.segWidth; 

					for (int x = segment.x; x < maxX; x++, destX++)
					{						
						bmp.SetPixel(destX, destY, segment.sourceImage.GetColor(x, y));
					}
				}
			}

			return bmp;
		}

		public bool AttemptIncorporation(SegmentedStrokes toBeIncorporated)
		{
			for (int y=0; y < this.MaxYOffset; y++)
			{
				for (int x=0; x < this.MaxXOffset; x++)
				{
					SegmentOffset offset = new SegmentOffset(x, y);

					if (toBeIncorporated.FitsInto(this, offset))
					{
						this.Incorporate(toBeIncorporated, offset);

						//System.Console.WriteLine("Region fits at location: " + offset.xOffset * toBeIncorporated.segWidth + ", " + offset.yOffset * toBeIncorporated.segHeight);

						return true;
					}
				}
			}	
		
			return false;
		}

		public bool FitsInto(SegmentedStrokes destRegion, SegmentOffset destOffset)
		{
			// For every segment within this "source" region, check to see if 
			// each can be placed in its relative location in the destRegion.
			foreach (SegmentOffset offset in this.segments.Keys)
			{
				SegmentOffset adjusted = new SegmentOffset(destOffset.xOffset + offset.xOffset, destOffset.yOffset + offset.yOffset);
				if (destRegion.segments.ContainsKey(adjusted))
				{
					StrokeCollection segment = (StrokeCollection)destRegion.segments[adjusted];
					if (segment.filled)
						return false;
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public SegmentedStrokes AboveAverageSegments()
		{
			bool performAverage = false;

			if (!performAverage)
				return this;
			
			Hashtable scores = new Hashtable();
			int accumulator = 0;
			foreach (SegmentOffset offset in this.segments.Keys)
			{
				StrokeCollection strokeCol = (StrokeCollection)this.segments[offset];
				int segmentScore = strokeCol.strokes.Count;

				accumulator += segmentScore;

				ArrayList list = null;
				if (scores.ContainsKey(segmentScore))
					list = (ArrayList)scores[segmentScore];
				else
				{
					list = new ArrayList();
					scores[segmentScore] = list;
				}

				OffsetStrokesPair pair = new OffsetStrokesPair();
				pair.offset = offset;
				pair.strokeCollection = strokeCol;
				list.Add(pair);					
			}
				
			// Determine the average across all segments
			double average = 0.0;
			if (this.segments.Keys.Count > 0)
				average = accumulator / this.segments.Keys.Count;

			SegmentedStrokes result = new SegmentedStrokes(this.segWidth, this.segHeight, this.Width, this.Height);
			foreach (int score in scores.Keys)
			{
				if (score > (average / 2.0))
				{
					ArrayList list = (ArrayList)scores[score];
					foreach (OffsetStrokesPair pair in list)
					{
						result.AddSegment(pair.strokeCollection, pair.offset);						
					}
				}
			}

			return result;
		}
	}

	public struct OffsetStrokesPair
	{
		public SegmentOffset offset;
		public StrokeCollection strokeCollection;
	}

	public struct StrokeCollection
	{
		public ArrayList strokes;
		public Point strokeOffset;
		public bool filled;
	}
}
