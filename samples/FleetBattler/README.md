# Fleet Battler

The default Frontier scenario starts with a surveyed homeworld, one science scout, two miners, and 50 ore. Survey an outer body, build an extraction site, then defend the new settlement when Amber notices the operation. Helios, Caldera, and Nereid each retain their own scenario state when you switch systems.

| Control | Action |
| --- | --- |
| Click or drag a box | Select Cyan ships. Double-click a ship to select its squadron |
| Fleet outliner | Click a squadron to select it. Double-click to center the camera |
| Time controls / Space / + | Pause or resume, and cycle 1×, 2×, and 4× simulation speed |
| Shift / Ctrl | Add to / toggle the selection. Ctrl+A selects all living Cyan ships |
| Right-click | Move in open space, attack an Amber ship, or open a body’s order menu |
| R | Return selected ships to their formation slots |
| Click a planet or moon | Inspect its survey and construction status |
| E / N / L / T / I | Assign survey / build mine / build lab / research weapons / research industry |
| Tab / Command | Open or close the Economy, Fleet, and Events tabs |
| F4 | Open the latest Frontier dispatch |
| B / Queue Wing | Order a wing from an operational orbital shipyard |
| Scroll / middle-drag | Zoom toward the pointer / pan |
| G / F11 / M | Galaxy map / fullscreen / mute |

## Playing Frontier

1. Select the science scout in the fleet outliner. Right-click an unsurveyed body and assign the survey, or use the action in its inspection panel. The flight plan shows the assigned craft's course. A finished survey reveals the local deposit and pays 15 ore.
2. Select a miner, inspect the surveyed body, and choose **Place Mine**. The miner travels to the construction site before the timer starts. The build reserves 35 ore. Reassigning the miner during construction cancels the build and refunds its cost. The completed mine runs independently.
3. Use the homeworld panel or **Economy** to order another miner for 20 ore. Build a 55-ore orbital shipyard on a surveyed world. The site grows as it builds and the yard opens production after 24 seconds at 1×.
4. Open **Fleet** to choose a weapon and defense package for the next three-corvette wing. Pulse cycles quickly and hits shields first. Rail fires farther and bypasses shields. A shield generator recovers after a lull. Heavy plating trades shield capacity for armor and adds 8 ore and four seconds at 1×.
5. Follow the **Events** dispatches as extraction attracts an Amber raid. Richer deposits pay for defenses faster and draw an earlier response. Move miners and the scout clear, deploy escorts, and defeat both waves. An orbital lab generates science for weapons and industry research.

| Order | Ore | Construction at 1× |
| --- | ---: | ---: |
| Homeworld miner | 20 | 16 s |
| Mine | 35 | 12 s after miner arrival |
| Lab | 45 | 12 s |
| Orbital shipyard | 55 | 24 s |
| Three-corvette wing | 60 | 32 s |
| Wing after industry research | 42 | 20 s |

The yard can produce two wings per scenario. The queue reserves ore and records the loadout and duration at order time.

## Relay skirmish

Launch with `--skirmish` or an explicit `--ships=<count>` to play the original relay scenario. Defeat Amber or launch two six-corvette wings to secure the system.

1. Send squadrons toward the relay. A squadron contributes when at least half its survivors are in the zone. A Cyan majority captures it and produces ore. Right-click an Amber ship to focus fire, or right-click empty space to move. Ships can also return fire while following orders.
2. Select one ship or one complete squadron, right-click a body, and choose the survey order bearing that unit’s ID. The planet panel shows the assigned unit and its scan progress. A green dashed course and destination diamond mark its rendezvous. Scouts scan twice as fast. Canceling a survey keeps the unit in orbit and preserves completed scan progress. A new move or attack order frees the old assignment.
3. A completed survey awards 40 ore and unlocks construction. Mines cost 35 ore and yield 8 ore per production interval. Labs cost 45 ore and yield 6 science per interval. Weapons research costs 30 science. Defeat Amber or complete two reinforcement wings to secure the system.
4. Build one orbital shipyard at a surveyed world for 55 ore. Construction takes about 24 real seconds at 1×. An operational yard accepts two wings in total, one at a time, with ore charged on enqueue. Completed wings spawn beside the orbiting yard. The queue and production timer are visible in the shipyard panel.

| Wing | Ore | Time at 1× | Time at 4× |
| --- | ---: | ---: | ---: |
| Standard | 60 | 32 s | 8 s |
| After industry research (24 science) | 42 | 20 s | 5 s |

Research affects new orders. The yard keeps the cost and duration already committed to its queue. The 1× setting runs at a quarter of the previous pace, and 4× matches the old 1×. Ship positions and headings interpolate between fixed simulation ticks. Each system retains its fleet, survey assignments, colonies, resources, production queue, and research while you visit another system.

```powershell
dotnet run --project samples/FleetBattler -c Release
dotnet run --project samples/FleetBattler -c Release -- --fullscreen --galaxy
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --traffic=65536 --debris=16384 --mute
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --benchmark-ticks=600
```

The fixed-tick CPU simulation owns combat, movement orders, casualties, and the economy. Damage resolves after each tick's volley. Rendering packs living ships and keeps their original selection IDs. GPU compute steers the civilian freighters and evaluates the asteroid field. Living tactical ships draw in batches by hull class. Civilian freighters read the compute-generated instance buffer directly. Distant or dense traffic uses a 24-vertex silhouette to bound draw cost. Asteroids use irregular faceted silhouettes. The compute shaders are in [Shaders](Shaders).

The [jukebox configuration](Music/README.md) includes system, galaxy, engagement, and victory playlists. The original procedural tracks play by default. The sample includes order, capture, launch, and combat effects.

## Celestial textures

Fleet Battler accepts a 2:1 equirectangular Moon color image up to 4096×2048. The [NASA Scientific Visualization Studio CGI Moon Kit](https://svs.gsfc.nasa.gov/4720/) provides a 2048×1024 LROC color map. Use the albedo image with `--moon-map="C:\assets\lroc_color_2k.jpg"`, or save it as `Celestial/Moon.jpg` in the sample to stage it on build. The default moon is procedural. Credit the NASA Scientific Visualization Studio and check the source asset’s attribution and [media usage terms](https://www.nasa.gov/nasa-brand-center/images-and-media/) before redistributing it.

The star uses a local two-scale halo in the sky shader. A general bloom pass would need an HDR scene target, a downsample/blur chain, and composition before the HUD.

## Houdini asset handoff

Each file embeds its geometry and base color image. Import it into a Houdini geometry network with a glTF SOP and load the mesh. The files use meters, +Y up, bow toward −Z, and a centered local origin. Fleet Battler applies separate display scales to keep the hulls legible next to planets and orbital paths.

| Reference file | Length | Role |
| --- | ---: | --- |
| [SurveyScout.gltf](Models/SurveyScout.gltf) | 28 m | Kestrel science scout with side sensor booms |
| [Corvette.gltf](Models/Corvette.gltf) | 48 m | Lancer escort and reinforcement hull |
| [Freighter.gltf](Models/Freighter.gltf) | 96 m | Mule hauler with six external cargo pods. Also used as the temporary miner hull |
| [MiningRelay.gltf](Models/MiningRelay.gltf) | 48 m | Capture site and temporary orbital facility hull |

Use the Static Model Viewer to inspect each authored asset with an auto-fit camera and to check replacement exports. Use `--ship-model`, `--scout-model`, `--miner-model`, `--freighter-model`, and `--station-model` to replace the corresponding placeholder with a `.glb` or `.gltf`. Include the mesh and texture sidecars if you export a separate glTF. The override uses the exported node transforms and dimensions. Export a tactical corvette about 2.4 units long, with thrusters about 0.9 units off center and 1.12 units aft. Keep an authored-scale copy for Houdini.

```powershell
blender -b --python-exit-code 1 --python tools/ConvertModel.py -- C:\exports\Corvette.fbx C:\exports\Corvette.glb
dotnet run --project samples/FleetBattler -c Release -- --ship-model="C:\exports\Corvette.glb"
```

The FBX converter runs at authoring time. The game reads the resulting GLB. See the [static model tutorial](../../doc/tutorials/07-static-models.md) for material and format support.

The assets use the repository's [MIT license](../../LICENSE.md). The [fleet roadmap](../../doc/FleetBattler.md) describes the larger game and content pipeline.
