The Dreamer is a software application which createds abstract paintings through user-supplied text -- but how does it work?  The process can be broken down into 5 steps.  This post will focus the first 3 steps in this process.

<b>STEP #1: HUMAN INPUT</b>

Everything starts with some input text from a person.  It can be anything.  I’ve found that even text such as “!!!” can produce interesting results.  But a lot of the fun of Dreamer is to input text that meaningful to you – whether it’s a saying, quote, or something that just popped into your head.  Seeing what The Dreamer will come up with is just fun.



<b>STEP #2: GATHERING AND SELECTING SOURCE IMAGES</b>

Obviously, the Dreamer isn't creating the abstract paintings from scratch.  Instead, it's using several source images that will serve as a the basis for the painting.  

Using the text input, Dreamer uses image search engines to find matching images.  Currently, Dreamer can make use of both Google Custom Search (via the .NET API) and FlickR, via their REST interface.  Dreamer selects a handful of these images at random to be used in the process.  What I find particularly interesting is that the source images are somehow related to the input text, hence their showing up in the search results.  This can sometimes result in the final paintings appearing to have some meaningful representation to the input text.  



<b>STEP #3: IMAGE ANALYSIS & GENERATION OF STROKES</b>

With the source images chosen, each is individually analyzed by a painterly algorithm which was described by Shiraishi [1].  This analysis, in a nutshell, is comprised of two steps:  a dithering, followed by brush stroke generation.  Here's an example input image, along with its dithered result:

![Original](../project_images/bigtiger.jpg?raw=true "Original")
![Dithered](../project_images/dithered.jpg?raw=true "Dithered")

As you can see, the dithered image consists of a large number of "dots."  These dots essentially tell the algorithm where to do some analysis and create a brush stroke. 

The result of this algorithm is a collection of painting strokes which completely describe how to "paint" the original source image. Each painting stroke is comprised of a length, width, rotation, position, and color attribute. 

Here's the painting of our sample image:

![Painting](../project_images/sample_painting.jpg?raw=true "Painting")


So far, however, all these strokes are grouped by their original source image.  If they were rendered at this point, the details of the first couple of pictures would be entirely lost as they would be covered up by the large background strokes of the pictures to be rendered last.  What's needed is a way to combine and mix up all the strokes so that the details of each painting has a chance to be present in the final work.  

References:
[1]     Shiraishi, M.  Adaptive parameter control for image moment-based painterly rendering. In <I>Proceedings of the9th International Conference on Geometry and Graphics</I>, 2000.

