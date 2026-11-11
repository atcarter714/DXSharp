# LitCube

A lit rotating cube uses 24 vertices and 36 UInt16 indices in GPU working memory. The input layout supplies position, normal, and color from an unmanaged vertex struct. World and projection matrices use root constants, and a D32 target provides depth testing. The desktop host updates the depth target on resize.

Mesh creation records copies and state transitions into an upload command list. The staging allocation retires at the upload fence. Each draw retains the vertex and index buffers through its own submission fence.

```powershell
dotnet run --project samples/LitCube -c Debug
dotnet run --project samples/LitCube -c Debug -- --warp --frames=4
```
