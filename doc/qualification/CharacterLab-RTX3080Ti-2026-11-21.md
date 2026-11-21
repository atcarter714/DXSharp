# CharacterLab hardware smoke report — 21 November 2026

User-supplied Windows run of the post-PR #40 CharacterLab on an NVIDIA GeForce RTX 3080 Ti:

- Debug build, `net10.0-windows10.0.26100.0`.
- 37,141 vertices, 22 joints, nine animation clips.
- Persistent GPU skinning, two fenced frame slots, horizontal in-place root motion and four material bindings.
- 892 presented frames; exit code zero.
- Reported mean CPU recording time: **0.18 ms**.
- User observation: animation is beautiful and silky smooth.

The console also contained repeated warnings that `ID3D12Device::CheckFeatureSupport` is superseded by `ID3D12Device16::CheckFeatureSupport1`. The follow-up routes Core's shader-model, DXR and texture-format queries through the versioned interface with the SDK constant from the pinned Agility metadata. Older runtimes without that interface retain the original query path. Debug-layer warnings are not suppressed.

The accompanying GitKraken screenshot shows main at `b063eeed5735af5e991715d0eaf4c8a11f92feb6`, whose files match feature head `304623b458484faa7169291ab6f164e9f53dc18b`. The exact executable revision, local working-tree modifications, driver, resolution and display refresh rate were not supplied. This records a successful user hardware smoke run, not a reproducible performance benchmark. CPU recording time does not establish total frame time or GPU time.
