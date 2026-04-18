using System;
using System.Reflection;

using Sackrany.Actor.Modules;

using UnityEngine;

namespace Sackrany.Actor.Static
{
    public static class DependencyInjector
    {
        public static bool Inject(object target, ModulesController modules, object template = null)
        {
            if (template != null)
                InjectTemplate(target, template);
            return InjectDependencies(target, modules);
        }
        public static void InjectTemplate(object target, object template)
        {
            var meta = ModuleReflectionCache.GetMetadata(target.GetType());
            if (meta.Template.Field == null) return;
            if (meta.Template.FieldType == template.GetType())
                meta.Template.Field.SetValue(target, template);
        }
        public static bool InjectDependencies(object target, ModulesController modules)
        {
            var meta = ModuleReflectionCache.GetMetadata(target.GetType());

            foreach (var dep in meta.Dependencies)
            {
                if (dep.IsArray)
                {
                    var found = modules.GetAllAssignable(dep.ElementType);
                    if ((found == null || found.Length == 0) && !dep.IsOptional)
                        return false;

                    var array = Array.CreateInstance(dep.ElementType, found.Length);
                    for (int i = 0; i < found.Length; i++)
                        array.SetValue(found[i], i);

                    dep.Field.SetValue(target, array);
                }
                else
                {
                    if (!TryResolve(dep.FieldType, modules, out var resolved))
                    {
                        if (!dep.IsOptional) return false;
                        continue;
                    }

                    dep.Field.SetValue(target, resolved);
                }
            }

            return true;
        }
        public static bool TryResolve(Type type, ModulesController modules, out object result)
        {
            if (modules.TryGet(type, out var module, tryAssignable: true))
            {
                result = module;
                return true;
            }

            if (typeof(Component).IsAssignableFrom(type))
            {
                var unit = modules.Unit;
                var comp = unit.GetComponent(type);
                if (comp == null)
                    comp = unit.GetComponentInChildren(type, true);
                if (comp != null)
                {
                    result = comp;
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}