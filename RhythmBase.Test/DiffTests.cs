using RhythmBase.Global.Diff;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Diff;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.Test;

[TestClass]
public sealed class DiffTests
{
	[TestMethod]
	public void PropertyDiffReportsSerializationAndModelPaths()
	{
		Chart left = new();
		Chart right = new();
		left.Add(new Comment { TickTime = new TickTime(1), Text = "left" });
		right.Add(new Comment { TickTime = new TickTime(1), Text = "right" });

		LevelDiff diff = ChartDiffer.Diff(left, right, new DiffOptions { IncludeModelPaths = true });

		DiffPropertyChange text = diff.Events.Single().Properties.Single(i => i.Path == "text");
		Assert.AreEqual("Text", text.ModelPath);
	}

	[TestMethod]
	public void PropertyDiffOmitsModelPathsByDefault()
	{
		Chart left = new();
		Chart right = new();
		left.Add(new Comment { TickTime = new TickTime(1), Text = "left" });
		right.Add(new Comment { TickTime = new TickTime(1), Text = "right" });

		LevelDiff diff = ChartDiffer.Diff(left, right);

		DiffPropertyChange text = diff.Events.Single().Properties.Single(i => i.Path == "text");
		Assert.IsNull(text.ModelPath);
	}

	[TestMethod]
	public void NestedStructureDiffReportsModelPath()
	{
		Chart left = new();
		Chart right = new();
		left.Rows.Add(new Row());
		right.Rows.Add(new Row());
		right.Rows[0].Sound.Volume = 50;

		LevelDiff diff = ChartDiffer.Diff(left, right, new DiffOptions
		{
			IncludeRows = true,
			IncludeModelPaths = true,
		});

		DiffEventChange row = diff.Structures.Single(i => i.TypeName == "Row 0");
		DiffPropertyChange volume = row.Properties.Single(i => i.Path == "sound.volume");
		Assert.AreEqual("Sound.Volume", volume.ModelPath);
	}

	[TestMethod]
	public void SequenceUsesBracketPaths()
	{
		DiffCollector collector = new(
			type => type == typeof(Item) ? new ItemDiffer() : null,
			canonicalizer: null,
			includeModelPaths: true);

		collector.Compare("items", "Items", new[] { new Item(1) }, new[] { new Item(2) });

		DiffPropertyChange change = collector.Changes.Single();
		Assert.AreEqual("items[0].value", change.Path);
		Assert.AreEqual("Items[0].Value", change.ModelPath);
	}

	private sealed class Item
	{
		public Item(int value) => Value = value;
		public int Value { get; }
	}

	private sealed class ItemDiffer : EventDiffer<Item>
	{
		protected override void DiffCore(Item left, Item right, DiffCollector collector)
			=> collector.Compare("value", "Value", left.Value, right.Value);
	}
}
