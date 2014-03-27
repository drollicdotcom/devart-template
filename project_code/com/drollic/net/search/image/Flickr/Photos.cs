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
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;


namespace com.drollic.net.search.image
{
	/// <summary>
	/// Which photo search extras to be included. Can be combined to include more than one
	/// value.
	/// </summary>
	/// <example>
	/// The following code sets options to return both the license and owner name along with
	/// the other search results.
	/// <code>
	/// PhotoSearchOptions options = new PhotoSearchOptions();
	/// options.Extras = PhotoSearchExtras.License &amp; PhotoSearchExtras.OwnerName
	/// </code>
	/// </example>
	[Flags]
	public enum PhotoSearchExtras
	{
		/// <summary>
		/// No extras selected.
		/// </summary>
		None = 0,
		/// <summary>
		/// Returns a license.
		/// </summary>
		License = 1,
		/// <summary>
		/// Returned the date the photos was uploaded.
		/// </summary>
		DateUploaded = 2,
		/// <summary>
		/// Returned the date the photo was taken.
		/// </summary>
		DateTaken = 4,
		/// <summary>
		/// Returns the name of the owner of the photo.
		/// </summary>
		OwnerName = 8,
		/// <summary>
		/// Returns the server for the buddy icon for this user.
		/// </summary>
		IconServer = 16,
		/// <summary>
		/// Returns the extension for the original format of this photo.
		/// </summary>
		OriginalFormat = 32,
		/// <summary>
		/// Returns the date the photo was last updated.
		/// </summary>
		LastUpdated = 64,
		/// <summary>
		/// Returns all the above information.
		/// </summary>
		All = License | DateUploaded | DateTaken | OwnerName | IconServer | OriginalFormat | LastUpdated
	}

	/// <remarks/>
	[Serializable]
	public sealed class Photos 
	{
    
		/// <remarks/>
		[XmlElement("photo", Form=XmlSchemaForm.Unqualified)]
        public List<Photo> PhotoCollection = new List<Photo>();
    
		/// <remarks/>
		[XmlAttribute("page", Form=XmlSchemaForm.Unqualified)]
		public long PageNumber;
    
		/// <remarks/>
		[XmlAttribute("pages", Form=XmlSchemaForm.Unqualified)]
		public long TotalPages;
    
		/// <remarks/>
		[XmlAttribute("perpage", Form=XmlSchemaForm.Unqualified)]
		public long PhotosPerPage;
    
		/// <remarks/>
		[XmlAttribute("total", Form=XmlSchemaForm.Unqualified)]
		public long TotalPhotos;
	}

	/// <remarks/>
	[System.Serializable]
	public sealed class Photo 
	{
    
		/// <remarks/>
		[XmlAttribute("id", Form=XmlSchemaForm.Unqualified)]
		public string PhotoId;
    
		/// <remarks/>
		[XmlAttribute("owner", Form=XmlSchemaForm.Unqualified)]
		public string UserId;
    
		/// <remarks/>
		[XmlAttribute("secret", Form=XmlSchemaForm.Unqualified)]
		public string Secret;
    
		/// <remarks/>
		[XmlAttribute("server", Form=XmlSchemaForm.Unqualified)]
		public string Server;
    
		/// <remarks/>
		[XmlAttribute("title", Form=XmlSchemaForm.Unqualified)]
		public string Title;
    
		/// <remarks/>
		[XmlAttribute("ispublic", Form=XmlSchemaForm.Unqualified)]
		public int IsPublic;
    
		/// <remarks/>
		[XmlAttribute("isfriend", Form=XmlSchemaForm.Unqualified)]
		public int IsFriend;
    
		/// <remarks/>
		[XmlAttribute("isfamily", Form=XmlSchemaForm.Unqualified)]
		public int IsFamily;

		/// <remarks/>
		[XmlAttribute("isprimary", Form=XmlSchemaForm.Unqualified)]
		public int IsPrimary;

		/// <remarks/>
		[XmlAttribute("license", Form=XmlSchemaForm.Unqualified)]
		public string License;

		/// <remarks/>
		[XmlAttribute("dateupload", Form=XmlSchemaForm.Unqualified)]
		public string dateupload_raw;

		/// <summary>
		/// Converts the raw dateupload field to a <see cref="DateTime"/>.
		/// </summary>
		[XmlIgnore]
		public DateTime DateUploaded
		{
			get { return Utils.UnixTimestampToDate(dateupload_raw); }
		}

		/// <summary>
		/// Converts the raw lastupdate field to a <see cref="DateTime"/>.
		/// </summary>
		[XmlIgnore]
		public DateTime LastUpdated
		{
			get { return Utils.UnixTimestampToDate(lastupdate_raw); }
		}

		/// <remarks/>
		[XmlAttribute("lastupdate", Form=XmlSchemaForm.Unqualified)]
		public string lastupdate_raw;

		/// <remarks/>
		[XmlAttribute("datetaken", Form=XmlSchemaForm.Unqualified)]
		public string datetaken_raw;

		/// <summary>
		/// Converts the raw datetaken field to a <see cref="DateTime"/>.
		/// </summary>
		[XmlIgnore]
		public DateTime DateTaken
		{
			get { return System.DateTime.Parse(datetaken_raw); }
		}

		/// <remarks/>
		[XmlAttribute("ownername", Form=XmlSchemaForm.Unqualified)]
		public string OwnerName;

		/// <remarks/>
		[XmlAttribute("iconserver", Form=XmlSchemaForm.Unqualified)]
		public string IconServer;

		/// <summary>
		/// Optional extra field containing the original format (jpg, png etc) of the 
		/// photo.
		/// </summary>
		[XmlAttribute("originalformat", Form=XmlSchemaForm.Unqualified)]
		public string OriginalFormat;

		private const string photoUrl = "http://static.flickr.com/{0}/{1}_{2}{3}.{4}";

		/// <summary>
		/// The url to the web page for this photo. Uses the users userId, not their web alias, but
		/// will still work.
		/// </summary>
		[XmlIgnore()]
		public string WebUrl
		{
			get { return string.Format("http://www.flickr.com/photos/{0}/{1}/", UserId, PhotoId); }
		}

		/// <summary>
		/// The URL for the square thumbnail of a photo.
		/// </summary>
		[XmlIgnore()]
		public string SquareThumbnailUrl
		{
			get { return string.Format(photoUrl, Server, PhotoId, Secret, "_s", "jpg"); }
		}

		/// <summary>
		/// The URL for the thumbnail of a photo.
		/// </summary>
		[XmlIgnore()]
		public string ThumbnailUrl
		{
			get { return string.Format(photoUrl, Server, PhotoId, Secret, "_t", "jpg"); }
		}

		/// <summary>
		/// The URL for the small copy of a photo.
		/// </summary>
		[XmlIgnore()]
		public string SmallUrl
		{
			get { return string.Format(photoUrl, Server, PhotoId, Secret, "_m", "jpg"); }
		}

		/// <summary>
		/// The URL for the medium copy of a photo.
		/// </summary>
		/// <remarks>There is a chance that extremely small images will not have a medium copy.
		/// Use <see cref="Flickr.PhotosGetSizes"/> to get the available URLs for a photo.</remarks>
		[XmlIgnore()]
		public string MediumUrl
		{
			get { return string.Format(photoUrl, Server, PhotoId, Secret, "", "jpg"); }
		}

		/// <summary>
		/// The URL for the large copy of a photo.
		/// </summary>
		/// <remarks>There is a chance that small images will not have a large copy.
		/// Use <see cref="Flickr.PhotosGetSizes"/> to get the available URLs for a photo.</remarks>
		[XmlIgnore()]
		public string LargeUrl
		{
			get { return string.Format(photoUrl, Server, PhotoId, Secret, "_b", "jpg"); }
		}

		/// <summary>
		/// If <see cref="OriginalFormat"/> was returned then this will contain the url of the original file.
		/// </summary>
		[XmlIgnore()]
		public string OriginalUrl
		{
			get 
			{ 
				if( OriginalFormat == null || OriginalFormat.Length == 0 )
					throw new InvalidOperationException("No original format information available.");

				return string.Format(photoUrl, Server, PhotoId, Secret, "_o", OriginalFormat);
			}
		}
	}

	/// <summary>
	/// Permissions for the selected photo.
	/// </summary>
	[System.Serializable]
	public sealed class PhotoPermissions
	{
		/// <remarks/>
		[XmlAttribute("id", Form=XmlSchemaForm.Unqualified)]
		public string PhotoId;

		/// <remarks/>
		[XmlAttribute("ispublic", Form=XmlSchemaForm.Unqualified)]
		public int IsPublic;
    
		/// <remarks/>
		[XmlAttribute("isfriend", Form=XmlSchemaForm.Unqualified)]
		public int IsFriend;
    
		/// <remarks/>
		[XmlAttribute("isfamily", Form=XmlSchemaForm.Unqualified)]
		public int IsFamily;
	}
}