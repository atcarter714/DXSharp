# Fleet Battler

Command two patrol formations from a tactical camera. Left-click a ship to select its fleet, right-click to move the formation, and scroll to zoom. The formations begin with opposing movement orders. The current scenario covers steering and presentation. The [fleet roadmap](../../doc/FleetBattler.md) covers resource sites, reinforcements, combat, and GPU effects.

```powershell
dotnet run --project samples/FleetBattler -c Release
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --seed=37
dotnet run --project samples/FleetBattler -c Debug -- --warp --ships=129 --frames=7 --resize-test
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --benchmark-ticks=600
```

- The CPU steps position and velocity arrays at 60 Hz. Formation slot offsets and entity order are seeded. Replaying the same commands at the same ticks reproduces the state hash on a given build and architecture.
- The renderer uploads the three patrol mesh primitives once. It writes 32 bytes per ship into the current frame slot and submits three indexed instanced draws for the entire fleet. `DesktopFrame.FrameSlotIndex` identifies the slot after its previous fence completes.
- The benchmark runs without creating a graphics device. It measures CPU simulation, instance packing, and thread allocations for the measured ticks. The displayed instance bytes are the size of one packed stream. GPU timing requires a rendered run.
- Qualification compares replay checkpoints, checks formation spacing and instance layout, and verifies cyan and amber geometry in separate regions of an offscreen WARP render. The windowed test exercises both frame slots through two resizes.

The ships use the original [patrol asset](../StaticModelViewer/Models/Patrol.gltf) under the repository's [MIT license](../../LICENSE.md).
