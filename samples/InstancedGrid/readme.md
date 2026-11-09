# InstancedGrid

A single draw animates 256 tiles. The shader uses `SV_InstanceID` for placement and color and `SV_VertexID` for quad geometry. Three root constants carry time, aspect ratio, and grid size.

```powershell
dotnet run --project samples/InstancedGrid -c Debug
dotnet run --project samples/InstancedGrid -c Debug -- --warp --frames=4
```
