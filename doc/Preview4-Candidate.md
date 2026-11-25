# Preview 4 candidate cutoff

The intended package version is `0.4.0-preview.4`. The feature cutoff combines the modern Framework/character workload, texture transfers, shader build tooling, native/WinForms/WPF/WinUI hosts and the first reusable Media Foundation playback path. CharacterLab variants remain shared host regression tests. DirectStorage, current GameInput, X3DAudio and the visual DXR scene continue in later bounded iterations.

The release manifest contains ten modern package IDs, including the optional `DXSharp.Media`, for twenty package/symbol archives. Public release preparation must retain the exact qualifying commit and archive hashes. Legacy packages remain separate compatibility artifacts.

Before publication:

- Complete Windows Debug/Release source and isolated package qualification on .NET 10/11, including VideoPlayer.
- Pass native COM JIT/NativeAOT gates, native API coverage and shader package checks.
- Retest both WinUI samples on affected hardware: no VideoPlayer `0xC000027B` startup/activation failure, no CharacterLab move/resize freeze, successful repeated close/reopen and monitor DPI changes. Preserve diagnostic logs for any recurrence.
- Review Media Lab on hardware with its audio-bearing fixture: play/pause/seek/loop, correct audio/video alignment, resize and repeated open/close. Record actual decoder/transfer behavior and unsupported cases.
- Document Framework's breaking migration and the JIT-only qualification of optional desktop/media workloads.
- Finalize matching versioned release notes only when the candidate is approved for publication; merging a versioned release-note file triggers the release workflow.
- Qualify and publish the immutable candidate, then verify every public NuGet package with a fresh public-feed consumer.

The generated Media Foundation/D3D12 video inventory is distinct from runtime-qualified playback. Direct D3D12 video commands, zero-copy video transfer and independently measured audio-output latency are not claims of this candidate.

## Accepted code cutoff

[PR #48](https://github.com/atcarter714/DXSharp/pull/48) passed [all six CI jobs](https://github.com/atcarter714/DXSharp/actions/runs/45662746089), and Aaron accepted functionality/stability on Roci on 2026-11-25. See the [hardware record](qualification/WinUI-RTX3080Ti-2026-11-25.md). WinUI fractional slider increments are deferred to [#49](https://github.com/atcarter714/DXSharp/issues/49). The versioned [Preview 4 notes](releases/0.4.0-preview.4.md) initiate fresh qualification and publication; acceptance is not a claim that the new public-feed packages have already been published or verified.
