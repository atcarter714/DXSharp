# DXSharp 🖥️🎮

![DXSharp](./file/img/DXSharp_Logo_00_512.png)

**Modern DirectX 12 for .NET 7 & 8**

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE) [![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

---

> **Disclaimer:**  
> DXSharp is currently in an experimental stage of development. 
> While it serves as a proof of concept and a powerful tool for graphics experimentation, API coverage is incomplete, and it is **not yet recommended for production** use. Contributions, feedback, and testing are greatly appreciated as we move toward a more mature release. This project will require support from others to be fully brought to fruition!

"DXSharp" can be viewed as a large-scale set of messy but successful graphics experiments with DirectX 12 and .NET. It is massive in size and was inspired by the "glory days" of projects like [SlimDX](https://github.com/SlimDX/slimdx) and [SharpDX](https://github.com/sharpdx/SharpDX) which attempted to provide idiomatic and elegant C#/.NET bindings for DirectX. These experiments were built and conducted by a single developer ([Aaron Carter](https://github.com/atcarter714)) with a limited amount of time. It is far from perfect but serves as a proof of concept for a future vision of DirectX 12 programming with C#, for things such as game/engine development, simulation and even machine-learning.

---

## Overview 🚀

**DXSharp** is a lightweight C# wrapper for the DirectX 12 API, designed specifically for .NET 8. This project provides high-performance access to DirectX for real-time 3D applications, game engines, and more. Originally started as a prototype under the codename *DXSharp*, this library bridges the gap for experienced .NET developers looking to work directly with modern graphics APIs while taking full advantage of .NET's powerful features. It leverages the power of the [CsWin32](https://github.com/microsoft/CsWin32) code generator packages, to generate interop signatures and type definitions which can be hand-crafted into a well-organized, idiomatic wrapper library.

## Why DXSharp? 🤔

- **⚡ Lightweight & Modern:** Tailored for .NET 8 and C# programmers, DXSharp aims to use modern language features and implement a lightweight and modular design. Modern .NET performance has made the idea of using C# and .NET in the game industry something worthy of serious consideration!
- **🔗 Discontinuations:** With the discontinuation of **SharpDX**, **SlimDX**, **XNA**, and **Managed DirectX**, more contemporary DirectX 12 solutions are desired in the .NET ecosystem.
- **🛠️ Low-Level Graphics Access:** For developers who want full control over rendering pipelines without the overhead of heavy game engines like Unity or Unreal.
- **📚 Learning & Experimentation:** Encourages developers to dive into 3D programming and low-level graphics development—something often abstracted away by mainstream commercial engines.
- **🚀 Build Faster Engines:** Create faster, more streamlined game engines, rendering systems, and tools with C# and DirectX 12.

## Key Features ✨

- Support for most of the **DirectX 12 API** needed for 3D applications and games.
- High-performance managed code interface 🎯
- Direct access to Direct3D, DXGI, and more
- 🚧 *In Development*: Support for DirectCompute, Raytracing, and DXC compiler, more samples and a high-level framework for fast development of custom engines and 3D apps.

## Getting Started 🌟

### Prerequisites
- **.NET 8 SDK** installed
- **Windows 10/11** with DirectX 12 capable hardware

### Installation

1. **Clone the repository:**
    - git clone https://github.com/atcarter714/DXSharp.git

2. Once you've checked out the repo, you can simply build the solution and run the "Hello Triangle" sample application.

## Roadmap 🛤️

- [x] Initial DirectX 12 support
- [x] DXGI support
- [x] Azure DevOps build pipelines and scripts
- [ ] DXC Advanced Shader support
- [ ] Extensive sample projects (game engine demos, tools, etc.)

## Contributing 🤝

Contributions are welcome! Please read the [contributing guidelines](CONTRIBUTING.md) before submitting a pull request. If you are unable to contribute directly to development but would like to see this become a production-ready DirectX solution for the .NET ecosystem, please reach out to discuss possibilities of sponsorship and/or donation.

## License 📄

This project is licensed under the [MIT License](./license.md).

---

</br>

**Note from the developer**: 

DXSharp was an attempt to prove that a full-scale DirectX 12 solution for C#/.NET would be feasible and that it could be implemented in a familiar and idiomatic style for .NET developers. It includes tons of supplementary features, ranging from an Azure DevOps build pipelines, local scripts (Powershell, CSX, etc), customized MSBuid processes and scripts and more. Although there are some flaws in the initial design and approach and things are a bit messy, I feel like it was overwhelmingly successful as an experimental proof of concept. DirectX 12 is absolutely massive, a highly complex beast, and this project required a deep understanding of both C/C++ and C# for the interoperation. With the help of modern .NET capabilities and packages like CsWin32, I was able to tame it single-handedly and create the sort of proof of concept I was after.

DXSharp has a few design flaws that must be addressed to turn it into a production-ready solution. In my haste to get it working, I took an admittedly bad approach in the way the DirectX/DXGI interfaces are designed and then implemented by wrapper classes. It was a choice I came to regret as it grew uglier and more difficult to work on. While it does work just fine, this will have to be addressed to continue development longer-term. I had plans to do this myself before releasing the solution, but as a single father staying at home with a 2.5-year-old son, my financial and domestic situation has gotten in the way. Thus, I decided to just make the project public "as-is" and let it fly. I hope that it will at least be a valuable learning resource for others, or possible find a small community of people interested in turning it into a serious building block for game engines and games in the .NET ecosystem.

If able to continue development on this project, I have several plans in mind, including:
- Train a SLM ("small language model") on DirectX 12 and the code base
- Utilize .NET 8's new COM interop features
- Cleaner and more optimized implementation
- Create custom Roslyn generators/analyzers
- Provide unmanaged struct implementations

___


