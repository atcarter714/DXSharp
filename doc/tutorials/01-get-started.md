# Create your first DXSharp application

Use a Windows x64 machine with the SDK version selected in [global.json](../../global.json). If you have the source checkout, run `./build.ps1 -Configuration Release` from an x64 Visual Studio Developer PowerShell. That builds and validates the packages under `artifacts/packages/Release`. You can also download the Release package artifact from a qualified CI run.

In a separate directory, create `FirstGpuApp.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0-windows10.0.26100.0</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>false</SelfContained>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="DXSharp.Core" Version="0.4.0-net10-preview.1" />
  </ItemGroup>
</Project>
```

Create `Program.cs`:

```csharp
using DXSharp.Graphics;

using var device = GraphicsDevice.Create(new() { UseWarp = args.Contains("--warp") });
Console.WriteLine(device.AdapterName);
```

Restore with the downloaded or locally built packages and NuGet.org for their dependencies. Set `$packageDirectory` to the actual artifact directory on disk.

```powershell
dotnet restore --source $packageDirectory --source https://api.nuget.org/v3/index.json
dotnet run --no-restore -- --warp
```

The program prints the software adapter name. Omit `--warp` to let DXSharp choose a hardware adapter. The packaged Agility SDK is enabled by default. The application owns the device, so use `using` or a longer-lived owner for it and dispose objects after their final CPU use.

Next, [present a buffered frame](02-buffered-rendering.md).
