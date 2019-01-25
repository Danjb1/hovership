# Importing Models into Unity

## File Format

While Unity can import *.blend* files, internally it always uses *.fbx* files, so it makes sense for us to export these directly. This gives us more control over the conversion process.

## Blender Tips

 - Remove the default light and camera, they are not needed in Unity.

 - If the object should rest on the floor, move it so that it sits atop the grid.

## Co-ordinate Systems

Blender uses a Z-up co-ordinate system while Unity uses a Y-up co-ordinate system, resulting in models being rotated by 90 degrees after being imported into Unity.

This can be fixed one of two ways:

### 1. Apply rotation in Blender

#### Convert to Unity co-ordinates

 - **A** (select all)
 - **R + X, -90** (rotate -90 degrees in x-axis)
 - **Ctrl + A** (apply transform)
 - **R** (apply rotate transform)
 - Now **Export**!

#### Convert to Blender co-ordinates

 - **A** (select all)
 - **R + X, 90** (rotate 90 degrees in x-axis)
 - **Ctrl + A** (apply transform)
 - **R** (apply rotate transform)

### 2. Rotate during export

When exporting to FBX, use the following options:

 - **Forward:** -Z Forward
 - **Up:** Y up
 - **TICK** "!EXPERIMENTAL! Apply Transform"

## Scale

Sometimes when importing meshes into Unity, the scale is off by a factor of 100. To fix this, untick "Use File Scale" in the import settings and enter a Scale Factor of 1.
