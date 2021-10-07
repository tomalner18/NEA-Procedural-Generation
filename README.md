# NEA-Procedural-Generation
An investigation in the potential uses of procedural generation in games to mimic the randomness of the natural world.

# Overview
## Landmass Generation
By overlaying Perlin Noise with varying parameters alongside a falloff map, we can extract a mesh from the 2D image 
that strongly resembles an earthly island. With some simple shaders and finetuning, the resulting terrain looks realistic (at 
least for a free game). From there we can use an object placement algorithm (Poisson Disc Sampling) to fill our world with flora,
and as long as we sample the height of our mesh, we can make sure we don't place a tree atop a mountain, or in the ocean!

# Further Details
Full project details can be found in the [Project Report](Project Report.pdf). This details the project at all stages 
of its lifecycle providing my personal insights and reflection at each stage.

Unity project access available on request :)
