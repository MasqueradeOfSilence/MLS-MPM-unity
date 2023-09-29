# Basic logic for 2D water surfacing

Basic principle: I will be generating a mesh using [this article for terrain generation](https://straypixels.net/delaunay-triangulation-terrain/), minus the Perlin noise and anything beyond that, because adding elevation to a vertical layer of water would look bizarre. 

Thankfully, there are existing libraries for this, which should save me from having to implement Delaunay triangulation manually both for surfacing and computing the bubble contact surface mesh in the next step. 

## Tentative steps (based on the linked article):
1. Add the Triangle.NET functionality to Unity. Note, the original page is a dead link, but it should be doable using [this repo I found](https://github.com/Nox7atra/Triangle.Net-for-Unity) and [forked too](https://github.com/MasqueradeOfSilence/Triangle.Net-for-Unity) because I am paranoid. 
2. Create a Polygon object. But instead of filling it with random points like they do in the above article, we can use our fluid points. 
3. Triangulate the points using the built-in functionality. 
4. Visualize it using Unity gizmos. 
5. Skip the terrain height computations since that isn't applicable for us. Instead, code up `makeMesh()` and `GetPoint3D()` with the latter fixing z-positions for now (until we expand it to 3D). 

At this point, we should have a water mesh. I don't think it will be efficient to compute this at every point, so we might need to do it only on even frames for example. 