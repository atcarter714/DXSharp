# LitCube

A lit rotating cube demonstrates world and projection matrices in root constants, per-face normals, and a D32 depth target. The desktop host updates the depth target on resize.

```powershell
dotnet run --project samples/LitCube -c Debug
dotnet run --project samples/LitCube -c Debug -- --warp --frames=4
```
