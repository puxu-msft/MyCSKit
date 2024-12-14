
## ADO Feeds

https://github.com/microsoft/artifacts-credprovider#setup

```ps1
iex "& { $(irm https://aka.ms/install-artifacts-credprovider.ps1) } -AddNetfx"
```

## 多目标

MSBuild 可以生成多目标，但一个 .NET Project 在一次构建时只能指定一个 TFM + Target Platform。

[Target Framework Moniker][TFM] 即 `TargetFramework` 如 `net45` `netcoreapp3.1` `net6.0`。

[TFM]: https://learn.microsoft.com/en-us/dotnet/standard/frameworks

`<TargetFrameworks>` 用于生成多目标，分别位于 `$(OutputPath)\$(TargetFramework)`。

Target Platform 即 `<PlatformTarget>`，对应于 C# 编译器参数 [`-platform`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/output#platformtarget)

未指定时，该值由 [`Platform`] 推断确定。[参见讨论](https://stackoverflow.com/questions/4090967/difference-between-platform-and-platform-target-in-vs)

可以通过 VS IDE\Project Properties\Build\Platform Target 修改它。[参见文档](https://learn.microsoft.com/en-us/visualstudio/ide/how-to-configure-projects-to-target-platforms?view=vs-2022)
注意：Project Properties 又称为 Project Designer。

`<Platforms>` 对应于 VS IDE\Solution Configuration Manager\Project\Platform 的可选项。

`<PlatformTarget>` 是不存在的。

[RuntimeIdentifiers][RID] 自称用于标识运行时平台，但主要用于 nuget restore 时选择合适的包。[参见官方 RID 列表][RID-catalog]

[RID]: https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#runtimeidentifiers
[RID-catalog]: https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
