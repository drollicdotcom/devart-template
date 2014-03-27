using System;
using System.Drawing;
using System.Security.Permissions;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// Summary description for IDitheringAlgorithm.
	/// </summary>
	public interface IDitheringAlgorithm
	{
		Bitmap Dither(ColorImage inImage);
	}
}
