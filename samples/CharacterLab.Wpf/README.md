# CharacterLab.Wpf

Run `dotnet run --project samples/CharacterLab.Wpf` from the repository root. This sample shares the existing CharacterLab animation, GPU skinning and camera sources, adding native WPF playback controls and an optional `DXSharp.Framework.Wpf` viewport. Click the viewport for camera/clip shortcuts; edit the text field to verify focus isolation.

Use `--warp --frames=9` for bounded presentation and `--warp --host-test` for lifecycle/focus/resize checks. Select .NET 11 with `-p:DXSharpRuntimeMajor=11`. The sample joins the standard isolated package-consumption gate. See [the WPF host contract](../../doc/Wpf-Hosting.md) for ownership, teardown, DPI, airspace and support limits.
