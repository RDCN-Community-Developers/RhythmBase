![logo](RhythmBase_banner.png)

<p align="center">
  <a href="/LICENSE"><img src="https://img.shields.io/github/license/RDCN-Community-Developers/RhythmToolkit" alt="License"></a>
  <a href="https://www.nuget.org/packages/RhythmBase/"><img src="https://img.shields.io/nuget/v/RhythmBase?logo=nuget" alt="Nuget Download"></a>
  <img src="https://img.shields.io/nuget/dt/RhythmBase" alt="Downloads"/>
</p>

> 如果您觉得这个项目对您有帮助，可以考虑通过[爱发电（中国大陆）](https://afdian.com/a/obugs)或
> [Ko-fi（国际）](https://ko-fi.com/obugs)进行赞助！

# RhythmBase

#### \[ [English](./README.md) | 中文 \]

本项目面向音乐游戏的关卡开发者，旨在为开发者提供独立于游戏引擎的、更高性能、系统化、直观的关卡编辑代理开发库。

开发进度详见[此处](./TODO.zh-cn.md)。

您可以在[这里](/RhythmBase.Test/Tutorial.cs)查看示例。

## 架构概览

```
RhythmBase                         ← 核心库（NuGet 包）
├── Global/                        ← 公共接口、类型、序列化基础设施
│   ├── Components/                ← Color, EnumCollection, TickTime 接口, Level 接口...
│   ├── Events/                    ← IEvent, IDurationEvent, IFileEvent, IForwardEvent...
│   ├── Converters/                ← MetadataJsonConverter, MemberConverter, TypeConverterRegistry...
│   └── Extensions/                ← LINQ 查询, 事件导航...

RhythmBase.Generator              ← 源代码生成器（Roslyn Incremental SourceGenerator）
└── 自动生成: EventTypeRegistry / EventConverterMap / EnumConverterExtensions

RhythmBase.RhythmDoctor            ← 节奏医生适配
RhythmBase.Adofai                  ← 冰与火之舞适配
RhythmBase.BeatBlock               ← BeatBlock 适配
RhythmBase.Rizline                 ← Rizline 适配
```

**序列化体系**：核心库提供 `MetadataJsonConverter<T>`（处理复合对象如 Level、Settings、Row 等）和 `MemberConverter<T>`（逐字段读写事件属性）。源代码生成器根据 `AssemblyInfo.cs` 中的声明自动为每个事件类生成转换器和类型-枚举映射表，无需手写序列化代码。详见[教程第二部分](docs/Tutorial.zh-cn.md#二实现新的关卡类型)。

**支持的关卡格式**：

| 游戏       | 单文件     | 多文件目录                             | 压缩包          | JSON 读写          | 时间解析           | 历史版本适配       |
| ---------- | ---------- | -------------------------------------- | --------------- | ------------------ | ------------------ | ------------------ |
| 节奏医生   | `.rdlevel` | -                                      | `.rdzip` `.zip` | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| 冰与火之舞 | `.adofai`  | -                                      | `.zip`          | :white_check_mark: |                    |                    |
| BeatBlock  | -          | `manifest.json` + `level.json` + chart | `.zip`          | :white_check_mark: |                    |                    |
| Rizline    | -          | `metadata.json` + chart                | `.zip`          | -                  |                    |                    |

## 特别感谢

- 项目维护
  - [0x4D2](https://github.com/0x4D25F2) 提供了大量测试和反馈。
  - [mfgujhgh](https://github.com/mfgujhgh) 提供了算法指导。
  - **节奏医生玩家社区** 对本项目的大力支持。
- 赞助
  - [NTide](https://space.bilibili.com/351700081)
  - [来因洛特 | layinloty](https://space.bilibili.com/406743035)
  - [狗小白 | Dogbai](https://space.bilibili.com/1129425006)
  - [只能用宽判的屑 | kuanpan](https://space.bilibili.com/1928620300)
  - [mfgujhgh](https://space.bilibili.com/1369651)

| 项目                    | 描述                                               | 状态     | 链接                                                                     |
| -------------------     | -------------------------------------------------- | -------- | :----------------------------------------------------------------------- |
| RhythmBase              | 关卡编辑代理核心库。                               | 维护中   | **您在这里**                                                             |
| RhythmBase.RhythmDoctor | Rhythm Doctor 实现。                               | 维护中   | **您在这里**                                                             |
| RhythmBase.Adofai       | Adofai 实现。                                      | 维护中   | **您在这里**                                                             |
| RhythmBase.BeatBlock    | BeatBlock 实现。                                   | 维护中   | **您在这里**                                                             |
| RhythmBase.Rizline      | Rizline 实现。                                     | 维护中   | **您在这里**                                                             |
| RhythmBase.View         | 绘制所有节奏医生事件。（包括 TypeScript DOM 版本） | 开发中   | [前往](https://github.com/OLDRedstone/RhythmBase.View)                   |
| RhythmBase.Addition     | 为核心库扩展功能。                                 | 开发中   | [前往](https://github.com/RDCN-Community-Developers/RhythmBase.Addition) |
| RhythmBase.Interact     | 与游戏关卡编辑器交互。                             | _未公开_ | -                                                                        |
| RhythmBase.Hospital     | 关卡的审核、提示、辅助等功能。                     | _未公开_ | -                                                                        |
| RhythmBase.Lite         | RhythmBase 的轻量版本。                            | 开发中   | [前往](https://github.com/RDCN-Community-Developers/RhythmToolkitLite)   |
| RhythmBase.Control      | 关卡代理 UI 控件库。                               | _未公开_ | -                                                                        |

## 核心特性

- **完整的事件系统支持**\
  为音乐游戏提供强类型事件模型，兼容未来新事件模型。
- **智能事件处理**\
  灵活的 LINQ 查询、自动关系管理、内置时间线生成工具。
- **RichText 与对话组件**\
  完整的富文本语法解析和代码生成，用于对话和标题事件。
- **源代码生成器驱动的序列化**\
  AOT 兼容的 JSON 序列化，无需反射，由 Roslyn 源代码生成器自动为事件类型生成转换器。
- **位图枚举集合**\
  高性能 `EnumCollection<T>` / `ReadOnlyEnumCollection<T>` 用于事件类型分类和筛选。
- **跨平台**\
  基于 .NET Standard 2.0 / .NET 8.0，支持 Windows、Linux、macOS。
- **AOT 兼容**\
  无反射调用，完全支持 AOT 编译。

## 快速开始

```powershell
dotnet add package RhythmBase.RhythmDoctor
```

```cs
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;

// 读取关卡
using var level = Level.FromFile("level.rdlevel");

// 添加事件
level.Add(new Comment() { Text = "hello", Tab = Tab.Windows, Y = 2 });

// 保存
level.SaveToFile("out.rdlevel");
```

## 文档

- [完整使用教程 (中文)](docs/Tutorial.zh-cn.md)
- [实现新关卡类型 (中文)](docs/Tutorial.zh-cn.md#二实现新的关卡类型)
- [贡献指南 (中文)](CONTRIBUTION.zh-cn.md)

## AI 辅助

本项目在开发过程中的以下内容使用了 AI 辅助工具（如 GitHub Copilot、ChatGPT 等）。

- 代码补全
- API 文档查询
- 算法设计与优化建议
- XML 注释编写
- 文档翻译（原始语言为简体中文）

## 关于这个项目

这个项目最初命名为
`RhythmToolkit`，目标是为《节奏医生》开发一些简化关卡处理的小工具。\
随着项目逐渐完善，发展方向逐步偏向成为其他工具的基础框架，并扩展支持了《冰与火之舞》（Adofai）的关卡模型。\
基于以上原因，项目更名为
`RhythmBase`，工具性质的内容迁移至其他仓库。当然，你也可以简称它为 `RDTK`！
