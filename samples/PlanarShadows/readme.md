# PlanarShadows

An orbiting camera views three rotating pyramids on a tiled receiver. The sample uses two pipeline states and three draw phases. It renders the floor, projects each caster onto the floor along the light direction, then renders the lit geometry with depth testing. The projection is specific to a flat receiver.

```powershell
dotnet run --project samples/PlanarShadows -c Debug
dotnet run --project samples/PlanarShadows -c Debug -- --warp --frames=4
```
