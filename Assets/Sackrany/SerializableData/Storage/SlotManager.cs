using System;
using System.Collections.Generic;

namespace Sackrany.SerializableData.Storage
{
    public class SlotManager
    {
        readonly Action _onSlotSwitched;

        internal SlotManager(Action onSlotSwitched)
        {
            _onSlotSwitched = onSlotSwitched;
        }

        public int Current => SaveDataStorage.CurrentSlot;

        public void Switch(int slot)
        {
            SaveDataStorage.SwitchSlot(slot);
            _onSlotSwitched?.Invoke();
        }

        public int Create()
        {
            var index = SaveDataStorage.LoadSlotIndex();

            int newSlot = 0;
            while (index.Slots.ContainsKey(newSlot))
                newSlot++;

            index.Slots[newSlot] = new SaveSlotMeta { Slot = newSlot };
            SaveDataStorage.FlushSlotIndex();

            SaveDataStorage.SwitchSlot(newSlot);
            SaveDataStorage.Reset();
            _onSlotSwitched?.Invoke();

            return newSlot;
        }

        public void Clear(int slot)
        {
            SaveDataStorage.ClearSlot(slot);

            var index = SaveDataStorage.LoadSlotIndex();
            index.Slots.Remove(slot);
            SaveDataStorage.FlushSlotIndex();
        }

        public SaveSlotMeta GetMeta(int slot)
        {
            var index = SaveDataStorage.LoadSlotIndex();
            return index.Slots.TryGetValue(slot, out var meta)
                ? meta
                : new SaveSlotMeta { Slot = slot };
        }

        public IReadOnlyDictionary<int, SaveSlotMeta> GetAllMeta()
            => SaveDataStorage.LoadSlotIndex().Slots;

        public void SetMetaField(string key, object value)
        {
            var index = SaveDataStorage.LoadSlotIndex();

            if (!index.Slots.TryGetValue(Current, out var meta))
            {
                meta = new SaveSlotMeta { Slot = Current };
                index.Slots[Current] = meta;
            }

            meta.SetCustom(key, value);
        }

        internal void SetThumbnail(byte[] bytes)
        {
            var index = SaveDataStorage.LoadSlotIndex();

            if (!index.Slots.TryGetValue(Current, out var meta))
            {
                meta = new SaveSlotMeta { Slot = Current };
                index.Slots[Current] = meta;
            }

            meta.Thumbnail = bytes;
        }

        internal void StampCurrentSlot(TimeSpan playTime)
        {
            var index = SaveDataStorage.LoadSlotIndex();

            if (!index.Slots.TryGetValue(Current, out var meta))
            {
                meta = new SaveSlotMeta { Slot = Current };
                index.Slots[Current] = meta;
            }

            meta.SavedAt = DateTime.Now;
            meta.PlayTime += playTime;

            SaveDataStorage.FlushSlotIndex();
        }
    }
}
