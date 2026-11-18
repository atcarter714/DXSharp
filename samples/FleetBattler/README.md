# Fleet Battler

The Frontier scenario opens at Earth with one science scout, two constructors, 50 ore, and 25 energy. The galaxy map charts neighboring stars after Deep Sky Astrometry. A chart reveals a system's bodies. Local traffic requires an arriving Sol expedition.

| Control | Action |
| --- | --- |
| Click or drag a box | Select Sol ships. Double-click a ship to select its command group |
| Fleet outliner | Select a group. Double-click to center the camera. Scroll the wheel over the list or click the scrollbar track to see further groups |
| Shift / Ctrl | Add to or toggle the selection. Ctrl+A selects all living Sol ships |
| Merge / Split or J / Shift+J | Merge selected combat groups or split selected squadrons into independent groups |
| Right-click | Plot a transfer, attack a hostile contact, or open a body's order menu |
| Click a body | Open its inventory. **Build** opens the orbital catalog, and **Surface** switches to local construction |
| F2 / Ctrl+F2 | Rename the inspected body or selected unit. Ctrl+F2 or right-click an outliner row renames a fleet. Simulation continues while the editor is open |
| Click a yard or press B | Open its design catalog and build queue |
| E / N / L / T | Assign a survey, dispatch a constructor for extraction or a lab, or open Research |
| Q / V / Y / U | Cycle stance, flee, repair, or refit |
| Colony / Research / Design / Orders / Log | Inspect resources, run research, fit and save class-specific warship designs, find yards, or read illustrated dispatches |
| Tab / Escape / F4 | Open Colony, close the top panel, or open the dispatch log |
| Space / − / + | Pause or resume, slow down, or speed up. Both maps offer 1×, 2×, and 4× |
| Scroll / middle-drag / arrow keys | Zoom toward the pointer or pan |
| Shift + middle-drag / Home / F | Rotate through 360° with limited tilt, reset the camera, or focus the inspected object |
| G / F11 / M | Galaxy map / fullscreen / mute |

## Playing Frontier

1. **Survey a body.** A scout takes 16 seconds at 1× to scan after arrival, then reveals the deposit and awards 10 ore. The inspector shows travel or scan progress. A scanning beam and audio cue start on arrival. The first completed survey asks you to choose a permanent extraction or power charter for Sol and brings a merchant offer in **Log**.
2. **Build or settle a surveyed body.** A constructor travels to build orbital projects, including orbital extraction. Local crews build surface facilities once colonists arrive. Build a logistics depot in orbit above the destination first. Order a colony ship at an operational orbital yard, select it, and press **Settle** in the body inspector. The ship enters orbit and establishes a three-population settlement after 60 seconds in orbit at 1×. Remote yards require an online depot and six population. Housing is tied to body size and limits growth. Colonies grow when supplied by a depot or solar array. Surface facilities appear in the body inspector. Each four population, rounded up, staffs one physical surface slot. Pending projects reserve their slots.
3. **Supply the colony.** Ore funds hulls and construction. Earth and solar arrays produce energy. Fleets and infrastructure pay recurring upkeep, displayed alongside net energy in the colony ledger. Belt miners are separate from constructors and gather ore while orbiting a reachable belt object. Celestial extraction sites can be upgraded in place after the applicable surface or orbital research. The colony ledger shows storage limits. A surface warehouse adds 250 ore, 100 energy, and 50 science capacity. An orbital storage station adds 180 ore, 250 energy, and 50 science capacity. Labs generate 6 science per income cycle, and surface observatories generate 10. Each additional six points of combined science output, rounded up, contributes one unit of research work per tick. Research is divided into Industry, Propulsion, Sensors, Hulls, Structures, and Doctrine. Projects in the same exclusive group close their alternatives on completion. Locked projects name their prerequisites. Completion opens a briefing with a shortcut back to research. Habitat Engineering unlocks the O'Neill cylinder, which adds four naval slots once built.
4. **Fit and build ships.** Open **Design**, choose a hull, select a slot, and drag a module card onto that slot. Clicking a card and then its slot also works. **Save Design** commits a valid fit, with the station queue preserving the ordered version. Frigates, destroyers, cruisers, battleships, carriers, and titans unlock through hull research. Ships use names from their hull lists, and new hulls consume naval capacity and energy upkeep.
5. **Use the fleet controls.** Normal ships fire at enemies in range. Aggressive corvettes also pursue nearby hostiles when free of other orders. Passive ships fire only at explicit attack targets. **Flee** orders a retreat and sets passive stance. **Fix** routes a damaged hull to the nearest reachable orbital yard. A green cross identifies repairs in orbit, which cost 1 energy per service interval. **Fit** sends a corvette to a yard to install the current valid blueprint. Refit costs 10 ore and 4 energy plus module premiums, paid after arrival.
6. **Choose an agreement.** The merchant dispatch offers either 25 ore and an allied trading contact for 12 energy, or 18 science for 15 ore. A trade agreement exchanges 5 energy for 12 ore every fifth income cycle when affordable. Your choice is recorded and cannot be taken twice.
7. **Escort production.** Pirates contest extraction sites. When they outnumber the local combat escort, they take up to half that site's ore output and one energy per raider each income cycle. They pick less-protected extraction sites and withdraw from locally superior military forces. The first piracy briefing offers a paid Ceres patrol that delays and weakens the raid. Amber patrols can return after the first raid sequence. Rival Command remains a military opponent.

## Navigation

Ships start and launch in host-relative parking orbits. The cyan route traces the remaining transfer and the diamond marks the predicted insertion. Gold brackets follow the live host or moving ship. A dashed connector separates the object's current position from the intercept. Attack pursuit updates its short-horizon lead as the target moves. Civilian hulls, armor mass, drive modules, and propulsion research affect cruise speed, acceleration, or turning. Open-space orders enter a stellar parking orbit at the selected radius.

**Long Burn Drive** expands the operating envelope to the outer belt. Long-range corvette modules extend reach within the current technology limits. Sol's tactical projection uses rounded semimajor axes and eccentricities from the [JPL J2000 approximate elements](https://ssd.jpl.nasa.gov/planets/approx_pos.html) and NASA's fact sheets for [Ceres and Vesta](https://nssdc.gsfc.nasa.gov/planetary/factsheet/asteroidfact.html), [Pluto](https://nssdc.gsfc.nasa.gov/planetary/factsheet/plutofact.html), and [Halley](https://nssdc.gsfc.nasa.gov/planetary/factsheet/cometfact.html). Orbital spacing is 29% wider than the previous map and Sol transfers include longer burn and coast time. The [Kuiper Belt](https://science.nasa.gov/solar-system/kuiper-belt/) is a region bounded by two tracks. Survey [Arrokoth](https://science.nasa.gov/solar-system/kuiper-belt/arrokoth-2014-mu69/) or Pluto as individual bodies. Phases and radii are illustrative. The GPU asteroid field stays within the [main belt](https://science.nasa.gov/solar-system/asteroids/) between Mars and Jupiter, and uses the fractional simulation clock for rendering.

### The First Crossing

The campaign starts on 2184.01.01. At 1× an Earth sidereal orbit takes 16 minutes of unpaused play and advances the date by about 365 days. Other Sol orbits use the same calendar, with compressed tactical distances. Local ship speeds use their existing simulation units.

1. Open **Galaxy** and click the ship badge beside **Sol**. Select an idle scout from the roster or press **Find Scout**. Shift adds and Ctrl toggles selections.
2. Click **Proxima**, then **Survey** to cross and automatically scan the first reachable unsurveyed body on arrival. **Move Fleet** and right-click issue movement orders. Double-click a star or badge to open the system.
3. The first scout arrival opens regular routes in both directions. Select a local fleet and right-click another system, or select the system and press **Move Fleet**. Transit uses the campaign clock and continues while viewing another map.
4. Explore telescope charts before landing. They show major bodies with reduced detail and hide local contacts. An arrival briefing offers **Focus Arrival**.
5. Send a constructor to surveyed Receiver World to build an orbital research lab and restore the receiver. Choose whether to share its data with the Consortium or defend an independent claim.

### Modding Frontier

The installed sample loads the versioned JSON files in `Content` at startup. Edit the deployed copies or the source files before building, and restart to apply changes.

| File | Data |
| --- | --- |
| [research.json](Content/research.json) | Categories, prerequisites, exclusive groups, prices, work, descriptions, and supported effect values |
| [hulls.json](Content/hulls.json) | Hull names, prices, construction time, naval weight, health, flight stats, display scale, and research unlocks |
| [modules.json](Content/modules.json) | Slot variants, power, stats, cost, and technology requirements |
| [structures.json](Content/structures.json) | Surface or orbital placement, costs, build times, outputs, and upgrade requirements |
| [ship-names.json](Content/ship-names.json) | Name pools per hull role |

Keep built-in IDs and slot types so catalog references still resolve. Research dependencies and exclusive groups are validated on load. Orbital guidance, combat scheduling, and rendering remain in C# and HLSL.

### Remaining goals

The [finish-line design](../../doc/FleetBattler.md) covers the campaign, tech progression, ship designer, rendering, and SDK toolkit milestones.

| Priority | Remaining work |
| --- | --- |
| Primary | Add session menus and save/load. Extend the fitting preview and continue the presentation pass. |
| Primary | Expand the playable crossing with freight routes, stronger faction consequences, and save/load during transit. |
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

Each file embeds its geometry and materials. Textured models also embed their base color images. Import it into a Houdini geometry network with a glTF SOP and load the mesh. The files use meters, +Y up, bow toward −Z, and a centered local origin. Fleet Battler applies separate display scales to keep the hulls legible next to planets and orbital paths.

| Reference file | Length | Role |
| --- | ---: | --- |
| [SurveyScout.gltf](Models/SurveyScout.gltf) | 28 m | Kestrel science scout with side sensor booms |
| [EscortFrigate.gltf](Models/EscortFrigate.gltf) | 75 m | Mariner heavy escort with twin exhaust sockets |
| [Corvette.gltf](Models/Corvette.gltf) | 48 m | Lancer escort and reinforcement hull |
| [Freighter.gltf](Models/Freighter.gltf) | 96 m | Mule hauler with six external cargo pods. Also used as the temporary miner hull |
| [MiningRelay.gltf](Models/MiningRelay.gltf) | 48 m | Capture site and temporary orbital facility hull |
| [ONeillHabitat.gltf](Models/ONeillHabitat.gltf) | 98 m | Cylindrical orbital habitat with radiator bands |
| [ProximaReceiver.gltf](Models/ProximaReceiver.gltf) | 16.8 m across | Faceted dish, copper support struts, and emissive feed |

Use the Static Model Viewer to inspect each authored asset with an auto-fit camera and to check replacement exports. Use `--ship-model`, `--frigate-model`, `--scout-model`, `--miner-model`, `--freighter-model`, `--station-model`, `--habitat-model`, and `--receiver-model` to replace the corresponding placeholder with a `.glb` or `.gltf`. Include the mesh and texture sidecars if you export a separate glTF. The override uses the exported node transforms and dimensions. Export a tactical corvette about 2.4 units long, with thrusters about 0.9 units off center and 1.12 units aft. Keep an authored-scale copy for Houdini.

```powershell
blender -b --python-exit-code 1 --python tools/ConvertModel.py -- C:\exports\Corvette.fbx C:\exports\Corvette.glb
dotnet run --project samples/FleetBattler -c Release -- --ship-model="C:\exports\Corvette.glb"
```

The FBX converter runs at authoring time. The game reads the resulting GLB. See the [static model tutorial](../../doc/tutorials/07-static-models.md) for material and format support.

The [operations art atlas](Art/OperationsAtlas.jpg) depicts a scout, ore hauler, corvette, orbital shipyard, extraction rig, and research station. The artwork appears in the outliner and planet inspector. The assets use the repository's [MIT license](../../LICENSE.md). The [fleet roadmap](../../doc/FleetBattler.md) describes the larger game and content pipeline.

The [receiver briefing illustration](Art/ReceiverBriefing.jpg) was generated for the sample and staged in the HUD atlas at startup. The navigation and signal glyphs use the existing bitmap HUD batch.
