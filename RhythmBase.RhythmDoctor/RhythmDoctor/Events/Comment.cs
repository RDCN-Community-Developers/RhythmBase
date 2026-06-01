using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a description event that can be used to display comments or notes in the rhythm doctor editor or during gameplay.
/// </summary>
[JsonObjectSerializable]
public partial record class Comment : BaseDecorationAction, IColorEvent
{
	/// <summary>
	/// Gets or sets the Y coordinate value, which is influenced by the current tab selection.
	/// </summary>
	/// <remarks>When the current tab is set to <see cref="Tab.Decorations"/>, the property returns the base Y value
	/// and ignores attempts to set a new value. For all other tabs, the property allows getting and setting a custom Y
	/// coordinate. This behavior enables context-sensitive positioning based on the selected tab.</remarks>
	public override int Y
	{
		get => Tab == Tab.Decorations ? base.Y : field;
		set
		{
			if (Tab != Tab.Decorations)
				field = value;
		}
	}
	/// <summary>
	/// Gets or sets the custom tab.
	/// </summary>
	[JsonAlias(name: "tab")]
	public Tab CustomTab { get; set; }
	/// <summary>
	/// Gets the custom tab associated with this instance.
	/// <remark>
	/// If you want to set the tab for this comment, use the <see cref="CustomTab"/> property instead.
	/// The <see cref="Tab"/> property is overridden to return the value of <see cref="CustomTab"/>,
	/// ensuring that the comment's tab is determined by the custom tab setting rather than any default behavior.
	/// </remark>
	/// </summary>
	public override Tab Tab => CustomTab;
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="Comment"/> is shown.
	/// </summary>
	public bool Show { get; set; } = false;
	/// <summary>
	/// Gets or sets the text of the comment.
	/// It may be used to run custom methods sometimes.
	/// </summary>
	public string Text { get; set; } = "";
	/// <summary>
	/// Gets or sets the color of the comment.
	/// </summary>
	public PaletteColor Color { get; set; } = new Color(0xFFF2E644u);
	/// <inheritdoc />
	public override EventType Type => EventType.Comment;
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {Text}";
}
