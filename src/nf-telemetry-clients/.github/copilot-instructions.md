# Copilot Instructions

## Repository Overview

Solution contains **.NET nanoFramework** applications, class libraries, and utility scripts. The nanoFramework targets a subset of the .NET framework, **not** standard .NET. Code runs on resource-constrained ESP32 microcontrollers via the nanoFramework runtime.

## Architecture

- `Clients/`: non related deployable device applications.
- `Peripherals/`: hardware drivers and adapters (sensors, displays, IO, etc).
- `Shared/Client.Communication`: cross-client networking and MQTT communication (M2Mqtt + Azure IoT Hub with TLS certificate auth).
- `Shared/Client.Services`: telemetry services and runtime metrics logic.
- `Shared/Shared`: shared models, configuration reading, and common utilities.
- `Shared/Tests` and `Peripherals/Tests`: nanoFramework tests and benchmarks.

## nanoFramework Constraints
**Critical for code generation — nanoFramework is NOT full .NET:**
- No generics — use `ArrayList` instead of `List<T>`, cast on retrieval (use `nanoFramework.System.Collections` from NuGet).
- No LINQ, no `async`/`await`, no `Span<T>`, no `string` interpolation.
- Limited BCL: `Hashtable` not `Dictionary<K,V>`, `Thread` not `Task`.
- Limited reflection and no dynamic code generation.
- Use `Debug.WriteLine()` for logging (no `Console`).
- All projects target `TargetFrameworkVersion v1.0` in `.nfproj` files.
- **Important** Always add `<Compile Include="..." />` to `.nfproj` when creating new `.cs` files
- Do not use `dotnet build` or standard MSBuild – these projects require the nanoFramework project system extension

## Testing

- Unit tests are are using **nanoFramework.TestFramework**
- Test classes use `[TestClass]` / `[TestMethod]` / `[DataRow]` attributes (similar to MSTest but nanoFramework-specific)
- `Assert` class is from `nanoFramework.TestFramework`
- Tests run on a nanoFramework device or emulator — **not** on the host machine
- The `nano.runsettings` file configures the test runner
- **You cannot run tests locally** in the sandbox
