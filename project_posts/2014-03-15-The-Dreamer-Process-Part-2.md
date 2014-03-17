This post will focus on the final 2 steps of the Dreamer process.  Please see the previous post for steps 1-3 in the process.

<b>STEP 4: STROKE COMBINATION AND ORDERING</b>

With all images having been analyzed by the painterly algorithm, Dreamer now has N disjoint collections of painting stroke parameters, where N is the total number of images analyzed. This constitutes all the information needed by Dreamer to render the final artifact.  Again, rendering these collections individually would result in a very poor blending of the images. 

To achieve a more interwoven and interesting blending, the N disjoint collections are merged into a single unified collection. This unified collection is then sorted according to the area of each individual stroke parameter. The strokes with the largest area, representing the big background strokes of a painting, will then be rendered first. The strokes with the smallest area, representing the finer details, will then be the last to be rendered, thus attempting to preserve those finer details for all source images.

![Stroke Render Order](../project_images/render_orger.png?raw=true "Stroke Render Order")



<b>STEP 5: RENDERING </b>

With a unified and sorted stroke collection, Dreamer can now begin the rendering process.  So how does it do that?  Again, each stroke in the collection is comprised of the following parameters:

- Length
- Width
- Color
- Rotation
- Location

For every stroke in the collection, Dreamer uses these parameters to first create an isolated stroke which is ready to be blended into the final image.  For starters, it begins by choosing, at random, one of the available brush stroke images.  What are these brush stroke images?  Here are a few examples of what Dreamer uses:

![Sample Brush Stroke 1](../project_images/b1.jpg?raw=true "Sample Brush Stroke 1")

![Sample Brush Stroke 2](../project_images/brush1.jpg?raw=true "Sample Brush Stroke 2")

![Sample Brush Stroke 3](../project_images/brush2.jpg?raw=true "Sample Brush Stroke 3")

![Sample Brush Stroke 4](../project_images/brush4.jpg?raw=true "Sample Brush Stroke 4")

![Sample Brush Stroke 5](../project_images/brush6.jpg?raw=true "Sample Brush Stroke 5")


With the brush stroke image chosen, Dreamer then rotates and scales the stroke image according to the parameters.  Once ready, this brush image can then be blended into the final painting as it currently exists.  This is done by essentially blending each non-black pixel value from the brush stroke image, with every corresponding pixel value in the final painting.  This is also done at the location parameter contained within the stroke.  The new blended pixel value then replaces the original pixel value in the final painting.

And here's an example of what the final result looks like:

![Sample Dream](../project_images/dreams/turbid.png?raw=true "Sample Dream")
