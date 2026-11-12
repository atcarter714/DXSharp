# Fleet Battler

The default Frontier scenario starts at Earth with one science scout, two constructors, 50 ore, and 25 energy. Sol is the only system with player assets at the start. Caldera and Nereid are unclaimed systems where you can observe neutral Free Merchants. The galaxy map switches views and preserves each system's state. It does not transfer ships between systems.

| Control | Action |
| --- | --- |
| Click or drag a box | Select Sol ships. Double-click a ship to select its command group |
| Fleet outliner | Select a group. Double-click to center the camera |
| Shift / Ctrl | Add to or toggle the selection. Ctrl+A selects all living Sol ships |
| Merge / Split or J / Shift+J | Merge selected combat groups or split selected squadrons into independent groups |
| Right-click | Plot a transfer, attack a hostile contact, or open a body’s order menu |
| R | Return selected ships to their formation slots |
| Click a planet or moon | Inspect the survey, orbital sites, and available actions |
| Click a yard or press B | Open that yard’s design catalog and build queue. B falls back to the first available yard or Earth dock |
| E / N / L / T | Assign a survey / dispatch a constructor for a rig / dispatch one for a lab / open Research |
| Colony / Research / Design / Orders / Log | Inspect resource flows, run research, configure future corvettes, find production sites, and read dispatches |
| Tab / Escape | Open Colony or close the current panel |
| Hover a ship, body, or station | See its faction and disposition, survey status, or work in progress |
| Space / + | Pause or resume, and cycle 1×, 2×, and 4× |
| F4 | Open the latest Frontier dispatch |
| Scroll / middle-drag / arrow keys | Zoom toward the pointer / pan |
| Shift + middle-drag / Home | Tilt and rotate / reset the camera |
| F | Focus the inspected body or selected group |
| G / F11 / M | Galaxy map / fullscreen / mute |

## Playing Frontier

1. Assign the science scout to survey a body. Completion reveals its deposit and pays 15 ore. Select a constructor, inspect the surveyed body, and order an **Extraction Rig**. Every facility requires a constructor. Work begins after arrival and shows a welding beam and progress bar. Cancel from the body inspector or redirect the constructor to release the assignment and return the reserved cost.
2. Use Earth's **Civilian Dock** to replace scouts and constructors. Build an **Orbital Shipyard** to unlock corvette production. Select the station in orbit, click **Open** on its body card, or choose it from **Orders**.
3. Select a hull in the shipyard catalog and click **Add**. Each order reserves its own resources and launches one ship. Each yard has four queue slots and works independently. The active row and in-world yard show progress. Canceling returns the order's reserved resources and starts the next job if needed.
4. **Design** configures future corvettes. Pulse strips shields quickly. Rail fires farther and bypasses shields. Plating trades shield capacity for armor and costs 3 more ore, 2 more energy, and 20 more ticks. The yard captures the loadout, cost, and build time when you place an order.
5. In **Research**, choose a project after bringing a lab online. The lab generates science. Each project reserves science and energy, runs on simulation time, and applies its effect at completion. Ore funds structures and hulls. Earth's colony and orbital solar arrays generate energy each income cycle.
6. Follow **Log** as extraction attracts Amber Raiders, followed by Rival Command. Both are hostile to Sol. Free Merchants are neutral to Sol. Group escorts with **Merge** to select and issue orders together, or use **Split** to return them to independent control.

| Order | Ore | Energy | Time at 1× |
| --- | ---: | ---: | ---: |
| Constructor | 20 | 5 | 16 s |
| Scout | 24 | 6 | 16 s |
| Extraction rig | 35 | 8 | 12 s after arrival |
| Research lab | 45 | 10 | 12 s after arrival |
| Orbital shipyard | 55 | 15 | 24 s after arrival |
| Logistics depot | 40 | 9 | 12 s after arrival |
| Solar array | 28 | 4 | 12 s after arrival |
| Corvette with shield generator | 20 | 8 | 11 s |
| Corvette after industry research | 14 | 8 | 7 s |

Each completed shipyard adds six naval slots and each completed depot adds three. Living and queued corvettes consume naval capacity. After a wipe, available yards can produce replacements using the same queue.

## Navigation

Ships start and launch in host-relative parking orbits. An orbit or survey command targets an intercept on the destination body's future orbit. The dashed route samples the same transfer the ship flies. The diamond marks the rendezvous and the **Flight Plan** card shows the active flight phase and ETA at the current simulation speed. Open-space orders transfer into a stellar parking orbit at the chosen radius. Ships burn, coast, rotate for turnover, and brake before entering the destination orbit. Hull attitude and formation orientation update independently. The route uses fixed samples as the completed portion disappears.

The round-trip operating envelope is enforced before accepting a route. Research **Long Burn Drive** in Research for 36 science and 20 energy to expand it and reach the outer belt. The Kuiper Belt, Cinder Belt, and Dark Shoal each contain a high-yield prospect. Zoom out to find the outer track. The navigation model uses scaled orbital distances and bounded-burn guidance. Gravity assists and per-ship propellant accounting are future work.

Sol includes the Sun, eight planets, Earth's Moon, and a Kuiper Belt prospect. The catalog uses rounded planetary semimajor axes and eccentricities from the [JPL J2000 approximate elements](https://ssd.jpl.nasa.gov/planets/approx_pos.html). The tactical projection compresses distance logarithmically and uses illustrative starting phases, radii, and periods.

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

The fixed-tick CPU simulation owns combat, movement orders, casualties, and the economy. Damage resolves after each tick's volley. Rendering packs living ships. Recycled slots carry generation IDs so replacement hulls do not inherit a destroyed ship’s selection. GPU compute steers the civilian freighters and evaluates the asteroid field as separate jobs over contiguous UAV buffers. `ComputeJobBatch` binds each kernel, rounds work to thread groups, and inserts UAV barriers for declared read/write hazards. The renderer owns resource transitions and command submission. Tactical combat and player commands run on the CPU. Living tactical ships draw in batches by hull class. Civilian freighters read the compute-generated instance buffer directly. Distant or dense traffic uses a 24-vertex silhouette to bound draw cost. Asteroids use irregular faceted silhouettes. The compute shaders are in [Shaders](Shaders).

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

The [operations art atlas](Art/OperationsAtlas.jpg) depicts a scout, ore hauler, corvette, orbital shipyard, extraction rig, and research station. The artwork appears in the outliner and planet inspector. The assets use the repository's [MIT license](../../LICENSE.md). The [fleet roadmap](../../doc/FleetBattler.md) describes the larger game and content pipeline.
