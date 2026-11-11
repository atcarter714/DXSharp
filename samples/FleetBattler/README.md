# Fleet Battler

Command cyan squadrons in a small orbital system. Secure the mining beacon and launch two reinforcement wings from the shipyard. Amber patrols the beacon and can take it from you.

- **Left-click** near a cyan squadron to select it. **Right-click** to set a destination. The cyan bracket marks the selected squadron and the dashed line shows its order.
- **Scroll** to zoom toward the pointer. **Middle-drag** to pan across the system.
- Move squadrons inside the beacon's circle. The side with more squadrons inside takes control after 1.5 seconds. Equal presence contests the site.
- Hold a cyan majority to earn 15 ore per second. Press **B** to spend 60 ore on a six-ship wing. The new wing spawns at the cyan shipyard and becomes selected.

```powershell
dotnet run --project samples/FleetBattler -c Release
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --seed=37
dotnet run --project samples/FleetBattler -c Debug -- --warp --ships=129 --frames=7 --resize-test
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --benchmark-ticks=600
```

The default scenario has 72 ships in twelve six-ship wedges. The 60 Hz simulation uses seeded formation offsets and stable squad order. It preallocates space for two reinforcement wings. Replaying commands at the same ticks reproduces the state hash within a build and runtime.

The tactical view uses procedural stars and nebulae, a sun, a terrestrial world and its moon, and a ringed gas giant. The bodies follow fixed-tick elliptical orbits. The rendered bodies are shaded billboards below the tactical plane. The camera projects the orbital tracks, the beacon, and move orders in world space. A HUD reports ownership, ore, and the reinforcement objective.

The renderer uploads three patrol mesh primitives once and issues three indexed instanced ship draws. It writes 32 bytes per active ship into a fence-retired frame slot. The sky and HUD each add one draw. The headless benchmark measures CPU stepping, instance packing, and allocation without creating a graphics device.

Qualification checks same-runtime replay, independent orders, contested capture, recapture, the complete reinforcement objective, and orbital positions. The Windows gate verifies both ship colors, reads back a complete tactical frame, and presents through two resizes. The `interop-windows-2025` CI artifact contains `fleet-system.ppm`.

The ships use the original [patrol asset](../StaticModelViewer/Models/Patrol.gltf) under the repository's [MIT license](../../LICENSE.md). See the [fleet roadmap](../../doc/FleetBattler.md) for combat, authored star systems, and shared GPU effects.
