# Terrain Creation in Blender

## Create a Plane

1. Create a Grid of the desired size where each subdivision is roughly 2 square metres.

1. Delete some faces around the edge to achieve the rough desired shape.

1. Use the knife tool to "triangulate" the edges.

1. Enable proportional editing with smooth / sharp falloff.

1. Drag the outer vertices around to make it look more natural.

1. Apply a Triangulate modifier with the Beauty setting.

## Create the Edges

1. Extrude downwards, and delete the bottom faces.

1. Create an edge loop around the sides and scale it down, as well as the bottom edge loop.

1. Create a seam separating the top from the sides.

1. Lower the edges along this seam a little so that the land slopes downwards at the edges.

## Roughen

1. Enable proportional editing with random falloff.

1. Displace some vertices vertically.

1. Select all faces and scale in Z-axis by an appropriate amount.

## Finishing Touches

1. Mark the top edges as sharp (?).

1. Recalculate normals.

1. UV unwrap and texture.

## Import into Unity

1. Add the mesh into the scene.

1. Mark it as static.

1. Select the appropriate texture for the material.

1. It may be necessary to lower the "Scale in Lightmap" setting, if Unity shows a warning.

1. Add a Mesh Collider component.
