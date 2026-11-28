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
| Space / − / + | Pause or resume, slow down, or speed up. Both maps offer 0.125× through 32× |
| Scroll / middle-drag / arrow keys | Zoom around the locked body or the pointer. Middle-drag, WASD and arrows pan and release the camera lock |
| Shift + middle-drag / Home / End / F | Tilt and rotate, reset to Earth, frame the whole system, or follow the inspected body. Double-click also follows a body |
| G / F11 / M | Galaxy map / fullscreen / mute |

## Playing Frontier

1. **Survey a body.** A scout takes 16 seconds at 1× to scan after arrival, then reveals the deposit and awards 10 ore. The inspector shows travel or scan progress. A scanning beam and audio cue start on arrival. The first completed survey asks you to choose a permanent extraction or power charter for Sol and brings a merchant offer in **Log**.
2. **Build or settle a surveyed body.** A constructor travels to build orbital projects, including orbital extraction. Local crews build surface facilities once colonists arrive. Build a logistics depot in orbit above the destination first. Order a colony ship at an operational orbital yard, select it, and press **Settle** in the body inspector. The ship enters orbit and establishes a three-population settlement after 60 seconds in orbit at 1×. Remote yards require an online depot and six population. Housing is tied to body size and limits growth. Colonies grow when supplied by a depot or solar array. Surface facilities appear in the body inspector. Each four population, rounded up, staffs one physical surface slot. Pending projects reserve their slots.
3. **Supply the colony.** Ore funds hulls and construction. Earth and solar arrays produce energy. Fleets and infrastructure pay recurring upkeep, displayed alongside net energy in the colony ledger. Belt miners are separate from constructors and gather ore while orbiting a reachable belt object. Celestial extraction sites can be upgraded in place after the applicable surface or orbital research. The colony ledger shows storage limits. A surface warehouse adds 250 ore, 100 energy, and 50 science capacity. An orbital storage station adds 180 ore, 250 energy, and 50 science capacity. Labs generate 6 science per income cycle, and surface observatories generate 10. Each additional six points of combined science output, rounded up, contributes one unit of research work per tick. Research is divided into Industry, Propulsion, Sensors, Hulls, Structures, and Doctrine. Projects in the same exclusive group close their alternatives on completion. Locked projects name their prerequisites. Completion opens a briefing with a shortcut back to research. Habitat Engineering unlocks the O'Neill cylinder, which adds four naval slots once built.
4. **Fit and build ships.** Open **Design**, choose a hull, select a slot, and drag a module card onto that slot. Clicking a card and then its slot also works. **Save Design** commits a valid fit, with the station queue preserving the ordered version. Frigates, destroyers, cruisers, battleships, carriers, and titans unlock through hull research. Ships use names from their hull lists, and new hulls consume naval capacity and energy upkeep.
5. **Use the fleet controls.** Normal ships fire at enemies in range. Aggressive corvettes also pursue nearby hostiles when free of other orders. Passive ships fire only at explicit attack targets. **Flee** orders a retreat and sets passive stance. **Fix** routes a damaged hull to the nearest reachable orbital yard. A green cross identifies repairs in orbit, which cost 1 energy per service interval. **Fit** sends a corvette to a yard to install the current valid blueprint. Refit costs 10 ore and 4 energy plus module premiums, paid after arrival.
6. **Choose an agreement.** The merchant dispatch offers either 25 ore and an allied trading contact for 12 energy, or 18 science for 15 ore. A trade agreement exchanges 5 energy for 12 ore every fifth income cycle when affordable. Your choice is recorded and cannot be taken twice.
7. **Escort production.** In progression campaigns, the Breakwater Freeholds start watching after three surveys, five campaign days, and the start of extraction away from Earth and Luna. The warning window varies by campaign seed. The Freeholds blockade lightly defended sites outside Earth and Luna. Each cycle drains local ore and energy and damages the targeted facility. The HUD reports active blockades and per-cycle losses, the colony ledger shows net income, and each site's inventory shows its integrity. The Ceres Combine offers a patrol that delays and weakens the first raid. Rival Command remains a military opponent.

8. **Follow the anomalies.** A survey at Ceres opens a concession negotiation with the Combine. Sign for supplies and owe a permanent local extraction royalty, or publish the assay for science. Europa, Titan, Triton, and Charon have their own survey dispatches. Charon's occultation record can calibrate a faction-wide surveying upgrade.

## Navigation

Ships start and launch in host-relative parking orbits. The cyan route traces the remaining transfer and the diamond marks the predicted insertion. Gold brackets follow the live host or moving ship. A dashed connector separates the object's current position from the intercept. Attack pursuit updates its short-horizon lead as the target moves. Civilian hulls, armor mass, drive modules, and propulsion research affect cruise speed, acceleration, or turning. Open-space orders enter a stellar parking orbit at the selected radius.

**Long Burn Drive** expands the operating envelope to the outer belt. Moons inherit the range requirement of their parent system. Sol has 45 authored bodies, including the Galilean and Martian moons, additional moons of Saturn, Uranus, Neptune and Pluto, and the asteroids Pallas and Psyche. Positions and distances are compressed for play, with authored satellite periods. [JPL approximate planetary elements](https://ssd.jpl.nasa.gov/planets/approx_pos.html) · [JPL satellite elements](https://ssd.jpl.nasa.gov/sats/orbits.html) · [NASA Pluto and Charon fact sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/plutofact.html).

The main belt defaults to 36 visible debris bodies on eccentric Keplerian paths, with gaps in the semimajor-axis distribution around two major Jupiter resonances. Ceres and Vesta remain individual destinations. Campaign vessels are represented by simulation entities. GPU freighter traffic is available through `--traffic` in skirmish and stress runs. [NASA asteroid orbits and Kirkwood gaps](https://science.nasa.gov/learn/basics-of-space-flight/chapter1-3/).

### The First Crossing

The campaign starts on 2184.01.01 at 1×. Each real second advances one campaign hour at that rate. At 32×, one Earth sidereal orbit takes roughly four and a half minutes. Orbital periods, body spin, transfers and the displayed date advance from the fixed-step campaign clock. Distances and ship acceleration use compressed tactical units.

1. Open **Galaxy** and select **Proxima**. The first-crossing panel shows the next unmet requirement and enables **Launch Expedition** once the route is ready.
2. Use **Rally Crew** to select or return an idle scout and constructor to Earth orbit. The shared roster shows the selected fleet. Shift adds and Ctrl toggles selections.
3. The expedition's arrival opens regular routes in both directions. Open Proxima and assign an outer-body survey directly. For later transfers, select a fleet and right-click another system or press **Move Fleet**. **Survey** sends a scout and scans the first reachable unsurveyed body on arrival.
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

The yard keeps the cost and duration already committed to its queue. The 1× setting runs 60 ticks per second. The campaign defaults to 1×, and pause stops campaign time. Ship positions and headings interpolate between simulation ticks. Each system retains its fleet, survey assignments, colonies, stockpiles and production queue. Completed research follows the player faction. Choose its name using **Faction** on the title screen or Ctrl+Shift+F2 in a campaign.

The opening camera follows Earth until you manually pan, zoom toward a point or focus another target. **Home** restores Earth tracking. In tactical engagements, ships carry their orbital momentum into the intercept, drift under the local gravity model and turn before applying their main thrust. Drive performance affects acceleration and turn rate, and the intercept controller slows the approach to match the target's motion at standoff range.

```powershell
dotnet run --project samples/FleetBattler -c Release
dotnet run --project samples/FleetBattler -c Release -- --fullscreen --galaxy
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --traffic=65536 --debris=16384 --mute
dotnet run --project samples/FleetBattler -c Release -- --ships=10000 --benchmark-ticks=600
```

The fixed-tick CPU simulation owns combat, movement orders, casualties, and the economy. Damage resolves after each tick's volley. Rendering packs living ships. Recycled slots carry generation IDs so replacement hulls do not inherit a destroyed ship’s selection. GPU compute steers the civilian freighters and evaluates the asteroid field as separate jobs over contiguous UAV buffers. `ComputeJobBatch` binds each kernel, rounds work to thread groups, and inserts UAV barriers for declared read/write hazards. The renderer owns resource transitions and command submission. Tactical combat and player commands run on the CPU. Living tactical ships draw in batches by hull class. Civilian freighters read the compute-generated instance buffer directly. Distant or dense traffic uses a 24-vertex silhouette to bound draw cost. Asteroids use irregular faceted silhouettes. The compute shaders are in [Shaders](Shaders).

The [jukebox configuration](Music/README.md) includes system, galaxy, engagement, and victory playlists. The original procedural tracks play by default. The sample includes order, capture, launch, and combat effects. A mood change can switch tracks after 28 seconds. Tracks fade over 2.2 seconds. Contact music uses sustained nearby hostile presence or a blockade, with a 20-second clearance delay. Incoming raids and Rival Command reinforcements display a hostile-contact alert and sound a two-tone alarm.

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

## Content packs and Houdini exports

Launch without arguments to open the title screen. The read-only Technology Archive lets you browse projects and prerequisites before choosing Launch Campaign. Orbital Assembly starts with a funded research lab and operational shipyard. Use Research and Design in flight to queue a project or customize a hull. The designer renders the selected hull model, including content-pack replacements. Use the arrow keys and Enter or click a title menu row. Esc opens flight settings. Return to Title ends the current session. From the repository root, try the Glass Meridian campaign with:

~~~powershell
dotnet run --project samples/FleetBattler -- --mod=samples/FleetBattler/Mods/GlassMeridian --scenario=glass-meridian
~~~

Pass the same arguments with `--validate-content` for a headless catalog and hull import check. Repeat `--mod=<directory>` in load order. Each JSON file has version 1 and a named record array. A later file replaces the entire record with the same case-insensitive ID. Omit files to inherit bundled records. Module keys combine slot and ID.

| File | Record array | Authoring contract |
| --- | --- | --- |
| research.json | projects | Give new projects a namespaced ID, existing branch, cost, work in ticks, artwork, and prerequisite IDs. New projects support extractionBonus, surveyBonus, upkeepReduction, scanRate, damageBonus, rangeBonus and an optional pulseEffect. Additional branch pages appear automatically. |
| scenarios.json | scenarios | Set the menu description, starting resources, raids, optional startingSites, and ordered events. Enable `frontierProgression` for longer economy and research cycles, staged settlement, and first-crossing requirements. |
| events.json | events | Reference one lore fragment. Set `system` to Sol (default), an authored system name or a generated system designation such as DX 017. Use `body` to require a survey of a particular target, by stable body ID in Sol or by displayed name in another system. Set `unlock` to complete a research ID on acceptance and `oreRoyalty` to apply a percentage to future local extraction. An optional `declineReward` object supplies the alternate choice label and its rewards. Both choices credit the event system's local stores up to capacity. |
| lore.json | fragments | Original narrative text keyed by ID. |
| modules.json | modules | Add consecutive numeric variant IDs to Drive, Power, Shield or Computer. Set power demand, cost, performance and an optional research ID. Drive modules can set `exhaust` to `Thermal`, `HighThrust`, `Ion`, or `Fusion`. Weapon modules can set `effect` to `Pulse`, `Rail`, `Ion` or `Thermal`. These IDs also work for `pulseEffect` in research. Click a module to fit it or drag it onto its slot, then save the design. |
| models.json | models | Override one hull role with a glTF or GLB path relative to the catalog, display scale, flatten flag and plume half-width. Legacy paired nozzle positions are used for models without markers. |
| bodies.json | bodies | Override a solar body by stable ID and retain its index. Set parent index, axisAu or mapAxis, orbital period in days, eccentricity, initial phase in turns, display radius, color, rings, class and habitability. |
| factions.json | factions | Set campaign faction names, HDR tint and hostile relationships. Hostility is reciprocal. |
| quotes.json | quotes | Loading screen text and credit. |

In Houdini, build in metres, Y up, bow along negative Z, with the center of rotation at the origin. Export triangle meshes with normals, UV0 and base-color materials. Keep each variant in its own GLB or bundle every glTF sidecar. Use flatten=true to apply node transforms. Inspect AssayTender and SpindleCarrier for instanced components, radiator clearance and paired nozzles. Socket distances in the manifest are tactical units after model scale and before the hull role's instance scale. Coast and ballistic flyby phases cut main thrust. For new gameplay roles extend HullRole, blueprint mapping, hull definitions, names and model records. For an additional solar body append a bodies.json record after its parent, update BodyCount and review survey bit masks and shader body limits. Body periods are in campaign days; the tactical display compresses distances.

Right-click a body and choose the named flyby route. A Luna pass continues to Mars, a Jupiter pass to Europa, and a Pluto pass to Charon. Guidance evaluates both sides of the moving flyby body and matches velocities at both ends of the unpowered hyperbola. Powered transfers ramp thrust up and down and reserve time to turn before braking into the destination orbit. See [JPL orbital mechanics](https://spsweb.fltops.jpl.nasa.gov/portaldataops/mpg/MPG_Docs/MPG%20Book/Release/Chapter7-OrbitalMechanics.pdf).

### Thruster emitters

Place an empty transform at the lip of each nozzle in Houdini. Name it `exhaust_port`, `exhaust_starboard`, or any other unique name beginning with `exhaust_`. Aim its local **+Z** axis in the direction the plasma should travel. Export the transforms as named empty glTF/GLB nodes in the same scene as the hull. Keep parent transforms on export. The importer applies the accumulated node transform and hull scale to the socket, then the game applies the ship's orientation and instance scale. Tune `plumeWidth` in the hull's manifest in tactical units. The legacy `nozzleOffset` and `nozzleAft` also use tactical units.

For a Houdini cook pipeline, preserve the same names and transforms in the resulting glTF/GLB. Fleet Battler currently loads that exported asset at startup.

### Frontier progression and galaxy generation

**First Crossing**, the default campaign, and **The Quiet Sun Survey** produce ore and energy every 12 seconds and science every 24 seconds at 1×. Research work takes eight times the authored base work with a cap on lab throughput. The faction can run one project at a time, paid from and staffed by its host system. **Orbital Assembly** uses the accelerated sandbox economy.

Research Habitat Engineering to settle marginal surfaces, then Closed-loop Ecology and a local orbital habitat for harsher ones. Climate Engineering opens **Condition World** in the planetary inspector. The surveyed target needs an online habitat and solar array, plus local funds. Conditioning takes 3,600 ticks and adds 20 points to that world's rating. Further engineering unlocks the orbital Soletta. Its construction requires a habitat and solar array and takes 10,800 ticks once work starts.

Raiders can blockade a built site, steal local ore and energy, and damage its infrastructure. Integrity affects production, and shipyards below half integrity stall. Assign escorts or drive the pirates away to end the blockade and let repair crews restore integrity using local energy.

The progression campaign requires Pulsed Fusion, an operational Earth orbital shipyard, 145 ore and 185 energy for the first Proxima crossing. Open Expedition, use the Fusion Research shortcut if needed, and rally an idle scout and constructor to Earth orbit. Launch Expedition uses that pair when the current selection is unsuitable. One ship must stay in Sol.

The map contains six authored systems and 122 deterministic systems. Generated systems use representative main-sequence stars, Kepler periods from stellar mass and semimajor axis, and an insolation-based surface rating with an atmosphere penalty. The display compresses distances. Each remote system loads on demand and begins advancing on player arrival. The Asteri and Veyran field local patrols in a subset of generated systems. The favorable-surface rate is a gameplay choice. Kepler occurrence estimates refer to rocky planets in habitable zones, which are a different population from worlds humans could settle without specialised infrastructure. [NASA planet taxonomy](https://science.nasa.gov/exoplanets/exoplanet-catalog/) · [Bryson et al., Kepler DR25 occurrence study](https://doi.org/10.3847/1538-3881/abc418).

### Body authoring in Houdini

Use a sphere centered at the origin, +Y as the north pole, latitude running from north at V=0 to south at V=1, and U wrapping around the equator. Bake seam-safe textures in a 2:1 equirectangular layout. Use sRGB for base color and linear data for height, tangent-space normal (+Y green), roughness and metalness. Keep clouds and atmospheric scattering separate from the surface. Export optional rings as a separate annulus in the equatorial plane with opacity and color independent of the planet. Retain the real radius and orbital inputs in your authoring project and put the compressed display radius and orbit properties in `bodies.json`. Supported classes are Star, Terrestrial, SuperEarth, IceGiant, GasGiant, Moon and SmallBody.

The procedural renderer's reusable material contract in `SpaceMaterials.cs` returns albedo, normal, height, metalness and roughness. The shipped rock material computes height and the shading normal together. The stellar material uses animated granulation, limb darkening, sunspots and prominence loops. The current tactical renderer consumes body metadata and evaluates procedural surfaces, with a base-color texture slot for the Moon. Arbitrary per-body texture sets and Houdini mesh exports use the authoring convention above and require renderer bindings before they can replace the procedural surface. [NASA Solar Dynamics Observatory prominence imagery](https://science.nasa.gov/earth/earth-observatory/prominence-from-the-solar-dynamics-observatory-43717/).

`rotationHours` is the signed sidereal rotation period in hours. Use a negative value for retrograde rotation or zero for a fixed surface. The surface follows the campaign clock through pause and time acceleration. [NASA Planetary Fact Sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/).

The galaxy overview groups systems along a spiral chart. Rings and labels mark key, occupied, or selected systems. Hover any star to reveal its designation, and select it to see the route and available orders in the right-hand panel. Use the mouse wheel to zoom around the pointer, middle-drag to pan, and Home to restore the overview.

The particle renderer blends additive sprites from a shared atlas before scene bloom. Bloom extracts highlights from the display-referred scene into floating-point textures, blurs two scales on both axes, and composites in linear light before drawing the HUD. Ship exhaust uses the same nozzle transforms as the main plumes. The current emitters cover stellar wisps, exhaust motes, and combat explosions. [Atlas credits](Art/Particles/CREDITS.md).
