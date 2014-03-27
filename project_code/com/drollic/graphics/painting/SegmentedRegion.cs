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
using System.Collections.Generic;
using System.Security.Permissions;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// Summary description for SegmentedRegion.
	/// </summary>
	public sealed class SegmentedRegion
	{
        // RSM: CHANGED THIS TO A DICTIONARY!!!
        //public Hashtable segments = new Hashtable();
        public Dictionary<SegmentOffset, List<ProcessingWindow>> segments = new Dictionary<SegmentOffset, List<ProcessingWindow>>();
		public int segWidth;
		public int segHeight;
		public int Width;
		public int Height;

		public int MaxXOffset = 0;
		public int MaxYOffset = 0;


		public SegmentedRegion(int segWidth, int segHeight, int width, int height)
		{
			this.Width = width;
			this.Height = height;
			this.segWidth = segWidth;
			this.segHeight = segHeight;
		}

		public SegmentedRegion(int segWidth, int segHeight, ColorImage imageToSegment)
		{			
			this.Width = imageToSegment.Width;
			this.Height = imageToSegment.Height;
			this.segWidth = segWidth;
			this.segHeight = segHeight;

			int xCount = 0, yCount = 0;
			for (int y=0; y < (imageToSegment.Height - segHeight) ; y+= segHeight)
			{
				for (int x=0; x < (imageToSegment.Width - segWidth); x+= segWidth)
				{
					AddSegment(new ProcessingWindow(x, y, segWidth, segHeight, imageToSegment), new SegmentOffset(xCount++, yCount));
				}

				xCount = 0;
				yCount++;
			}
		}


		public void AddSegment(ProcessingWindow segment, SegmentOffset offset)
		{
			if (segment == null)
				return;

            if (! this.segments.ContainsKey(offset))
            {
                this.segments.Add(offset, new List<ProcessingWindow>());
            }

            this.segments[offset].Add(segment);

			this.MaxXOffset = System.Math.Max(this.MaxXOffset, offset.xOffset);
			this.MaxYOffset = System.Math.Max(this.MaxYOffset, offset.yOffset);
		}


		public void Incorporate(SegmentedRegion toBeIncorporated, SegmentOffset absoluteOffset)
		{
			foreach (SegmentOffset relativeOffset in toBeIncorporated.segments.Keys)
			{
				SegmentOffset windowOffset = new SegmentOffset(absoluteOffset.xOffset + relativeOffset.xOffset, absoluteOffset.yOffset + relativeOffset.yOffset);

                // RSM: Changed to support dictionary segments!!!
				//ProcessingWindow segment = (ProcessingWindow)toBeIncorporated.segments[relativeOffset];
				//segment.filled = true;
				//AddSegment(segment, windowOffset);

                foreach (ProcessingWindow segment in toBeIncorporated.segments[relativeOffset])
                {
                    segment.filled = true;
                    AddSegment(segment, windowOffset);
                }

			}
		}

		public Bitmap GenerateBitmap()
		{
			Bitmap bmp = new Bitmap(this.Width, this.Height);

            /// RSM: Commented out the following to get Dictionary working!!!
            /*
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
						//bmp.SetPixel(destX, destY, segment.sourceImage.data[y][x]);
						bmp.SetPixel(destX, destY, segment.sourceImage.GetColor(x, y));
					}
				}
			}
            */

			return bmp;
		}

		public bool AttemptIncorporation(SegmentedRegion toBeIncorporated)
		{
            System.Random rand = new Random();

            double percentageCollisionsToAllow = 0.10 + (rand.Next(0, 3) / 100.0);
            int acceptableCollisions = (int)(toBeIncorporated.segments.Count * percentageCollisionsToAllow); // random segment segment collision is OK

			for (int y=0; y < this.MaxYOffset; y++)
			{
				for (int x=0; x < this.MaxXOffset; x++)
				{
					SegmentOffset offset = new SegmentOffset(x, y);

                    if (toBeIncorporated.FitsInto(this, offset, acceptableCollisions))
					{
						this.Incorporate(toBeIncorporated, offset);

						//System.Console.WriteLine("Region fits at location: " + offset.xOffset * toBeIncorporated.segWidth + ", " + offset.yOffset * toBeIncorporated.segHeight);

						return true;
					}
				}
			}	
		
			return false;
		}

		public bool FitsInto(SegmentedRegion destRegion, SegmentOffset destOffset, int acceptableCollisions)
		{
            int collisions = 0;

			// For every segment within this "source" region, check to see if 
			// each can be placed in its relative location in the destRegion.
			foreach (SegmentOffset offset in this.segments.Keys)
			{
				SegmentOffset adjusted = new SegmentOffset(destOffset.xOffset + offset.xOffset, destOffset.yOffset + offset.yOffset);
				if (destRegion.segments.ContainsKey(adjusted))
				{
					ProcessingWindow segment = (ProcessingWindow)destRegion.segments[adjusted][0];
                    if (segment.filled)
                    {
                        if (++collisions >= acceptableCollisions)
                        {
                            return false;
                        }
                    }
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public SegmentedRegion AboveAverageSegments()
		{
			Hashtable scores = new Hashtable();
			int accumulator = 0;
			foreach (SegmentOffset offset in this.segments.Keys)
			{
				ProcessingWindow segment = (ProcessingWindow)this.segments[offset][0];

				int maxY = segment.y + segment.Height;
				int maxX = segment.x + segment.Width;
				int segmentScore = 0;
				for (int y = segment.y; y < maxY; y++)
				{
					for (int x = segment.x; x < maxX; x++)
					{
						//if (segment.sourceImage.data[y][x].R == 0)
						if (segment.sourceImage.GetColor(x, y).R == 0)
							segmentScore++;
					}
				}

				accumulator += segmentScore;

				ArrayList list = null;
				if (scores.ContainsKey(segmentScore))
					list = (ArrayList)scores[segmentScore];
				else
				{
					list = new ArrayList();
					scores[segmentScore] = list;
				}

				OffsetSegmentPair pair = new OffsetSegmentPair();
				pair.offset = offset;
				pair.segment = segment;
				list.Add(pair);					
			}
				
			// Determine the average across all segments
			double average = accumulator / this.segments.Keys.Count;

			SegmentedRegion result = new SegmentedRegion(this.segWidth, this.segHeight, this.Width, this.Height);
			foreach (int score in scores.Keys)
			{
				if (score > average)
				{
					ArrayList list = (ArrayList)scores[score];
					foreach (OffsetSegmentPair pair in list)
					{
						result.AddSegment(pair.segment, pair.offset);						
					}
				}
			}

			return result;
		}
	}


	public struct SegmentOffset
	{
		private int XOffset;
		private int YOffset;
		private int hashCode;

		public SegmentOffset(int x, int y)
		{
			this.XOffset = x;
			this.YOffset = y;
			this.hashCode = this.XOffset << 15 | this.YOffset;
		}

		public int xOffset
		{
			get
			{
				return this.XOffset;
			}
		}

		public int yOffset
		{
			get
			{
				return this.YOffset;
			}
		}

		public override int GetHashCode()
		{			
			//return this.xOffset ^ this.yOffset;
			//return base.GetHashCode();
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			SegmentOffset right = (SegmentOffset)obj;
			return ((this.xOffset == right.xOffset) && (this.yOffset == right.yOffset));
			//return base.Equals(obj);
		}
	}

	public struct OffsetSegmentPair
	{
		public SegmentOffset offset;
		public ProcessingWindow segment;
	}
}
