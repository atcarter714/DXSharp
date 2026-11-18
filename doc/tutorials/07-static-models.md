# Load and draw a static model

```powershell
dotnet run --project samples/StaticModelViewer -c Debug -- --warp --frames=4
```

Expect three shared primitives, nine placed primitives across the patrol formation, and a clean exit after four frames. Drop `--frames=4` for the orbiting viewer. Pass `--model=<path>` to open another asset, including the [Fleet Battler hulls](../../samples/FleetBattler/Models).

`DXSharp.Assets` is a portable .NET 10 project in the SDK source tree. It reads glTF 2.0 and GLB with SharpGLTF. `Load(path)` resolves local buffer and image sidecars under the model directory. Use a self-contained export with `Read(stream)`. It returns local vertex and index data, placed primitives with scene transforms, and conservative scene bounds. It can run offline or without a graphics device.

```csharp
using DXSharp.Assets;

var model = GltfModel.Load( path );
foreach ( var instance in model.Instances ) {
    var geometry = model.Primitives[instance.PrimitiveIndex];
    // Use the geometry uploaded for this primitive and the instance's world transform.
}
```

1. Export an opaque triangle-list scene as a self-contained GLB, embedded glTF, or glTF with local sidecars. The importer preserves linear base-color factors and `COLOR_0` tint. It decodes PNG and JPEG base-color images once per source image and keeps the material-selected UVs parallel to the vertices. Missing normals produce flat shading with the original UVs. The current material path samples a single mip with linear repeat filtering.
2. Call `CopyTexturedVertices` for an interleaved position, normal, color, and UV stream. Record one [mesh upload](06-indexed-meshes.md) per shared primitive. The sample uses 16-bit indices when the vertex count permits and 32-bit indices for larger primitives.
3. Record `CreateSampledTexture2D` on the same direct list with `srgb: true` for base color. Submit the batch before drawing. The sample uses a white texture for primitives without a map, and reuses the GPU view when multiple primitives share a source image.
4. Bind `CreateTextureRootSignature(constantCount, linearWrap: true)` and set its b0 constants and t0 view with `SetGraphicsConstants` and `SetGraphicsTexture(texture, 1)`. Multiply the sampled linear RGB by the vertex tint in the shader. Read `geometry.Surface` for the metallic, roughness, and emissive factors. The samples use `SetGraphicsConstants(values, destinationOffset)` to update material constants independently of the camera. The viewport shaders perform their output transfer for the UNORM target.
5. Use the mesh-to-scene matrix in row-vector convention. The viewer fits the scene to the camera, transforms normals with the inverse transpose, and draws mirrored instances with culling disabled. A back-face-culling renderer must account for negative determinants when setting winding.

The current import contract covers opaque static geometry, base-color maps, material factors, UVs, scene transforms, and bounds. Normal and metallic-roughness maps, texture transforms, skinning, animation, morph targets, and required extensions are outside this contract. For FBX, run the [Blender converter](../../tools/ConvertModel.py) during authoring to produce a static GLB.

The qualification suite compares glTF and GLB geometry, checks the bundled models' dimensions and decoded UVs and pixels, and renders the viewer and Fleet Battler on Windows WARP.
