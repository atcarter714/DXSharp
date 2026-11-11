# Fleet Battler

Command Cyan in Helios, Caldera, or Nereid. Defeat Amber or launch two reinforcement wings to secure a system. Amber sends an opening squadron to the beacon, intercepts Cyan units near it, and commits reserves as the battle develops.

| Control | Action |
| --- | --- |
| Click or drag a box | Select Cyan ships. Double-click a ship to select its squadron |
| Fleet outliner | Click a squadron to select it. Double-click to center the camera |
| Time controls / Space / + | Pause or resume, and cycle 1×, 2×, and 4× simulation speed |
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
3. Select a squadron, click a world, and choose **Survey**. The squadron orbits while a ship within survey range completes a two-second survey and finds 40 ore. A scout squadron in range completes the survey in one second. Build one mine and one lab at each surveyed world. Each takes three seconds to build. Mines cost 35 ore and yield 8 ore per second. Labs cost 45 ore and yield 6 science per second. Spend 30 science on a permanent weapons upgrade for the system.
4. Spend ore on reinforcements or clear out Amber. Each system retains its fleet, colonies, resources, and research while you visit another system.

```powershell
dotnet run --project samples/FleetBattler -c Release
dotnet run --project samples/FleetBattler -c Release -- --fullscreen --galaxy
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --traffic=65536 --debris=16384 --mute
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --benchmark-ticks=600
```

The fixed-tick CPU simulation owns combat, movement orders, casualties, and the economy. Damage resolves after each tick's volley. Rendering packs living ships and keeps their original selection IDs. GPU compute steers the civilian freighters and evaluates the asteroid field. Living tactical ships draw in batches by hull class. Civilian freighters read the compute-generated instance buffer directly. Distant or dense traffic uses a 24-vertex silhouette to bound draw cost. Asteroids use irregular faceted silhouettes. The compute shaders are in [Shaders](Shaders).

The [jukebox configuration](Music/README.md) includes system, galaxy, engagement, and victory playlists. The original procedural tracks play by default. The sample includes order, capture, launch, and combat effects.

## Houdini asset handoff

Each file embeds its geometry and base color image. Import it into a Houdini geometry network with a glTF SOP and load the mesh. The files use meters, +Y up, bow toward −Z, and a centered local origin. Fleet Battler applies separate display scales to keep the hulls legible next to planets and orbital paths.

| Reference file | Length | Role |
| --- | ---: | --- |
| [SurveyScout.gltf](Models/SurveyScout.gltf) | 28 m | Kestrel scout with side sensor booms. Cyan squads 01 and 04 survey at twice the standard rate |
| [Corvette.gltf](Models/Corvette.gltf) | 48 m | Lancer escort and reinforcement hull |
| [Freighter.gltf](Models/Freighter.gltf) | 96 m | Mule hauler with six external cargo pods |
| [MiningRelay.gltf](Models/MiningRelay.gltf) | 48 m | Capture site with panel wings and an ore processing spine |

Use the Static Model Viewer to inspect each authored asset with an auto-fit camera and to check replacement exports. The Fleet Battler `--ship-model="C:\exports\Corvette.glb"` override draws the asset's mesh-local geometry as the corvette class. For that override, bake transforms and export a tactical hull about 2.4 units long with two thrusters about 0.9 units off center and 1.12 units aft. Keep an authored-scale copy for Houdini. See the [static model tutorial](../../doc/tutorials/07-static-models.md) for supported materials.

The assets use the repository's [MIT license](../../LICENSE.md). The [fleet roadmap](../../doc/FleetBattler.md) describes the larger game and content pipeline.
