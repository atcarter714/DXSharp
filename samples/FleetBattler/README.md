# Fleet Battler

Command Cyan in Helios, Caldera, or Nereid. Defeat Amber or launch two reinforcement wings to secure a system. Amber sends an opening squadron to the beacon, intercepts Cyan units near it, and commits reserves as the battle develops.

| Control | Action |
| --- | --- |
| Click or drag a box | Select Cyan ships. Double-click a ship to select its squadron |
| Shift / Ctrl | Add to / toggle the selection. Ctrl+A selects all living Cyan ships |
| Right-click | Move in open space, orbit a body, or attack an Amber ship |
| R | Return selected ships to their formation slots |
| Click a planet or moon | Inspect its survey and construction status |
| E / N / L / T | Survey / build mine / build lab / research weapons. The planet panel has the same actions |
| B | Spend 60 ore to launch a six-ship wing |
| Scroll / middle-drag | Zoom toward the pointer / pan |
| G / F11 / M | Galaxy map / fullscreen / mute |

## Playing the skirmish

1. Send two squadrons toward the beacon. A squadron contributes if at least half its surviving ships are inside the zone. Hold a squadron majority for 1.5 seconds to take control. A Cyan majority earns 15 ore per second.
2. Right-click an Amber ship to focus fire. Selected ships approach weapons range and continue engaging its squadron after that target is destroyed. Ships also return fire automatically while following other orders. Hull bars and selection brackets follow individual ships. The labeled beacon stays at its sector coordinates.
3. Select a squadron, click a world, and choose **Survey**. The squadron orbits while a ship within survey range completes a two-second survey and finds 40 ore. Build one mine and one lab at each surveyed world. Each takes three seconds to build. Mines cost 35 ore and yield 8 ore per second. Labs cost 45 ore and yield 6 science per second. Spend 30 science on a permanent weapons upgrade for the system.
4. Spend ore on reinforcements or clear out Amber. Each system retains its fleet, colonies, resources, and research while you visit another system.

```powershell
dotnet run --project samples/FleetBattler -c Release
dotnet run --project samples/FleetBattler -c Release -- --fullscreen --galaxy
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --traffic=65536 --debris=16384 --mute
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --benchmark-ticks=600
```

The fixed-tick CPU simulation owns combat, movement orders, casualties, and the economy. Damage resolves after each tick's volley. Rendering packs living ships and keeps their original selection IDs. GPU compute steers the civilian freighters and evaluates the asteroid field. The freighters have a separate procedural hull. Asteroids use irregular faceted silhouettes. The compute and freighter shaders are in [Shaders](Shaders).

The [jukebox configuration](Music/README.md) includes system, galaxy, engagement, and victory playlists. The original procedural tracks play by default. The sample includes order, capture, launch, and combat effects.

## Houdini asset handoff

Use `--ship-model="C:\exports\CyanScout.glb"` to preview a patrol replacement for both factions. Export one mesh at its local origin with +Y up and the bow toward −Z. Match the stock patrol's roughly 3.4-unit hull length for camera and thrust alignment. The sample loader accepts self-contained GLB or embedded-buffer glTF with opaque, untextured triangle primitives and base or vertex colors. See the [asset import tutorial](../../doc/tutorials/07-static-models.md).

Start with three silhouettes that read well at tactical zoom. Cyan can use a swept wedge, Amber can use a wider split-nose interceptor, and freighters can use a cargo spine with boxy pods. Keep separate Amber and cargo exports for the next asset integration. For debris, export a small set of irregular rock silhouettes at a consistent scale. A survey station, mining rig, and lab can share an orbital mounting kit while retaining distinct silhouettes.

The stock [patrol asset](../StaticModelViewer/Models/Patrol.gltf) is distributed under the repository's [MIT license](../../LICENSE.md). See the [fleet roadmap](../../doc/FleetBattler.md) for authored systems and expanded economy and combat.
