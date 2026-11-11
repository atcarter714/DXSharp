# Static Model Viewer

Three patrol ships share one mesh with three material primitives. The sample loads the scene from disk, uploads each primitive once, and draws the formation with an orbiting camera, depth testing, and directional lighting. The escorts exercise nonuniform scale, including a mirrored transform on the starboard ship.

```powershell
dotnet run --project samples/StaticModelViewer -c Debug
dotnet run --project samples/StaticModelViewer -c Debug -- --warp --frames=4
dotnet run --project samples/StaticModelViewer -c Debug -- --model="C:\assets\ship.glb"
```

Export opaque, untextured triangle meshes as a self-contained glTF or GLB. The viewer uses base-color factors and vertex colors. It preserves glTF's right-handed, Y-up geometry and applies the node transforms at draw time. See [Load and draw a static model](../../doc/tutorials/07-static-models.md) for the supported subset and how to reuse the importer.

[Patrol.gltf](Models/Patrol.gltf) is original DXSharp sample art distributed under the repository's [MIT license](../../LICENSE.md).
