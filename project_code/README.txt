THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

No Support. Nothing in this Agreement shall obligate the authors to provide any support for the 
Software. 



REQUIREMENTS TO BUILD:
 - Visual Studio 2012 for Desktop (express works fine)
 - NuGet Package Manager v2.7 or later (sorry, this was required by the Google Custom Search .NET library).

REQUIREMENTS TO RUN:
 - Your own FlickR and Google Custom Search API keys.


STEPS TO BUILD:
1. CD into com/drollic/app/dreamerservice
2. Open com.drollic.app.dreamerservice.sln
3. In VS, use menu to navigate to View -> Other Windows -> Package Manager Console
4. In Package Manager console, you should see a "Restore" button in the right hand side of the window.  Click it.
5. Once Package restoration is complete, you can now rebuild the solution
6. You will notice many build errors.  This is because you need to provide your own API keys for both FlickR
and Google.  Where you see errors like the following below, replace with a string containing your own API key:

The name 'REPLACE_WITH_YOUR_API_KEY' does not exist in the current context

7. Rebuild again and run the app.


Questions can be directed to raysblog@gmail.com.