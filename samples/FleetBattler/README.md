# Fleet Battler

Command cyan ships in a small sector. Secure a system's mining beacon and launch two reinforcement wings from its shipyard. The amber patrol can take the beacon from you.

| Control | Action |
| --- | --- |
| Left-click a cyan ship | Select one ship |
| Shift + left-click | Select its squadron |
| Right-click space | Move the selected ship or squadron |
| Right-click a labeled body | Order the selection into orbit |
| R | Rejoin the formation with the selected ship |
| B | Spend 60 ore to launch a six-ship wing |
| Scroll / middle-drag | Zoom toward the pointer / pan |
| G | Open the galaxy map or return to the current system |
| F11 | Toggle borderless fullscreen on the current monitor |
| M | Toggle music and effects |

The side with more squadrons inside the beacon takes control after 1.5 seconds. A cyan majority earns 15 ore per second. Send two squadrons early so the rival patrol cannot stall the operation. In the galaxy map, click Helios, Caldera, or Nereid to enter that system. Each system keeps its own ships, orders, and ore during the session. Simulation time advances in the active system.

```powershell
dotnet run --project samples/FleetBattler -c Release
dotnet run --project samples/FleetBattler -c Release -- --fullscreen --galaxy
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --traffic=65536 --debris=16384 --mute
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --benchmark-ticks=600
dotnet run --project samples/FleetBattler -c Debug -- --warp --mute --frames=9 --view-test
```

The sample opens at up to 1600 × 900 in windowed mode. The HUD scales with the viewport. The 72 starting command ships have individual acceleration, heading, braking, and local separation. Squadron orders rotate the formation and recall any detached ships. Orbit orders steer toward moving slots around the selected body. Animated engine plumes show the motion of command ships.

The CPU runs the 60 Hz command and economy simulation. Two compute passes update the orbital field and steer autonomous background ships. The results stay in GPU buffers for indexed instancing and debris rendering. Tactical and background traffic are separate populations. The CPU benchmark measures command-ship simulation and packing. Use `--traffic=0 --debris=0` to omit the GPU traffic layer.

The [jukebox configuration](Music/README.md) has system, galaxy, contested, and victory playlists, plus independent music and effects levels. The sample includes procedural tracks and cues. Copy your music into `Music/` and list the files in `Music/jukebox.json`.

Qualification covers independent orders, regrouping, moving orbits, contested capture, both wing purchases, and procedural PCM output. WARP reads back the compute-steered ships and orbital field, renders the tactical scene, and exercises window resizes, fullscreen restoration, and system transitions. Find `fleet-system.ppm` in the Windows interop CI artifact.

The ships use the original [patrol asset](../StaticModelViewer/Models/Patrol.gltf) under the repository's [MIT license](../../LICENSE.md). See the [fleet roadmap](../../doc/FleetBattler.md) for authored systems and fleet combat.
