// Поместить в папку Editor/ проекта
// Требует Unity 2021.2+

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sackrany.Editor
{
    [Overlay(typeof(SceneView), "scene-switcher", "Scene Switcher")]
    [Icon("d_SceneAsset Icon")]
    public class SceneSwitcherOverlay : Overlay
    {
        private DropdownField _dropdown;
        private Button _openButton;
        private List<SceneEntry> _scenes = new();

        private struct SceneEntry
        {
            public string Name;
            public string Path;
        }

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();
            root.style.minWidth = 220;
            root.style.paddingTop = 4;
            root.style.paddingBottom = 4;
            root.style.paddingLeft = 6;
            root.style.paddingRight = 6;

            RefreshSceneList();

            var names = _scenes.Select(s => s.Name).ToList();
            if (names.Count == 0)
                names.Add("— нет сцен —");

            _dropdown = new DropdownField("Сцена", names, 0);
            _dropdown.style.marginBottom = 4;
            root.Add(_dropdown);

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;

            _openButton = new Button(OnOpenClicked) { text = "Открыть" };
            _openButton.style.flexGrow = 1;
            _openButton.style.marginRight = 4;

            var refreshButton = new Button(OnRefreshClicked) { text = "↺" };
            refreshButton.style.width = 28;
            refreshButton.tooltip = "Обновить список сцен";

            row.Add(_openButton);
            row.Add(refreshButton);
            root.Add(row);

            return root;
        }

        private void RefreshSceneList()
        {
            _scenes.Clear();

            // Сначала добавляем сцены из Build Settings
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (string.IsNullOrEmpty(scene.path))
                    continue;

                var name = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                _scenes.Add(new SceneEntry { Name = name, Path = scene.path });
            }

            // Добавляем остальные сцены из проекта, которых нет в Build Settings
            var buildPaths = new HashSet<string>(_scenes.Select(s => s.Path));
            var allSceneGuids = AssetDatabase.FindAssets("t:Scene");

            foreach (var guid in allSceneGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (buildPaths.Contains(path))
                    continue;

                var name = System.IO.Path.GetFileNameWithoutExtension(path);
                _scenes.Add(new SceneEntry { Name = $"{name} *", Path = path });
            }
        }

        private void OnOpenClicked()
        {
            if (_scenes.Count == 0)
                return;

            var index = _dropdown.index;
            if (index < 0 || index >= _scenes.Count)
                return;

            var target = _scenes[index];

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(target.Path, OpenSceneMode.Single);
        }

        private void OnRefreshClicked()
        {
            RefreshSceneList();

            if (_dropdown == null)
                return;

            var names = _scenes.Select(s => s.Name).ToList();
            if (names.Count == 0)
                names.Add("— нет сцен —");

            _dropdown.choices = names;
            _dropdown.index = 0;
        }
    }
}
#endif