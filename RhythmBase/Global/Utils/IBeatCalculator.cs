using RhythmBase.RhythmDoctor.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.Global.Utils
{
    /// <summary>
    /// Provides methods for calculating beat values and converting between different beat representations.
    /// </summary>
    /// <typeparam name="TBeat">The type of beat. Must be a struct and implement <see cref="IBeat{TBeat}"/>.</typeparam>
    public interface IBeatCalculator<TBeat>
        where TBeat : struct, IBeat<TBeat>
    {
        /// <summary>
        /// Creates a beat from a beat-only value.
        /// </summary>
        /// <param name="beatOnly">The beat-only value.</param>
        /// <returns>A beat representing the specified beat-only value.</returns>
        TBeat BeatOf(float beatOnly);
        /// <summary>
        /// Creates a beat from a time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>A beat representing the specified time span.</returns>
        TBeat BeatOf(TimeSpan timeSpan);
        /// <summary>
        /// Converts a beat-only value to a time span.
        /// </summary>
        /// <param name="beat">The beat-only value.</param>
        /// <returns>The time span corresponding to the beat-only value.</returns>
        TimeSpan BeatOnlyToTimeSpan(float beat);
        /// <summary>
        /// Gets the beats per minute at the specified beat.
        /// </summary>
        /// <param name="beat">The beat.</param>
        /// <returns>The beats per minute at the specified beat.</returns>
        float BeatsPerMinuteOf(TBeat beat);
        /// <summary>
        /// Creates a beat range from two beats.
        /// </summary>
        /// <param name="beat1">The first beat.</param>
        /// <param name="beat2">The second beat.</param>
        /// <returns>A beat range from <paramref name="beat1"/> to <paramref name="beat2"/>.</returns>
        IBeatRange<TBeat> IntervalOf(TBeat beat1, TBeat beat2);
        /// <summary>
        /// Creates a beat range from two time spans.
        /// </summary>
        /// <param name="timeSpan1">The first time span.</param>
        /// <param name="timeSpan2">The second time span.</param>
        /// <returns>A beat range from <paramref name="timeSpan1"/> to <paramref name="timeSpan2"/>.</returns>
        IBeatRange<TBeat> IntervalOf(TimeSpan timeSpan1, TimeSpan timeSpan2);
        /// <summary>
        /// Refreshes the calculator state.
        /// </summary>
        void Refresh();
        /// <summary>
        /// Converts a time span to a beat-only value.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The beat-only value corresponding to the time span.</returns>
        float TimeSpanToBeatOnly(TimeSpan timeSpan);
    }
}
