using RhythmBase.RhythmDoctor.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Extensions.Collaboration.RDEditorPlus;

/// <summary>
/// Provides extension methods for RDEditorPlus collaboration features.
/// </summary>
public static class RDEditorPlus
{
    // https://github.com/9thCore/RDEditorPlus/blob/737c8df41b242125eed272d5876860b55ed38578/ExtraData/SubRowStorage.cs#L378
    private const string SubRowKey = "mod_rdEditorPlus_subRow";
    extension(IBaseEvent e)
    {
        /// <summary>
        /// Gets or sets the sub-row index associated with the current item.
        /// </summary>
        /// <remarks>If the value has not been set, this property returns 0. Setting this property
        /// updates the corresponding entry in the underlying dictionary.</remarks>
        public int SubRow
        {
            get
            {
                JsonElement value = e[SubRowKey];
                if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int subRow))
                {
                    return subRow;
                }
                return 0;
            }
            set
            {
                e[SubRowKey] = JsonElement.Parse(value.ToString());
            }
        }
    }
}