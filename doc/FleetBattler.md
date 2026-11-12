# DXSharp Fleet Battler

Fleet Battler is a Windows strategy game sample for DXSharp. Frontier currently starts at Earth with a science scout, two constructors, and a civilian dock. It has six map systems, local surveys and transfers, timed construction and research, colonization, orbital shipyard queues, corvette fitting, and piracy. The galaxy map can inspect neighboring systems. The command simulation runs on the CPU and GPU compute updates decorative traffic and debris. See the [sample README](../samples/FleetBattler/README.md) for the playable build and controls.

## When to stop developing the sample

Freeze feature development once the Windows sample demonstrates the complete workflow below and the SDK features it uses have reusable entry points. From that point, work on bugs, readability, balance, performance regressions, packaging, and documentation until the sample qualifies. Further game features can live in a separate game repository. Keep the sample in DXSharp as a maintained reference for the SDK.

| Milestone | Scope | Gate |
| --- | --- | --- |
| Feature freeze | One authored campaign in Sol, the existing skirmish, and a chart of neighboring systems. At most six hulls, eight structures, and twelve research nodes. Include menus, save/load, a working module designer, inspectors, alerts, and a complete art and effects pass for the campaign. | A first-time player can survey, build, resolve the convoy crisis, design and commission a ship, and complete the campaign using only the game UI. A player can quit and continue the same run. |
| Stabilization | Fix crashes, lost state, progression blockers, ambiguous orders, rendering and audio defects, and measured hot paths. | Repeat the campaign from a clean install. Resume saves made during construction, production, and a scenario choice. Qualify on WARP and hardware and run Windows Debug/Release and NativeAOT CI. Record one reproducible stress run and its frame-time and memory results. |
| Sample maintenance | Keep the documented Windows build, controls, asset provenance, and sample working as DXSharp evolves. | SDK changes that affect the sample pass its smoke tests. New content, grand strategy mechanics, and an expanded galaxy are independent project work. |

A playable second system, a huge simulated galaxy, a standalone VFX editor, and complete toolkit parity are separate milestones. A controlled interstellar departure can be the campaign payoff. Use an epilogue for the arrival if it would otherwise require a second simulation and UI pass before feature freeze.

## The campaign

Build **The First Crossing** as a compact campaign. Use 30–45 minutes as an initial pacing estimate to test. Start with player presence in Sol only. A coalition on Earth charters a first permanent off-world settlement while an independent belt compact controls logistics and raiders skim unescorted shipments. Survey the Moon or Mars, establish a colony, and outfit an escort or negotiate convoy coverage. After the colony reaches sustained operation, authorize a fusion-powered expedition bound for Alpha Centauri. Make the two approaches to the convoy crisis affect what the player can afford, who offers aid, and what condition the expedition launches in. Keep a small post-victory sandbox and the existing skirmish.

Six hulls are enough for the scenario. Each has a distinct job and an identifiable silhouette at tactical zoom. Four utilities teach the economy. Two military hulls create a choice between cheap coverage and concentrated defense.

| Hull | Job | Slot emphasis |
| --- | --- | --- |
| Science scout | Surveys and anomaly investigation | Sensor, avionics, drive, reactor |
| Constructor | Builds and services orbital sites | Fabricator, utility, drive, reactor |
| Freighter | Hauls ore and propellant between owned or allied ports | Cargo, radiator, drive, reactor |
| Colony ship | Carries crew and habitat equipment to a surveyed solid body | Habitat payload, life support, drive, reactor |
| Corvette | Fast intercept and convoy escort | Small weapon, armor or shield, drive, reactor |
| Frigate | Protects convoys and stations against heavier threats | Medium weapon, point defense, radiator, drive, reactor |

The first complete construction catalog needs an extraction rig, solar array, research lab, storage depot, orbital shipyard, survey outpost, defense platform, and habitat. Give the defense platform a limited coverage radius and ongoing energy cost. A depot adds storage and a resupply stop. A survey outpost extends detection and route knowledge. Surface buildings need a settled solid body with available labor and slots. An O’Neill habitat is a late milestone that consumes freight, energy, and construction time and creates housing and new economic capacity. Gas giants support orbital infrastructure.

Use the following checks for the feature freeze and stabilization milestones.

| Area | Accept when |
| --- | --- |
| Session | A small loading screen reports real initialization progress. Main menu offers New, Continue, Load, Options, Quit. Pause menu offers Resume, Save, Load, Options, Return to Menu, Quit. Quit is reachable entirely through the UI. |
| Persistence | A versioned save round-trips bodies, fleets, names, designs, inventory, orders, colony work, tech, queues, faction decisions, campaign tick, and travel manifests. Save via a temporary file in the same directory and atomic replacement. Restore from a checkpoint mid-construction and before the expedition launch with identical completion outcomes. When interstellar transit becomes playable, add a mid-transit round trip. Keep user settings separate. |
| Orders | Every long-running action identifies actor, target, phase, ETA or progress, and cancelability. Route lines follow the live rendezvous solution. Invalid orders explain the blocker. Critical notices show on the map and persist in the log. Selecting a notice focuses the relevant object where one exists. |
| Shipyard and inspector | A player can design, validate, name, order, inspect, repair, and refit one hull end to end. A queue item preserves the purchased design revision and quoted cost and time. The planet panel shows built sites and pending work. Construction catalogs open separately. |
| Content and pacing | Two routes through the convoy crisis are viable. The first resource chain, first unlock, and first escalation each have time to read and act at 1×. A playtest reaches the crossing without external instructions or waiting for a fleet to spawn on its own. |
| Presentation | Body labels, alerts, route markers, and panels remain readable at 1080p and 4K. HDR bloom, thrusters, survey/assembly effects, and hostile audio make the map legible. Run on Windows hardware and retain functional WARP checks. |

Defer full diplomacy, ship interiors, tactical boarding, fully simulated populations across the galaxy, more hull classes, a standalone VFX editor, and speculative megastructures until this checklist passes. The game’s completion and the SDK’s toolkit coverage have separate acceptance criteria.

## Research and campaign progression

Use a directed acyclic graph with explicit prerequisites, a single active project, a visible queue, and science output as work per tick. The panel shows what a node unlocks, what it costs, time at the current output, and which lab, survey, or demonstration is gating it. Introduce 12 nodes across five disciplines and reveal them in tiers as the colony develops. Make early research complete during an active survey or build. Tune later projects so a second lab or a better route changes a meaningful wait. Unlock components, capabilities, and operating envelopes so every branch changes a decision.

| Discipline | Nodes in order | Tradeoff |
| --- | --- | --- |
| Propulsion | Orbital Refueling → High-Efficiency Drives → Pulsed Fusion | Refueling extends repeat trips. Efficient drives save propellant but trade away thrust. Fusion needs a stronger power and heat budget. |
| Industry | In-Situ Processing → Beamed Power | Process local material and replace some fuel deliveries with power transmitters. A transmitter creates a new vulnerable route endpoint. |
| Habitats | Closed-Loop Life Support → Rotating Habitats | Sustain a colony with fewer supply runs. A larger spin habitat trades construction material and upkeep for population and industrial capacity. |
| Sensing and navigation | Interferometry → Deep-Sky Astrometry → Expedition Navigation | Reveal deposits and incoming contacts earlier, survey a neighboring system, and unlock flight plans for a fusion-equipped expedition. |
| Defense | Point Defense → Distributed Fire Control | Intercept incoming ordnance or improve several ships’ coordination. Both consume power and module space. |

**First act.** Survey and develop a small resource chain near Earth. Commit a constructor to a site and see it arrive, assemble the site, and turn it online. Put a scout on the next target and let the survey reveal a route or anomaly.

**Second act.** Choose whether to accept the belt compact’s terms for resupply, commit your own escorts, or invest in an outpost and defense platform. A successful raid costs cargo and energy and puts a new task in the log. It leaves a viable recovery path at Earth. Use a scenario dispatch with an illustration, a concise statement of stakes, explicit costs and likely effects, and a durable record of the decision.

**Third act.** Establish a self-sustaining colony and commission the expedition after researching Pulsed Fusion and Expedition Navigation. End the scenario with a sublight departure. Use the years of transit as part of the epilogue. If the later interstellar simulation runs that passage, process scheduled work on the campaign clock and draw attention to major changes. Beamed sails, autonomous industry, and rotating habitats can seed follow-on scenarios. Paired wormhole gates and other conjectural technologies belong on a later, explicitly speculative branch.

## Ship design and command UI

Give the designer its own screen with a named hull and design list, a turntable preview with socket overlays, a filterable component tray, and a stat inspector. Drag a module onto a compatible slot or select the module and click the socket. Show invalid drops and their reason. Start with two meaningful choices per common slot. Resolve reactor output, mass, thermal load, acceleration, delta-v, protection, and production costs from hull and component data. Evaluate modifiers in a documented order, with explicit additive, multiplicative, and clamp stages. Verify the same result in the designer, shipyard quote, simulation, and inspector. Persist designs as IDs and immutable revisions, so edits leave existing builds and ships intact until a player orders a refit.

A ship inspector shows hull and design revision, installed modules, health and shields, propellant, current order, ETA, and next available action. A fleet inspector shows member ships, readiness, range set by the limiting ship, current order, and entry points for split, merge, stance, and service. It should be possible to go from a fleet summary to a single ship and from an order to its target without hunting in the map.

Share layout, hit regions, scroll/clipping, focus, and glyph rendering across the HUD, menus, and designer. Use scalable type, consistent icon families, and a steady map area. Keep condition and progress on the object and explain the details in its panel. Show a single explicit pause state throughout. Rename keeps the game running. The first raid alert is a visible callout and a distinct two-tone warning. Extend it with durable event IDs, per-type coalescing, focus/dismiss actions, and separate alert volume.

## Galaxy size and simulation

Author Sol and five charted destinations for the campaign. Build one reachable neighboring system when interstellar play is developed. Use a separate generated benchmark of 100,000 catalog entries. Keep the catalogue compact and seed unvisited content from stable system IDs. Persist changes for explored or owned systems and active fleets. Run campaign work on a fixed CPU clock with scheduled events for arrivals, production, and research. Systems with economic activity tick at a coarser rate when offscreen. A fleet in transit has an immutable manifest and a unique arrival event. Consume it exactly once and preserve that rule across saves. Visiting a system changes presentation cadence only.

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
