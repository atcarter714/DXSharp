# Load and draw a static model

```powershell
dotnet run --project samples/StaticModelViewer -c Debug -- --warp --frames=4
```

Expect three shared primitives, nine placed primitives across the patrol formation, and a clean exit after four frames. Drop `--frames=4` for the orbiting viewer. Pass `--model=<path>` to open another asset.

`DXSharp.Assets` is a portable .NET 10 project in the SDK source tree. It reads embedded-buffer glTF 2.0 and GLB with SharpGLTF. It returns local vertex and index data, placed primitives with scene transforms, and conservative scene bounds. It can run offline or without a graphics device.

```csharp
using DXSharp.Assets;

var model = GltfModel.Load( path );
foreach ( var instance in model.Instances ) {
    var geometry = model.Primitives[instance.PrimitiveIndex];
    // Use the geometry uploaded for this primitive and the instance's world transform.
}
```

1. Export an opaque, untextured triangle-list scene. The importer uses the default scene, or the first scene when no default is declared. Use base-color factors and optional `COLOR_0` for tint. Missing normals produce flat shading. The static import format excludes external buffers and images, textures, skinning, animation, morphs, and required extensions.
2. Record one [mesh upload](06-indexed-meshes.md) per entry in `Primitives`. Reuse it for every matching `PrimitiveIndex`. The sample uses 16-bit indices when the vertex count permits and keeps 32-bit indices for larger primitives. Submit the uploads and wait before handing the geometry to the desktop host's queue.
3. Use the mesh-to-scene matrix in row-vector convention. The sample preserves glTF coordinates, then applies a uniform fit to the bounds for the camera. It uses the inverse transpose of the instance's linear transform for normals and a cull-none pipeline that also draws mirrored instances. A back-face-culling renderer must account for negative determinants when setting winding.

The qualification fixture loads the same patrol geometry as JSON glTF and as GLB, compares the arrays and transforms, and checks flat normals, normalized colors, bounds, and rejected inputs. Windows smoke tests also run the desktop viewer on WARP.
