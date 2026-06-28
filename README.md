![logo](RhythmBase_banner.png)

<p align="center">
  <a href="/LICENSE"><img src="https://img.shields.io/github/license/RDCN-Community-Developers/RhythmToolkit" alt="License"></a>
  <a href="https://www.nuget.org/packages/RhythmBase/"><img src="https://img.shields.io/nuget/v/RhythmBase?logo=nuget" alt="Nuget Download"></a>
  <img src="https://img.shields.io/nuget/dt/RhythmBase" alt="Downloads"/>
</p>

> If you find this project helpful, consider supporting via [Afdian (China)](https://afdian.com/a/obugs) or
> [Ko-fi (International)](https://ko-fi.com/obugs)!

# RhythmBase

#### \[ English | [中文](./README.zh-cn.md) \]

A high-performance, systematic, and intuitive level editing proxy development library for rhythm game developers, independent of any game engine.

Development progress is tracked [here](./TODO.md).

Examples can be found [here](/RhythmBase.Test/Tutorial.cs).

## Architecture Overview

```
RhythmBase                         ← Core library (NuGet package)
├── Global/                        ← Public interfaces, types, serialization infrastructure
│   ├── Components/                ← Color, EnumCollection, TickTime interface, Level interface...
│   ├── Events/                    ← IEvent, IDurationEvent, IFileEvent, IForwardEvent...
│   ├── Converters/                ← MetadataJsonConverter, MemberConverter, TypeConverterRegistry...
│   └── Extensions/                ← LINQ queries, event navigation...

RhythmBase.Generator              ← Source generator (Roslyn Incremental SourceGenerator)
└── Auto-generates: TickTime / TickTimeRange / Calculator / OrderedEventCollection / EventTypeRegistry / EventConverterMap / EnumConverterExtensions / event converters / ...

RhythmBase.RhythmDoctor            ← Rhythm Doctor adapter
RhythmBase.Adofai                  ← A Dance of Fire and Ice adapter
RhythmBase.BeatBlock               ← BeatBlock adapter
RhythmBase.Rizline                 ← Rizline adapter
```

**Serialization system**: The core library provides `MetadataJsonConverter<T>` (handles compound objects like Level, Settings, Row, etc.) and `MemberConverter<T>` (reads/writes event properties field by field). The source generator automatically produces converters and type-enum mappings for each event class based on declarations in `AssemblyInfo.cs`, eliminating the need to write serialization code by hand. See [Tutorial Part 2](docs/Tutorial.md#ii-implementing-a-new-level-type) for details.

**Supported level formats**:

| Game                  | Single file  | Multi-file directory                       | Archive         | JSON read/write    | Time parsing       | Version adaptation               |
| --------------------- | ------------ | ------------------------------------------ | --------------- | ------------------ | ------------------ | -------------------------------- |
| Rhythm Doctor         | `.rdlevel`   | -                                          | `.rdzip` `.zip` | :white_check_mark: | :white_check_mark: | :white_check_mark:               |
| A Dance of Fire and Ice | `.adofai`  | -                                          | `.zip`          | :white_check_mark: |                    |                                  |
| BeatBlock             | -            | `manifest.json` + `level.json` + chart     | `.bbz` `.zip`   |                    |                    | :white_check_mark:(needs verify) |
| Rizline               | -            | `metadata.json` + chart                    | `.rlz` `.zip`   |                    |                    |                                  |

## Special Thanks

- Project maintenance
  - [0x4D2](https://github.com/0x4D25F2) for extensive testing and feedback.
  - [mfgujhgh](https://github.com/mfgujhgh) for algorithm guidance.
  - **Rhythm Doctor player community** for strong support.
- Sponsors
  - [NTide](https://space.bilibili.com/351700081)
  - [layinloty](https://space.bilibili.com/406743035)
  - [Dogbai](https://space.bilibili.com/1129425006)
  - [kuanpan](https://space.bilibili.com/1928620300)
  - [mfgujhgh](https://space.bilibili.com/1369651)

| Project                 | Description                                                     | Status     | Link                                                                     |
| ----------------------- | --------------------------------------------------------------- | ---------- | :----------------------------------------------------------------------- |
| RhythmBase              | Core level editing proxy library.                               | Maintained | **You are here**                                                         |
| RhythmBase.RhythmDoctor | Rhythm Doctor implementation.                                   | Maintained | **You are here**                                                         |
| RhythmBase.Adofai       | Adofai implementation.                                          | Maintained | **You are here**                                                         |
| RhythmBase.BeatBlock    | BeatBlock implementation.                                       | Maintained | **You are here**                                                         |
| RhythmBase.Rizline      | Rizline implementation.                                         | Maintained | **You are here**                                                         |
| RhythmBase.View         | Renders all Rhythm Doctor events. (incl. TypeScript DOM version) | In development | [Go](https://github.com/OLDRedstone/RhythmBase.View)                 |
| RhythmBase.Addition     | Extends the core library.                                       | In development | [Go](https://github.com/RDCN-Community-Developers/RhythmBase.Addition) |
| RhythmBase.Interact     | Interacts with game level editors.                              | _Private_  | -                                                                        |
| RhythmBase.Hospital     | Level review, hints, and assistance.                            | _Private_  | -                                                                        |
| RhythmBase.Lite         | Lightweight version of RhythmBase.                              | In development | [Go](https://github.com/RDCN-Community-Developers/RhythmToolkitLite)   |
| RhythmBase.Control      | UI control library for level proxy.                             | _Private_  | -                                                                        |

## Key Features

- **Complete event system support**\
  Provides strongly-typed event models for rhythm games, compatible with future new event models.
- **Smart event handling**\
  Flexible LINQ queries, automatic relationship management, and built-in timeline generation tools.
- **RichText and dialogue components**\
  Full rich text syntax parsing and code generation for dialogue and title events.
- **Source generator driven serialization**\
  AOT-compatible JSON serialization with no reflection; converters for event types are automatically generated by the Roslyn source generator.
- **Bitmap enum collections**\
  High-performance `EnumCollection<T>` / `ReadOnlyEnumCollection<T>` for event type classification and filtering.
- **Cross-platform**\
  Based on .NET Standard 2.0 / .NET 8.0, supporting Windows, Linux, macOS.
- **AOT compatible**\
  No reflection calls, fully supports AOT compilation.

## Quick Start

```powershell
dotnet add package RhythmBase.RhythmDoctor
```

```cs
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;

// Load level
using var level = Level.FromFile("level.rdlevel");

// Add event
level.Add(new Comment() { Text = "hello", Tab = Tab.Windows, Y = 2 });

// Save
level.SaveToFile("out.rdlevel");
```

## Documentation

- [Full Tutorial (English)](docs/Tutorial.md)
- [Implementing a New Level Type (English)](docs/Tutorial.md#ii-implementing-a-new-level-type)
- [Contributing Guide (English)](CONTRIBUTION.md)
- [完整使用教程 (中文)](docs/Tutorial.zh-cn.md)
- [贡献指南 (中文)](CONTRIBUTION.zh-cn.md)

## AI Assistance

The following aspects of this project's development used AI assistance tools (such as GitHub Copilot, ChatGPT, etc.).

- Code completion
- API documentation lookup
- Algorithm design and optimization suggestions
- XML comment writing
- Documentation translation (original language is Simplified Chinese)

## About This Project

This project was originally named
`RhythmToolkit`, aiming to develop small tools to simplify level processing for Rhythm Doctor.\
As the project matured, its direction shifted toward becoming a foundational framework for other tools, and it expanded to support the A Dance of Fire and Ice (Adofai) level model.\
For this reason, the project was renamed
`RhythmBase`, and tool-oriented content was migrated to other repositories. Of course, you can also call it `RDTK`!
