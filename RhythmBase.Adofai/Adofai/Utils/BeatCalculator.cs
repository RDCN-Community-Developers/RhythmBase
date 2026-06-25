using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Events;
namespace RhythmBase.Adofai.Utils;

/// <summary>
/// Beat Calculator.
/// </summary>
public partial class BeatCalculator
{
	public partial void Refresh()
	{
		throw new NotImplementedException();
	}
#pragma warning disable IDE0052 // 删除未读的私有成员
	private float _DefaultBpm = 100;
	private Tile? _cacheTile = null;
#pragma warning restore IDE0052 // 删除未读的私有成员

	/*
	 * 更新：
	 * - 事件添加进砖块
	 * - 事件移除出砖块
	 * - 砖块添加进关卡
	 * - 砖块移除出关卡
	 * - 砖块中旋
	 * - 砖块发卡弯
	 * - 事件属性更新
	 */
}
