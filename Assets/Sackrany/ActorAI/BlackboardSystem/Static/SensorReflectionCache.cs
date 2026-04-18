using System;
using System.Collections.Generic;
using System.Reflection;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Static;
using Sackrany.ActorAI.BlackboardSystem.Base;

namespace Sackrany.ActorAI.BlackboardSystem.Static
{
    public static class SensorReflectionCache
    {
        public readonly struct SensorTemplateField
        {
            public readonly FieldInfo Field;
            public readonly Type      FieldType;

            public SensorTemplateField(FieldInfo field)
            {
                Field     = field;
                FieldType = field.FieldType;
            }
        }

        public readonly struct SensorMetadata
        {
            public readonly ModuleReflectionCache.DependencyField[] Dependencies;
            public readonly SensorTemplateField                  Template;

            public SensorMetadata(
                ModuleReflectionCache.DependencyField[] dependencies,
                SensorTemplateField                  template)
            {
                Dependencies = dependencies;
                Template     = template;
            }
        }

        static readonly Dictionary<Type, SensorMetadata> _cache = new();

        public static SensorMetadata GetMetadata(Type type)
        {
            if (_cache.TryGetValue(type, out var meta))
                return meta;

            meta = BuildMetadata(type);
            _cache[type] = meta;
            return meta;
        }

        static SensorMetadata BuildMetadata(Type type)
        {
            var dependencies = new List<ModuleReflectionCache.DependencyField>();
            var template     = default(SensorTemplateField);

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
                    if (field.GetCustomAttribute<SensorTemplateAttribute>() != null)
                    {
                        if (template.Field == null)
                            template = new SensorTemplateField(field);
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

            return new SensorMetadata(dependencies.ToArray(), template);
        }
    }
}