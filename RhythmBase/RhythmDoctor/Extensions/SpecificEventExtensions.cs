using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Events;
using System.Text;

namespace RhythmBase.RhythmDoctor.Extensions
{
    public static partial class Extensions
    {
        extension(AddClassicBeat e)
        {
            /// <summary>
            /// Gets the hit information for the current instance.
            /// </summary>
            public RDHit Hit => new(
                e,
                e.Beat + (e.Tick * (e.Length - ((e.Swing == 0) ? 1 : e.Swing))),
                e.Hold);
            /// <summary>
            /// Gets the synchronization offset value for the current event, based on the most recent active parent row's
            /// synchronization settings.
            /// </summary>
            public float SyncoOffset
            {
                get
                {
                    SetRowXs? x = e.Parent?
                        .OfEvent<SetRowXs>()
                        .LastOrDefault(i => i.Active && e.IsBehind(i));
                    if (x is null || 0 > x.SyncoBeat)
                        return 0f;
                    else return x.SyncoSwing == 0f ? 0.5f : x.SyncoSwing;
                }
            }
            /// <summary>
            /// Gets the collection of beat patterns associated with the current event.
            /// </summary>
            public PatternCollection Pattern
            {
                get
                {
                    if (e.SetXs is not ClassicBeatPattern.NoChange)
                    {
                        return e.SetXs switch
                        {
                            ClassicBeatPattern.ThreeBeat => "-xx-xx",
                            ClassicBeatPattern.FourBeat => "-x-x-x",
                            ClassicBeatPattern.NoChange or _ => "------",
                        };
                    }
                    SetRowXs? last = e.Beat.BaseLevel?
                        .OfEvent<SetRowXs>()
                        .InRange(null, e.Beat)
                        .LastOrDefault(i => i.Active && e.IsBehind(i));
                    return last?.Pattern ?? "------";
                }
            }
            /// <summary>
            /// Returns the pulse beat of the specified 0-based index.
            /// </summary>
            /// <exception cref="T:RhythmBase.Global.Exceptions.RhythmBaseException">THIS IS 7TH BEAT GAMES!</exception>
            public RDBeat BeatOf(byte index)
            {
                if (index >= 7)
                    throw new RhythmBaseException("THIS IS 7TH BEAT GAMES!");
                SetRowXs x = e.Parent?.OfEvent<SetRowXs>().LastOrDefault(i => i.Active && e.IsBehind(i)) ?? new();
                float synco = 0 <= x.SyncoBeat && x.SyncoBeat < (sbyte)index ? (float)((x.SyncoSwing == 0f) ? 0.5 : ((double)x.SyncoSwing)) : 0f;
                return e.Beat.DurationOffset(e.Tick * (index - synco));
            }
            /// <summary>
            /// Generate split event instances.
            /// </summary>
            public BaseBeat[] Splitted()
            {
                SetRowXs? x = e.Parent?.OfEvent<SetRowXs>().LastOrDefault(i => i.Active && e.IsBehind(i));
                return e.Splitted(x?.Pattern ?? PatternCollection.Default, x?.SyncoBeat ?? -1, x?.SyncoSwing ?? 0);
            }
            /// <summary>
            /// Generate split event instances.
            /// </summary>
            public BaseBeat[] Splitted(PatternCollection patterns, int syncoBeat = -1, float syncoSwing = 0)
            {
                List<BaseBeat> l = [];
                byte i = (byte)(7 - e.Length);
                AddFreeTimeBeat head = e.CloneAs<AddFreeTimeBeat>();
                head.Pulse = i++;
                head.Hold = e.Hold;
                l.Add(head);
                do
                {
                    if (!(i < 6 && patterns[i] == Pattern.X))
                    {
                        PulseFreeTimeBeat pulse = e.CloneAs<PulseFreeTimeBeat>();
                        PulseFreeTimeBeat pulseFreeTimeBeat;
                        (pulseFreeTimeBeat = pulse).Beat = pulseFreeTimeBeat.Beat + e.Tick * i;
                        if (i >= syncoBeat)
                            (pulseFreeTimeBeat = pulse).Beat = pulseFreeTimeBeat.Beat - syncoSwing;
                        if (i % 2 == 1)
                            (pulseFreeTimeBeat = pulse).Beat = pulseFreeTimeBeat.Beat + (e.Tick - ((e.Swing == 0f) ? e.Tick : e.Swing));
                        pulse.Hold = e.Hold;
                        pulse.Action = PulseAction.Increment;
                        l.Add(pulse);
                    }
                    i++;
                }
                while (i <= 6);
                return [.. l];
            }
        }
        extension(AddFreeTimeBeat e)
        {
            /// <summary>
            /// Check if it can be hit by player or cpu.
            /// </summary>
            public bool IsHitable
            {
                get
                {
                    return e.Pulse == 6;
                }
            }
            /// <summary>
            /// Gets the hit information for the current instance.
            /// </summary>
            public RDHit Hit => new(e, e.Beat, e.Hold);
            /// <summary>
            /// Get the sequence of <see cref="T:RhythmBase.RhythmDoctor.PulseFreeTimeBeat" /> belonging to this <see cref="T:RhythmBase.RhythmDoctor.AddFreeTimeBeat" />, return all of the <see cref="T:RhythmBase.Events.PulseFreeTimeBeat" /> from the time the pulse was created to the time it was removed or hit.
            /// </summary>
            public PulseFreeTimeBeat[] Pulses
            {
                get
                {
                    if (e.Parent == null)
                        return [];
                    List<PulseFreeTimeBeat> result = [];
                    byte pulse = e.Pulse;
                    foreach (PulseFreeTimeBeat item in e.Parent.OfEvent<PulseFreeTimeBeat>().Where(i => i.Active && e.IsInFrontOf(i)))
                    {
                        switch (item.Action)
                        {
                            case PulseAction.Increment:
                                pulse += 1;
                                result.Add(item);
                                break;
                            case PulseAction.Decrement:
                                pulse = (byte)((pulse > 0b1) ? (pulse - 0b1) : 0b1);
                                result.Add(item);
                                break;
                            case PulseAction.Custom:
                                pulse = (byte)item.CustomPulse;
                                result.Add(item);
                                break;
                            case PulseAction.Remove:
                                result.Add(item);
                                break;
                        }
                        if (pulse == e.Parent.Length - 1)
                            break;
                    }
                    return [.. result];
                }
            }
        }
        extension(AddOneshotBeat e)
        {
            /// <summary>
            /// Gets the hit information for the current instance.
            /// </summary>
            public RDHit[] Hits
            {
                get
                {
                    RDHit[] hits = new RDHit[e.Loop + 1];
                    for (int i = 0; i <= e.Loop; ++i)
                        hits[i] = new RDHit(
                            e,
                            e.Beat + (i * e.Interval) + e.Tick,
                            e.Hold ? e.Interval - e.Tick : 0);
                    return hits;
                }
            }
            /// <summary>
            /// Generate split event instances.
            /// </summary>
            public AddOneshotBeat[] Splitted()
            {
                e._beat.IfNullThrowException();
                AddOneshotBeat[] l = new AddOneshotBeat[e.Loop + 1];
                uint loops = e.Loop;
                for (uint i = 0U; i <= loops; i += 1U)
                {
                    AddOneshotBeat T = e with { };
                    T.Loop = 0U;
                    T.Interval = 0f;
                    T.Beat = new RDBeat(e._beat._calculator!, unchecked(e.Beat.BeatOnly + i * e.Interval));
                    l[i] = T;
                }
                return l;
            }
        }
        extension(PulseFreeTimeBeat e)
        {
            /// <summary>
            /// Check if it can be hit by player or cpu.
            /// </summary>
            public bool IsHitable
            {
                get
                {

                    int pulseIndexMin = 6;
                    int pulseIndexMax = 6;
                    if (e.Parent is null)
                        return false;
                    foreach (BaseBeat item in ((IEnumerable<BaseBeat>)e.Parent
                    .OfEvent<BaseBeat>()
                    .InRange(new RDRange(e.Beat, null))
                    .Where(e.IsBehind))
                    .Reverse())
                    {
                        EventType type = item.Type;
                        switch (type)
                        {
                            case EventType.AddFreeTimeBeat:
                                {
                                    AddFreeTimeBeat temp2 = (AddFreeTimeBeat)item;
                                    if (pulseIndexMin <= temp2.Pulse & temp2.Pulse <= pulseIndexMax)
                                        return true;
                                    break;
                                }
                            case EventType.PulseFreeTimeBeat:
                                {
                                    PulseFreeTimeBeat temp = (PulseFreeTimeBeat)item;
                                    switch (temp.Action)
                                    {
                                        case PulseAction.Increment:
                                            if (pulseIndexMin > 0)
                                                pulseIndexMin--;
                                            if (!(pulseIndexMax > 0))
                                                return false;
                                            pulseIndexMax--;
                                            break;
                                        case PulseAction.Decrement:
                                            if (pulseIndexMin > 0)
                                                pulseIndexMin++;
                                            if (!(pulseIndexMax < 6))
                                                return false;
                                            pulseIndexMax++;
                                            break;
                                        case PulseAction.Custom:
                                            if (!(pulseIndexMin <= temp.CustomPulse & temp.CustomPulse <= pulseIndexMax))
                                                return false;
                                            pulseIndexMin = 0;
                                            pulseIndexMax = 5;
                                            break;
                                        case PulseAction.Remove:
                                            return false;
                                        default:
                                            throw new RhythmBaseException("Unknown PulseAction");
                                    }
                                    if (pulseIndexMin > pulseIndexMax)
                                        return false;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    return false;
                }
            }
            /// <summary>
            /// Gets the hit information for the current instance.
            /// </summary>
            public RDHit Hit => new(e, e.Beat, e.Hold);
        }
        extension(FloatingText e)
        {
            /// <summary>
            /// Splits the <see cref="FloatingText"/> text into an array of strings based on custom delimiters.
            /// Supports '/' as a line break, '\n' as a newline, and escape sequences such as '\\n' and '\/'.
            /// </summary>
            public string[] Splitted
            {
                get
                {
                    if (string.IsNullOrEmpty(e.Text))
                        return [];
                    List<string> strs = [];
                    StringBuilder sb = new();
                    int i = 0;
                    while (i < e.Text.Length)
                    {
                        char c = e.Text[i];
                        switch (c)
                        {
                            case '/':
                                strs.Add(sb.ToString());
                                break;
                            case '\\':
                                if (i + 1 >= e.Text.Length)
                                {
                                    sb.Append(c);
                                    break;
                                }
                                i++;
                                char nextChar = e.Text[i];
                                switch (nextChar)
                                {
                                    case 'n':
                                        sb.Append('\n');
                                        break;
                                    case '/':
                                        sb.Append('/');
                                        break;
                                    default:
                                        sb.Append(c);
                                        sb.Append(nextChar);
                                        break;
                                }
                                break;
                            case '\n':
                                strs.Add(sb.ToString());
                                sb.Clear();
                                break;
                            default:
                                sb.Append(c);
                                break;
                        }
                        i++;
                    }
                    return [.. strs];
                }
            }
            /// <summary>
            /// Creates a new <see cref="T:RhythmBase.RhythmDoctor.AdvanceText" /> subordinate to <see cref="T:RhythmBase.RhythmDoctor.FloatingText" /> at the specified beat. The new event created will be attempted to be added to the <see cref="T:RhythmBase.RhythmDoctor.FloatingText" />'s source level.
            /// </summary>
            /// <param name="beat">Specified beat.</param>
            public AdvanceText CreateChild(RDBeat beat)
            {
                AdvanceText A = new()
                {
                    Parent = e,
                    Beat = beat.WithoutLink()
                };
                e.Children.Add(A);
                return A;
            }
        }
        extension(SayReadyGetSetGo e)
        {
            /// <summary>
            /// Generate split event instances. These instances are not insert into the level.
            /// </summary>
            public SayReadyGetSetGo[] Splitted() => e.PhraseToSay switch
            {
                SayReadyGetSetGoWord.SayReaDyGetSetGoNew => [
                    e.SplitCopy(0f, SayReadyGetSetGoWord.JustSayRea),
                e.SplitCopy(e.Tick, SayReadyGetSetGoWord.JustSayDy),
                e.SplitCopy(e.Tick * 2f, SayReadyGetSetGoWord.JustSayGet),
                e.SplitCopy(e.Tick * 3f, SayReadyGetSetGoWord.JustSaySet),
                e.SplitCopy(e.Tick * 4f, SayReadyGetSetGoWord.JustSayGo)
                ],
                SayReadyGetSetGoWord.SayGetSetGo => [
                    e.SplitCopy(0f, SayReadyGetSetGoWord.JustSayGet),
                e.SplitCopy(e.Tick, SayReadyGetSetGoWord.JustSaySet),
                e.SplitCopy(e.Tick * 2f, SayReadyGetSetGoWord.JustSayGo)
                ],
                SayReadyGetSetGoWord.SayReaDyGetSetOne => [
                    e.SplitCopy(0f, SayReadyGetSetGoWord.JustSayRea),
                e.SplitCopy(e.Tick, SayReadyGetSetGoWord.JustSayDy),
                e.SplitCopy(e.Tick * 2f, SayReadyGetSetGoWord.JustSayGet),
                e.SplitCopy(e.Tick * 3f, SayReadyGetSetGoWord.JustSaySet),
                e.SplitCopy(e.Tick * 4f, SayReadyGetSetGoWord.Count1)
                ],
                SayReadyGetSetGoWord.SayGetSetOne => [
                    e.SplitCopy(0f, SayReadyGetSetGoWord.JustSayGet),
                e.SplitCopy(e.Tick, SayReadyGetSetGoWord.JustSaySet),
                e.SplitCopy(e.Tick * 2f, SayReadyGetSetGoWord.Count1)
                ],
                SayReadyGetSetGoWord.SayReadyGetSetGo => [
                    e.SplitCopy(0f, SayReadyGetSetGoWord.JustSayReady),
                e.SplitCopy(e.Tick * 2f, SayReadyGetSetGoWord.JustSayGet),
                e.SplitCopy(e.Tick * 3f, SayReadyGetSetGoWord.JustSaySet),
                e.SplitCopy(e.Tick * 4f, SayReadyGetSetGoWord.JustSayGo)
                ],
                _ => [e],
            };
        }
        extension(Row e)
        {
            /// <summary>
            /// Gets the audio associated with the specified beat, taking into account any active sound overrides.
            /// </summary>
            public RDAudio PulseSoundAt(RDBeat beat) =>
                e.Parent?
                    .OfEvent<SetBeatSound>()
                    .InRange(null, beat)
                    .LastOrDefault(i => i.Active)?.Sound ??
                    e.Sound;
            /// <summary>
            /// Gets the player type assigned at the specified beat.
            /// </summary>
            public PlayerType PlayerAt(RDBeat beat)
            {
                return
                    e.Parent?
                        .OfEvent<ChangePlayersRows>()
                        .InRange(null, beat)
                        .LastOrDefault(i => i.Active && i.Players[e.Index] != PlayerType.NoChange)?.Players[e.Index]
                        ?? e.Player;
            }
            /// <summary>
            /// Gets the hit sound associated with the specified beat, based on the current player and event state.
            /// </summary>
            public RDAudio HitSoundAt(RDBeat beat)
            {
                IEnumerable<IBaseEvent>? events = e.Parent?.OfEvents(
                    EventType.ChangePlayersRows,
                    EventType.SetClapSounds
                    ).InRange(null, beat);
                PlayerType playerType = e.Player;
                RDAudio? p1s = null, p2s = null, cpus = null;
                foreach (var ev in events ?? [])
                {
                    switch (ev)
                    {
                        case ChangePlayersRows cpr:
                            {
                                PlayerType pt = cpr.Players[e.Index];
                                if (pt != PlayerType.NoChange)
                                    playerType = pt;
                                break;
                            }
                        case SetClapSounds scs:
                            {
                                p1s ??= scs.P1Sound;
                                p2s ??= scs.P2Sound;
                                cpus ??= scs.CpuSound;
                                break;
                            }
                    }
                }
                return playerType switch
                {
                    PlayerType.P1 => p1s,
                    PlayerType.P2 => p2s,
                    PlayerType.CPU => cpus,
                    _ => null
                } ?? new()
                {
                    Filename = "sndClapHit",
                    Offset = TimeSpan.Zero,
                    Pan = 100,
                    Pitch = 100,
                    Volume = 100
                };
            }
            /// <summary>
            /// Gets the index of the row at the specified beat.
            /// </summary>
            public int IndexAt(RDBeat beat) => e.InRange(null, beat).OfEvent<ReorderRow>().LastOrDefault()?.Order ?? e.Index;
            /// <summary>
            /// Gets the room of the row at the specified beat.
            /// </summary>
            public RDRoomIndex RoomAt(RDBeat beat) => e.InRange(null, beat).OfEvent<ReorderRow>().LastOrDefault()?.NewRoom ?? e.Room.Room;
        }
        extension(SetVFXPreset e)
        {
            /// <summary>
            /// Calculates the duration of the VFX effect for the given preset.
            /// </summary>
            public RDRange VfxDuration
            {
                get
                {
                    if (e.Preset != VfxPreset.DisableAll && e.Enable)
                    {
                        SetVFXPreset? close = e.After().FirstOrDefault(i =>
                            i.Rooms.Contains(e.Rooms) && (
                                i.Preset == e.Preset ||
                                i.Preset == VfxPreset.DisableAll
                        ));
                        return new(e.Beat, close?.Beat);
                    }
                    return new(e.Beat, e.Beat);
                }
            }
        }
        extension(Decoration e)
        {
            /// <summary>
            /// Gets the depth of the decoration at the specified beat.
            /// </summary>
            public int DepthAt(RDBeat beat) => e.InRange(null, beat).OfEvent<ReorderSprite>().LastOrDefault()?.Depth ?? e.Depth;
            /// <summary>
            /// Gets the room of the decoration at the specified beat.
            /// </summary>
            public RDRoomIndex RoomAt(RDBeat beat) => e.InRange(null, beat).OfEvent<ReorderSprite>().LastOrDefault()?.NewRoom ?? e.Room.Room;
        }
        /// <summary>
        /// Getting controlled events.
        /// </summary>
        public static IEnumerable<IGrouping<string, IBaseEvent>> ControllingEvents(this TagAction e) => e.Beat.BaseLevel?.GetTaggedEvents(e.ActionTag, e.Action
            is ActionTagAction.RunAll
            or ActionTagAction.EnableAll
            or ActionTagAction.DisableAll) ?? [];
        extension(RDBeat beat)
        {
            /// <summary>
            /// Get the end beat of the duration.
            /// </summary>
            /// <param name="duration"></param>
            /// <returns></returns>
            /// <exception cref="InvalidRDBeatException"></exception>
            public RDBeat DurationOffset(float duration)
            {
                beat.IfNullThrowException();
                RDBeat off = beat + duration;
                if (off.Bpm == beat.Bpm)
                    return off;
                TimeSpan offt = TimeSpan.FromMinutes(duration / beat.Bpm);
                off = beat + offt;
                return off;
            }
        }

        extension<TEvent, TType, TBeat>(OrderedEventCollection<TEvent, TType, TBeat> e)
            where TEvent : IEvent<TType, TBeat>
            where TType : struct, Enum
            where TBeat : struct, IBeat<TBeat>
        {
            /// <summary>
            /// Determine if <paramref name="item1" /> is after <paramref name="item2" />
            /// </summary>
            /// <returns><list type="table">
            /// <item>If <paramref name="item1" /> is after <paramref name="item2" />, <see langword="true" /></item>
            /// <item>Else, <see langword="false" /></item>
            /// </list></returns>
            public bool IsBehind(TEvent item1, TEvent item2) =>
                item1.Beat.CompareTo(item2.Beat) > 0 ||
                (item1.Beat.BeatOnly == item2.Beat.BeatOnly && e.eventsBeatOrder[item1.Beat].CompareTo(item2, item1));
            /// <summary>
            /// Determine if <paramref name="item1" /> is in front of <paramref name="item2" />
            /// </summary>
            /// <returns><list type="table">
            /// <item>If <paramref name="item1" /> is in front of <paramref name="item2" />, <see langword="true" /></item>
            /// <item>Else, <see langword="false" /></item>
            /// </list></returns>
            public bool IsInFrontOf(TEvent item1, TEvent item2) =>
                item1.Beat .CompareTo( item2.Beat)<0 ||
                ((item1.Beat.BeatOnly == item2.Beat.BeatOnly) && e.eventsBeatOrder[item1.Beat].CompareTo(item1, item2));
        }

        extension(IBaseEvent e)
        {
            /// <summary>
            /// Check if another event is after itself, including events of the same beat but executed after itself.
            /// </summary>
            public bool IsBehind(IBaseEvent item) => e.Beat.BaseLevel?.IsBehind(e, item) ?? throw new InvalidRDBeatException();

            /// <summary>
            /// Check if another event is in front of itself, including events of the same beat but executed before itself.
            /// </summary>
            public bool IsInFrontOf(IBaseEvent item) => e.Beat.BaseLevel?.IsInFrontOf(e, item) ?? throw new InvalidRDBeatException();
        }

        extension(MoveRoom e)
        {
            /// <summary>
            /// Specifies the position of the image. This method changes both the pivot and the angle to keep the image visually in its original position.
            /// </summary>
            /// <param name="target">Specified position. </param>
            public void MovePositionMaintainVisual(RDSize target)
            {
                e.Position = (RDPoint)target;
                e.Pivot = (e.VisualPosition() - target).Rotate(e.Angle ?? 0);
            }
            /// <summary>
            /// The visual position of the lower left corner of the image.
            /// </summary>
            public RDPoint VisualPosition()
            {
                RDPoint visualPosition = default;
                if (e is not { Position: not null, Pivot: not null, Angle: not null })
                    return visualPosition;
                RDPoint previousPosition = e.Position.Value;
                RDPoint previousPivot = new((e.Pivot?.X) * (e.Scale?.Width), (e.Pivot?.Y) * (e.Scale?.Height));
                visualPosition = previousPosition + new RDSize(previousPivot.Rotate(e.Angle ?? 0));
                return visualPosition;
            }
            /// <summary>
            /// Creates a rotated rectangle for the MoveRoom event.
            /// </summary>
            /// <returns>A rotated rectangle representing the room's position, scale, pivot, and angle.</returns>
            public RDRotatedRectE RotatedRect() => new(e.Position, e.Scale, e.Pivot, e.Angle);
        }

        extension(MoveCamera e)
        {
            /// <summary>
            /// Creates a rotated rectangle for the MoveCamera event.
            /// </summary>
            /// <returns>A rotated rectangle representing the camera's position, zoom, and angle.</returns>
            public RDRotatedRectE RotatedRect() => new(e.CameraPosition, new(e.Zoom, e.Zoom), null, e.Angle);
        }

        extension(MoveRow e)
        {
            /// <summary>
            /// Creates a rotated rectangle for the MoveRow event.
            /// </summary>
            /// <returns>A rotated rectangle representing the row's position, scale, pivot, and angle.</returns>
            public RDRotatedRectE RotatedRect() => new(e.Position, e.Scale, new(e.Pivot, e.Pivot), e.Angle);
        }

        extension(Move e)
        {
            /// <summary>
            /// Creates a rotated rectangle for the Move event.
            /// </summary>
            /// <returns>A rotated rectangle representing the position, scale, pivot, and angle.</returns>
            public RDRotatedRectE RotatedRect() => new(e.Position, e.Scale, e.Pivot, e.Angle);
        }

        extension(SayReadyGetSetGo e)
        {
            private SayReadyGetSetGo SplitCopy(float extraBeat, SayReadyGetSetGoWord word)
            {
                SayReadyGetSetGo temp = e with { };
                temp.Beat += extraBeat;
                temp.PhraseToSay = word;
                temp.Volume = e.Volume;
                return temp;
            }
        }
        extension(TagAction e)
        {
            /// <summary>
            /// Gets the events controlled by this <see cref="TagAction"/> event, grouped by their action tags.
            /// </summary>
            public IEnumerable<IGrouping<string, IBaseEvent>> ControllingEvents => e.Beat.BaseLevel?.GetTaggedEvents(e.ActionTag, e.Action
                is ActionTagAction.RunAll
                or ActionTagAction.EnableAll
                or ActionTagAction.DisableAll) ?? [];
        }
    }
}