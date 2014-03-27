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
using System.Security.Permissions;

namespace com.drollic.net.search.image
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
    /// 
	public abstract class ImageUrlProvider
	{
        private Random randomEngine = new Random();

        public abstract int TotalDesiredImages
        {
            get;
            set;
        }

        public abstract String Name
		{
			get;
		}


        protected abstract List<String> DiscoverImageUrls(String search);


        protected String pruneSearch(String searchText)
        {
            String lowerText = searchText.ToLower();
            String[] terms = lowerText.Split(new char[] { ' ', ',', ':', ';', '-' });
            List<String> parsed = new List<String>();
            int termsRemoved = 0;

            // First, eliminate obvious terms             
            foreach (String term in terms)
            {
                if (term.Equals("and") || term.Equals("but") || term.Equals("or") ||
                    term.Equals("the") || term.Equals("a"))
                {
                    termsRemoved++;
                    continue;
                }

                parsed.Add(term);
            }

            // Remove a random word
            int toBeRemoved = randomEngine.Next(0, parsed.Count - 1);
            parsed.RemoveAt(toBeRemoved);

            // Create a new search string
            String result = "";
            foreach (String term in parsed)
            {
                result += term;
                result += " ";
            }

            return result;
        }


        public List<String> Search(String searchText)
        {
            return DiscoverImageUrls(searchText);
        }
    }
}
