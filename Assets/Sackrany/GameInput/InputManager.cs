using System;
using System.Collections.Generic;
using System.Threading;

using Sackrany.ConfigSystem;
using Sackrany.GameInput.Caches;
using Sackrany.GameInput.Configurations;
using Sackrany.GameSettings.Configs;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Sackrany.GameInput
{
    public static partial class InputManager
    {
        static CancellationTokenSource _cancellation;
        static readonly Dictionary<Type, IDisposable> _caches = new();

        public static GameInputConfigs InputConfigs { get; private set; }
        public static string InputBindings { get; private set; }
        public static Pointer CurrentPointer { get; private set; }

        internal static void Init()
        {
            _cancellation = new CancellationTokenSource();
            CurrentPointer = new Pointer(_cancellation.Token);
            InputConfigs = ConfigGet<GameInputConfigs>.Value;
            
            InitGenerated();
            ApplySettings();

            Application.quitting -= OnQuitting;
            Application.quitting += OnQuitting;
        }

        public static T Register<T>(T cache) where T : InputActionsCache
        {
            _caches[typeof(T)] = cache;
            return cache;
        }

        public static T Get<T>() where T : InputActionsCache
            => (T)_caches[typeof(T)];

        public static void ApplySettings() => ApplySettingsGenerated();
        static void Internal_ApplySettings(InputActionAsset asset)
        {
            var cfg = ConfigGet<GameInputBindingsConfig>.Value;
            InputConfigs = ConfigGet<GameInputConfigs>.Value;
            InputBindings = cfg.BindingOverridesJson;
            
            asset.LoadBindingOverridesFromJson(cfg.BindingOverridesJson);
        }

        static void OnQuitting()
        {
            SaveControlsGenerated();
            DisposeGenerated();

            foreach (var cache in _caches.Values)
                cache.Dispose();

            _caches.Clear();

            _cancellation?.Cancel();
            _cancellation?.Dispose();
        }

        static partial void InitGenerated();
        static partial void DisposeGenerated();
        static partial void ApplySettingsGenerated();
        static partial void SaveControlsGenerated();
    }
}