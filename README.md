# SigilDoorCity
Unity Test for streaming open world on a non-euclidean surface

My attempt to program an open, streaming world, modeled after Planescape's City, Sigil.

## TODO
* Get Chunk Streaming working.  
* Get chunk streaming with non-eucledan looping.  Player should be able to walk in loops along Y-axis but not x-axis.
* Get curved world shader working on streamed-in chunks.  Have world curve upward in all directions.
* Draw LOD Sigil standin.  Use either skybox or some other funky think to draw inside of a tire.
* Generate LOD Mesh composed of all or sections of all chunks.  I need to be able to generate inside of a tire mesh for make LOD sigil.
* Match curvature of shader with actual curvature of distant LOD.
