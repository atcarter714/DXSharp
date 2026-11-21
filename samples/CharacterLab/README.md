# CharacterLab

A GPU-skinned animated knight built on DXSharp.Assets, Framework and Framework.Desktop. The sample uses the same in-repo project references and isolated package-consumption qualification as every other runnable sample.

Build and run:

```powershell
dotnet run --project samples/CharacterLab -c Release
# Bounded WARP qualification; cycles through different clips.
dotnet run --project samples/CharacterLab -c Debug -- --warp --frames=9
# Select the other supported runtime.
dotnet run --project samples/CharacterLab -c Release -p:DXSharpRuntimeMajor=11
```

Drag to orbit, +/- to zoom, 1–9 to select a clip, Space to pause, R to return to idle, and [ / ] to change playback speed. Clips 1–5 loop; action clips hold their final pose. Horizontal root motion is kept in place, preserving vertical movement such as jumps. Animation is sampled every rendered frame. Persistent GPU geometry replaces repeated mesh construction; only the frame-owned joint/normal palette changes.

Base color is sampled as sRGB; normal, metallic/roughness and occlusion images are linear shader inputs. This first renderer uses generated tangent frames, linear repeat sampling and one supplied mip per image. It is a bounded character/material workload, not a complete glTF or PBR implementation. See [framework direction and migration](../../doc/Framework-Vision.md).

The existing native fixture tests the real model's clips and compares GPU and CPU position/normal results across 27 poses. It also checks antipodal rotations, interrupted fades, one-shot playback, root-motion loops, clock behavior and allocation-free pose/palette evaluation. GPU checks run on Windows; portable tests are not hardware performance evidence.

The supplied model, credits and CC0 license are under Assets. The original credits describe seven clips; this supplied GLB contains nine. The asset manifest records the actual archive contents and hash. No model download is needed to run this checkout.
