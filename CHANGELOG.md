# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Jenny
- Extract TCPeasy to it's own repo: https://github.com/sschmid/Jenny

### Sherlog
- Extract Sherlog to it's own repo: https://github.com/sschmid/Sherlog

### TCPeasy
- Extract TCPeasy to it's own repo: https://github.com/sschmid/TCPeasy

### Other
- Restructure project using tests folder

### bee
- Remove bee

### [DesperateDevs.Caching/1.1.0] - 2022-09-26
- Add non-alloc `pool.Drain(buffer)`

### [DesperateDevs.Serialization/2.0.0] - 2022-09-26
- Add `preferences.Minified` bool

### [DesperateDevs.Serialization.Cli.Utils/1.0.1] - 2022-09-26
- Set `preferences.Minified` bool in `FormatCommand`

### [DesperateDevs.Unity.Editor/2.0.0] - 2022-09-26
- Set `preferences.Minified` bool in `PreferencesWindow`

### Other
- Convert concatenation to interpolation
- Remove Unity-2020.3 project
- Update packages

### bee
- Upgrade to bee 1.2.0
- Add `desperatedevs::pack`
- Add `desperatedevs::publish`

## 1.0.0 - 2022-09-01
### Notes
- Desperate Devs 1.0 is now open-source
- The whole project has been updated to use the official [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions) resulting in breaking changes. Most common breaking changes are due to using uppercase for fields and properties
- The migration to a modern SDK-style project structure using [.NET project SDKs](https://docs.microsoft.com/en-us/dotnet/core/project-sdk/overview) is complete
- All projects have been updated to `.NET 6.0` and `.NET Standard 2.1`.
- All unit tests have been fully migrated to [xunit](https://xunit.net) away from [nspec](http://nspec.org)

### Upgrade
- Remove all `using DesperateDevs.Utils;` and fix imports by using `DesperateDevs.Caching`, `DesperateDevs.Extensions` or `DesperateDevs.Reflection`
- `AppDomainExtension` is now under the `DesperateDevs.DesperateDevs.Reflection` namespace
- Update calls to `ToCSV()` to `string.ToCSV(bool minified, bool removeEmptyEntries)`
- Update calls to `FromCSV()` to `string.FromCSV(bool removeEmptyEntries)`
- Remove `<T>` when calling `CoroutineRunner.Run()` without `onComplete` parameter
- Update `Jenny.properties` and rename to `UpdateCsprojPostProcessor`
- Rename `fabl` to `Sherlog`
- Rename to `Logger.ClearAppenders()` and `Logger.ClearLoggers()`
- Rename `ICodeGenerationPlugin.priority` to `ICodeGenerationPlugin.Order`
- Rename `string.LowercaseFirst()` to `string.ToLowerFirst()`
- Rename `string.UppercaseFirst()` to `string.ToUpperFirst()`
- Create explicit logger because static logger (e.g. `fabl.Debug()`) has been removed
- Some namespaces have been renamed, some have been extracted to their own namespace, see table:

| Desperate Devs 0.1.0                                    | Desperate Devs 1.0.0                                             |
|:--------------------------------------------------------|:-----------------------------------------------------------------|
| DesperateDevs.Analytics                                 | -                                                                |
| DesperateDevs.CLI.Utils                                 | DesperateDevs.Cli.Utils                                          |
| DesperateDevs.CodeGeneration                            | [Jenny](https://github.com/sschmid/Jenny)                        |
| DesperateDevs.CodeGeneration.CodeGenerator              | [Jenny.Generator](https://github.com/sschmid/Jenny)              |
| DesperateDevs.CodeGeneration.CodeGenerator.CLI          | [Jenny.Generator.Cli](https://github.com/sschmid/Jenny)          |
| DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor | [Jenny.Generator.Unity.Editor](https://github.com/sschmid/Jenny) |
| DesperateDevs.CodeGeneration.Plugins                    | [Jenny.Plugins](https://github.com/sschmid/Jenny)                |
| DesperateDevs.Roslyn.CodeGeneration.Plugins             | [Jenny.Plugins.Roslyn](https://github.com/sschmid/Jenny)         |
| DesperateDevs.CodeGeneration.Unity.Plugins              | [Jenny.Plugins.Unity](https://github.com/sschmid/Jenny)          |
| DesperateDevs.Logging                                   | [Sherlog](https://github.com/sschmid/Sherlog)                    |
| DesperateDevs.Logging.Appenders                         | [Sherlog.Appenders](https://github.com/sschmid/Sherlog)          |
| DesperateDevs.Logging.Formatters                        | [Sherlog.Formatters](https://github.com/sschmid/Sherlog)         |
| DesperateDevs.Networking                                | [TCPeasy](https://github.com/sschmid/TCPeasy)                    |
| DesperateDevs.Networking.CLI                            | [TCPeasy.Cli](https://github.com/sschmid/TCPeasy)                |
| DesperateDevs.Serialization.CLI.Utils                   | DesperateDevs.Serialization.Cli.Utils                            |
| DesperateDevs.Utils                                     | DesperateDevs.Caching                                            |
| DesperateDevs.Utils                                     | DesperateDevs.Extensions                                         |
| DesperateDevs.Utils                                     | DesperateDevs.Reflection                                         |

### [DesperateDevs.Caching/1.0.0] - 2022-09-01
- Add `objectPool.Clear()`
- Add `ObjectCache.ObjectPools` to get all object pools

### [DesperateDevs.Cli.Utils/1.0.0] - 2022-09-01
- Add `args.IsYes` and `args.IsNo`

### [DesperateDevs.Extensions/1.0.0] - 2022-09-01
- Rename `string.LowercaseFirst()` to `string.ToLowerFirst()`
- Rename `string.UppercaseFirst()` to `string.ToUpperFirst()`
- Move `AppDomainExtension` to `DesperateDevs.DesperateDevs.Reflection`

### [DesperateDevs.Reflection/1.0.0] - 2022-09-01
- Move `AppDomainExtension` from `DesperateDevs.DesperateDevs.Extensions`

### [DesperateDevs.Roslyn/1.0.0] - 2022-09-01
- Add `Microsoft.Build.Locator` 1.4.1
- Update to `Microsoft.CodeAnalysis.CSharp.Workspaces` 4.1.0
- Update to `Microsoft.CodeAnalysis.Workspaces.MSBuild` 4.1.0
- Remove `Sherlog` dependencies

### [DesperateDevs.Serialization/1.0.0] - 2022-09-01
- Change to `string.ToCSV(thibool minified, bool removeEmptyEntries)`
- Change to `string.FromCSV(bool removeEmptyEntries)`

### [DesperateDevs.Serialization.Cli.Utils/1.0.0] - 2022-09-01
- Change `DumpCommand` format
- Add empty value support in `DumpCommand`
- Remove `SetKeyValueCommand`

### [DesperateDevs.Unity/1.0.0] - 2022-09-01
- Add `CoroutineRunner.StopAll()`
- Add non-generic `CoroutineRunner.Run()` without `onComplete` parameter
- Use `UnityEditor.TypeCache.GetTypesDerivedFrom`

### [DesperateDevs.Unity.Editor/1.0.0] - 2022-09-01
- Add section state indicator to `PreferencesDrawer`
- Add `ScriptingDefineSymbols.Add`
- Rename to `ScriptingDefineSymbols.AddForAll`
- Add `ScriptingDefineSymbols.Remove`
- Rename to `ScriptingDefineSymbols.RemoveForAll`

### Jenny 1.0.0 - 2022-09-01
- Rename `ICodeGenerationPlugin.priority` to `ICodeGenerationPlugin.Order`

### Jenny.Generator.Unity.Editor 1.0.0 - 2022-09-01
- Fix properties path being shared between projects
- Automatically add missing keys to `Jenny.properties`

### Jenny.Plugins 1.0.0 - 2022-09-01
- Rename to `UpdateCsprojPostProcessor`
- Use unix path separator in `UpdateCsprojPostProcessor`

### Sherlog 1.0.0 - 2022-09-01
- Add `ConditionalAttribute` to `Logger`
- Fix `Logger.ResetAppenders()` not being applied to existing loggers
- Rename to `Logger.ClearAppenders()` and `Logger.ClearLoggers()`
- Remove static logger

### Other
- Move project configuration to `Directory.Build.targets`
- Add .editorconfig
- Add `InspectionSettings.DotSettings`
- Add solution tests
- Update to `FluentAssertions` 6.5.1
- Update Unity version to 2021.3.0f1
- Update CodeStyle
- Update Dockerfile
- Use multi-stage docker builds
- Simplify `Unity3D.props`
- Move `*.CLI` to `*.Cli` namespace
- Remove Unity-2018.4
- Remove Unity-2019.4
- Remove `DesperateDevs.Analytics`
- Remove mono hosted msbuild

### bee
- Migrate to bee 1.1.0
- Add `desperatedevs::help`
- Add `desperatedevs::build`
- Add `desperatedevs::new`
- Add `desperatedevs::new_benchmark`
- Add `desperatedevs::sync_unity_solutions`
- Add `desperatedevs::generate_unity_packages`
- Add `desperatedevs::publish_local`
- Rename many desperatedevs functions (see help)
- Restore Unity with dlls instead of source code
- Move plugins to `Jenny` folder instead of `Plugins`
- Move coverage to build
- Delete symlink in Unity test projects and copy and ignore instead

## [0.1.0] - 2021-09-16

[Unreleased]: https://github.com/sschmid/DesperateDevs/compare/DesperateDevs.Caching/1.1.0...HEAD
[DesperateDevs.Caching/1.1.0]:https://github.com/sschmid/DesperateDevs/compare/DesperateDevs.Caching/1.0.0...DesperateDevs.Caching/1.1.0
[DesperateDevs.Serialization/2.0.0]:https://github.com/sschmid/DesperateDevs/compare/DesperateDevs.Serialization/1.0.0...DesperateDevs.Serialization/2.0.0
[DesperateDevs.Serialization.Cli.Utils/1.0.1]:https://github.com/sschmid/DesperateDevs/compare/DesperateDevs.Serialization.Cli.Utils/1.0.0...DesperateDevs.Serialization.Cli.Utils/1.0.1
[DesperateDevs.Unity.Editor/2.0.0]:https://github.com/sschmid/DesperateDevs/compare/DesperateDevs.Unity.Editor/1.0.0...DesperateDevs.Unity.Editor/2.0.0
[DesperateDevs.Caching/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...DesperateDevs.Caching/1.0.0
[DesperateDevs.Cli.Utils/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...DesperateDevs.Cli.Utils/1.0.0
[DesperateDevs.Extensions/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...DesperateDevs.Extensions/1.0.0
[DesperateDevs.Reflection/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...DesperateDevs.Reflection/1.0.0
[DesperateDevs.Roslyn/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...DesperateDevs.Roslyn/1.0.0
[DesperateDevs.Serialization/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...DesperateDevs.Serialization/1.0.0
[DesperateDevs.Serialization.Cli.Utils/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...DesperateDevs.Serialization.Cli.Utils/1.0.0
[DesperateDevs.Threading/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...DesperateDevs.Threading/1.0.0
[DesperateDevs.Unity/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...DesperateDevs.Unity/1.0.0
[DesperateDevs.Unity.Editor/1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0..DesperateDevs.Unity.Editor/1.0.0
[0.1.0]: https://github.com/sschmid/DesperateDevs/releases/tag/0.1.0
