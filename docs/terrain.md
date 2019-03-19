# Terrain Creation in Blender

1. Create a Grid of the desired size where each subdivision is roughly 2 square metres.

1. Delete some faces around the edge to achieve the rough desired shape.

1. Enable proportional editing with smooth falloff.

1. Drag the outer vertices around to make it look more natural.

1. Enable proportional editing with random falloff.

1. Displace some vertices vertically.

1. Select all faces and scale in Z-axis by an appropriate amount.

1. Apply a Triangulate modifier with the Beauty setting.

1. Extrude downwards, and delete the bottom faces.

1. Create an edge loop around the sides and delete the lower half to give the bottom a clean edge.

1. Mark the top edges as sharp (?).

1. Recalculate normals.

1. Create a seam separating the top from the sides.

1. UV unwrap and texture.

1. When importing into Unity, it may be necessary to lower the "Scale in Lightmap" setting.
