适配新的音游谱面流程：

1. 定义枚举类型，用于表示此关卡类型的所有事件类型。
1. 定义时间单位，实现 `IBeat<TSelf>` 接口，提供时间单位记录。
2. 定义时间范围，实现 `IBeatRange<TBeat>` 接口，提供按时间范围筛选功能。
1. 定义关卡模型，实现 `RhythmBase.Global.ILevel<TSelf，TEvent，TType，TBeat>` 接口，
	提供关卡基本信息和读写相关方法。\
	依据关卡格式，可以选择实现这些接口：
	- `IJsonLevel<TLevel，TEvent，TType，TBeat>`\
		关卡内容为 JSON 格式。提供 JSON 预览
	- `ISingleFileLevel<TSelf, TEvent, TType, TBeat>`\
		关卡为单文件。
	- `IMultiFileLevel<TSelf, TEvent, TType, TBeat>`\
		关卡为多文件。
	- `IArchiveLevel<TSelf, TEvent, TType, TBeat>`\
		关卡存在压缩包形式。	
1. 定义谱面模型，实现 `RhythmBase.Clobal.IChart<TBeat>` 接口。
4. 定义事件模型，实现 `IEvent<TType，TBeat>` 接口。
   建议编写一个命名空间限定接口用于事件模型实现。\
	依据事件功能，可以选择实现这些接口：
	- `IDurationEvent`\
		事件带有时长属性\
		此接口用于未来扩展动画帧功能
		- `IEaseEvent`\
			事件带有缓动属性
	- `IFileEvent`\
		事件包含外部文件引用，用于针对压缩包关卡类型收集关卡文件引用以打包资源
		- `IAudioFileEvent`\
			事件包含音频文件引用 
		- `IImageFileEvent`\
			事件包含图像文件引用
	- `IForwardEvent`\
		回退兼容事件
5. 对象依据事件和属性的需求分别标记特定的特性
	- `RDJsonEnumSerializableAttribute`\
		需要添加字符串转换的枚举类型
	- `RDJsonObjectSerializableAttribute`\
		需要自动生成序列化器和映射的事件类型
	- `RDJsonObjectHasSerializerAttribute`\
		已自行实现序列化器，但是需要映射的事件类型
	- `RDJsonObjectNotSerializableAttribute`\
		不需要自动生成序列化器和映射的事件类型，如回退兼容事件
	- `RDJsonIgnoreAttribute`\
		序列化和反序列化时需要忽略的公开属性
	- `RDJsonNotIgnoreAttribute`\
		序列化和反序列化时不能够忽略的非公开属性
	- `RDJsonConditionAttribute`\
		序列化和反序列化时按条件不忽略的属性，用 `$&` 指代对象本身\
		如 `RDJsonCondition("$&.Angle is not null")`
	- `RDJsonAliasAttribute`\
		序列化和反序列化时需要使用别名的属性
	- `RDJsonTimeAttribute`\
		需要序列化和反序列化为特定 `TimeSpan` 类型的属性
	- `RDJsonConverterAttribute`\
		需要使用特定序列化器序列化和反序列化的属性
	- `RDJsonConverterForAttribute`\
		需要指定为特定类型的序列化器的类型
6. 源生成器项目添加一个 `GenerationConfig` 配置，用于指定此音游格式下序列化器和枚举转换器的生成规则
```cs
new GenerationConfig() {
	ID="RD"，//区分音游类型的唯一ID
	SourceNamespace="RhythnBase.RhythnDoctor.Events"，//事件模型的命名空间
	TargetConverterNamespace="RhythnBase.RhythnDoctor.Converters"，//序列化器的命名空间
	TargetUtilsNamespace="RhythnBase.RhythnDoctor.Utils"，//救举转换器的命名空间
	TargetUtilsClassName="EventTypeUtils"，//救举转换器的类名
	BaseConverterClassName="EventInstanceConverter"，//序列化器的基类名
	BaseInterfaceFullName="RhythnBase.RhythnDoctor.Events.IBaseEvent"，//需要生成序列化器的事件模型类型的上层接口
	ClassTypeEnunFullnane="RhythnBase.RhythnDoctor.EventType"，//表示事件类型的救举类型
	ClassTypeEnunUnknounMenberName="ForuardEvent"，//回退兼容类型名 48
}
```
