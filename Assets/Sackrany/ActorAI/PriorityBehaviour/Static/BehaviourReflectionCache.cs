using System;
using System.Collections.Generic;
using System.Reflection;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Static;
using Sackrany.ActorAI.PriorityBehaviour.Base;

namespace Sackrany.ActorAI.PriorityBehaviour.Static
{
    public static class BehaviourReflectionCache
    {
        public readonly struct BehaviourTemplateField
        {
            public readonly FieldInfo Field;
            public readonly Type      FieldType;

            public BehaviourTemplateField(FieldInfo field)
            {
                Field     = field;
                FieldType = field.FieldType;
            }
        }

        public readonly struct BehaviourMetadata
        {
            public readonly ModuleReflectionCache.DependencyField[] Dependencies;
            public readonly BehaviourTemplateField                  Template;

            public BehaviourMetadata(
                ModuleReflectionCache.DependencyField[] dependencies,
                BehaviourTemplateField                  template)
            {
                Dependencies = dependencies;
                Template     = template;
            }
        }

        static readonly Dictionary<Type, BehaviourMetadata> _cache = new();

        public static BehaviourMetadata GetMetadata(Type type)
        {
            if (_cache.TryGetValue(type, out var meta))
                return meta;

            meta = BuildMetadata(type);
            _cache[type] = meta;
            return meta;
        }

        static BehaviourMetadata BuildMetadata(Type type)
        {
            var dependencies = new List<ModuleReflectionCache.DependencyField>();
            var template     = default(BehaviourTemplateField);

            var current = type;
            while (current != null && current != typeof(object))
            {
                var fields = current.GetFields(
                    BindingFlags.Instance     |
                    BindingFlags.Public       |
                    BindingFlags.NonPublic    |
                    BindingFlags.DeclaredOnly);

                foreach (var field in fields)
                {
                    if (field.GetCustomAttribute<BehaviourTemplateAttribute>() != null)
                    {
                        if (template.Field == null)
                            template = new BehaviourTemplateField(field);
                    }
                    else if (field.GetCustomAttribute<DependencyAttribute>() is { } dep)
                    {
                        bool isArray = field.FieldType.IsArray;
                        dependencies.Add(new ModuleReflectionCache.DependencyField(
                            field:       field,
                            fieldType:   field.FieldType,
                            elementType: isArray ? field.FieldType.GetElementType() : null,
                            isArray:     isArray,
                            isOptional:  dep.Optional));
                    }
                }

                current = current.BaseType;
            }

            return new BehaviourMetadata(dependencies.ToArray(), template);
        }
    }
}