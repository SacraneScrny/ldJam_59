using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Traits.Tags;
using Sackrany.Actor.UnitMono;
using Sackrany.Extensions;

using UnityEngine;

namespace Sackrany.Actor.Builder
{
    public class UnitBuildChain
    {
        readonly GameObject _go;
        Unit _unit;
        ModulesController _controller;

        internal UnitBuildChain(GameObject go)
        {
            _go = go;
        }

        // ─── Unit ────────────────────────────────────────────────────────
        public UnitBuildChain AddUnit()
        {
            _unit = _go.GetOrAdd<Unit>();
            _controller = _unit.GetController();
            _controller.Default = Array.Empty<ModuleTemplate>();
            return this;
        }

        public UnitBuildChain Trace()
        {
            _unit.DebugTracing = true;
            return this;
        }
        public UnitBuildChain DontTrace()
        {
            _unit.DebugTracing = false;
            return this;
        }

        public UnitBuildChain Seal()
        {
            if (_controller != null)
                _controller.Mode = ControllerMode.Sealed;
            return this;
        }

        public UnitBuildChain WithTag<T>() where T : ITag, new()
        {
            _unit?.Tag.Add<T>();
            _unit?.UpdateTeam();
            return this;
        }

        public UnitBuildChain WithTeam(UnitTag team)
        {
            // team задаётся через Tag до инициализации
            return this;
        }

        // ─── Modules ─────────────────────────────────────────────────────
        public UnitBuildChain WithModule(ModuleTemplate template)
        {
            _controller?.Add(template);
            return this;
        }

        public UnitBuildChain WithModules(params ModuleTemplate[] templates)
        {
            _controller?.Add(templates);
            return this;
        }

        public UnitBuildChain WithoutModule<T>() where T : Module
        {
            _controller?.Remove<T>();
            return this;
        }

        // ─── Unity Components ────────────────────────────────────────────
        public UnitBuildChain WithComponent<T>() where T : Component
        {
            if (!_go.TryGetComponent<T>(out _))
                _go.AddComponent<T>();
            return this;
        }

        public UnitBuildChain WithComponent<T>(Action<T> configure) where T : Component
        {
            var c = _go.GetOrAdd<T>();
            configure?.Invoke(c);
            return this;
        }

        public UnitBuildChain WithoutComponent<T>() where T : Component
        {
            if (_go.TryGetComponent<T>(out var c))
                UnityEngine.Object.Destroy(c);
            return this;
        }

        // ─── Transform ───────────────────────────────────────────────────
        public UnitBuildChain At(Vector3 position)
        {
            _go.transform.position = position;
            return this;
        }

        public UnitBuildChain Facing(Quaternion rotation)
        {
            _go.transform.rotation = rotation;
            return this;
        }

        public UnitBuildChain ScaledTo(Vector3 scale)
        {
            _go.transform.localScale = scale;
            return this;
        }

        public UnitBuildChain Parent(Transform parent, bool worldPositionStays = true)
        {
            _go.transform.SetParent(parent, worldPositionStays);
            return this;
        }

        // ─── Lifecycle ───────────────────────────────────────────────────
        public UnitBuildChain Activate()
        {
            _unit?.Run();
            return this;
        }

        public UnitBuildChain Configure(Action<Unit> configure)
        {
            if (_unit != null) configure?.Invoke(_unit);
            return this;
        }

        // ─── Build ───────────────────────────────────────────────────────
        public Unit Build()
        {
            return _unit;
        }

        public bool TryBuild(out Unit unit)
        {
            unit = _unit;
            return unit != null;
        }
    }
}