# DXSharp Fleet Battler

Fleet Battler is a Windows strategy game sample for DXSharp. Frontier currently starts at Earth with a science scout, two constructors, and a civilian dock. It has six map systems, local surveys and transfers, timed construction and research, colonization, orbital shipyard queues, corvette fitting, and piracy. Deep Sky Astrometry opens a chart of neighboring systems. A one-way expedition can travel from Sol to Proxima, reveal its local contacts, restore a receiver, and share it or face a challenge from Rival Command. The command simulation runs on the CPU and GPU compute updates decorative traffic and debris. See the [sample README](../samples/FleetBattler/README.md) for the playable build and controls.

## When to stop developing the sample

Fleet Battler is the showcase for building a polished Windows game with DXSharp. The feature freeze requires a playable trip from Sol to one authored neighboring system, meaningful relationships with several factions, a research tree, and an ending earned through play at the destination. Use the existing skirmish as a quick entry point. Keep the rest of the chart as context and keep large simulations in a separate benchmark mode.

| Milestone | Scope | Gate |
| --- | --- | --- |
| Feature freeze | One campaign across Sol and the Alpha Centauri region, plus skirmish. Use six hulls, eight core structures, and twelve techs as starting content budgets. Include a dedicated ship designer and a coherent UI across both maps. | A first-time player can survey, build, negotiate or fight, commission a ship, travel between systems, establish a foothold, and reach a clear ending through the game UI. A player can quit and continue from a save made in transit. |
| Stabilization | Fix progression blockers, tune competing choices and pacing, polish art, typography, sound, and camera, and qualify the packaged build. | The campaign is fun to replay by a different route. Queues, ETA, risks, and blocked actions are readable at 1080p and 4K. Run a Windows hardware playtest, keep WARP and CI green, and publish a reproducible CPU/GPU frame-time and memory baseline. |
| Maintenance | Keep the showcase, asset provenance, and build instructions working as DXSharp evolves. | Put a larger commercial game, its NoesisGUI integration, and its full content pipeline in a separate project. |

### Campaign and factional conflict

**The First Crossing** starts with player presence only in Sol. Earth funds a new survey mission after a robotic precursor goes silent near Proxima. The dispatches follow three events the player can affect: securing a Sol supply route, deciding who gets access to the expedition, and restoring the remote receiver. Use 30–60 minutes as the playtest target. After the local raid, replace the generic secured banner with the next objective and surface the missing prerequisites. Keep the system playable after the ending.

| Faction | Goal and activity | Interaction that matters |
| --- | --- | --- |
| Sol Authority | Establish an independent outpost and a durable link to Earth | The player chooses whether to share the route, can escort shipments, and can build a listening outpost or local defense. |
| Ceres Compact | Sell propellant and docking in exchange for guaranteed freight | A compact supplies a fuel berth and escort coverage. Its recurring fee and the port access appear on the route card. The player can instead invest in their own depot and escorts. |
| Free Merchants | Move freight along profitable safe routes | They can deliver contracted supplies or sell survey data. Lost shipments suspend service until the route is protected or replaced. |
| Survey Consortium | Recover and publish the precursor data | Joint research offers an early navigation project. Shared receiver access earns the agreement ending and changes the last dispatch. |
| Rival Command | Secure first control of the receiver | It contests the recovery site under clearly announced rules. Its arrival is an observable mission with an ETA. The first implementation has two outcomes. Share access with the Consortium, or claim the station and defeat the responding Rival Command patrol. |
| Amber Raiders | Steal cargo on lightly defended routes | They choose a viable shipment, intercept it, and withdraw with a bounded loss. They can delay the expedition. A civilian dock, recovery orders, and a replacement route keep the player in the game. |

Use directional faction relations with explicit **hostility**, **port access**, and **survey/data rights**. A compact or contract sets rights for a named party and route. A reputation value can gate offers. It cannot silently change targeting or access rules. Show the reason for every permission on the route, station, or faction inspector. Record offers and decisions as stable events with known costs, outcomes, and cancellation rules.

Use one recurring resource route to drive the crisis. Extraction adds to a site's stock, a freighter claims a shipment, and delivery adds it to the destination's stock. The economy UI shows *on site*, *in transit*, *delivered*, and *lost*. Threat selects routes from visible activity and escort strength. A station in range contributes protection, and the convoy card shows which ships are assigned. Raiders cannot both take a shipment and leave it queued for delivery. Start with ore and energy as spendable resources. Use propellant as a local port/ship stock and science as research throughput. Add resources only where they change a decision.

| Act | Player decision | Observable result |
| --- | --- | --- |
| Establish a foothold | Survey a solid body and choose a site for a depot, mine, and first colony. | A constructor builds the site, the first freighter leaves with cargo, and the player can follow the delivery. |
| Resolve the convoy crisis | Buy a Ceres berth and protection or build a depot, escort, and defense yourself. | The raid applies only to exposed freight. The choice changes fuel access, expedition funds, and which faction offers support. |
| Cross and recover | Fit a fusion-capable expedition, choose who travels, and commit with a route quote. Use the arriving scout and constructor to locate and restore the receiver. | The fleet becomes playable at the destination. Sol stays accessible. The scenario ends on a working receiver under independent or shared control. |

Keep the six-hull target. The scout surveys, constructor builds, freighter moves cargo, colony ship establishes settlement, corvette intercepts, and frigate protects a convoy or station. Give each an identifiable silhouette and a distinct role. Build the scenario around extraction rigs, solar arrays, labs, depots, shipyards, survey outposts, defense platforms, and habitats. An O’Neill habitat is a late optional expression of the same housing and freight rules. Surface work uses local labor and slots, and orbital facilities use constructor assignments.

## Research and interstellar travel

Use one active project and a reorderable queue. Eligibility depends on prerequisites and completed world milestones. Labs contribute work per campaign tick. Progress already earned stays with a paused project. Recalculate ETA as lab output changes. Unlock an action, module, route, or production process in each project. Show that result and the cheapest viable prerequisite path in the research view. Award the player progress during surveys, construction, or transfers so the fastest route involves active decisions.

| Track | First project | Follow-up | Result in play |
| --- | --- | --- | --- |
| Propulsion | Orbital Refueling | High-Efficiency Drives → Pulsed Fusion | Add depot refueling, offer a slower fuel-saving drive, then fit an expedition drive that needs a reactor and radiator budget. |
| Industry | In-Situ Processing | Beamed Power | Convert local ore to port propellant, then support outposts from a transmitter with visible coverage and uptime. |
| Habitats | Closed-Loop Life Support | Rotating Habitats | Reduce colony resupply and qualify a long-duration crewed flight. Later buy population capacity by funding a freight-heavy habitat. |
| Survey and navigation | Interferometry | Deep-Sky Astrometry → Expedition Navigation | Reveal deposits and contacts earlier, obtain an approach solution, and unlock the fleet routing action. |
| Defense | Point Defense | Distributed Fire Control | Escort loadouts can screen haulers, then coordinate fire across the group at additional power and slot cost. |

Cross-links keep these tracks tied to the map. A completed depot enables Orbital Refueling. A surveyed deposit and extraction site enable In-Situ Processing. A supplied colony demonstrates Closed-Loop Life Support. Deep-Sky Astrometry needs an operating survey outpost. Pulsed Fusion needs High-Efficiency Drives and demonstrated propellant supply through either In-Situ Processing or the Ceres berth. Expedition Navigation needs the destination survey solution. Crewed passage needs Closed-Loop Life Support. Beamed power, rotating habitats, and advanced defenses offer reasons to keep playing and should stay optional for first departure.

### One interstellar route end to end

Use an authored sublight route between Sol and the Proxima destination. Show a diegetic calendar duration and a clearly labeled compressed campaign ETA. The path is a strategic flight plan with deterministic phase deadlines. Inside a system, keep local orbital transfers. Use an authored range budget to qualify the route from wet mass, drive class, and propellant. The fleet quote displays the limiting hull, departure and arrival ports, one-way cost, return reserve, projected arrival, and blocked requirements. Allow a one-way expedition only with explicit confirmation and a viable plan for supplies at the destination.

1. **Plan and provision.** Preview from the galaxy map. Reserve fuel at the departure port only on confirmation. Allow cancellation and refund before departure.
2. **Depart.** Move selected hulls into a manifest with stable ship and design revision IDs, loadout, health, cargo, faction, and remaining propellant. Remove them from local ship commands atomically. Leave the rest of the fleet in Sol.
3. **Cruise and brake.** Show progress, phase, and arrival ETA in the outliner. The player can manage Sol while the ship is en route. This first route has fixed intermediate phases and no random destructive event.
4. **Insert and play.** Consume the unique arrival event once, instantiate the fleet in a safe orbit, and offer **Focus arrival**. The scout can survey the receiver, the constructor can repair it, and escorts can protect the work. Allow the return journey after a port or supply site can support it.

Keep a global campaign clock and store the manifest with its phase, due ticks, and event ID. Save and resume before departure, during cruise, and on the arrival tick. Verify that visiting either system, pausing, or changing view does not change outcomes. Verify that a fleet arrives once and its resources are neither duplicated nor lost. Never tie strategic time to the frame delta or current camera.

### UI and pacing

Put **Objectives**, **Research**, **Factions**, and **Fleets** in the same screen hierarchy. Use one pinned next objective that links to the relevant body, ship, queue, or research node. The map shows intent and the inspector answers why. An expedition in transit uses the same order card as a survey or delivery, with actor, phase, ETA, cost, and available action. Critical alerts are clickable and persist in a log. Use a short outcome cue for resolved milestones and continuous music for ongoing play.

Use the capture at 3:00 as the pacing test. The local raiders are cleared and the fleet remains controllable, but the interface only says to continue colony operations. At 7:35 the player can reach Vesta and open its actions, yet the HUD still gives no concrete strategic reason to choose a site. On completion of local defense, surface a near-term task that unlocks the next phase, such as contracting a fuel berth or delivering the first depot shipment. Keep the campaign beats visible without taking the camera away.

## Ship design and command UI

Give the designer its own screen with a named hull and design list, a turntable preview with socket overlays, a filterable component tray, and a stat inspector. Drag a module onto a compatible slot or select the module and click the socket. Show invalid drops and their reason. Start with two meaningful choices per common slot. Resolve reactor output, mass, thermal load, acceleration, delta-v, protection, and production costs from hull and component data. Evaluate modifiers in a documented order, with explicit additive, multiplicative, and clamp stages. Verify the same result in the designer, shipyard quote, simulation, and inspector. Persist designs as IDs and immutable revisions, so edits leave existing builds and ships intact until a player orders a refit.

A ship inspector shows hull and design revision, installed modules, health and shields, propellant, current order, ETA, and next available action. A fleet inspector shows member ships, readiness, range set by the limiting ship, current order, and entry points for split, merge, stance, and service. It should be possible to go from a fleet summary to a single ship and from an order to its target without hunting in the map.

Share layout, hit regions, scroll/clipping, focus, and glyph rendering across the HUD, menus, and designer. Use scalable type, consistent icon families, and a steady map area. Keep condition and progress on the object and explain the details in its panel. Show a single explicit pause state throughout. Rename keeps the game running. The first raid alert is a visible callout and a distinct two-tone warning. Extend it with durable event IDs, per-type coalescing, focus/dismiss actions, and separate alert volume.

## Galaxy size and simulation

Author Sol and one reachable neighboring system. The other four chart entries provide context for the first campaign. Use a separate generated benchmark of 100,000 catalog entries. Keep the catalogue compact and seed unvisited content from stable system IDs. Persist changes for explored or owned systems and active fleets. Run campaign work on a fixed CPU clock with scheduled events for arrivals, production, and research. Skip inactive systems and batch work to the next due tick where the simulation can preserve equivalent results. A fleet in transit has an immutable manifest and a unique arrival event. Consume it exactly once and preserve that rule across saves. Visiting a system changes presentation cadence only.

Keep simulation state in contiguous typed buffers for active tactical workloads. A deterministic CPU path resolves orders, economy, damage, and replay. Cull with a spatial index and feed frame-local visual snapshots to the GPU. Dispatch orbit, traffic, culling, emitter updates, and instance packing in batches through the SDK job API. Qualify any GPU steering or AI against a CPU reference first. Use GPU work for local steering and score candidates in bulk where measured gains justify it. Keep player-visible decisions deterministic and avoid synchronous readback in the frame loop. Use double-precision body/sector coordinates and pack camera-relative floats for rendering.

Measure the campaign and independent stress modes. Target 100,000 charted systems with bounded memory, 10,000 active ships in a separate tactical stress run, and 65,536 decorative instances in a separate GPU stress run. Report CPU simulation and submission time, GPU pass p95, 1% frame-time lows expressed as FPS or p99 frame time, bytes uploaded, and peak working set. Record the adapter, driver, resolution, and preset. Set the frame budget from the RTX 3080 Ti measurements. WARP qualifies correctness.

## Rendering and Houdini delivery

Render the world to a linear FP16 scene target. Use exposure, a soft-threshold bloom extraction, a downsample/upsample pyramid, then tone map into the SDR swap chain. Draw the HUD after the composite. This lets the solar limb, engines, shield hits, and beacons supply light to the same bloom pass. Drive the lens flare from the projected star and a depth-tested visibility estimate with temporal smoothing. Give bloom and flare independent options and validate that hidden stars stop flaring. See [DirectXTK12 post-processing](https://github.com/microsoft/DirectXTK12/wiki/PostProcess).

| Layer | Rendering | First art delivery |
| --- | --- | --- |
| Stars | Camera-relative sphere or disc, limb darkening, animated photosphere, emissive corona | Seamless linear granulation tile, corona mask, subtle flare ghost atlas |
| Rocky worlds and moons | Sphere LODs, Sun-lit BRDF, optional night lights, atmosphere for bodies with air | Earth/Moon/Mars 2:1 albedo, linear elevation or normal map, roughness and water masks where relevant, separate Earth night emissive and cloud layers |
| Gas giants and rings | Layered band textures, differential drift, atmosphere rim, translucent rings with planet shadow | Jupiter and Saturn color/band maps, a radial optical-depth ramp for Saturn’s rings |
| Asteroids and comets | Seeded instances in physically placed belts, body-relative clusters, impostors at distance | Six distinct asteroid silhouettes with three LODs and one material set, one comet nucleus, a dust flipbook |
| Deep sky | Camera-infinite sky cubemap or procedural layers with stable stars and separate foreground particles | A scene-linear EXR cubemap or lat-long Milky Way plate with no foreground planets, plus a star density mask |
| Fleet and industry | PBR instancing with emissive sockets and authored LODs | Six hulls, one modular station kit, dock/exhaust/weapon sockets, material IDs and proxies. Start with scout, corvette, freighter, yard, and outpost. |
| Activity effects | One GPU pool per emitter family, continuous socket emitters and batched burst records | Thruster, survey beam, assembly sparks, shield/impact and debris flipbooks. Export color/opacity or emissive masks and a named event cue for each. |

Use meters, +Y up, bow toward −Z, centered local origins, named sockets, and one export per asset. Keep base color authored in sRGB. Export normals and packed metallic/roughness/occlusion as linear data, with independent emissive. Deliver neutral-light silhouettes and no baked bloom. Keep separate display scales for map readability. Start with uncompressed source textures and preserve the Houdini scene. Bring the EXR sky, map images, and material channels through the planned cooker before adopting them in the packaged sample. The current static GLB path can qualify geometry and base color today. Record attribution and provenance per file for external assets. See the [NASA 3D Resources catalog](https://science.nasa.gov/3d-resources/), [NASA media guidance](https://www.nasa.gov/nasa-brand-center/images-and-media/), and the [static model tutorial](tutorials/07-static-models.md).

An emitter asset should define spawn rate or burst, life and size ranges, curves for scale and color, atlas or mesh, blend mode, and socket binding. Use a versioned, inspectable source file plus a packed runtime record. Validate it in the asset build and preview it in-game before investing in a standalone designer.

## SDK toolkit coverage

| Reference capability | DXSharp today | Next complete path |
| --- | --- | --- |
| DirectXTex texture processing | RGBA sampled texture upload, limited static glTF base color decode | Format-aware mip/subresource upload, DDS including BCn, WIC/common source decode, mip generation, compression, and color-space qualification. Use a pinned offline DirectXTex tool or adapter where it saves duplicated work. |
| DirectXMesh geometry processing | Indexed mesh upload and shared static glTF primitives, with flat normal fallback | Offline normals/tangents, vertex remap and cache optimization, bounds, LOD ingestion, then optional meshlets. Verify packed vertex/index layout against the shaders. Use a pinned DirectXMesh tool or adapter as needed. |
| DirectXTK12 runtime helpers | Core queues, resource/descriptor primitives, D3D12 pipelines, compute job batches, DXR and a desktop host | Reusable sprite/text batching, material/effect presets and samplers, upload batches/transient resources, render targets and post-processing, skinned model/runtime animation, and audio/input helpers. Document defaults and qualify resource lifetime and resize. |

Close texture/mesh cooking, runtime material binding, and the HDR target/composite path first. They unlock the assets above and pay for themselves in samples other than Fleet Battler. Use feature-specific smoke and numerical tests and compare packaged assets to reference fixtures. Track broader toolkit coverage by workflow rather than counting matching method names. [DirectXTex](https://github.com/microsoft/DirectXTex) · [DirectXMesh](https://github.com/microsoft/DirectXMesh) · [DirectXTK12](https://github.com/microsoft/DirectXTK12/wiki)

## Kepler 2.0 data path

Bake a small system catalog in Houdini and keep the source and frame explicit. The orbital-element input retains `a_AU`, `e`, `i_deg`, `Omega_deg`, `w_deg`, `M_deg`, `epoch_jd`, and `P_days`. Record the origin body, time scale, reference frame, and units. Convert axes and handedness once at the import boundary and verify the mapping with a tilted prograde orbit. Keep Horizons elements, Horizons or SPICE state samples, and seeded fictional elements as separate source kinds. Pin the vector query or kernel identity and validity interval in fixture metadata. [JPL ephemerides](https://ssd.jpl.nasa.gov/ephem.html) · [SPICE time systems](https://naif.jpl.nasa.gov/pub/naif/toolkit_docs/C/req/time.html)

For an elliptical orbit, advance time from the epoch, compute mean anomaly, solve Kepler's equation, and convert to true anomaly. Carter's normalized phase is `u = wrap01(ν / τ)`, where `τ = 2π`. Recover the position from the phase and stored orbit geometry. True anomaly advances at a variable rate. Treat phase as a reconstruction coordinate and use time as the simulation input. Direct state samples use a separate bounded interpolation path. Compare the GPU orbit evaluator against a double-precision CPU reference for wraparound, high eccentricity, inclined planes, and parent-relative moon motion. Compare real-system fixtures with source vectors within documented time spans and tolerances.

Keep CPU authority for commands, economy, combat, and replay. Use a GPU evaluator to reconstruct visible celestial transforms and a sector-relative origin for fleet presentation. Rebase in high precision before packing float positions for the GPU. Chunk the CPU SoA by work ranges, give each worker exclusive writes, and commit commands and reductions in stable order. Measure arrays plus spans and SIMD first. Use aligned unmanaged slabs and blittable GPU records where the upload, pinning, or allocation measurements justify them.

## Shared GPU effects

Use the buffer-driven VFX Graph pattern for one persistent emitter pool per effect family. Stream compact burst records for impacts and destruction, and derive continuous thruster emitters from visible ship instances and authored sockets. Keep particle state GPU-resident. Run bounded spawn, update, and compact passes, then use indirect draws to consume the visible counts. Define capacity, overflow policy, and frame-slot lifetime up front. When saturated, preserve priority for gameplay-readable effects and count dropped cosmetic spawns. [Unity VFX GraphicsBuffer binding](https://docs.unity.com/engine/6000.7/script-reference/unityengine/vfx/visualeffect/setgraphicsbuffer)

Start on a single graphics queue with batched submissions. Synchronize where writes are consumed by later UAV passes, shader reads, or indirect argument reads. Use the narrow required resource transitions and avoid per-emitter barriers and CPU readbacks. Try a compute queue only when GPU timestamps show overlap worth the queue handoff, and fence cross-queue dependencies and slot reuse. Record CPU simulation and submission time, GPU pass timings, upload bytes, barrier count, and pool occupancy at 1k, 10k, and 100k synthetic entities. These counts define benchmarks. Establish performance budgets from measured results. [Direct3D 12 enhanced barriers](https://learn.microsoft.com/windows-hardware/drivers/display/enhanced-barriers)

## Asset toolkit

Make FBX and glTF/GLB first-class inputs to a separate asset cooker shipped with the toolkit. Use a pinned importer behind an offline boundary, with [ufbx](https://ufbx.github.io/) as the FBX candidate. Feed both formats into the same mesh, material, skeleton, and clip representation. Keep raw file loading available for tools and development. Release builds load a versioned, validated binary pack with GPU-ready streams, packed indices, compressed textures, bounds, sockets, and baked animation clips. Keep import dependencies out of the graphics runtime package. Support DDS for GPU textures, common image sources such as PNG/JPEG, and static OBJ as a compatibility input. USD can remain a Houdini scene interchange path while the runtime consumes cooked assets.

Qualify FBX with an actual Houdini or Cascadeur export and the same rig exported as GLB. Check coordinate and unit conversion, winding, pivots, skin weights, inverse bind transforms, named clips, clip duration, and pose parity at selected frames. Record exporter version and import flags alongside each fixture. Use bulk transfers over the native boundary and profile mesh upload, clip sampling, and skinning independently. [Houdini FBX character export](https://www.sidefx.com/docs/houdini/nodes/sop/kinefx--rop_fbxcharacteroutput.html) · [Houdini glTF character export](https://www.sidefx.com/docs/houdini/nodes/sop/kinefx--rop_gltfcharacteroutput.html)

## Houdini art list

| Priority | Asset | First delivery |
| --- | --- | --- |
| First playable | Scout, interceptor, mining hauler, and frigate | Four distinct silhouettes from a modular hull kit. Two faction material variants, three LODs per hull, a collision proxy, and named engine, weapon, and docking sockets. |
| First playable | Refinery and mineable asteroid field | One refinery with a docking ring and several seeded asteroid variants. Export separate placement, bounds, and render data so instancing and culling can be tested independently. |
| First playable | Kepler system | One star, a rocky world, a gas giant, and a moon. Deliver catalog and orbit debug geometry alongside scalable body meshes, masks, and atmosphere parameters. |
| First playable | Thrusters, weapon streaks, shield hits, hull impacts, and destruction | Reusable atlases, mesh debris, curves, and emitter presets. Use socket attributes and shared pools so a ship variant adds content without adding an emitter instance. |
| Next asset milestone | Carrier, capital ship, and hangar crew | Carrier and capital variants stress larger hull LODs and socket counts. Export one skinned crew member with idle, locomotion, and a docking gesture in FBX and GLB. |
| Next effect milestone | Warp entry, mining beam, and refinery activity | Author effect parameters and distance LODs. Use these to qualify sustained emitters and scene-scale visibility budgets. |

## Shader capability policy

- **Retail.** Qualify Shader Model 6.9 with the bundled DXC. Samples can keep a lower baseline and opt into newer profiles per pass. Query the adapter and runtime through `GraphicsDevice.GetHighestShaderModel()` and check the feature tier a shader needs separately.
- **Preview.** Shader Model 6.10 is a developer preview. Test it with the matching preview DXC, Agility SDK, and vendor driver in a separate qualification run. Keep the public preview packages on the qualified retail native dependencies.
- **Dependency updates.** Review the [Agility SDK releases](https://devblogs.microsoft.com/directx/directx12agility/) and [DXC releases](https://github.com/microsoft/DirectXShaderCompiler/releases) together. Update the native pins, lock files, and native asset verification in one change. Run WARP, NativeAOT, and target-hardware checks for the affected shaders before promoting the update. Record the compiler, Agility runtime, adapter, driver, and shader profile in the evidence. [Microsoft's Shader Model 6.10 announcement](https://devblogs.microsoft.com/directx/shader-model-6-10-agilitysdk-720-preview/)

Use `dotnet run --project samples/ModernCompute -c Release -- --latest-shader-model` on hardware. Add `--warp` for WARP or `--system-runtime` to test the OS runtime. The probe reports the device ceiling and dispatch profile, compiles a Shader Model 6.9 shader, and validates the selected profile with compute readback.
