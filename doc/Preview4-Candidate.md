# Preview 4 candidate cutoff

The intended package version is `0.4.0-preview.4`. The feature cutoff combines the modern Framework/character workload, texture transfers, shader build tooling, native/WinForms/WPF/WinUI hosts and the first reusable Media Foundation playback path. CharacterLab variants remain shared host regression tests. DirectStorage, current GameInput, X3DAudio and the visual DXR scene continue in later bounded iterations.

The release manifest contains ten modern package IDs, including the optional `DXSharp.Media`, for twenty package/symbol archives. Public release preparation must retain the exact qualifying commit and archive hashes. Legacy packages remain separate compatibility artifacts.

Before publication:

- Complete Windows Debug/Release source and isolated package qualification on .NET 10/11, including VideoPlayer.
- Pass native COM JIT/NativeAOT gates, native API coverage and shader package checks.
- Review Media Lab on hardware with its audio-bearing fixture: play/pause/seek/loop, correct audio/video alignment, resize and repeated open/close. Record actual decoder/transfer behavior and unsupported cases.
- Document Framework's breaking migration and the JIT-only qualification of optional desktop/media workloads.
- Finalize matching versioned release notes only when the candidate is approved for publication; merging a versioned release-note file triggers the release workflow.
- Qualify and publish the immutable candidate, then verify every public NuGet package with a fresh public-feed consumer.

The generated Media Foundation/D3D12 video inventory is distinct from runtime-qualified playback. Direct D3D12 video commands, zero-copy video transfer and independently measured audio-output latency are not claims of this candidate.
