using RhythmBase.Global.Components;
using RhythmBase.Global.Extensions;
using RhythmBase.Global.Settings;
using RhythmBase.RhythmDoctor;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Converters;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using RhythmBase.RhythmDoctor.Utils;
using System.Diagnostics;
using System.Text;

namespace RhythmBase.Test;

[TestClass]
public sealed class Test
{
	[TestMethod]
	public void AddRemoveTest()
	{
		Level level = [];
		Decoration deco = [];
		Move move = new();
		deco.Add(move);
		Console.WriteLine($"{level.Count}, {deco.Count}");
		level.Decorations.Add(deco);
		Console.WriteLine($"{level.Count}, {deco.Count}");
		level.Add(move);
		Console.WriteLine($"{level.Count}, {deco.Count}");
		level.Decorations.Remove(deco);
		Console.WriteLine($"{level.Count}, {deco.Count}");
		level.Remove(move);
		Console.WriteLine($"{level.Count}, {deco.Count}");
	}
	[TestMethod]
	public void GenerateLevelWithAllEventTypes()
	{
		Level level = [];
		level.Rows.Add(new() { RowType = RowType.Classic });
		level.Rows.Add(new() { RowType = RowType.Oneshot });
		level.Decorations.Add([]);
		Dictionary<Tab, int> count = [];
		foreach (EventType type in ((EventType[])Enum.GetValues(typeof(EventType))).Where(i => !EventTypeRegistry.CustomTypes.Contains(i)))
		{
			if (EventTypeRegistry.CustomTypes.Contains(type))
				continue;
			if (type == EventType.AdvanceText)
				continue;
			IBaseEvent? e = (IBaseEvent?)Activator.CreateInstance(EventTypeRegistry.ToType(type));
			if (e is null)
				continue;
			if (e.GetType().Name != e.Type.ToEnumString())
				throw new NotImplementedException();
			if (e is IBarBeginningEvent eb)
				level.Add(eb);
			else
			{
				if (!count.TryGetValue(e.Tab, out int value))
				{
					value = 0;
					count[e.Tab] = value;
				}
				e.TickTime = new(count[e.Tab] + 1);
				if (e is BaseRowAction er)
				{
					if (er is BaseBeat beat)
					{
						if (er is AddClassicBeat or AddFreeTimeBeat or PulseFreeTimeBeat or SetRowXs)
							level.Rows[0].Add(beat);
						else
							level.Rows[1].Add(beat);
					}
					else
						level.Rows[0].Add(er);
				}
				else if (e is BaseDecorationAction ed)
					level.Decorations[0].Add(ed);
				else
					level.Add(e);
				count[e.Tab] = ++value;
			}
		}
		lock (this)
			level.SaveToFile("out.rdlevel");
		level = Level.FromJsonString(level.ToJsonString());
	}
	[TestMethod]
	public void ReadWriteSpeedTest()
	{
		GlobalSettings.CachePath = "cache";
		{
			List<(double, double)> results = [];
			LevelReadSettings settings = new()
			{
				InactiveEventsHandling = InactiveEventsHandling.Ignore,
				UnreadableEventsHandling = UnreadableEventHandling.Store,
				ZipProcessingMode = ZipProcessingMode.AllEntries,
				Strictness = Global.Converters.JsonStrictness.Relaxed,
				LoadAssets = false,
			};
			settings.FileReferenceEncountered += (s, e) => 			{
				Console.WriteLine($"Encountered file reference: {e.Reference.Path}");
			};
			Console.WriteLine("|Action\t|Read\t|Write|");
			Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
			for (int i = 0; i < 10; i++)
			{
				double readTime, writeTime;
				sw.Restart();
				using Level level = Level.FromZip("the-powe-S7V1kg9RWYK.rdzip", settings);
				readTime = sw.Elapsed.TotalMilliseconds;
				Console.Write($"|{i}\t|{readTime,10} ms\t|");
				sw.Restart();
				lock (this)
					level.SaveToFile("out.rdlevel");
				writeTime = sw.Elapsed.TotalMilliseconds;
				Console.WriteLine($"{writeTime,10} ms\t|");
				results.Add((readTime, writeTime));
			}
			Console.WriteLine(string.Join(",", results.Select(i => i.Item1)));
			Console.WriteLine(string.Join(",", results.Select(i => i.Item2)));
		}
		//{
		//	LevelReadOrWriteSettings settings = new()
		//	{
		//		InactiveEventsHandling = InactiveEventsHandling.Ignore,
		//		UnreadableEventsHandling = UnreadableEventHandling.Store,
		//		ZipFileProcessMethod = ZipFileProcessMethod.LevelFileOnly,
		//	};
		//	Console.WriteLine("|Action\t|Elapsed\t|");
		//	Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
		//	for (int i = 0; i < 10; i++)
		//	{
		//		sw.Restart();
		//		using RDLevel level = RDLevel.FromFile("the-powe-S7V1kg9RWYK.rdzip", settings);
		//		Console.WriteLine($"|Read\t|{sw.ElapsedMilliseconds,10} ms\t|");
		//		sw.Restart();
		//		level.SaveToFile("out.rdlevel");
		//		Console.WriteLine($"|Write\t|{sw.ElapsedMilliseconds,10} ms\t|");
		//	}
		//}
	}
	[TestMethod]
	public void CPBTest()
	{
		Level level = Level.Default;
		SetCrotchetsPerBar cpb0 = new() { CrotchetsPerBar = 4 };
		SetCrotchetsPerBar cpb1 = new() { TickTime = new(9), CrotchetsPerBar = 3 };
		SetCrotchetsPerBar cpb2 = new() { TickTime = new(17), CrotchetsPerBar = 5 };
		Comment cmt1 = new() { TickTime = new(2) };
		Comment cmt2 = new() { TickTime = new(10) };
		Comment cmt3 = new() { TickTime = new(19) };

		level.Add(cpb0);
		level.Add(cmt1);
		level.Add(cmt2);
		level.Add(cpb2);
		level.Add(cmt3);
		foreach (SetCrotchetsPerBar cpb in level.OfEvent<SetCrotchetsPerBar>())
		{
			Console.WriteLine(cpb);
		}
		for (int i = 1; i <= 20; i++)
		{
			Console.WriteLine($"{i}: {level.Calculator.BeatOf(i)}");
		}
		Console.WriteLine();

		level.Add(cpb1);
		foreach (SetCrotchetsPerBar cpb in level.OfEvent<SetCrotchetsPerBar>())
		{
			Console.WriteLine(cpb);
		}
		for (int i = 1; i <= 20; i++)
		{
			Console.WriteLine($"{i}: {level.Calculator.BeatOf(i)}");
		}
		Console.WriteLine();

		level.Remove(cpb1);
		foreach (SetCrotchetsPerBar cpb in level.OfEvent<SetCrotchetsPerBar>())
		{
			Console.WriteLine(cpb);
		}
		for (int i = 1; i <= 20; i++)
		{
			Console.WriteLine($"{i}: {level.Calculator.BeatOf(i)}");
		}
		Console.WriteLine();

		level.Add(cpb1);
		foreach (SetCrotchetsPerBar cpb in level.OfEvent<SetCrotchetsPerBar>())
		{
			Console.WriteLine(cpb);
		}
		for (int i = 1; i <= 20; i++)
		{
			Console.WriteLine($"{i}: {level.Calculator.BeatOf(i)}");
		}
		Console.WriteLine();
	}
	[TestMethod]
	public void EventTypeCollectionsTest()
	{
		var allTypes = (EventType[])Enum.GetValues(typeof(EventType));
		foreach (var type in EventTypeRegistry.CustomTypes)
			Console.WriteLine(type);
	}
	[TestMethod]
	public void RDColorTest()
	{
		Random random = new();
		string color = $"{((ushort)random.Next(0, 0x10000)):X4}";
		char a = color[0], r = color[1], g = color[2], b = color[3];
		Console.WriteLine(Color.FromArgb($"{r}{g}{b}").ToString());
		Console.WriteLine(Color.FromArgb($"{a}{r}{g}{b}").ToString());
		Console.WriteLine(Color.FromArgb($"{r}{r}{g}{g}{b}{b}").ToString());
		Console.WriteLine(Color.FromArgb($"{a}{a}{r}{r}{g}{g}{b}{b}").ToString());
		Console.WriteLine(Color.FromRgba($"{r}{g}{b}").ToString());
		Console.WriteLine(Color.FromRgba($"{r}{g}{b}{a}").ToString());
		Console.WriteLine(Color.FromRgba($"{r}{r}{g}{g}{b}{b}").ToString());
		Console.WriteLine(Color.FromRgba($"{r}{r}{g}{g}{b}{b}{a}{a}").ToString());
		Console.WriteLine(Color.FromArgb(Encoding.UTF8.GetBytes($"{r}{g}{b}")).ToString());
		Console.WriteLine(Color.FromArgb(Encoding.UTF8.GetBytes($"{a}{r}{g}{b}")).ToString());
		Console.WriteLine(Color.FromArgb(Encoding.UTF8.GetBytes($"{r}{r}{g}{g}{b}{b}")).ToString());
		Console.WriteLine(Color.FromArgb(Encoding.UTF8.GetBytes($"{a}{a}{r}{r}{g}{g}{b}{b}")).ToString());
		Console.WriteLine(Color.FromRgba(Encoding.UTF8.GetBytes($"{r}{g}{b}")).ToString());
		Console.WriteLine(Color.FromRgba(Encoding.UTF8.GetBytes($"{r}{g}{b}{a}")).ToString());
		Console.WriteLine(Color.FromRgba(Encoding.UTF8.GetBytes($"{r}{r}{g}{g}{b}{b}")).ToString());
		Console.WriteLine(Color.FromRgba(Encoding.UTF8.GetBytes($"{r}{r}{g}{g}{b}{b}{a}{a}")).ToString());
		Console.WriteLine(Color.FromName("grAy").ToString());
		Console.WriteLine(Color.FromName("GRAY").ToString());
		try
		{
			Console.WriteLine(Color.FromName("None").ToString());
		}
		catch (ArgumentException) { }
		catch (Exception ex) { Console.WriteLine(ex.ToString()); }
	}
	[TestMethod]
	public void WindowTabSerializationTest()
	{
		Level level = Level.Default;
		level.Add(new NewWindowDance() { CustomTab = Tab.Actions });
		level.Add(new NewWindowDance() { CustomTab = Tab.Windows });
		level.Add(new WindowResize() { CustomTab = Tab.Actions });
		level.Add(new WindowResize() { CustomTab = Tab.Windows });

		LevelWriteSettings settings = new()
		{

		};
		Console.WriteLine(level.ToJsonString(settings));
	}
	[TestMethod]
	public void OthersTest()
	{
		Level level = Level.Default;
		level.Add(new Tutorial.MyEvent());
            ReadOnlyEnumCollection<EventType> eventTypes = EventTypeRegistry.CustomTypes;
	}
}
