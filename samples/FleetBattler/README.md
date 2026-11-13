# Fleet Battler

The Frontier scenario opens at Earth with one science scout, two constructors, 50 ore, and 25 energy. The galaxy map shows six systems. Sol is the player's starting system. The other systems have neutral contacts from the Free Merchants, Ceres Compact, or Survey Consortium. You can observe those systems and inspect their contacts in **Log**.

| Control | Action |
| --- | --- |
| Click or drag a box | Select Sol ships. Double-click a ship to select its command group |
| Fleet outliner | Select a group. Double-click to center the camera |
| Shift / Ctrl | Add to or toggle the selection. Ctrl+A selects all living Sol ships |
| Merge / Split or J / Shift+J | Merge selected combat groups or split selected squadrons into independent groups |
| Right-click | Plot a transfer, attack a hostile contact, or open a body's order menu |
| Click a body | Open its inventory. **Build** opens the orbital catalog, and **Surface** switches to local construction |
| F2 / Ctrl+F2 | Rename the inspected body or selected unit. Ctrl+F2 or right-click an outliner row renames a fleet. Simulation continues while the editor is open |
| Click a yard or press B | Open its design catalog and build queue |
| E / N / L / T | Assign a survey, dispatch a constructor for extraction or a lab, or open Research |
| Q / V / Y / U | Cycle stance, flee, repair, or refit |
| Colony / Research / Design / Orders / Log | Inspect resources, run research, configure corvettes, find yards, or read illustrated dispatches |
| Tab / Escape / F4 | Open Colony, close the top panel, or open the dispatch log |
| Space / + | Pause or resume, and cycle 1×, 2×, and 4× |
| Scroll / middle-drag / arrow keys | Zoom toward the pointer or pan |
| Shift + middle-drag / Home / F | Rotate through 360° with limited tilt, reset the camera, or focus the inspected object |
| G / F11 / M | Galaxy map / fullscreen / mute |

## Playing Frontier

1. **Survey a body.** A scout takes 16 seconds at 1× to scan after arrival, then reveals the deposit and awards 15 ore. The inspector shows travel or scan progress. A scanning beam and audio cue start on arrival. The first completed survey also brings a merchant offer in **Log**.
2. **Build or settle a surveyed body.** A constructor travels to build orbital projects, including orbital extraction. Local crews build surface facilities once colonists arrive. Order a colony ship at an operational orbital yard, select it, and press **Settle** in the body inspector. The ship enters orbit and establishes a three-population settlement after a landing interval. Housing is tied to body size and limits growth. Each four population, rounded up, staffs one physical surface slot. Pending projects reserve their slots.
3. **Supply the colony.** Ore funds hulls and construction. Earth and solar arrays produce energy. The colony ledger shows storage limits. A surface warehouse adds 250 ore, 100 energy, and 50 science capacity. An orbital storage station adds 180 ore, 250 energy, and 50 science capacity. Labs generate 6 science per income cycle, and surface observatories generate 10. Each additional six points of combined science output, rounded up, contributes one unit of research work per tick. A single lab runs the core projects in 64–88 seconds at 1×. Switch to **Advanced** for Habitat Engineering and Deep Sky Astrometry. Habitat Engineering unlocks the O'Neill cylinder, which adds four naval slots once built.
4. **Fit and build ships.** Set the weapon, armor, shield, computer, reactor, and drive in **Design**. The power meter and derived stats update as modules change. An over-budget design cannot be ordered. Build one hull at a time from an orbital yard with four queue slots per yard. Earth's civilian dock also builds scouts and constructors. Purchase captures the fit, cost, and duration. Completed yards and logistics depots add six and three naval slots respectively.
5. **Use the fleet controls.** Normal ships fire at enemies in range. Aggressive corvettes also pursue nearby hostiles when free of other orders. Passive ships fire only at explicit attack targets. **Flee** orders a retreat and sets passive stance. **Fix** routes a damaged hull to the nearest reachable orbital yard. A green cross identifies repairs in orbit, which cost 1 energy per service interval. **Fit** sends a corvette to a yard to install the current valid blueprint. Refit costs 10 ore and 4 energy plus module premiums, paid after arrival.
6. **Choose an agreement.** The merchant dispatch offers either 25 ore and an allied trading contact for 12 energy, or 18 science for 15 ore. A trade agreement exchanges 5 energy for 12 ore every fifth income cycle when affordable. Your choice is recorded and cannot be taken twice.
7. **Escort production.** Pirates contest extraction sites. When they outnumber the local combat escort, they take up to half that site's ore output and one energy per raider each income cycle. They attack escorts and leave civilian ships and colonies alone. Rival Command remains a military opponent.

## Navigation

Ships start and launch in host-relative parking orbits. The cyan route traces the remaining transfer and the diamond marks the predicted insertion. Gold brackets follow the live host or moving ship. A dashed connector separates the object's current position from the intercept. Attack pursuit updates its short-horizon lead as the target moves. Civilian hulls, armor mass, drive modules, and propulsion research affect cruise speed, acceleration, or turning. Open-space orders enter a stellar parking orbit at the selected radius.

**Long Burn Drive** expands the operating envelope to the outer belt. Long-range corvette modules extend reach within the current technology limits. Sol's tactical projection uses rounded semimajor axes and eccentricities from the [JPL J2000 approximate elements](https://ssd.jpl.nasa.gov/planets/approx_pos.html) and NASA's fact sheets for [Ceres and Vesta](https://nssdc.gsfc.nasa.gov/planetary/factsheet/asteroidfact.html), [Pluto](https://nssdc.gsfc.nasa.gov/planetary/factsheet/plutofact.html), and [Halley](https://nssdc.gsfc.nasa.gov/planetary/factsheet/cometfact.html). Orbital spacing is 28% wider in the tactical projection. Phases, radii, and periods are illustrative. The GPU asteroid field stays within the [main belt](https://science.nasa.gov/solar-system/asteroids/) between Mars and Jupiter, and uses the fractional simulation clock for rendering.

### Interstellar travel design

The current galaxy map supports observation. The campaign milestone adds a playable Sol–Proxima route with a fuel and range quote, scheduled departure and arrival, and a recoverable transit manifest. On arrival, the fleet can survey the system, recover a receiver, and establish a foothold. See [the campaign design](../../doc/FleetBattler.md#one-interstellar-route-end-to-end) for prerequisites, faction access, and save/load requirements.

### Remaining goals

The [finish-line design](../../doc/FleetBattler.md) covers the campaign, tech progression, ship designer, rendering, and SDK toolkit milestones.

| Priority | Remaining work |
| --- | --- |
| Primary | Add session menus and save/load. Complete the ship designer, inspectors, alerts, and presentation pass. |
| Primary | Ship one authored campaign with factional choices, a research tree, and a playable crossing from Sol to a neighboring system. |
| Primary | Tune the colony and trade economy through playtests, including population needs, housing, freight, pirate pressure, and additional faction decisions. |
| Stretch | Add authored ship, habitat, surface, and faction art, more hulls and effects, and bespoke sound design. |
| Stretch | Expand colonization, exploration, megastructures, and the inhabited galaxy. |

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

The [jukebox configuration](Music/README.md) includes system, galaxy, engagement, and victory playlists. The original procedural tracks play by default. The sample includes order, capture, launch, and combat effects. Incoming raids and Rival Command reinforcements display a hostile-contact alert and sound a two-tone alarm.

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
| [ONeillHabitat.gltf](Models/ONeillHabitat.gltf) | 98 m | Cylindrical orbital habitat with radiator bands |

Use the Static Model Viewer to inspect each authored asset with an auto-fit camera and to check replacement exports. Use `--ship-model`, `--scout-model`, `--miner-model`, `--freighter-model`, `--station-model`, and `--habitat-model` to replace the corresponding placeholder with a `.glb` or `.gltf`. Include the mesh and texture sidecars if you export a separate glTF. The override uses the exported node transforms and dimensions. Export a tactical corvette about 2.4 units long, with thrusters about 0.9 units off center and 1.12 units aft. Keep an authored-scale copy for Houdini.

```powershell
blender -b --python-exit-code 1 --python tools/ConvertModel.py -- C:\exports\Corvette.fbx C:\exports\Corvette.glb
dotnet run --project samples/FleetBattler -c Release -- --ship-model="C:\exports\Corvette.glb"
```

The FBX converter runs at authoring time. The game reads the resulting GLB. See the [static model tutorial](../../doc/tutorials/07-static-models.md) for material and format support.

The [operations art atlas](Art/OperationsAtlas.jpg) depicts a scout, ore hauler, corvette, orbital shipyard, extraction rig, and research station. The artwork appears in the outliner and planet inspector. The assets use the repository's [MIT license](../../LICENSE.md). The [fleet roadmap](../../doc/FleetBattler.md) describes the larger game and content pipeline.
