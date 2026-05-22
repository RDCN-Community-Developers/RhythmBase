![logo](RhythmBase_banner.png)

<p align="center">
  <a href="/LICENSE"><img src="https://img.shields.io/github/license/RDCN-Community-Developers/RhythmToolkit" alt="License"></a>
  <a href="https://www.nuget.org/packages/RhythmBase/"><img src="https://img.shields.io/nuget/v/RhythmBase?logo=nuget" alt="Nuget Download"></a>
  <img src="https://img.shields.io/nuget/dt/RhythmBase" alt="Downloads"/>
</p>

> 如果您觉得这个项目对您有帮助，可以考虑通过[爱发电（中国大陆）](https://afdian.com/a/obugs)或 [Ko-fi（国际）](https://ko-fi.com/obugs)进行赞助！

# RhythmBase

#### \[ [English](./README.md) | 中文 \]

本项目面向音乐游戏的关卡开发者，旨在为开发者提供独立于游戏引擎的、更高性能、系统化、直观的关卡编辑代理开发库。\
目前计划或已完成对以下音乐游戏的文件格式兼容：
          
| 游戏                                  | ID           | 状态   | 简短 ID |
| ------------------------------------- | ------------ | ------ | ------- |
| 节奏医生（Rhythm Doctor）             | RhythmDoctor | 维护中 | RD      |
| 冰与火之舞（A Dance of Fire and Ice） | Adofai       | 维护中 | AD      |
| 节奏方块（BeatBlock）                 | BeatBlock    | 开发中 | BB      |
| 律动轨迹（Rizline）                   | Rizline      | 计划中 | RL      |
| Rhythm Quest                          | RhythmQuest  | 计划中 | RQ      |

感谢节奏医生玩家社区对本项目的支持。\
您可以在[这里](/RhythmBase.Test/Tutorial.cs)查看示例。

## 特别感谢
- 项目维护
    - [0x4D2](https://github.com/0x4D25F2) 提供了大量测试和反馈。
    - [mfgujhgh](https://github.com/mfgujhgh) 提供了算法指导。
- 赞助
    - [NTide](https://space.bilibili.com/351700081)
    - [来因洛特 | layinloty](https://space.bilibili.com/406743035)
    - [狗小白 | Dogbai](https://space.bilibili.com/1129425006)
    - [只能用宽判的屑 | kuanpan](https://space.bilibili.com/1928620300)
    - [mfgujhgh](https://space.bilibili.com/1369651)

| 项目                | 描述                                              | 状态     | 链接                                                                     | 
|---------------------|---------------------------------------------------|----------|:-------------------------------------------------------------------------|
| RhythmBase          | 关卡编辑代理核心库。                              | 维护中   | **您在这里**                                                             |
| RhythmBase.View     | 绘制所有节奏医生事件。（包括 TypeScript DOM 版本）| 开发中   | [前往](https://github.com/OLDRedstone/RhythmBase.View)                   |
| RhythmBase.Addition | 为核心库扩展功能。                                | 开发中   | [前往](https://github.com/RDCN-Community-Developers/RhythmBase.Addition) |
| RhythmBase.Interact | 与游戏关卡编辑器交互。                            | *未公开* | -                                                                        |
| RhythmBase.Hospital | 关卡的审核、提示、辅助等功能。                    | *未公开* | -                                                                        |
| RhythmBase.Lite     | RhythmBase 的轻量版本。                           | 开发中   | [前往](https://github.com/RDCN-Community-Developers/RhythmToolkitLite)   |
| RhythmBase.Control  | 关卡代理 UI 控件库。                              | *未公开* | -                                                                        |

```mermaid
flowchart RL
RBLite[RhythmBase.Lite]
subgraph RD[节奏医生]
  RDLE[节奏医生关卡编辑器]
end
subgraph AD[冰与火之舞]
  ADLE[冰与火之舞关卡编辑器]
end
RDL([节奏医生关卡])
ADL([冰与火之舞关卡])
subgraph RBTitle[RhythmBase]
  RB[RhythmBase]
  RBAdd[RhythmBase.Addition]
  RBInt[RhythmBase.Interact]
  RBHos[RhythmBase.Hospital]
  RBV[RhythmBase.View]
  RBVts["RhythmBase.View(TypeScript)"]
  subgraph RBC[RhythmBase.Control]
    RBCCore[RhythmBase.Control.Core]
    RBCWPF[RhythmBase.Control.WPF]
    RBCWF[RhythmBase.Control.WinForm]
    RBCAva[RhythmBase.Control.Avalonia]
  end
end

RBLite ---> RDL
RBCWPF & RBCWF & RBCAva --> RBCCore --> RBV
RBV & RBHos & RBAdd & RBInt --> RB ---> RDL & ADL
RBInt ---> RDLE --> RDL
RBInt ---> ADLE --> ADL
RBV <-.-> RBVts
```

## 核心特性

- **完整的事件系统支持** — 为《节奏医生》和《冰与火之舞》提供强类型事件模型，涵盖所有官方事件类型及 Adofai 高级滤镜系统，兼容未来新事件模型。
- **智能事件处理** — 灵活的 LINQ 查询、自动关系管理、内置时间线生成工具。
- **RichText 与对话组件** — 完整的富文本语法解析和代码生成，用于对话和标题事件。
- **缓动函数库** — 包含游戏全部缓动曲线，支持自定义插值与曲线拟合。
- **跨平台** — 基于 .NET Standard 2.0 / .NET 8.0，支持 Windows、Linux、macOS。
- **多语言调用** — 除 C#/F#/VB.NET 外，可通过 [pythonnet](https://github.com/pythonnet/pythonnet) 在 Python 中调用。

## 快速开始

```powershell
dotnet add package RhythmBase
```

```cs
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;

// 读取关卡
using var level = RDLevel.FromFile("level.rdlevel");

// 添加事件
level.Add(new Comment() { Text = "hello", CustomTab = Tab.Windows, Y = 2 });

// 保存
level.SaveToFile("out.rdlevel");
```

## 性能

以关卡 *The Power of Terry* (`the-powe-S7V1kg9RWYK.rdzip`) 为基准，在 .NET 8.0 环境下 RhythmBase 的读写速度可达官方关卡编辑器的数倍。  
- :red_square: 游戏编辑器
- :green_square: .NET framework 4.8 下运行 .NET standard 2.0
- :blue_square: .NET 8.0 下运行 .NET 8.0

```mermaid
---
config:
    xyChart:
        height: 300
    themeVariables:
        xyChart:
            plotColorPalette: '#F00, #0F84, #0F88,  #0F8, #08F4, #08F8, #08F'
---
xychart
  title "读取时间"
    y-axis "耗时 (ms)"
    %% RDLE
    line [2130.5224, 2855.4669, 2269.7906, 2210.3916, 2385.5643, 2238.7013, 2011.8578, 2121.1231, 2014.9036, 1984.7555]
    %% RDTK .NET Standard 2.0 v1.3.4
    line [3273.9261, 2793.1168, 2796.0169, 2750.7386, 2757.9804, 2734.5127, 2715.0270, 2765.0712, 2749.9744, 2764.9854]
    %% RDTK .NET Standard 2.0 v1.3.9
    line [3148.9088,1226.1298,1054.2334,1134.9715,1033.4164,1030.4823,1088.7369,1178.1487,1120.97,1055.3118]
    %% RDTK .NET Standard 2.0 v1.3.10
    line [2511.4484,935.1044,922.5633,908.0549,921.5173,961.0245,971.9857,1705.2193,1025.6835,1170.5473]
    %% RDTK .NET 8.0 v1.3.4
    line [1887.2656, 1353.3896, 1076.3732, 1121.0746, 1080.6883, 1074.4476, 1122.7722, 1091.6990, 1103.6302, 1149.2975]
    %% RDTK .NET 8.0 v1.3.9
    line [2367.0337,1056.9628,966.5576,1035.8415,913.1249,946.951,908.2692,917.0895,1036.6879,1048.7651]
    %% RDTK .NET 8.0 v1.3.10
    line [1365.457,1017.3662,753.053,615.3958,620.0609,617.4589,635.7308,612.3335,661.5626,605.837]
```
```mermaid
---
config:
    xyChart:
        height: 300
    themeVariables:
        xyChart:
            plotColorPalette: '#F00, #0F84, #0F88,  #0F8, #08F4, #08F8, #08F'
---
xychart
  title "写入时间"
    y-axis "耗时 (ms)"
    %% RDLE
    line [242.5163, 280.5837, 243.9327, 238.8927, 244.9178, 243.3303, 251.8815, 243.9217, 331.3641, 265.0274]
    %% RDTK .NET Standard 2.0 v1.3.4
    line [513.1026, 361.6694, 350.3818, 347.8336, 349.8539, 356.7689, 353.0014, 350.7653, 354.2552, 348.2180]
    %% RDTK .NET Standard 2.0 v1.3.9
    line [403.8953,215.1384,235.0083,239.653,231.9253,215.1407,205.9433,235.8352,237.2161,252.8412]
    %% RDTK .NET Standard 2.0 v1.3.10
    line [378.2349,199.9493,190.2174,204.2167,249.4367,219.3135,197.8157,297.8816,233.1203,223.2512]
    %% RDTK .NET 8.0 v1.3.4
    line [460.3911, 224.0186, 165.0900, 175.7229, 150.6010, 144.8973, 155.9880, 150.8031, 155.9035, 153.7161]
    %% RDTK .NET 8.0 v1.3.9
    line [391.9549,256.5255,113.1473,74.3901,75.7604,126.1112,107.5059,99.014,108.391,90.731]
    %% RDTK .NET 8.0 v1.3.10
    line [419.8573,277.4102,195.8157,173.8777,109.3214,64.7145,80.4896,77.9989,85.8971,57.5985]
```

## 文档

- [完整使用教程 (中文)](docs/Tutorial.zh-cn.md)
- [Full Tutorial (English)](docs/Tutorial.md)

## AI 辅助

本项目在开发过程中的以下内容使用了 AI 辅助工具（如 GitHub Copilot、ChatGPT 等）。
- 代码补全
- Source Generator 代码重构（#6bdbc108）
- API 文档查询
- 算法设计与优化建议
- XML 注释编写
- 文档翻译（原始语言为简体中文）

## 关于这个项目

这个项目最初命名为 `RhythmToolkit`，目标是为《节奏医生》开发一些简化关卡处理的小工具。\
随着项目逐渐完善，发展方向逐步偏向成为其他工具的基础框架，并扩展支持了《冰与火之舞》（Adofai）的关卡模型。\
基于以上原因，项目更名为 `RhythmBase`，工具性质的内容迁移至其他仓库。当然，你也可以简称它为 `RDTK`！
