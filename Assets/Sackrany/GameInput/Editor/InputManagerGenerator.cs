using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Sackrany.GameInput.Editor
{
    public class InputManagerGeneratorWindow : EditorWindow
    {
        InputActionAsset _asset;
        const string PrefKey = "Sackrany.InputManagerGenerator.AssetGuid";

        [MenuItem("Sackrany/Generate Input Managers")]
        static void Open()
        {
            var window = GetWindow<InputManagerGeneratorWindow>("Input Manager Generator");
            window.minSize = new Vector2(360, 120);
            window.Show();
            window.TryRestoreAsset();
        }

        void TryRestoreAsset()
        {
            var guid = EditorPrefs.GetString(PrefKey, "");
            if (string.IsNullOrEmpty(guid)) return;
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path))
                _asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
        }

        void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Input Action Asset", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            EditorGUI.BeginChangeCheck();
            _asset = (InputActionAsset)EditorGUILayout.ObjectField(
                _asset, typeof(InputActionAsset), false);
            if (EditorGUI.EndChangeCheck() && _asset != null)
            {
                var path = AssetDatabase.GetAssetPath(_asset);
                var guid = AssetDatabase.AssetPathToGUID(path);
                EditorPrefs.SetString(PrefKey, guid);
            }

            EditorGUILayout.Space(8);

            if (_asset == null)
            {
                EditorGUILayout.HelpBox("Выбери InputActionAsset для генерации.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Action Maps:", EditorStyles.miniLabel);
            foreach (var map in _asset.actionMaps)
                EditorGUILayout.LabelField($"  • {map.name} ({map.actions.Count} actions)", EditorStyles.miniLabel);

            EditorGUILayout.Space(8);

            if (GUILayout.Button("Generate", GUILayout.Height(32)))
                InputManagerGenerator.Generate(_asset);
        }
    }

    public static class InputManagerGenerator
    {
        const string OutputDir = "Assets/_Generated/GameInput";

        public static void Generate(InputActionAsset asset)
        {
            Directory.CreateDirectory(OutputDir);

            var schemeName = asset.name;
            var mapInfos = new List<MapInfo>();

            foreach (var map in asset.actionMaps)
            {
                var info = BuildMapInfo(map);
                mapInfos.Add(info);
                GenerateCacheFile(info, schemeName);
            }

            GenerateManagerFile(mapInfos, schemeName);

            AssetDatabase.Refresh();
            Debug.Log($"[InputManagerGenerator] Generated {mapInfos.Count} caches + InputManager partial. Scheme: {schemeName}");
        }

        // ── Cache file ──────────────────────────────────────────────────────

        static void GenerateCacheFile(MapInfo map, string schemeName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System.Threading;");
            sb.AppendLine();
            sb.AppendLine("using Cysharp.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("namespace Sackrany.GameInput.Caches");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {map.CacheName} : InputActionsCache");
            sb.AppendLine("    {");
            sb.AppendLine($"        {schemeName}.{map.MapName}Actions _actions;");
            sb.AppendLine();

            foreach (var a in map.ButtonActions)
                sb.AppendLine($"        readonly ToggleableBool _{a.FieldName};");

            if (map.ButtonActions.Count > 0)
                sb.AppendLine();

            foreach (var a in map.ButtonActions)
            {
                sb.AppendLine($"        public bool {a.PropName} => _{a.FieldName}.Value;");
                sb.AppendLine($"        public bool {a.PropName}JustPressed => _{a.FieldName}.JustPressed;");
                sb.AppendLine($"        public ToggleableBool {a.PropName}Mode => _{a.FieldName};");
                sb.AppendLine();
            }

            foreach (var a in map.ValueActions)
                sb.AppendLine($"        public {a.TypeName} {a.PropName} {{ get; private set; }}");

            if (map.ValueActions.Count > 0)
                sb.AppendLine();

            sb.AppendLine($"        public {map.CacheName}({schemeName} input, CancellationToken token)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _actions = input.{map.MapName};");
            sb.AppendLine();

            foreach (var a in map.ButtonActions)
                sb.AppendLine($"            _{a.FieldName} = Register(_actions.{a.ActionName});");

            sb.AppendLine();
            sb.AppendLine("            Update(token).Forget();");
            sb.AppendLine("        }");
            sb.AppendLine();

            sb.AppendLine("        async UniTaskVoid Update(CancellationToken token)");
            sb.AppendLine("        {");
            sb.AppendLine("            while (!token.IsCancellationRequested)");
            sb.AppendLine("            {");
            sb.AppendLine("                await UniTask.Yield(PlayerLoopTiming.LastUpdate, token);");
            sb.AppendLine();
            sb.AppendLine("                ResetAll();");

            if (map.ValueActions.Count > 0)
            {
                sb.AppendLine();
                foreach (var a in map.ValueActions)
                    sb.AppendLine($"                {a.PropName} = _actions.{a.ActionName}.ReadValue<{a.TypeName}>();");
            }

            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(OutputDir, $"{map.CacheName}.cs"), sb.ToString());
        }

        // ── Manager partial ─────────────────────────────────────────────────

        static void GenerateManagerFile(List<MapInfo> maps, string schemeName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using Sackrany.GameInput.Caches;");
            sb.AppendLine("using Sackrany.GameSettings;");
            sb.AppendLine();
            sb.AppendLine("namespace Sackrany.GameInput");
            sb.AppendLine("{");
            sb.AppendLine("    public static partial class InputManager");
            sb.AppendLine("    {");

            // Scheme field
            sb.AppendLine($"        static {schemeName} _inputScheme;");
            sb.AppendLine();

            // Action map shortcuts
            foreach (var map in maps)
            {
                sb.AppendLine($"        public static {schemeName}.{map.MapName}Actions {map.MapName}");
                sb.AppendLine("            { get; private set; }");
                sb.AppendLine();
            }

            // Cache backing fields
            foreach (var map in maps)
                sb.AppendLine($"        static {map.CacheName} _{map.CacheFieldName};");

            sb.AppendLine();

            // Cache public shortcuts
            foreach (var map in maps)
            {
                sb.AppendLine($"        public static {map.CacheName} {map.CacheShortcut}");
                sb.AppendLine($"            => _{map.CacheFieldName};");
                sb.AppendLine();
            }

            // Enable helpers
            foreach (var map in maps)
            {
                sb.AppendLine($"        public static void Enable{map.MapName}()");
                sb.AppendLine("        {");
                foreach (var other in maps)
                {
                    if (other.MapName == map.MapName)
                        sb.AppendLine($"            {map.MapName}.Enable();");
                    else
                        sb.AppendLine($"            {other.MapName}.Disable();");
                }
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            // InitGenerated
            sb.AppendLine("        static partial void InitGenerated()");
            sb.AppendLine("        {");
            sb.AppendLine("            _inputScheme = new();");
            sb.AppendLine("            _inputScheme.Enable();");
            sb.AppendLine();
            foreach (var map in maps)
                sb.AppendLine($"            {map.MapName} = _inputScheme.{map.MapName};");
            sb.AppendLine();
            foreach (var map in maps)
            {
                sb.AppendLine($"            _{map.CacheFieldName} = new {map.CacheName}(_inputScheme, _cancellation.Token);");
                sb.AppendLine($"            Register(_{map.CacheFieldName});");
            }
            sb.AppendLine("        }");
            sb.AppendLine();

            // DisposeGenerated
            sb.AppendLine("        static partial void DisposeGenerated()");
            sb.AppendLine("        {");
            foreach (var map in maps)
                sb.AppendLine($"            {map.MapName}.Disable();");
            sb.AppendLine();
            sb.AppendLine("            _inputScheme?.Disable();");
            sb.AppendLine("            _inputScheme?.Dispose();");
            sb.AppendLine("        }");
            sb.AppendLine();

            // ApplyControlsGenerated
            sb.AppendLine("        static partial void ApplySettingsGenerated()");
            sb.AppendLine("        {");
            sb.AppendLine("            Internal_ApplySettings(_inputScheme.asset);");
            sb.AppendLine("        }");

            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(OutputDir, "InputManager.Generated.cs"), sb.ToString());
        }

        // ── Data ────────────────────────────────────────────────────────────

        static MapInfo BuildMapInfo(InputActionMap map)
        {
            var name = map.name;
            var info = new MapInfo
            {
                MapName = name,
                CacheName = $"{name}ActionsCache",
                CacheShortcut = $"{name}Cache",
                CacheFieldName = char.ToLower(name[0]) + name.Substring(1) + "Cache"
            };

            foreach (var action in map.actions)
            {
                var controlType = action.expectedControlType;

                if (IsButtonType(controlType))
                {
                    var propName = action.name;
                    info.ButtonActions.Add(new ActionInfo
                    {
                        ActionName = action.name,
                        PropName = propName,
                        FieldName = char.ToLower(propName[0]) + propName.Substring(1)
                    });
                }
                else
                {
                    info.ValueActions.Add(new ValueActionInfo
                    {
                        ActionName = action.name,
                        PropName = action.name,
                        TypeName = ControlTypeToCSType(controlType)
                    });
                }
            }

            return info;
        }

        static bool IsButtonType(string controlType)
            => string.IsNullOrEmpty(controlType) || controlType == "Button";

        static string ControlTypeToCSType(string controlType) => controlType switch
        {
            "Vector2" => "UnityEngine.Vector2",
            "Stick" => "UnityEngine.Vector2",
            "Dpad" => "UnityEngine.Vector2",
            "Axis" => "float",
            "Vector3" => "UnityEngine.Vector3",
            "Quaternion" => "UnityEngine.Quaternion",
            _ => "float"
        };

        // ── Models ──────────────────────────────────────────────────────────

        class MapInfo
        {
            public string MapName;
            public string CacheName;
            public string CacheShortcut;
            public string CacheFieldName;
            public List<ActionInfo> ButtonActions = new();
            public List<ValueActionInfo> ValueActions = new();
        }

        class ActionInfo
        {
            public string ActionName;
            public string PropName;
            public string FieldName;
        }

        class ValueActionInfo
        {
            public string ActionName;
            public string PropName;
            public string TypeName;
        }
    }
}