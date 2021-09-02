https://github.com/dotnet/roslyn

Install packages:
Microsoft.Net.Compilers
Microsoft.CodeAnalysis
  - Microsoft.CodeAnalysis.CSharp.Workspaces (use this instead)

Fix, after roslyn update:
Microsoft.CodeAnalysis.Workspaces.MSBuild

Copy Mono Hosted MSBuild into bin/Release
https://github.com/Microsoft/msbuild/releases/tag/mono-hosted-msbuild-v0.03
