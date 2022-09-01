# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2022-09-01
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

| Desperate Devs 0.1.0                                    | Desperate Devs 1.0.0                  |
|:--------------------------------------------------------|:--------------------------------------|
| DesperateDevs.Analytics                                 | -                                     |
| DesperateDevs.CLI.Utils                                 | DesperateDevs.Cli.Utils               |
| DesperateDevs.CodeGeneration                            | Jenny                                 |
| DesperateDevs.CodeGeneration.CodeGenerator              | Jenny.Generator                       |
| DesperateDevs.CodeGeneration.CodeGenerator.CLI          | Jenny.Generator.Cli                   |
| DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor | Jenny.Generator.Unity.Editor          |
| DesperateDevs.CodeGeneration.Plugins                    | Jenny.Plugins                         |
| DesperateDevs.Roslyn.CodeGeneration.Plugins             | Jenny.Plugins.Roslyn                  |
| DesperateDevs.CodeGeneration.Unity.Plugins              | Jenny.Plugins.Unity                   |
| DesperateDevs.Logging                                   | Sherlog                               |
| DesperateDevs.Logging.Appenders                         | Sherlog.Appenders                     |
| DesperateDevs.Logging.Formatters                        | Sherlog.Formatters                    |
| DesperateDevs.Networking                                | TCPeasy                               |
| DesperateDevs.Networking.CLI                            | TCPeasy.Cli                           |
| DesperateDevs.Serialization.CLI.Utils                   | DesperateDevs.Serialization.Cli.Utils |
| DesperateDevs.Utils                                     | DesperateDevs.Caching                 |
| DesperateDevs.Utils                                     | DesperateDevs.Extensions              |
| DesperateDevs.Utils                                     | DesperateDevs.Reflection              |

### DesperateDevs.Caching
- Add `objectPool.Clear()`
- Add `ObjectCache.ObjectPools` to get all object pools

### DesperateDevs.Cli.Utils
- Add `args.IsYes` and `args.IsNo`

### DesperateDevs.Extensions
- Rename `string.LowercaseFirst()` to `string.ToLowerFirst()`
- Rename `string.UppercaseFirst()` to `string.ToUpperFirst()`
- Move `AppDomainExtension` to `DesperateDevs.DesperateDevs.Reflection`

### DesperateDevs.Reflection
- Move `AppDomainExtension` from `DesperateDevs.DesperateDevs.Extensions`

### DesperateDevs.Roslyn
- Add `Microsoft.Build.Locator` 1.4.1
- Update to `Microsoft.CodeAnalysis.CSharp.Workspaces` 4.1.0
- Update to `Microsoft.CodeAnalysis.Workspaces.MSBuild` 4.1.0
- Remove `Sherlog` dependencies

### DesperateDevs.Serialization
- Change to `string.ToCSV(thibool minified, bool removeEmptyEntries)`
- Change to `string.FromCSV(bool removeEmptyEntries)`

### DesperateDevs.Serialization.Cli.Utils
- Change `DumpCommand` format
- Add empty value support in `DumpCommand`
- Remove `SetKeyValueCommand`

### DesperateDevs.Unity
- Add `CoroutineRunner.StopAll()`
- Add non-generic `CoroutineRunner.Run()` without `onComplete` parameter
- Use `UnityEditor.TypeCache.GetTypesDerivedFrom`

### DesperateDevs.Unity.Editor
- Add section state indicator to `PreferencesDrawer`
- Add `ScriptingDefineSymbols.Add`
- Rename to `ScriptingDefineSymbols.AddForAll`
- Add `ScriptingDefineSymbols.Remove`
- Rename to `ScriptingDefineSymbols.RemoveForAll`

### Jenny.Generator.Unity.Editor
- Fix properties path being shared between projects
- Automatically add missing keys to `Jenny.properties`

### Jenny
- Rename `ICodeGenerationPlugin.priority` to `ICodeGenerationPlugin.Order`

### Jenny.Plugins
- Rename to `UpdateCsprojPostProcessor`
- Use unix path separator in `UpdateCsprojPostProcessor`

### Sherlog
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

[Unreleased]: https://github.com/sschmid/DesperateDevs/compare/1.0.0...HEAD
[1.0.0]: https://github.com/sschmid/DesperateDevs/compare/0.1.0...1.0.0
[0.1.0]: https://github.com/sschmid/DesperateDevs/releases/tag/0.1.0
