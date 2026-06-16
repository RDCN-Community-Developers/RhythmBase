using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Settings;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using RhythmBase.RhythmDoctor.Utils;
using System.Text.Json;
using RhythmBase.RhythmDoctor;
using RhythmBase.Global.Components;
using RhythmBase.Global.Converters;
using RhythmBase.RhythmDoctor.Converters;
using RhythmBase.Global.Components.RichText;
using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Test;

    [TestClass]
    public sealed class Tutorial
    {
        static Tutorial()
        {
            _rdlevel = Level.Default;
        }
        private static Level _rdlevel;
        [TestMethod]
        public void CreateAnEmptyLevel()
        {
            using Level emptyLevel = [];
            Console.WriteLine(emptyLevel); // "" Count = 0
        }
        [TestMethod]
        public void CreateADefaultLevel()
        {
            using Level defaultLevel = Level.Default;
            Console.WriteLine(defaultLevel); // "" Count = 3
        }
        [TestMethod]
        public void ReadOrWriteLevel()
        {
            lock (this)
            {
                // Directly read a level file
                using Level rdlevel1 = Level.FromFile(@"your\level.rdlevel");

                // Read a level pack file
                using Level rdlevel2 = Level.FromZip(@"your\level.rdzip");

                // Read a compressed level pack
                using Level rdlevel3 = Level.FromZip(@"your\level.zip");

                // Write a level file
                rdlevel1.SaveToFile(@"your\outLevel.rdlevel");
            }
        }
        [TestMethod]
        public void ReadOrWriteLevelWithSettings()
        {
            // Create custom read settings
          LevelReadSettings settings = new()
            {
                // Handling of inactive events
                InactiveEventsHandling = InactiveEventsHandling.Store,
                // Handling of unreadable events
                // Common when sprite events are not bound to sprite tracks, etc.
                UnreadableEventsHandling = UnreadableEventHandling.Store,
                // Unzip all files in a zip level pack into the cache path below
                // This is usually the faster option
                ZipProcessingMode = ZipProcessingMode.AllEntries,
                // Load asset paths when reading or writing the level
                LoadAssets = true,
            };
            // The cache path is required when reading zip level packs
            // Default is the Temp path.
            GlobalSettings.CachePath = @"your\cache\path";

            lock (this)
            { using Level _rdlevel1 = Level.FromFile(@"your\level.rdlevel", settings); }
        }
        [TestMethod]
        public void ConvertLevelToJsonDocument()
        {
            LevelWriteSettings settings = new();

            JsonDocument jdoc = _rdlevel.ToJsonDocument();
            string json = _rdlevel.ToJsonString(settings);
            Console.WriteLine(jdoc);
            Console.WriteLine(json);
        }
        [TestMethod]
        public void FindEventsInLevel()
        {
            // Find MoveRow events between bar 3 and 5, and in event rows 0 to 2
            IEnumerable<MoveRow> list = _rdlevel
                .OfEvent<MoveRow>()
                .InRange(new(3, 1), new(5, 1))// From Bar 3 to 5
                .Where(i => 0 <= i.Y && i.Y < 3);  // In event rows 0 to 2
        }
        [TestMethod]
        public void FindEventsInDecoration()
        {
            _rdlevel.Decorations.Add([]);
            // Find the AddClassicBeat event in the row decoration between beat (11,1) and (13,1)
            IEnumerable<AddClassicBeat> list = _rdlevel.Decorations[0]
                .OfEvent<AddClassicBeat>()
                .InRange(
                    new TickTime(11, 1), // Start searching from bar 11, beat 1
                    new TickTime(13, 1)  // End searching at bar 13, beat 1
                );
        }
        [TestMethod]
        public void CreateBeatWithoutBinding()
        {
            // Create a beat not associated with a level
            TickTime beat1 = new(11);
            TickTime beat2 = new(2, 3);
            TickTime beat3 = (2, 3);
            TickTime beat4 = new(TimeSpan.FromSeconds(11.45));

            Console.WriteLine(beat1); // [10,?,?]
            Console.WriteLine(beat2); // [?,(2, 3),?]
            Console.WriteLine(beat3); // [?,(2, 3),?]
            Console.WriteLine(beat4); // [?,?,00:00:11.4500000]

            (int bar, float beat) = beat2;
        }
        [TestMethod]
        public void CreateBeatWithBinding()
        {
            // Create a beat associated with a level
            TickTime beat1 = _rdlevel.BeatOf(11);
            TickTime beat2 = _rdlevel.Calculator.BeatOf(2, 3);
            TickTime beat3 = beat1 - 10 + TimeSpan.FromSeconds(11.45);

            Console.WriteLine(beat1); // [2,3]
            Console.WriteLine(beat2); // [2,3]
            Console.WriteLine(beat3); // [3,4.083334]
        }
        [TestMethod]
        public void LinkBeats()
        {
            TickTime beat1 = _rdlevel.BeatOf(1);
            TickTime beat2 = beat1.WithoutLink();

            Console.WriteLine(beat1.FromSameLevel(beat2));       // False
            Console.WriteLine(beat1.FromSameLevelOrNull(beat2)); // True

            TickTime beat3 = beat2.WithLink(_rdlevel.Calculator);

            (int bar, float beat) = beat3; // (1, 1)
        }
        [TestMethod]
        public void ConvertTimeUnit()
        {
            (float, float) barbeat = _rdlevel.Calculator.TimeSpanToBarBeat(TimeSpan.FromSeconds(19.19));
            Console.WriteLine(barbeat); // (4, 8.983334)
        }
        [TestMethod]
        public void RangeUsage()
        {
            IEnumerable<IBaseEvent> result = _rdlevel.InRange(new RhythmDoctor.Components.Range(_rdlevel.DefaultBeat + 10, null));
        }
        [TestMethod]
        public void ExpressionUsage()
        {

            Expression exp1 = new("i2+1");
            Expression exp2 = new(30);
            Expression exp3 = new("25.5");

            Expression result = exp1 - exp2 * exp3;

            Console.WriteLine(result.ExpressionValue); // i2+1-765
        }
        [TestMethod]
        public void AddAndRemoveEvent()
        {
            Comment comment = new() { TickTime = new(12), Text = "My_comment." };
            Console.WriteLine(comment); // [11,?,?] Comment My_comment.

            _rdlevel.Add(comment);
            Console.WriteLine(comment); // [2,4] Comment My_comment.

            _rdlevel.Remove(comment);
            Console.WriteLine(comment); // [11,?,?] Comment My_comment.
        }
        [TestMethod]
        public void AddAndRemoveCpbEvent()
        {
            Level level = [];
            Comment temp = new() { TickTime = (5, 3) };
            level.Add(temp);
            Console.WriteLine(temp.TickTime);               // [5,3]
            Console.WriteLine(temp.TickTime.Tick);      // 35
            SetCrotchetsPerBar cpb = new() { TickTime = (3, 1), CrotchetsPerBar = 6 };

            level.Add(cpb, BeatChangeStrategy.Default);
            Console.WriteLine(temp.TickTime);               // [6,1]
            Console.WriteLine(temp.TickTime.Tick);      // 35 (unchanged due to Default strategy)
            level.Remove(cpb, BeatChangeStrategy.Default);

            level.Add(cpb, BeatChangeStrategy.RDLE);
            Console.WriteLine(temp.TickTime);               // [5,3] (unchanged due to RDLE strategy)
            Console.WriteLine(temp.TickTime.Tick);      // 31
            level.Remove(cpb, BeatChangeStrategy.RDLE);
        }
        [TestMethod]
        public void AddForwardEvent()
        {
            MyEvent myEvent = new();

            myEvent.MyProperty = (2, 1);

            _rdlevel.Add(myEvent);

            myEvent.TickTime = new(8);

            Console.WriteLine(myEvent.Type);        // ForwardEvent  
            Console.WriteLine(myEvent.ActualType);  // MyEvent  
        }
        [TestMethod]
        public void EventTypeUtilsConvert()
        {
            Console.WriteLine(EventType.Tint.ToType());                                              // RhythmBase.Events.Tint
            Console.WriteLine(EventTypeRegistry.ToEnum(typeof(Tint)));                                    // Tint
            Console.WriteLine(EventTypeRegistry.ToEnum<Tint>());                                          // Tint
            Console.WriteLine(string.Join(", ", EventTypeRegistry.ToEnums(typeof(IBarBeginningEvent))));  // PlaySong,SetCrotchetsPerBar, SetHeartExplodeVolume
            Console.WriteLine(string.Join(", ", EventTypeRegistry.ToEnums<IBarBeginningEvent>()));        // PlaySong,SetCrotchetsPerBar, SetHeartExplodeVolume
        }
        [TestMethod]
        public void EventTypeUtilsStatic()
        {
            Console.WriteLine(string.Join(",\n", EventTypeRegistry.DecorationTypes));
            // Comment,
            // CustomDecorationEvent,
            // Move,
            // PlayAnimation,
            // SetVisible,
            // Tile,
            // Tint

            Console.WriteLine(string.Join(",\n", EventTypeRegistry.EventTypeEnumsForCameraFX));
            // MoveCamera,
            // ShakeScreen,
            // FlipScreen,
            // PulseCamera

            Console.WriteLine(string.Join(",\n", EventTypeRegistry.EventTypeEnumsForUtility));
            // Comment,
            // TagAction,
            // CallCustomMethod
        }
        [TestMethod]
        public void RichTextUsage()
        {
            RichLine<RichStringStyle> line = RichLine<RichStringStyle>.Deserialize("Hel<color=#00FF00>lo");

            Console.WriteLine(line.ToString()); // Hello
            Console.WriteLine(line.Serialize()); // Hel<color=lime>lo</color>

            line +=
                new Phrase<RichStringStyle>(" Rhythm")
                {
                    Style = new()
                    {
                        Color = Color.Lime
                    }
                };

            line += " Doctor!";

            Console.WriteLine(line.ToString()); // Hello Rhythm Doctor!
            Console.WriteLine(line.Serialize()); // Hel<color=lime>lo Rhythm</color> Doctor!
        }
        [TestMethod]
        public void RichTextModify()
        {
            RichLine<RichStringStyle> line = RichLine<RichStringStyle>.Deserialize("Hel<color=#00FF00>lo Rhythm</color> Doctor!");

#if NETCOREAPP
            Console.WriteLine(line[6..].ToString()); // Rhythm Doctor!
            Console.WriteLine(line[6..].Serialize()); // <color=lime>Rhythm</color> Doctor!

            line[5] = " and Welcome to ";
#endif

            Console.WriteLine(line.ToString()); // Hello and Welcome to Rhythm Doctor!
            Console.WriteLine(line.Serialize()); // Hel<color=lime>lo</color> and Welcome to <color=lime>Rhythm</color> Doctor!

            return;
        }
        [TestMethod]
        public void RichTextBuild()
        {
            DialogueExchange exchange =
            [
                new DialogueBlock()
                {
                    Character = "Paige",
                    Expression = "neutral",
                    Content = RichLine<DialoguePhraseStyle>.Deserialize("Hel<color=#00FF00>lo [2]<shake>Rhythm</color> Doctor</shake>!"),
                },
                new DialogueBlock()
                {
                    Character = "Ian",
                    Content = "Hello Paige!",
                },
                new DialogueBlock()
                {
                    Character = "Paige",
                    Expression = "happy",
                    Content = new Phrase<DialoguePhraseStyle>("What a good day!")
                    {
                        Events =
                        [
                            new DialogueTone(DialogueToneType.VerySlow,6),
                            new DialogueTone(DialogueToneType.Static,11),
                        ],
                        Style = new DialoguePhraseStyle()
                        {
                            Volume = 0.5f,
                            Bold = true,
                        },
                    }
                }
            ];

            Console.WriteLine(exchange.Serialize());
            // Paige_neutral:Hel<color=lime>lo [2]<shake>Rhythm</color> Doctor</shake>!
            // Ian:Hello Paige!
            // Paige_happy:<volume=0.5><bold>What a[vslow] good[static] day!</volume></bold>
        }
        [TestMethod]
        public void EasingCalculate()
        {
            double var1 = EaseType.InSine.Calculate(0.25);
            double var2 = EaseType.Linear.Calculate(0.5, 4, 9);

            Console.WriteLine(var1); // 0.07612046748871326
            Console.WriteLine(var2); // 6.5
        }
        //[TestMethod]
        //public void RDCode()
        //{
        //    RDLang.Variables.i[1] = 9;

        //    RDLang.TryRun("numMistakesP2 = 3", out float result); // 3
        //    Console.WriteLine(result);
        //    RDLang.TryRun("numMistakesP2+i1", out result); // 12
        //    Console.WriteLine(result);
        //    RDLang.TryRun("atLeastRank(A)", out result); // 1
        //    Console.WriteLine(result);
        //}
        public void Example_01()
        {

            // Read the visual effects level file
            using Level vfxLevel = Level.FromFile(@"vfx.rdlevel");
            // Read the audio level file
            using Level audioLevel = Level.FromFile(@"beat.rdlevel");

            // Remove all rows from the visual effects level
            Row[] vfxrows = [.. vfxLevel.Rows];
            foreach (Row row in vfxrows)
                vfxLevel.Rows.Remove(row);

            // Copy all rows from the audio level into the new level
            foreach (Row row in audioLevel.Rows)
            {
                // Copy row information
                Row row2 = new()
                {
                    Room = row.Room,
                    Character = row.Character,
                    Sound = row.Sound,
                    RowType = row.RowType
                };
                vfxLevel.Rows.Add(row2);

                // Copy events within the row
                BaseBeat[] evts = [.. row.OfEvent<BaseBeat>()];
                foreach (BaseBeat evt in evts)
                    row2.Add(evt);
            }

            // Copy necessary sound events
            foreach (IBaseEvent sound in audioLevel.Where(e =>
                e.Tab == Tab.Sounds &&       // Event is in the Sounds tab
                e is not BaseRowAction &&     // Sound events contain row events; adding row events here would cause reference errors
                e is not PlaySong &&          // No need to copy PlaySong if the music is the same
                e is not SetCrotchetsPerBar)) // The timing of these events is independent of the number of crotchets per bar, so they don't need to be added
            {
                vfxLevel.Add(sound);
            }

            // Write to a new level file
            vfxLevel.SaveToFile(@"result.rdlevel");
        }
        // Create a MyEvent type  
        //   Inherit from CustomEvent  
        public record class MyEvent : ForwardEvent
        {
            // Override property  
            public override Tab Tab => Tab.Actions;

            // All implemented properties need to be bound to and checked for null in the ForwardEvent.ExtraData field.  

            // Implement an PointN type property  
            public PointN? MyProperty
            {
                get
                {
                    // Get the required content from the Data field and check for null  
                    return ExtraData.TryGetValue("myProperty", out JsonElement jsonElement)
                        && jsonElement.GetProperty("x").GetSingle() is float x
                        && jsonElement.GetProperty("y").GetSingle() is float y
                        ? (x, y)
                        : null;
                }
                set
                {
                    // Save the content in the Data field  
                    ExtraData["myProperty"] =
                        value.HasValue ?
                        JsonDocument.Parse($$"""{"x": {{value.Value.X}}, "y": {{value.Value.Y}}}""").RootElement :
                        default;
                }
            }

            // Initialize the type in the constructor  
            public MyEvent()
            {
                // Initialize the ActualType property.
                ActualType = nameof(MyEvent);
            }
        }
    }
