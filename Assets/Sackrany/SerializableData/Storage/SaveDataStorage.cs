using System.IO;

using Newtonsoft.Json;

using UnityEngine;

namespace Sackrany.SerializableData.Storage
{
    internal static class SaveDataStorage
    {
        const string DataFileName = "saveData";
        const string FileExtension = ".json";
        const string AppVersionKey = "AppVersion";
        const string SlotIndexKey = "save_slots";
        const string SlotFolderPrefix = "Slot_";
        const string PlayerPrefsPrefix = "save_";

        static string _dataPath;
        static bool _usePlayerPrefs;
        static int _currentSlot;
        static SaveDataStructure _cache;
        static SlotIndex _slotIndex;

        internal static void Initialize(string dataPath, bool usePlayerPrefs)
        {
            _dataPath = dataPath;
            _usePlayerPrefs = usePlayerPrefs;
            _currentSlot = 0;
            _cache = null;
            _slotIndex = null;
        }

        internal static int CurrentSlot => _currentSlot;

        internal static void SwitchSlot(int slot)
        {
            _currentSlot = slot;
            _cache = null;
        }

        internal static T Get<T>(string key) where T : new()
        {
            EnsureSlotLoaded();

            if (_cache[key] is not T)
                _cache[key] = new T();

            _cache[key] ??= new T();

            return (T)_cache[key];
        }

        internal static void Set(string key, object value)
        {
            EnsureSlotLoaded();
            _cache[key] = value;
        }

        internal static void Flush()
        {
            EnsureSlotLoaded();
            _cache[AppVersionKey] = Application.version;

            var json = Serialize(_cache);

            if (_usePlayerPrefs)
            {
                PlayerPrefs.SetString(SlotPlayerPrefsKey(_currentSlot), json);
                PlayerPrefs.Save();
                return;
            }

            EnsureSlotDirectoryExists(_currentSlot);
            File.WriteAllText(SlotFilePath(_currentSlot), json);
        }

        internal static void Reset()
        {
            _cache = new SaveDataStructure();
        }

        internal static SlotIndex LoadSlotIndex()
        {
            if (_slotIndex != null) return _slotIndex;

            _slotIndex = _usePlayerPrefs
                ? LoadSlotIndexFromPlayerPrefs()
                : LoadSlotIndexFromFile();

            _slotIndex ??= new SlotIndex();
            return _slotIndex;
        }

        internal static void FlushSlotIndex()
        {
            if (_slotIndex == null) return;

            var json = Serialize(_slotIndex);

            if (_usePlayerPrefs)
            {
                PlayerPrefs.SetString(SlotIndexKey, json);
                PlayerPrefs.Save();
                return;
            }

            EnsureRootDirectoryExists();
            File.WriteAllText(SlotIndexFilePath(), json);
        }

        internal static void ClearSlot(int slot)
        {
            if (_currentSlot == slot)
                _cache = new SaveDataStructure();

            if (_usePlayerPrefs)
            {
                PlayerPrefs.DeleteKey(SlotPlayerPrefsKey(slot));
                return;
            }

            var path = SlotFilePath(slot);
            if (File.Exists(path)) File.Delete(path);
        }

        static void EnsureSlotLoaded()
        {
            if (_cache != null) return;

            _cache = _usePlayerPrefs
                ? LoadSlotFromPlayerPrefs(_currentSlot)
                : LoadSlotFromFile(_currentSlot);

            _cache ??= new SaveDataStructure();

            if (IsVersionMismatch())
                _cache = new SaveDataStructure();
        }

        static SaveDataStructure LoadSlotFromPlayerPrefs(int slot)
        {
            var key = SlotPlayerPrefsKey(slot);
            return PlayerPrefs.HasKey(key)
                ? Deserialize<SaveDataStructure>(PlayerPrefs.GetString(key))
                : null;
        }

        static SaveDataStructure LoadSlotFromFile(int slot)
        {
            var path = SlotFilePath(slot);
            return File.Exists(path)
                ? Deserialize<SaveDataStructure>(File.ReadAllText(path))
                : null;
        }

        static SlotIndex LoadSlotIndexFromPlayerPrefs()
            => PlayerPrefs.HasKey(SlotIndexKey)
                ? Deserialize<SlotIndex>(PlayerPrefs.GetString(SlotIndexKey))
                : null;

        static SlotIndex LoadSlotIndexFromFile()
        {
            var path = SlotIndexFilePath();
            return File.Exists(path)
                ? Deserialize<SlotIndex>(File.ReadAllText(path))
                : null;
        }

        static bool IsVersionMismatch()
            => _cache.TryGetValue(AppVersionKey, out var v)
               && GetMajorVersion((string)v) != GetMajorVersion(Application.version);

        static void EnsureSlotDirectoryExists(int slot)
        {
            var dir = SlotDirPath(slot);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        static void EnsureRootDirectoryExists()
        {
            if (!Directory.Exists(_dataPath)) Directory.CreateDirectory(_dataPath);
        }

        static string SlotDirPath(int slot) => $"{_dataPath}{SlotFolderPrefix}{slot}/";
        static string SlotFilePath(int slot) => $"{SlotDirPath(slot)}{DataFileName}{FileExtension}";
        static string SlotIndexFilePath() => $"{_dataPath}slots{FileExtension}";
        static string SlotPlayerPrefsKey(int slot) => $"{PlayerPrefsPrefix}{slot}";

        static string GetMajorVersion(string version)
            => string.IsNullOrEmpty(version) ? "0" : version.Split('.')[0];

        static string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);
        static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);
    }
}
