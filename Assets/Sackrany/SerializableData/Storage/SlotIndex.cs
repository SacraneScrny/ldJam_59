using System.Collections.Generic;

namespace Sackrany.SerializableData.Storage
{
    internal class SlotIndex
    {
        public Dictionary<int, SaveSlotMeta> Slots { get; set; } = new();
    }
}
